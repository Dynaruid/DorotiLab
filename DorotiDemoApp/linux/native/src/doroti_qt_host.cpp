#include "doroti_qt_host_v2.h"

#include <QApplication>
#include <QAccessible>
#include <QAccessibleEvent>
#include <QClipboard>
#include <QCoreApplication>
#include <QElapsedTimer>
#include <QEvent>
#include <QGuiApplication>
#include <QtGui/qguiapplication_platform.h>
#include <QInputMethod>
#include <QInputMethodEvent>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QHash>
#include <QKeyEvent>
#include <QLocale>
#include <QMetaObject>
#include <QMouseEvent>
#include <QOpenGLContext>
#include <QOpenGLFunctions>
#include <QOpenGLWindow>
#include <QPointerEvent>
#include <QScreen>
#include <QSurfaceFormat>
#include <QStyleHints>
#include <QTabletEvent>
#include <QTimer>
#include <QTouchEvent>
#include <QWheelEvent>
#include <QWindow>
#include <QtCore/qglobal.h>
#include <algorithm>
#include <cmath>
#include <cstring>
#include <exception>
#include <limits>
#include <string>
#include <utility>
#include <wayland-client.h>

#include "ext-background-effect-v1-client-protocol.h"
#include "kde-blur-client-protocol.h"

// Qt deliberately keeps the per-window native resource accessor in its QPA
// compatibility surface. Keep the small ABI prefix used here isolated; the
// public QWaylandApplication interface supplies the display and compositor.
QT_BEGIN_NAMESPACE
class QPlatformNativeInterface : public QObject {
 public:
  virtual void* nativeResourceForIntegration(const QByteArray& resource);
  virtual void* nativeResourceForContext(const QByteArray& resource,
                                         QOpenGLContext* context);
  virtual void* nativeResourceForScreen(const QByteArray& resource, QScreen* screen);
  virtual void* nativeResourceForWindow(const QByteArray& resource, QWindow* window);
};
QT_END_NAMESPACE

namespace {
constexpr std::uint32_t kAbiVersion = 2;
constexpr std::uint64_t kSupportedFeatures =
    DOROTI_QT_FEATURE_OPENGL_FBO | DOROTI_QT_FEATURE_SWAP_ACK |
    DOROTI_QT_FEATURE_CONTEXT_LIFETIME |
    DOROTI_QT_FEATURE_LENGTH_PREFIXED_UTF8 |
    DOROTI_QT_FEATURE_METRICS_LIFECYCLE |
    DOROTI_QT_FEATURE_POINTER_INPUT |
    DOROTI_QT_FEATURE_KEY_FOCUS_INPUT |
    DOROTI_QT_FEATURE_TEXT_INPUT |
    DOROTI_QT_FEATURE_PLATFORM_SERVICES |
    DOROTI_QT_FEATURE_SEMANTICS;
constexpr std::uint32_t kGlRgba8 = 0x8058;

doroti_qt_utf8_v2 Utf8(const char* value) {
  const auto length = value == nullptr ? 0 : std::char_traits<char>::length(value);
  return {reinterpret_cast<const std::uint8_t*>(value), length};
}

doroti_qt_utf8_v2 Utf8(const QByteArray& value) {
  return {reinterpret_cast<const std::uint8_t*>(value.constData()),
          static_cast<std::uint64_t>(value.size())};
}

struct SemanticNode {
  std::int64_t id = 0;
  std::int64_t parent = -1;
  QString label;
  QString value;
  QString role;
  QRectF rect;
  QList<std::int64_t> children;
  std::int64_t actions = 0;
  bool enabled = true;
  bool focused = false;
  bool hidden = false;
  bool button = false;
  bool text_field = false;
  bool header = false;
  bool image = false;
  bool slider = false;
  bool read_only = false;
};

class DorotiAccessibleNode;

QString String(doroti_qt_utf8_v2 value) {
  if (value.data == nullptr || value.length == 0) return {};
  if (value.length > static_cast<std::uint64_t>(std::numeric_limits<qsizetype>::max()))
    return {};
  return QString::fromUtf8(reinterpret_cast<const char*>(value.data),
                           static_cast<qsizetype>(value.length));
}

class DorotiSurface final : public QOpenGLWindow {
 public:
  DorotiSurface(void* callback_context, const doroti_qt_callbacks_v2& callbacks,
                std::uint32_t backdrop_mode, std::uint32_t backdrop_fallback)
      : QOpenGLWindow(QOpenGLWindow::NoPartialUpdate),
        callback_context_(callback_context), callbacks_(callbacks),
        backdrop_mode_(backdrop_mode), backdrop_fallback_(backdrop_fallback) {
    // Wayland compositors can only preserve intentional transparent pixels when
    // the wl_buffer has an alpha channel. Request it before the native surface is
    // created, while retaining full-frame repaint semantics for every swapchain
    // image.
    auto surface_format = format();
    surface_format.setAlphaBufferSize(8);
    surface_format.setSwapBehavior(QSurfaceFormat::DoubleBuffer);
    setFormat(surface_format);
    clock_.start();
    connect(this, &QOpenGLWindow::frameSwapped, this, [this] { FrameSwapped(); });
    connect(this, &QWindow::screenChanged, this, [this](QScreen*) { SendMetrics(); });
    connect(QGuiApplication::styleHints(), &QStyleHints::colorSchemeChanged,
            this, [this](Qt::ColorScheme) { SendConfiguration(); });
  }

  ~DorotiSurface() override {
    ClearSemanticsTree();
    ReleaseBackdrop();
    ReleaseSurface();
    callbacks_.lifecycle_changed(callback_context_, this, 0, Micros());
    callbacks_.closed(callback_context_, this);
  }

  const SemanticNode* Semantic(std::int64_t id) const {
    const auto found = semantics_.constFind(id);
    return found == semantics_.cend() ? nullptr : &found.value();
  }
  QAccessibleInterface* Accessible(std::int64_t id) const;
  void ApplySemantics(const QByteArray& json);
  void ClearSemanticsTree();
  void InitializeBackdrop() {
    Diagnostic("backdrop.requested", BackdropModeName(backdrop_mode_));
    Diagnostic("backdrop.fallback", BackdropFallbackName(backdrop_fallback_));
    if (backdrop_mode_ != DOROTI_QT_BACKDROP_ACRYLIC) {
      ReportBackdrop(backdrop_mode_ == DOROTI_QT_BACKDROP_TRANSPARENT
                         ? "transparent"
                         : "solid",
                     "none", false);
      return;
    }
    if (QGuiApplication::platformName().compare("wayland", Qt::CaseInsensitive) != 0) {
      ApplyBackdropFallback();
      return;
    }
    auto* application = qobject_cast<QGuiApplication*>(QCoreApplication::instance());
    auto* wayland = application == nullptr
                        ? nullptr
                        : application->nativeInterface<QNativeInterface::QWaylandApplication>();
    auto* native = QGuiApplication::platformNativeInterface();
    if (wayland == nullptr || native == nullptr) {
      ApplyBackdropFallback();
      return;
    }
    wayland_display_ = wayland->display();
    wayland_compositor_ = wayland->compositor();
    wayland_surface_ = static_cast<wl_surface*>(
        native->nativeResourceForWindow(QByteArrayLiteral("surface"), this));
    if (wayland_display_ == nullptr || wayland_compositor_ == nullptr ||
        wayland_surface_ == nullptr) {
      ApplyBackdropFallback();
      return;
    }
    wayland_registry_ = wl_display_get_registry(wayland_display_);
    backdrop_event_queue_ = wl_display_create_queue(wayland_display_);
    if (wayland_registry_ == nullptr || backdrop_event_queue_ == nullptr) {
      ReleaseBackdrop();
      ApplyBackdropFallback();
      return;
    }
    wl_proxy_set_queue(reinterpret_cast<wl_proxy*>(wayland_registry_),
                       backdrop_event_queue_);
    if (wl_registry_add_listener(wayland_registry_, &kRegistryListener, this) != 0) {
      ReleaseBackdrop();
      ApplyBackdropFallback();
      return;
    }
    ApplyBackdropFallback();
    backdrop_event_timer_ = new QTimer(this);
    connect(backdrop_event_timer_, &QTimer::timeout, this,
            [this] { PumpBackdropEvents(); });
    backdrop_event_timer_->start(16);
    wl_display_flush(wayland_display_);
  }
  void DispatchSemanticsAction(std::int64_t id, std::int64_t action) {
    callbacks_.semantics_action(callback_context_, this, id, action, Utf8("null"));
  }

