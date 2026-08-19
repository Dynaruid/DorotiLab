#include "doroti_qt_host_v2.h"

#include <QApplication>
#include <QElapsedTimer>
#include <QMetaObject>
#include <QOpenGLContext>
#include <QOpenGLFunctions>
#include <QOpenGLWidget>
#include <QSurfaceFormat>
#include <QVBoxLayout>
#include <QWidget>
#include <algorithm>
#include <exception>
#include <string>
#include <utility>

namespace {
constexpr std::uint32_t kAbiVersion = 2;
constexpr std::uint64_t kSupportedFeatures =
    DOROTI_QT_FEATURE_OPENGL_FBO | DOROTI_QT_FEATURE_SWAP_ACK |
    DOROTI_QT_FEATURE_CONTEXT_LIFETIME |
    DOROTI_QT_FEATURE_LENGTH_PREFIXED_UTF8;
constexpr std::uint32_t kGlRgba8 = 0x8058;

doroti_qt_utf8_v2 Utf8(const char* value) {
  const auto length = value == nullptr ? 0 : std::char_traits<char>::length(value);
  return {reinterpret_cast<const std::uint8_t*>(value), length};
}

class DorotiSurface final : public QOpenGLWidget {
 public:
  DorotiSurface(void* callback_context, const doroti_qt_callbacks_v2& callbacks)
      : callback_context_(callback_context), callbacks_(callbacks) {
    setFocusPolicy(Qt::StrongFocus);
    setAttribute(Qt::WA_InputMethodEnabled, true);
    clock_.start();
    connect(this, &QOpenGLWidget::frameSwapped, this, [this] { FrameSwapped(); });
  }

  ~DorotiSurface() override { ReleaseSurface(); }

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
      if (auto* window = surface->window()) window->close();
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
    if (current != nullptr) {
      auto* functions = current->functions();
      Diagnostic("gl.vendor", reinterpret_cast<const char*>(functions->glGetString(GL_VENDOR)));
      const auto* renderer = reinterpret_cast<const char*>(functions->glGetString(GL_RENDERER));
      Diagnostic("gl.renderer", renderer);
      Diagnostic("gl.version", reinterpret_cast<const char*>(functions->glGetString(GL_VERSION)));
      const auto normalized = QByteArray(renderer == nullptr ? "" : renderer).toLower();
      software_renderer_ = normalized.contains("llvmpipe") || normalized.contains("softpipe") ||
                           normalized.contains("swiftshader");
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
                       Utf8("Qt OpenGL software renderer is not accepted by the Linux GPU backend"));
      RequestClose(this);
      return;
    }
    const auto result = callbacks_.render(callback_context_, this, &descriptor, token);
    if (result != DOROTI_QT_OK) {
      fatal_ = true;
      Terminal(token, DOROTI_QT_TERMINAL_FAILED, surface_generation_);
      callbacks_.fatal(callback_context_, result, Utf8("managed render callback failed"));
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

 private:
  std::int64_t Micros() const { return clock_.nsecsElapsed() / 1000; }

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
  bool software_renderer_ = false;
  bool fatal_ = false;
  bool closing_ = false;
};

const doroti_qt_host_api_v2 kHostApi{
    kAbiVersion,
    sizeof(doroti_qt_host_api_v2),
    kSupportedFeatures,
    &DorotiSurface::RequestFrame,
    &DorotiSurface::RequestClose,
    &DorotiSurface::GetGlProcAddress,
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
      callbacks->diagnostic == nullptr || callbacks->fatal == nullptr)
    return DOROTI_QT_ERROR_REQUIRED_CALLBACK;
  if (configuration->title.data == nullptr || configuration->title.length == 0 ||
      configuration->logical_width <= 0 || configuration->logical_height <= 0)
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
    QWidget window;
    const auto title = QString::fromUtf8(
        reinterpret_cast<const char*>(configuration->title.data),
        static_cast<qsizetype>(configuration->title.length));
    window.setWindowTitle(title);
    auto* layout = new QVBoxLayout(&window);
    layout->setContentsMargins(0, 0, 0, 0);
    auto* surface = new DorotiSurface(callbacks->callback_context, *callbacks);
    layout->addWidget(surface);
    const auto created = callbacks->view_created(
        callbacks->callback_context, surface, &kHostApi);
    if (created != DOROTI_QT_OK) return created;
    window.resize(configuration->logical_width, configuration->logical_height);
    window.show();
    return app.exec();
  } catch (const std::exception&) {
    return DOROTI_QT_ERROR_NATIVE_EXCEPTION;
  } catch (...) {
    return DOROTI_QT_ERROR_NATIVE_EXCEPTION;
  }
}
