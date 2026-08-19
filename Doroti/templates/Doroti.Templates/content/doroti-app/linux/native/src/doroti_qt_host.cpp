#include <QApplication>
#include <QCloseEvent>
#include <QElapsedTimer>
#include <QFocusEvent>
#include <QGuiApplication>
#include <QInputMethodEvent>
#include <QKeyEvent>
#include <QMouseEvent>
#include <QOpenGLWidget>
#include <QResizeEvent>
#include <QScreen>
#include <QTimer>
#include <QVBoxLayout>
#include <QWidget>
#include <cstdint>

#if defined(_WIN32)
#define DOROTI_EXPORT __declspec(dllexport)
#else
#define DOROTI_EXPORT __attribute__((visibility("default")))
#endif

extern "C" {
struct doroti_qt_callbacks_v1 {
  void (*frame)(void* context, double timestamp_seconds);
  void (*resize)(void* context, int pixel_width, int pixel_height, double scale, std::int64_t surface_generation);
  void (*lifecycle)(void* context, int state);
  void (*pointer)(void* context, int kind, double x, double y, int buttons, std::int64_t timestamp_microseconds);
  void (*key)(void* context, int key, int scan_code, int modifiers, std::int64_t timestamp_microseconds);
  void (*text)(void* context, const char* utf8_text);
  void (*surface_changed)(void* context, std::int64_t surface_generation);
};

DOROTI_EXPORT int doroti_qt_run(const char* title, int width, int height,
                                void* managed_context, const doroti_qt_callbacks_v1* callbacks);
}

class DorotiSurface final : public QOpenGLWidget {
 public:
  DorotiSurface(void* context, const doroti_qt_callbacks_v1& callbacks)
      : context_(context), callbacks_(callbacks) {
    setFocusPolicy(Qt::StrongFocus);
    setAttribute(Qt::WA_InputMethodEnabled, true);
    clock_.start();
  }

 protected:
  void initializeGL() override {
    ++surface_generation_;
    callbacks_.surface_changed(context_, surface_generation_);
  }

  void paintGL() override {
    // The managed renderer will acquire the current Qt OpenGL surface in the next vertical slice.
    callbacks_.frame(context_, clock_.nsecsElapsed() / 1'000'000'000.0);
  }

  void resizeGL(int width, int height) override {
    const auto scale = devicePixelRatioF();
    callbacks_.resize(context_, static_cast<int>(width * scale), static_cast<int>(height * scale), scale, surface_generation_);
  }

  void mousePressEvent(QMouseEvent* event) override { sendPointer(0, event); }
  void mouseMoveEvent(QMouseEvent* event) override { sendPointer(1, event); }
  void mouseReleaseEvent(QMouseEvent* event) override { sendPointer(2, event); }

  void keyPressEvent(QKeyEvent* event) override {
    callbacks_.key(context_, event->key(), static_cast<int>(event->nativeScanCode()), event->modifiers(), micros());
    const auto text = event->text().toUtf8();
    if (!text.isEmpty()) callbacks_.text(context_, text.constData());
  }

  void keyReleaseEvent(QKeyEvent* event) override {
    callbacks_.key(context_, -event->key(), static_cast<int>(event->nativeScanCode()), event->modifiers(), micros());
  }

  void inputMethodEvent(QInputMethodEvent* event) override {
    const auto text = event->commitString().toUtf8();
    if (!text.isEmpty()) callbacks_.text(context_, text.constData());
    QOpenGLWidget::inputMethodEvent(event);
  }

  void focusInEvent(QFocusEvent* event) override {
    callbacks_.lifecycle(context_, 1);
    QOpenGLWidget::focusInEvent(event);
  }

  void focusOutEvent(QFocusEvent* event) override {
    callbacks_.lifecycle(context_, 2);
    QOpenGLWidget::focusOutEvent(event);
  }

 private:
  std::int64_t micros() const { return clock_.nsecsElapsed() / 1000; }

  void sendPointer(int kind, QMouseEvent* event) {
    callbacks_.pointer(context_, kind, event->position().x(), event->position().y(), event->buttons(), micros());
  }

  void* context_;
  doroti_qt_callbacks_v1 callbacks_;
  QElapsedTimer clock_;
  std::int64_t surface_generation_ = 0;
};

extern "C" DOROTI_EXPORT int doroti_qt_run(const char* title, int width, int height,
                                            void* managed_context, const doroti_qt_callbacks_v1* callbacks) {
  if (callbacks == nullptr || callbacks->frame == nullptr || callbacks->resize == nullptr) return 64;
  int argc = 1;
  char app_name[] = "doroti";
  char* argv[] = {app_name, nullptr};
  QApplication app(argc, argv);
  QWidget window;
  window.setWindowTitle(QString::fromUtf8(title));
  auto* layout = new QVBoxLayout(&window);
  layout->setContentsMargins(0, 0, 0, 0);
  layout->addWidget(new DorotiSurface(managed_context, *callbacks));
  window.resize(width, height);
  window.show();
  callbacks->lifecycle(managed_context, 0);
  const auto exit_code = app.exec();
  callbacks->lifecycle(managed_context, 3);
  return exit_code;
}