  static void RequestFrame(void* view_handle, std::uint64_t frame_token) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr) return;
    QMetaObject::invokeMethod(surface, [surface, frame_token] {
      if (surface->closing_) return;
      if (surface->pending_frame_token_ != 0) {
        surface->Terminal(surface->pending_frame_token_, DOROTI_QT_TERMINAL_SUPERSEDED,
                          surface->surface_generation_);
      }
      surface->pending_frame_token_ = frame_token;
      surface->update();
    }, Qt::QueuedConnection);
  }

  static void RequestClose(void* view_handle) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr) return;
    QMetaObject::invokeMethod(surface, [surface] {
      surface->closing_ = true;
      surface->close();
    }, Qt::QueuedConnection);
  }

  static void* GetGlProcAddress(void* view_handle, doroti_qt_utf8_v2 name) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    auto* current = QOpenGLContext::currentContext();
    if (surface == nullptr || current == nullptr || current != surface->context() ||
        name.data == nullptr || name.length == 0 || name.length > 4096)
      return nullptr;
    const QByteArray symbol(reinterpret_cast<const char*>(name.data),
                            static_cast<qsizetype>(name.length));
    return reinterpret_cast<void*>(current->getProcAddress(symbol));
  }

  static void Resize(void* view_handle, double width, double height) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr || !std::isfinite(width) || !std::isfinite(height) ||
        width <= 0 || height <= 0) return;
    QMetaObject::invokeMethod(surface, [surface, width, height] {
      if (!surface->closing_)
        surface->resize(std::max(1, static_cast<int>(std::round(width))),
                        std::max(1, static_cast<int>(std::round(height))));
    }, Qt::QueuedConnection);
  }

  static void SetClipboardText(void* view_handle, doroti_qt_utf8_v2 text) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr) return;
    const auto copied = String(text);
    QMetaObject::invokeMethod(surface, [copied] {
      if (auto* clipboard = QGuiApplication::clipboard()) clipboard->setText(copied);
    }, Qt::QueuedConnection);
  }

  static void RequestClipboardText(void* view_handle, std::uint64_t request_id) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr) return;
    QMetaObject::invokeMethod(surface, [surface, request_id] {
      const auto value = QGuiApplication::clipboard() == nullptr
                             ? QByteArray{}
                             : QGuiApplication::clipboard()->text().toUtf8();
      surface->callbacks_.clipboard_text(surface->callback_context_, surface,
                                         request_id, Utf8(value));
    }, Qt::QueuedConnection);
  }

  static void SetCursor(void* view_handle, std::uint32_t cursor) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr) return;
    QMetaObject::invokeMethod(surface, [surface, cursor] {
      Qt::CursorShape shape = Qt::ArrowCursor;
      switch (cursor) {
        case 1: shape = Qt::PointingHandCursor; break;
        case 2: shape = Qt::ForbiddenCursor; break;
        case 3: shape = Qt::WaitCursor; break;
        case 4: shape = Qt::BusyCursor; break;
        case 6: shape = Qt::WhatsThisCursor; break;
        case 7: shape = Qt::IBeamCursor; break;
        case 8: shape = Qt::SizeVerCursor; break;
        case 10: shape = Qt::CrossCursor; break;
        case 11: case 17: shape = Qt::SizeAllCursor; break;
        case 12: shape = Qt::OpenHandCursor; break;
        case 13: shape = Qt::ClosedHandCursor; break;
        case 18: shape = Qt::SizeHorCursor; break;
        case 19: shape = Qt::SizeVerCursor; break;
        case 20: shape = Qt::SizeFDiagCursor; break;
        case 21: shape = Qt::SizeBDiagCursor; break;
        case 34: shape = Qt::BlankCursor; break;
        default: break;
      }
      surface->setCursor(shape);
    }, Qt::QueuedConnection);
  }

  static void SetTextClient(void* view_handle,
                            const doroti_qt_text_configuration_v2* configuration,
                            const doroti_qt_text_state_v2* state) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr || configuration == nullptr || state == nullptr) return;
    const auto config = *configuration;
    const auto text = String(state->text);
    const auto selection_base = state->selection_base;
    const auto selection_extent = state->selection_extent;
    const auto composing_base = state->composing_base;
    const auto composing_extent = state->composing_extent;
    QMetaObject::invokeMethod(surface, [surface, config, text, selection_base,
                                        selection_extent, composing_base, composing_extent] {
      surface->text_client_active_ = true;
      surface->text_configuration_ = config;
      surface->ApplyTextState(text, selection_base, selection_extent,
                              composing_base, composing_extent);
      surface->setFlag(Qt::WindowDoesNotAcceptFocus, false);
      surface->requestActivate();
      QGuiApplication::inputMethod()->show();
    }, Qt::QueuedConnection);
  }

  static void UpdateTextState(void* view_handle,
                              const doroti_qt_text_state_v2* state) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr || state == nullptr) return;
    const auto text = String(state->text);
    const auto selection_base = state->selection_base;
    const auto selection_extent = state->selection_extent;
    const auto composing_base = state->composing_base;
    const auto composing_extent = state->composing_extent;
    QMetaObject::invokeMethod(surface, [surface, text, selection_base,
                                        selection_extent, composing_base, composing_extent] {
      surface->ApplyTextState(text, selection_base, selection_extent,
                              composing_base, composing_extent);
    }, Qt::QueuedConnection);
  }

  static void SetCaretRect(void* view_handle, double left, double top,
                           double width, double height) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr) return;
    QMetaObject::invokeMethod(surface, [surface, left, top, width, height] {
      surface->caret_rect_ = QRectF(left, top, width, height);
      QGuiApplication::inputMethod()->update(Qt::ImCursorRectangle);
    }, Qt::QueuedConnection);
  }

  static void ClearTextClient(void* view_handle) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr) return;
    QMetaObject::invokeMethod(surface, [surface] {
      surface->text_client_active_ = false;
      surface->composing_base_ = surface->composing_extent_ = -1;
      QGuiApplication::inputMethod()->reset();
      QGuiApplication::inputMethod()->hide();
    }, Qt::QueuedConnection);
  }

  static void UpdateSemantics(void* view_handle, doroti_qt_utf8_v2 json) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr || json.data == nullptr || json.length == 0 ||
        json.length > 16 * 1024 * 1024) return;
    const QByteArray copied(reinterpret_cast<const char*>(json.data),
                            static_cast<qsizetype>(json.length));
    QMetaObject::invokeMethod(surface, [surface, copied] {
      QJsonParseError error;
      const auto document = QJsonDocument::fromJson(copied, &error);
      if (error.error != QJsonParseError::NoError || !document.isObject()) {
        surface->callbacks_.fatal(surface->callback_context_, DOROTI_QT_ERROR_INVALID_ARGUMENT,
                                  Utf8("invalid managed semantics JSON"));
        return;
      }
      surface->ApplySemantics(copied);
      const auto nodes = document.object().value("nodes").toArray().size();
      const auto count = QByteArray::number(nodes);
      surface->Diagnostic("semantics.nodes", count.constData());
    }, Qt::QueuedConnection);
  }

  static void ClearSemantics(void* view_handle) noexcept {
    auto* surface = static_cast<DorotiSurface*>(view_handle);
    if (surface == nullptr) return;
    QMetaObject::invokeMethod(surface, [surface] { surface->ClearSemanticsTree(); },
                              Qt::QueuedConnection);
  }

 protected:
  void initializeGL() override {
    ++surface_generation_;
    surface_released_ = false;
    auto* current = QOpenGLContext::currentContext();
    context_identity_ = reinterpret_cast<std::uintptr_t>(current);
    if (current != nullptr) {
      connect(current, &QOpenGLContext::aboutToBeDestroyed, this,
              [this] { ReleaseSurface(); }, Qt::DirectConnection);
    }
    const auto qpa = QGuiApplication::platformName().toUtf8();
    Diagnostic("qpa", qpa.constData());
    const auto generation = QByteArray::number(surface_generation_);
    Diagnostic("surface.generation", generation.constData());
    if (current != nullptr) {
      auto* functions = current->functions();
      Diagnostic("gl.vendor", reinterpret_cast<const char*>(functions->glGetString(GL_VENDOR)));
      const auto* renderer = reinterpret_cast<const char*>(functions->glGetString(GL_RENDERER));
      Diagnostic("gl.renderer", renderer);
      Diagnostic("gl.version", reinterpret_cast<const char*>(functions->glGetString(GL_VERSION)));
      const auto normalized = QByteArray(renderer == nullptr ? "" : renderer).toLower();
      software_renderer_ = normalized.contains("llvmpipe") || normalized.contains("softpipe") ||
                           normalized.contains("swiftshader");
      const auto alpha_bits = QByteArray::number(current->format().alphaBufferSize());
      Diagnostic("surface.alphaBits", alpha_bits.constData());
      Diagnostic("surface.repaint", "full-transparent-clear");
    }
  }

  void paintGL() override {
    if (pending_frame_token_ == 0 || fatal_) return;
    const auto token = std::exchange(pending_frame_token_, 0);
    const auto scale = devicePixelRatioF();
    const auto format = context() == nullptr ? QSurfaceFormat{} : context()->format();
    const doroti_qt_surface_v2 descriptor{
        kAbiVersion,
        sizeof(doroti_qt_surface_v2),
        surface_generation_,
        context_identity_,
        defaultFramebufferObject(),
        std::max(1, static_cast<int>(width() * scale)),
        std::max(1, static_cast<int>(height() * scale)),
        scale,
        std::max(0, format.samples()),
        std::max(0, format.stencilBufferSize()),
        kGlRgba8,
        context() != nullptr && context()->isOpenGLES() ? 2u : 1u,
        static_cast<std::uint32_t>(format.profile()),
        format.majorVersion(),
        format.minorVersion(),
        Micros(),
    };
    const auto fbo = QByteArray::number(descriptor.framebuffer_object);
    const auto samples = QByteArray::number(descriptor.sample_count);
    const auto stencil = QByteArray::number(descriptor.stencil_bits);
    const auto dpr = QByteArray::number(descriptor.device_pixel_ratio, 'f', 3);
    Diagnostic("surface.fbo", fbo.constData());
    Diagnostic("surface.samples", samples.constData());
    Diagnostic("surface.stencil", stencil.constData());
    Diagnostic("surface.dpr", dpr.constData());
    if (software_renderer_) {
      fatal_ = true;
      Terminal(token, DOROTI_QT_TERMINAL_FAILED, surface_generation_);
      callbacks_.fatal(callback_context_, DOROTI_QT_ERROR_UNSUPPORTED_FEATURE,
                       Utf8("Qt OpenGL software or non-accelerated renderer is not accepted by the Linux GPU backend"));
      QCoreApplication::exit(DOROTI_QT_ERROR_UNSUPPORTED_FEATURE);
      RequestClose(this);
      return;
    }
    // A Wayland/EGL swapchain does not promise that a newly acquired buffer is
    // zeroed. Clear the complete native target before handing it to Skia so a
    // transparent framework background means transparent *this frame*, rather
    // than blending with pixels left in an older swapchain image. Reset the GL
    // write state explicitly because paintGL may follow arbitrary Skia state.
    auto* functions = context()->functions();
    functions->glBindFramebuffer(GL_FRAMEBUFFER, descriptor.framebuffer_object);
    functions->glDisable(GL_SCISSOR_TEST);
    functions->glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
    functions->glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
    functions->glClear(GL_COLOR_BUFFER_BIT);
    const auto result = callbacks_.render(callback_context_, this, &descriptor, token);
    if (result != DOROTI_QT_OK) {
      fatal_ = true;
      Terminal(token, DOROTI_QT_TERMINAL_FAILED, surface_generation_);
      callbacks_.fatal(callback_context_, result, Utf8("managed render callback failed"));
      QCoreApplication::exit(result);
      RequestClose(this);
      return;
    }
    rasterized_frame_token_ = token;
    rasterized_generation_ = surface_generation_;
  }

  void resizeGL(int, int) override {
    if (next_automatic_frame_token_ == 0) next_automatic_frame_token_ = 1;
    RequestFrame(this, next_automatic_frame_token_++);
  }

  bool event(QEvent* event) override {
    switch (event->type()) {
      case QEvent::Show:
        lifecycle_state_ = 1;
        callbacks_.lifecycle_changed(callback_context_, this, lifecycle_state_, Micros());
        SendMetrics();
        SendConfiguration();
        break;
      case QEvent::Hide:
        lifecycle_state_ = 3;
        callbacks_.lifecycle_changed(callback_context_, this, lifecycle_state_, Micros());
        SendMetrics();
        break;
      case QEvent::Resize:
        // Background-effect regions are wl_surface double-buffered state. Queue
        // the new logical bounds before QOpenGLWindow handles the resize so Qt's
        // corresponding surface commit applies the buffer size and blur region
        // atomically. resizeGL runs later in the GL paint path and can otherwise
        // leave the compositor using the previous window bounds.
        UpdateBackdropRegion();
        SendMetrics();
        break;
      case QEvent::WindowActivate:
        lifecycle_state_ = 1;
        callbacks_.lifecycle_changed(callback_context_, this, lifecycle_state_, Micros());
        callbacks_.focus(callback_context_, this, 1, Micros());
        break;
      case QEvent::WindowDeactivate:
        lifecycle_state_ = 2;
        callbacks_.lifecycle_changed(callback_context_, this, lifecycle_state_, Micros());
        callbacks_.focus(callback_context_, this, 0, Micros());
        break;
      case QEvent::Close:
        if (!close_requested_) {
          close_requested_ = true;
          callbacks_.close_requested(callback_context_, this);
        }
        closing_ = true;
        break;
      case QEvent::Enter: {
        auto* enter = static_cast<QEnterEvent*>(event);
        SendPointer(enter->position(), QPointF{}, 1, 1, 0, 1, 0, 0, 0, 0);
        break;
      }
      case QEvent::Leave:
        SendPointer(last_pointer_position_, QPointF{}, 2, 1, 0, 1, 0, 0, 0, 0);
        break;
      case QEvent::MouseButtonPress:
      case QEvent::MouseButtonRelease:
      case QEvent::MouseMove: {
        auto* mouse = static_cast<QMouseEvent*>(event);
        const auto change = event->type() == QEvent::MouseButtonPress ? 4u
                            : event->type() == QEvent::MouseButtonRelease ? 6u
                            : mouse->buttons() == Qt::NoButton ? 3u : 5u;
        const auto delta = mouse->position() - last_pointer_position_;
        SendPointer(mouse->position(), delta, change, 1,
                    static_cast<std::int64_t>(mouse->buttons()), 1, 0, 0,
                    static_cast<std::uint32_t>(mouse->modifiers()), 0);
        break;
      }
      case QEvent::Wheel: {
        auto* wheel = static_cast<QWheelEvent*>(event);
        auto scroll = wheel->pixelDelta();
        if (scroll.isNull()) scroll = wheel->angleDelta() / 8;
        const auto factor = wheel->inverted() ? 1.0 : -1.0;
        SendPointer(wheel->position(), QPointF{}, 3, 1,
                    static_cast<std::int64_t>(wheel->buttons()), 1, 0, 1,
                    static_cast<std::uint32_t>(wheel->modifiers()),
                    wheel->phase(), scroll.x() * factor, scroll.y() * factor);
        break;
      }
      case QEvent::TabletPress:
      case QEvent::TabletMove:
      case QEvent::TabletRelease: {
        auto* tablet = static_cast<QTabletEvent*>(event);
        const auto change = event->type() == QEvent::TabletPress ? 4u
                            : event->type() == QEvent::TabletRelease ? 6u : 5u;
        const auto delta = tablet->position() - last_pointer_position_;
        SendPointer(tablet->position(), delta, change, 2,
                    static_cast<std::int64_t>(tablet->buttons()),
                    static_cast<std::uint64_t>(tablet->device()->systemId()),
                    static_cast<std::uint64_t>(tablet->pointingDevice()->uniqueId().numericId()),
                    0, static_cast<std::uint32_t>(tablet->modifiers()), 0, 0, 0,
                    tablet->pressure(), std::hypot(tablet->xTilt(), tablet->yTilt()));
        break;
      }
      case QEvent::TouchBegin:
      case QEvent::TouchUpdate:
      case QEvent::TouchEnd:
      case QEvent::TouchCancel: {
        auto* touch = static_cast<QTouchEvent*>(event);
        for (const auto& point : touch->points()) {
          std::uint32_t change = 5;
          if (event->type() == QEvent::TouchCancel) change = 0;
          else if (point.state() == QEventPoint::Pressed) change = 4;
          else if (point.state() == QEventPoint::Released) change = 6;
          SendPointer(point.position(), point.position() - point.lastPosition(), change, 0,
                      change == 6 ? 0 : 1,
                      static_cast<std::uint64_t>(touch->pointingDevice()->systemId()),
                      static_cast<std::uint64_t>(point.id()), 0,
                      static_cast<std::uint32_t>(touch->modifiers()), 0, 0, 0,
                      point.pressure(), 0);
        }
        break;
      }
      case QEvent::KeyPress:
      case QEvent::KeyRelease: {
        auto* key = static_cast<QKeyEvent*>(event);
        const auto text = key->text().toUtf8();
        const doroti_qt_key_v2 descriptor{
            kAbiVersion, sizeof(doroti_qt_key_v2),
            static_cast<std::int64_t>(key->nativeScanCode()),
            static_cast<std::int64_t>(key->key()),
            event->type() == QEvent::KeyRelease ? 1u : key->isAutoRepeat() ? 2u : 0u,
            static_cast<std::uint32_t>(key->modifiers()), Utf8(text), Micros()};
        callbacks_.key(callback_context_, this, &descriptor);
        if (text_client_active_ && event->type() == QEvent::KeyPress &&
            (key->key() == Qt::Key_Return || key->key() == Qt::Key_Enter) &&
            text_configuration_.input_action != 0 && text_configuration_.input_action != 12) {
          callbacks_.text_action(callback_context_, this, text_configuration_.input_action);
          event->accept();
          return true;
        }
        break;
      }
      case QEvent::InputMethod:
        HandleInputMethod(static_cast<QInputMethodEvent*>(event));
        return true;
      case QEvent::InputMethodQuery: {
        auto* query = static_cast<QInputMethodQueryEvent*>(event);
        for (auto item : {Qt::ImEnabled, Qt::ImCursorRectangle, Qt::ImCursorPosition,
                          Qt::ImAnchorPosition, Qt::ImSurroundingText,
                          Qt::ImCurrentSelection, Qt::ImHints})
          query->setValue(item, inputMethodQuery(item));
        query->accept();
        return true;
      }
      case QEvent::LocaleChange:
      case QEvent::ApplicationPaletteChange:
      case QEvent::ThemeChange:
        SendConfiguration();
        break;
      case QEvent::DevicePixelRatioChange:
        SendMetrics();
        break;
      default:
        break;
    }
    return QOpenGLWindow::event(event);
  }

  QVariant inputMethodQuery(Qt::InputMethodQuery query) const {
    switch (query) {
      case Qt::ImEnabled: return text_client_active_ && !text_configuration_.read_only;
      case Qt::ImCursorRectangle: return caret_rect_;
      case Qt::ImCursorPosition: return selection_extent_;
      case Qt::ImAnchorPosition: return selection_base_;
      case Qt::ImSurroundingText: return text_;
      case Qt::ImCurrentSelection: {
        const auto start = std::min(selection_base_, selection_extent_);
        return text_.mid(start, std::abs(selection_extent_ - selection_base_));
      }
      case Qt::ImHints: return static_cast<int>(InputMethodHints());
      default: return {};
    }
  }

 private:
  std::int64_t Micros() const { return clock_.nsecsElapsed() / 1000; }

  void SendMetrics() {
    const auto scale = devicePixelRatio();
    const doroti_qt_metrics_v2 metrics{
        kAbiVersion, sizeof(doroti_qt_metrics_v2), surface_generation_,
        std::max(1, static_cast<int>(std::round(width() * scale))),
        std::max(1, static_cast<int>(std::round(height() * scale))),
        scale, lifecycle_state_, 0, ++metrics_generation_, Micros()};
    callbacks_.metrics_changed(callback_context_, this, &metrics);
  }

  void SendConfiguration() {
    const auto languages = QLocale::system().uiLanguages().join('\n').toUtf8();
    const auto dark = QGuiApplication::styleHints()->colorScheme() == Qt::ColorScheme::Dark;
    const auto time_format = QLocale::system().timeFormat(QLocale::ShortFormat);
    const auto always_24 = !time_format.contains("AP", Qt::CaseInsensitive) &&
                           !time_format.contains('a', Qt::CaseInsensitive);
    callbacks_.configuration_changed(callback_context_, this, Utf8(languages),
                                     dark ? 0u : 1u, always_24 ? 1u : 0u);
  }

  void SendPointer(const QPointF& logical_position, const QPointF& logical_delta,
                   std::uint32_t change, std::uint32_t kind, std::int64_t buttons,
                   std::uint64_t device, std::uint64_t pointer_identifier,
                   std::uint32_t signal_kind, std::uint32_t modifiers,
                   std::uint32_t phase, double scroll_x = 0, double scroll_y = 0,
                   double pressure = 1, double tilt = 0) {
    const auto scale = devicePixelRatio();
    last_pointer_position_ = logical_position;
    const doroti_qt_pointer_v2 descriptor{
        kAbiVersion, sizeof(doroti_qt_pointer_v2), device, pointer_identifier,
        change, kind, buttons, logical_position.x() * scale,
        logical_position.y() * scale, logical_delta.x() * scale,
        logical_delta.y() * scale, pressure, tilt, signal_kind,
        modifiers | (phase << 24), scroll_x * scale, scroll_y * scale, Micros()};
    callbacks_.pointer(callback_context_, this, &descriptor);
  }

  Qt::InputMethodHints InputMethodHints() const {
    Qt::InputMethodHints hints = Qt::ImhNone;
    switch (text_configuration_.input_type) {
      case 2: hints |= Qt::ImhFormattedNumbersOnly; break;
      case 3: hints |= Qt::ImhDialableCharactersOnly; break;
      case 5: hints |= Qt::ImhEmailCharactersOnly; break;
      case 6: hints |= Qt::ImhUrlCharactersOnly; break;
      case 7: hints |= Qt::ImhSensitiveData; break;
      default: break;
    }
    if (text_configuration_.obscure_text) hints |= Qt::ImhHiddenText | Qt::ImhSensitiveData;
    if (!text_configuration_.autocorrect) hints |= Qt::ImhNoAutoUppercase;
    if (!text_configuration_.enable_suggestions) hints |= Qt::ImhNoPredictiveText;
    return hints;
  }

  void ApplyTextState(const QString& text, int selection_base, int selection_extent,
                      int composing_base, int composing_extent) {
    text_ = text;
    const auto text_size = static_cast<int>(text_.size());
    selection_base_ = std::clamp(selection_base, 0, text_size);
    selection_extent_ = std::clamp(selection_extent, 0, text_size);
    composing_base_ = composing_base < 0 ? -1 : std::clamp(composing_base, 0, text_size);
    composing_extent_ = composing_extent < 0 ? -1 : std::clamp(composing_extent, 0, text_size);
    QGuiApplication::inputMethod()->update(
        Qt::ImEnabled | Qt::ImCursorRectangle | Qt::ImCursorPosition |
        Qt::ImAnchorPosition | Qt::ImSurroundingText | Qt::ImCurrentSelection | Qt::ImHints);
  }

  void HandleInputMethod(QInputMethodEvent* event) {
    if (!text_client_active_ || text_configuration_.read_only) {
      event->ignore();
      return;
    }
    if (composing_base_ >= 0 && composing_extent_ >= composing_base_) {
      text_.remove(composing_base_, composing_extent_ - composing_base_);
      selection_base_ = selection_extent_ = composing_base_;
    }
    composing_base_ = composing_extent_ = -1;
    auto start = std::min(selection_base_, selection_extent_);
    auto length = std::abs(selection_extent_ - selection_base_);
    if (event->replacementLength() != 0) {
      const auto text_size = static_cast<int>(text_.size());
      start = std::clamp(selection_extent_ + event->replacementStart(), 0, text_size);
      length = std::clamp(event->replacementLength(), 0, text_size - start);
    }
    text_.replace(start, length, event->commitString());
    auto cursor = start + event->commitString().size();
    if (!event->preeditString().isEmpty()) {
      text_.insert(cursor, event->preeditString());
      composing_base_ = cursor;
      composing_extent_ = cursor + event->preeditString().size();
      selection_base_ = selection_extent_ = composing_extent_;
      for (const auto& attribute : event->attributes()) {
        if (attribute.type == QInputMethodEvent::Cursor) {
          selection_base_ = selection_extent_ =
              std::clamp(composing_base_ + attribute.start, composing_base_, composing_extent_);
        } else if (attribute.type == QInputMethodEvent::Selection) {
          const auto text_size = static_cast<int>(text_.size());
          selection_base_ = std::clamp(attribute.start, 0, text_size);
          selection_extent_ = std::clamp(attribute.start + attribute.length, 0, text_size);
        }
      }
    } else {
      selection_base_ = selection_extent_ = cursor;
    }
    const auto utf8 = text_.toUtf8();
    const doroti_qt_text_state_v2 state{
        kAbiVersion, sizeof(doroti_qt_text_state_v2), Utf8(utf8),
        selection_base_, selection_extent_, composing_base_, composing_extent_};
    callbacks_.text_editing(callback_context_, this, &state);
    event->accept();
  }

  void FrameSwapped() {
    if (rasterized_frame_token_ == 0) return;
    const auto token = std::exchange(rasterized_frame_token_, 0);
    Terminal(token, DOROTI_QT_TERMINAL_PRESENTED, rasterized_generation_);
  }

  void Terminal(std::uint64_t token, std::uint32_t state, std::uint64_t generation) {
    callbacks_.frame_terminal(callback_context_, this, token, state, generation, Micros());
  }

  void ReleaseSurface() {
    if (surface_released_ || context_identity_ == 0) return;
    surface_released_ = true;
    if (context() != nullptr && QOpenGLContext::currentContext() != context()) makeCurrent();
    callbacks_.surface_destroying(callback_context_, this, surface_generation_, context_identity_);
    if (context() != nullptr && QOpenGLContext::currentContext() == context()) doneCurrent();
    context_identity_ = 0;
  }

  static const char* BackdropModeName(std::uint32_t mode) {
    switch (mode) {
      case DOROTI_QT_BACKDROP_SOLID: return "solid";
      case DOROTI_QT_BACKDROP_TRANSPARENT: return "transparent";
      case DOROTI_QT_BACKDROP_ACRYLIC: return "acrylic";
      default: return "system";
    }
  }

  static const char* BackdropFallbackName(std::uint32_t fallback) {
    return fallback == DOROTI_QT_BACKDROP_FALLBACK_SOLID ? "solid" : "transparent";
  }

  static void RegistryGlobal(void* data, wl_registry* registry, std::uint32_t name,
                             const char* interface, std::uint32_t version) {
    auto* surface = static_cast<DorotiSurface*>(data);
    if (std::strcmp(interface, ext_background_effect_manager_v1_interface.name) == 0) {
      surface->ext_manager_name_ = name;
      surface->ext_manager_ = static_cast<ext_background_effect_manager_v1*>(
          wl_registry_bind(registry, name, &ext_background_effect_manager_v1_interface,
                           std::min(version, 1u)));
      if (surface->ext_manager_ != nullptr)
        ext_background_effect_manager_v1_add_listener(
            surface->ext_manager_, &kExtManagerListener, surface);
    } else if (std::strcmp(interface, org_kde_kwin_blur_manager_interface.name) == 0) {
      surface->kde_manager_name_ = name;
      surface->kde_manager_ = static_cast<org_kde_kwin_blur_manager*>(
          wl_registry_bind(registry, name, &org_kde_kwin_blur_manager_interface,
                           std::min(version, 1u)));
      surface->ApplyBackdrop();
    }
  }

  static void RegistryGlobalRemove(void* data, wl_registry*, std::uint32_t name) {
    auto* surface = static_cast<DorotiSurface*>(data);
    if (name == surface->ext_manager_name_) {
      surface->DestroyExtBackdrop();
      if (surface->ext_manager_ != nullptr)
        ext_background_effect_manager_v1_destroy(surface->ext_manager_);
      surface->ext_manager_ = nullptr;
      surface->ext_manager_name_ = 0;
      surface->ext_blur_available_ = false;
      surface->ApplyBackdrop();
    }
    if (name == surface->kde_manager_name_) {
      surface->DestroyKdeBackdrop();
      if (surface->kde_manager_ != nullptr)
        wl_proxy_destroy(reinterpret_cast<wl_proxy*>(surface->kde_manager_));
      surface->kde_manager_ = nullptr;
      surface->kde_manager_name_ = 0;
      surface->ApplyBackdrop();
    }
  }

  static void ExtCapabilities(void* data, ext_background_effect_manager_v1*,
                              std::uint32_t flags) {
    auto* surface = static_cast<DorotiSurface*>(data);
    surface->ext_blur_available_ =
        (flags & EXT_BACKGROUND_EFFECT_MANAGER_V1_CAPABILITY_BLUR) != 0;
    surface->ApplyBackdrop();
  }

  void ApplyBackdrop() {
    if (backdrop_mode_ != DOROTI_QT_BACKDROP_ACRYLIC || wayland_surface_ == nullptr) return;
    if (ext_manager_ != nullptr && ext_blur_available_) {
      DestroyKdeBackdrop();
      if (ext_effect_ == nullptr)
        ext_effect_ = ext_background_effect_manager_v1_get_background_effect(
            ext_manager_, wayland_surface_);
      UpdateBackdropRegion();
      ReportBackdrop("acrylic", "ext-background-effect-v1", true);
      return;
    }
    if (kde_manager_ != nullptr) {
      DestroyExtBackdrop();
      if (kde_effect_ == nullptr)
        kde_effect_ = org_kde_kwin_blur_manager_create(kde_manager_, wayland_surface_);
      UpdateBackdropRegion();
      ReportBackdrop("acrylic", "kde-blur-v1", true);
      return;
    }
    DestroyExtBackdrop();
    DestroyKdeBackdrop();
    ApplyBackdropFallback();
  }

  void PumpBackdropEvents() {
    if (wayland_display_ == nullptr) return;
    // Qt owns reads from the shared display socket. Our proxies use an isolated
    // queue, so dispatching their already-pending events is non-blocking and
    // cannot consume Qt's own Wayland events.
    if (backdrop_event_queue_ == nullptr ||
        wl_display_dispatch_queue_pending(wayland_display_, backdrop_event_queue_) < 0) {
      backdrop_event_timer_->stop();
      ApplyBackdropFallback();
      return;
    }
    wl_display_flush(wayland_display_);
  }

  void ApplyBackdropFallback() {
    ReportBackdrop(BackdropFallbackName(backdrop_fallback_), "none", false);
  }

  void UpdateBackdropRegion() {
    if (wayland_compositor_ == nullptr || wayland_surface_ == nullptr ||
        (ext_effect_ == nullptr && kde_effect_ == nullptr)) return;
    auto* region = wl_compositor_create_region(wayland_compositor_);
    if (region == nullptr) return;
    wl_region_add(region, 0, 0, std::max(1, width()), std::max(1, height()));
    if (ext_effect_ != nullptr)
      ext_background_effect_surface_v1_set_blur_region(ext_effect_, region);
    if (kde_effect_ != nullptr) {
      org_kde_kwin_blur_set_region(kde_effect_, region);
      org_kde_kwin_blur_commit(kde_effect_);
    }
    wl_region_destroy(region);
    update();
  }

  void DestroyExtBackdrop() {
    if (ext_effect_ == nullptr) return;
    ext_background_effect_surface_v1_destroy(ext_effect_);
    ext_effect_ = nullptr;
  }

  void DestroyKdeBackdrop() {
    if (kde_effect_ == nullptr) return;
    // The platform window may already have destroyed its wl_surface during
    // close(). Releasing the blur object is sufficient and never references a
    // potentially stale surface proxy.
    org_kde_kwin_blur_release(kde_effect_);
    kde_effect_ = nullptr;
  }

  void ReleaseBackdrop() {
    if (backdrop_event_timer_ != nullptr) backdrop_event_timer_->stop();
    DestroyExtBackdrop();
    DestroyKdeBackdrop();
    if (ext_manager_ != nullptr) ext_background_effect_manager_v1_destroy(ext_manager_);
    if (kde_manager_ != nullptr)
      wl_proxy_destroy(reinterpret_cast<wl_proxy*>(kde_manager_));
    if (wayland_registry_ != nullptr) wl_registry_destroy(wayland_registry_);
    if (backdrop_event_queue_ != nullptr) wl_event_queue_destroy(backdrop_event_queue_);
    ext_manager_ = nullptr;
    kde_manager_ = nullptr;
    wayland_registry_ = nullptr;
    backdrop_event_queue_ = nullptr;
    wayland_surface_ = nullptr;
    wayland_compositor_ = nullptr;
    wayland_display_ = nullptr;
  }

  void ReportBackdrop(const char* effective, const char* provider, bool supported) {
    const QByteArray next_effective(effective);
    const QByteArray next_provider(provider);
    if (next_effective == backdrop_effective_ && next_provider == backdrop_provider_) return;
    backdrop_effective_ = next_effective;
    backdrop_provider_ = next_provider;
    Diagnostic("backdrop.effective", backdrop_effective_.constData());
    Diagnostic("backdrop.provider", backdrop_provider_.constData());
    Diagnostic("backdrop.compositorBlur", supported ? "true" : "false");
  }

  inline static const wl_registry_listener kRegistryListener{
      &DorotiSurface::RegistryGlobal, &DorotiSurface::RegistryGlobalRemove};
  inline static const ext_background_effect_manager_v1_listener kExtManagerListener{
      &DorotiSurface::ExtCapabilities};

  void Diagnostic(const char* key, const char* value) {
    callbacks_.diagnostic(callback_context_, Utf8(key), Utf8(value == nullptr ? "unknown" : value));
  }

  void* callback_context_;
  doroti_qt_callbacks_v2 callbacks_;
  QElapsedTimer clock_;
  std::uint64_t surface_generation_ = 0;
  std::uint64_t context_identity_ = 0;
  std::uint64_t pending_frame_token_ = 0;
  std::uint64_t rasterized_frame_token_ = 0;
  std::uint64_t rasterized_generation_ = 0;
  std::uint64_t next_automatic_frame_token_ = 1;
  bool surface_released_ = true;
  std::uint32_t backdrop_mode_ = DOROTI_QT_BACKDROP_SYSTEM;
  std::uint32_t backdrop_fallback_ = DOROTI_QT_BACKDROP_FALLBACK_TRANSPARENT;
  wl_display* wayland_display_ = nullptr;
  wl_compositor* wayland_compositor_ = nullptr;
  wl_surface* wayland_surface_ = nullptr;
  wl_registry* wayland_registry_ = nullptr;
  wl_event_queue* backdrop_event_queue_ = nullptr;
  ext_background_effect_manager_v1* ext_manager_ = nullptr;
  ext_background_effect_surface_v1* ext_effect_ = nullptr;
  org_kde_kwin_blur_manager* kde_manager_ = nullptr;
  org_kde_kwin_blur* kde_effect_ = nullptr;
  std::uint32_t ext_manager_name_ = 0;
  std::uint32_t kde_manager_name_ = 0;
  bool ext_blur_available_ = false;
  QByteArray backdrop_effective_;
  QByteArray backdrop_provider_;
  QTimer* backdrop_event_timer_ = nullptr;
  bool software_renderer_ = false;
  bool fatal_ = false;
  bool closing_ = false;
  bool close_requested_ = false;
  bool text_client_active_ = false;
  std::uint32_t lifecycle_state_ = 0;
  std::uint64_t metrics_generation_ = 0;
  QPointF last_pointer_position_;
  QString text_;
  int selection_base_ = 0;
  int selection_extent_ = 0;
  int composing_base_ = -1;
  int composing_extent_ = -1;
  QRectF caret_rect_;
  doroti_qt_text_configuration_v2 text_configuration_{};
  QHash<std::int64_t, SemanticNode> semantics_;
  mutable QHash<std::int64_t, QAccessible::Id> accessible_ids_;
};

class DorotiAccessibleNode final : public QAccessibleInterface,
                                   public QAccessibleActionInterface {
 public:
  DorotiAccessibleNode(DorotiSurface* surface, std::int64_t id)
      : surface_(surface), id_(id) {}
  ~DorotiAccessibleNode() override = default;

  bool isValid() const override { return surface_ != nullptr && Node() != nullptr; }
  QObject* object() const override { return id_ == 0 ? surface_ : nullptr; }
  QWindow* window() const override { return surface_; }

  QAccessibleInterface* childAt(int x, int y) const override {
    const auto* node = Node();
    if (node == nullptr) return nullptr;
    for (auto it = node->children.crbegin(); it != node->children.crend(); ++it) {
      auto* child = surface_->Accessible(*it);
      if (child != nullptr && child->rect().contains(x, y)) return child;
    }
    return nullptr;
  }

  QAccessibleInterface* parent() const override {
    const auto* node = Node();
    return node == nullptr || node->parent < 0 ? nullptr : surface_->Accessible(node->parent);
  }
  QAccessibleInterface* child(int index) const override {
    const auto* node = Node();
    return node == nullptr || index < 0 || index >= node->children.size()
               ? nullptr : surface_->Accessible(node->children[index]);
  }
  int childCount() const override {
    const auto* node = Node();
    return node == nullptr ? 0 : node->children.size();
  }
  int indexOfChild(const QAccessibleInterface* child) const override {
    const auto* accessible = dynamic_cast<const DorotiAccessibleNode*>(child);
    const auto* node = Node();
    return accessible == nullptr || node == nullptr
               ? -1 : node->children.indexOf(accessible->id_);
  }

  QString text(QAccessible::Text type) const override {
    const auto* node = Node();
    if (node == nullptr) return {};
    if (type == QAccessible::Name) return node->label;
    if (type == QAccessible::Value) return node->value;
    return {};
  }
  void setText(QAccessible::Text, const QString&) override {}

  QRect rect() const override {
    const auto* node = Node();
    if (node == nullptr || surface_ == nullptr) return {};
    const auto origin = surface_->mapToGlobal(
        QPoint(qRound(node->rect.left()), qRound(node->rect.top())));
    return {origin, QSize(qRound(node->rect.width()), qRound(node->rect.height()))};
  }

  QAccessible::Role role() const override {
    const auto* node = Node();
    if (node == nullptr) return QAccessible::NoRole;
    if (id_ == 0) return QAccessible::Client;
    if (node->button) return QAccessible::Button;
    if (node->text_field) return QAccessible::EditableText;
    if (node->header) return QAccessible::Heading;
    if (node->image) return QAccessible::Graphic;
    if (node->slider) return QAccessible::Slider;
    if (node->role == "dialog" || node->role == "alertDialog") return QAccessible::Dialog;
    if (node->role == "table") return QAccessible::Table;
    if (node->role == "cell") return QAccessible::Cell;
    if (node->role == "row") return QAccessible::Row;
    if (node->role == "columnHeader") return QAccessible::ColumnHeader;
    if (node->role == "list") return QAccessible::List;
    if (node->role == "listItem") return QAccessible::ListItem;
    if (node->role == "tab") return QAccessible::PageTab;
    if (node->role == "tabBar") return QAccessible::PageTabList;
    if (node->role == "menu") return QAccessible::PopupMenu;
    if (node->role.startsWith("menuItem")) return QAccessible::MenuItem;
    if (node->role == "progressBar") return QAccessible::ProgressBar;
    if (node->role == "form") return QAccessible::Form;
    if (node->role == "tooltip") return QAccessible::ToolTip;
    if (node->role == "status") return QAccessible::StatusBar;
    if (node->role == "alert") return QAccessible::AlertMessage;
    return node->label.isEmpty() ? QAccessible::Grouping : QAccessible::StaticText;
  }

  QAccessible::State state() const override {
    QAccessible::State state;
    const auto* node = Node();
    if (node == nullptr) {
      state.invalid = true;
      return state;
    }
    state.disabled = !node->enabled;
    state.focused = node->focused;
    state.focusable = (node->actions & (1ll << 22)) != 0 || node->text_field;
    state.invisible = node->hidden;
    state.readOnly = node->read_only;
    state.editable = node->text_field && !node->read_only;
    state.selectableText = node->text_field;
    return state;
  }

  void* interface_cast(QAccessible::InterfaceType type) override {
    return type == QAccessible::ActionInterface
               ? static_cast<QAccessibleActionInterface*>(this) : nullptr;
  }

  QStringList actionNames() const override {
    QStringList result;
    const auto* node = Node();
    if (node == nullptr) return result;
    if ((node->actions & 1) != 0) result << pressAction();
    if ((node->actions & (1ll << 6)) != 0) result << increaseAction();
    if ((node->actions & (1ll << 7)) != 0) result << decreaseAction();
    if ((node->actions & (1ll << 22)) != 0) result << setFocusAction();
    if ((node->actions & (1ll << 2)) != 0) result << scrollLeftAction();
    if ((node->actions & (1ll << 3)) != 0) result << scrollRightAction();
    if ((node->actions & (1ll << 4)) != 0) result << scrollUpAction();
    if ((node->actions & (1ll << 5)) != 0) result << scrollDownAction();
    return result;
  }

  void doAction(const QString& name) override {
    std::int64_t action = 0;
    if (name == pressAction()) action = 1;
    else if (name == increaseAction()) action = 1ll << 6;
    else if (name == decreaseAction()) action = 1ll << 7;
    else if (name == setFocusAction()) action = 1ll << 22;
    else if (name == scrollLeftAction()) action = 1ll << 2;
    else if (name == scrollRightAction()) action = 1ll << 3;
    else if (name == scrollUpAction()) action = 1ll << 4;
    else if (name == scrollDownAction()) action = 1ll << 5;
    if (action != 0 && surface_ != nullptr) surface_->DispatchSemanticsAction(id_, action);
  }
  QStringList keyBindingsForAction(const QString&) const override { return {}; }

 private:
  const SemanticNode* Node() const { return surface_ == nullptr ? nullptr : surface_->Semantic(id_); }
  DorotiSurface* surface_;
  std::int64_t id_;
};

QAccessibleInterface* DorotiSurface::Accessible(std::int64_t id) const {
  if (Semantic(id) == nullptr) return nullptr;
  if (id == 0) return QAccessible::queryAccessibleInterface(const_cast<DorotiSurface*>(this));
  const auto found = accessible_ids_.constFind(id);
  if (found != accessible_ids_.cend()) return QAccessible::accessibleInterface(found.value());
  auto* interface = new DorotiAccessibleNode(const_cast<DorotiSurface*>(this), id);
  accessible_ids_.insert(id, QAccessible::registerAccessibleInterface(interface));
  return interface;
}

void DorotiSurface::ClearSemanticsTree() {
  for (const auto accessible_id : std::as_const(accessible_ids_))
    QAccessible::deleteAccessibleInterface(accessible_id);
  accessible_ids_.clear();
  semantics_.clear();
}

void DorotiSurface::ApplySemantics(const QByteArray& json) {
  const auto document = QJsonDocument::fromJson(json);
  const auto array = document.object().value("nodes").toArray();
  QHash<std::int64_t, SemanticNode> next;
  for (const auto value : array) {
    const auto object = value.toObject();
    SemanticNode node;
    node.id = object.value("id").toInteger();
    node.label = object.value("label").toString();
    node.value = object.value("value").toString();
    node.role = object.value("role").toString();
    node.actions = object.value("actions").toInteger();
    for (const auto child : object.value("children").toArray())
      node.children.append(child.toInteger());
    const auto rect = object.value("rect").toArray();
    if (rect.size() == 4)
      node.rect = QRectF(rect[0].toDouble(), rect[1].toDouble(),
                         rect[2].toDouble() - rect[0].toDouble(),
                         rect[3].toDouble() - rect[1].toDouble());
    const auto flags = object.value("flags").toObject();
    node.enabled = !flags.contains("enabled") || flags.value("enabled").isNull() ||
                   flags.value("enabled").toBool();
    node.focused = flags.value("focused").toBool();
    node.hidden = flags.value("hidden").toBool();
    node.button = flags.value("button").toBool();
    node.text_field = flags.value("textField").toBool();
    node.header = flags.value("header").toBool();
    node.image = flags.value("image").toBool();
    node.slider = flags.value("slider").toBool();
    node.read_only = flags.value("readOnly").toBool();
    next.insert(node.id, node);
  }
  for (auto parent = next.begin(); parent != next.end(); ++parent)
    for (const auto child_id : parent->children)
      if (auto child = next.find(child_id); child != next.end()) child->parent = parent->id;
  for (auto it = accessible_ids_.begin(); it != accessible_ids_.end();) {
    if (next.contains(it.key())) { ++it; continue; }
    QAccessible::deleteAccessibleInterface(it.value());
    it = accessible_ids_.erase(it);
  }
  semantics_ = std::move(next);
  QAccessibleEvent changed(this, QAccessible::ObjectReorder);
  QAccessible::updateAccessibility(&changed);
  if (qEnvironmentVariableIsSet("DOROTI_QT_VALIDATION_ACCESSIBILITY_DUMP")) {
    auto* root = Accessible(0);
    const auto children = QByteArray::number(root == nullptr ? 0 : root->childCount());
    Diagnostic("accessibility.rootChildren", children.constData());
  }
}

QAccessibleInterface* AccessibleFactory(const QString&, QObject* object) {
  auto* surface = dynamic_cast<DorotiSurface*>(object);
  return surface == nullptr || surface->Semantic(0) == nullptr
             ? nullptr : new DorotiAccessibleNode(surface, 0);
}

const doroti_qt_host_api_v2 kHostApi{
    kAbiVersion,
    sizeof(doroti_qt_host_api_v2),
    kSupportedFeatures,
    &DorotiSurface::RequestFrame,
    &DorotiSurface::RequestClose,
    &DorotiSurface::GetGlProcAddress,
    &DorotiSurface::Resize,
    &DorotiSurface::SetClipboardText,
    &DorotiSurface::RequestClipboardText,
    &DorotiSurface::SetCursor,
    &DorotiSurface::SetTextClient,
    &DorotiSurface::UpdateTextState,
    &DorotiSurface::SetCaretRect,
    &DorotiSurface::ClearTextClient,
    &DorotiSurface::UpdateSemantics,
    &DorotiSurface::ClearSemantics,
};

std::int32_t Validate(const doroti_qt_configuration_v2* configuration,
                      const doroti_qt_callbacks_v2* callbacks) {
  if (configuration == nullptr || callbacks == nullptr) return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  if (configuration->abi_version != kAbiVersion || callbacks->abi_version != kAbiVersion)
    return DOROTI_QT_ERROR_ABI_VERSION;
  if (configuration->struct_size < sizeof(doroti_qt_configuration_v2) ||
      callbacks->struct_size < sizeof(doroti_qt_callbacks_v2))
    return DOROTI_QT_ERROR_ABI_SIZE;
  const auto required = configuration->required_features | callbacks->required_features;
  if ((required & ~kSupportedFeatures) != 0) return DOROTI_QT_ERROR_UNSUPPORTED_FEATURE;
  if (callbacks->view_created == nullptr || callbacks->render == nullptr ||
      callbacks->frame_terminal == nullptr || callbacks->surface_destroying == nullptr ||
      callbacks->diagnostic == nullptr || callbacks->fatal == nullptr ||
      callbacks->metrics_changed == nullptr || callbacks->lifecycle_changed == nullptr ||
      callbacks->close_requested == nullptr || callbacks->closed == nullptr ||
      callbacks->pointer == nullptr || callbacks->key == nullptr || callbacks->focus == nullptr ||
      callbacks->text_editing == nullptr || callbacks->text_action == nullptr ||
      callbacks->clipboard_text == nullptr || callbacks->configuration_changed == nullptr ||
      callbacks->semantics_action == nullptr)
    return DOROTI_QT_ERROR_REQUIRED_CALLBACK;
  if (configuration->title.data == nullptr || configuration->title.length == 0 ||
      configuration->logical_width <= 0 || configuration->logical_height <= 0)
    return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  if (configuration->backdrop_mode > DOROTI_QT_BACKDROP_ACRYLIC ||
      configuration->backdrop_fallback > DOROTI_QT_BACKDROP_FALLBACK_SOLID)
    return DOROTI_QT_ERROR_INVALID_ARGUMENT;
  return DOROTI_QT_OK;
}
}  // namespace

extern "C" DOROTI_QT_EXPORT std::int32_t doroti_qt_run_v2(
    const doroti_qt_configuration_v2* configuration,
    const doroti_qt_callbacks_v2* callbacks) {
  try {
    const auto validation = Validate(configuration, callbacks);
    if (validation != DOROTI_QT_OK) return validation;

    int argc = 1;
    char app_name[] = "doroti";
    char* argv[] = {app_name, nullptr};
    QApplication app(argc, argv);
    QAccessible::installFactory(&AccessibleFactory);
    const auto title = QString::fromUtf8(
        reinterpret_cast<const char*>(configuration->title.data),
        static_cast<qsizetype>(configuration->title.length));
    auto* surface = new DorotiSurface(callbacks->callback_context, *callbacks,
                                      configuration->backdrop_mode,
                                      configuration->backdrop_fallback);
    surface->setTitle(title);
    const auto created = callbacks->view_created(
        callbacks->callback_context, surface, &kHostApi);
    if (created != DOROTI_QT_OK) return created;
    surface->resize(configuration->logical_width, configuration->logical_height);
    surface->show();
    surface->InitializeBackdrop();
    bool stress_ok = false;
    const auto stress_cycles = qEnvironmentVariableIntValue(
        "DOROTI_QT_VALIDATION_RESIZE_CYCLES", &stress_ok);
    if (stress_ok && stress_cycles > 0) {
      auto* timer = new QTimer(surface);
      auto* completed = new int(0);
      QObject::connect(timer, &QTimer::timeout, surface,
                       [surface, timer, completed, stress_cycles,
                        base_width = configuration->logical_width,
                        base_height = configuration->logical_height] {
        if (*completed >= stress_cycles) {
          surface->setProperty("doroti.validation.resizeCycles", *completed);
          timer->stop();
          delete completed;
          DorotiSurface::RequestClose(surface);
          return;
        }
        const auto delta = (*completed % 2 == 0) ? 37 : 0;
        surface->resize(base_width + delta, base_height + delta);
        ++*completed;
      });
      timer->start(80);
    }
    const auto result = app.exec();
    delete surface;
    QAccessible::removeFactory(&AccessibleFactory);
    return result;
  } catch (const std::exception&) {
    return DOROTI_QT_ERROR_NATIVE_EXCEPTION;
  } catch (...) {
    return DOROTI_QT_ERROR_NATIVE_EXCEPTION;
  }
}
