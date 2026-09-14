// Loaded only by sample-platform-views.py into the real Qt product process.
// Synthetic Qt input is not physical input qualification.
#include <QApplication>
#include <QLineEdit>
#include <QPointer>
#include <QPushButton>
#include <QScreen>
#include <QTest>
#include <QTimer>
#include <QWindow>
#include <cstdio>
#include <cstdlib>
#include <dlfcn.h>

static void Require(bool condition, const char* message) {
  if (!condition) {
    fprintf(stderr, "PROBE FAIL: %s\n", message);
    std::exit(1);
  }
}

static QLineEdit* Controls(int count) {
  int editors = 0, buttons = 0;
  QLineEdit* editor = nullptr;
  for (auto* widget : QApplication::allWidgets()) {
    if (!widget->isVisible()) continue;
    if (auto* value = qobject_cast<QLineEdit*>(widget)) {
      editor = value;
      ++editors;
    }
    if (qobject_cast<QPushButton*>(widget)) ++buttons;
  }
  Require(editors == count && buttons == count, "visible native control count");
  return editor;
}

// Interpose only the event-loop entry; the app still creates its real host,
// factories, renderer and widgets. No validation hooks enter production code.
int QCoreApplication::exec() {
  QTimer timer;
  int stage = 0;
  QPointer<QLineEdit> saved;
  QObject::connect(&timer, &QTimer::timeout, qApp, [&] {
    QWindow* window = nullptr;
    for (auto* candidate : QGuiApplication::topLevelWindows())
      if (candidate->title() == "Doroti Material Testbed") window = candidate;
    if (!window) return;
    fprintf(stderr, "PROBE stage=%d window=%dx%d\n", stage, window->width(), window->height());
    auto click = [window](int x, int y) {
      QTest::mouseClick(window, Qt::LeftButton, Qt::NoModifier, QPoint(x, y));
    };
    auto capture = [window](const char* name) {
      // Wayland may refuse window capture; never substitute a raster-only image.
      window->screen()->grabWindow(window->winId()).save(
          qEnvironmentVariable("DOROTI_QT_SAMPLE_EVIDENCE") + "/" + name + ".png");
    };
    switch (stage) {
      case 0:
        click(40, 294); // Platform views navigation destination.
        break;
      case 1: {
        auto* editor = Controls(1);
        saved = editor;
        QTest::mouseClick(editor, Qt::LeftButton);
        editor->selectAll();
        QTest::keyClicks(editor, "native input verified");
        Require(editor->text() == "native input verified", "native input");
        for (auto* widget : QApplication::allWidgets()) {
          if (auto* button = qobject_cast<QPushButton*>(widget)) {
            int clicks = 0;
            auto connection = QObject::connect(button, &QPushButton::clicked, [&] { ++clicks; });
            QTest::mouseClick(button, Qt::LeftButton);
            QObject::disconnect(connection);
            Require(clicks == 1, "native button click");
          }
        }
        capture("tab");
        click(680, 208); // Dispose controls.
        break;
      }
      case 2:
        Controls(0);
        Require(saved.isNull(), "dispose releases editor");
        click(680, 208); // Recreate controls.
        break;
      case 3:
        Controls(1);
        click(40, 162); // Color destination.
        break;
      case 4:
        Controls(0);
        click(40, 294); // Remount with fresh instance identities.
        break;
      case 5:
        saved = Controls(1);
        saved->setText("preserved");
        window->resize(800, 700); // Cross the navigation rail/bar breakpoint.
        break;
      case 6:
        Require(Controls(1) == saved && saved->text() == "preserved", "resize preserves editor");
        capture("narrow");
        window->resize(1280, 800);
        break;
      case 7:
        Controls(1);
        click(40, window->height() - 80); // Unsupported popup is disabled.
        break;
      case 8:
        Controls(1);
        QTest::mouseMove(window, QPoint(40, window->height() - 160)); // Tooltip guard.
        break;
      case 9:
        Require(Controls(1) == saved && saved->text() == "preserved",
                "disabled menu and tooltip preserve editor");
        fprintf(stderr, "PROBE PASS\n");
        timer.stop();
        window->close();
        break;
    }
    ++stage;
  });
  timer.start(2000);
  auto realExec = reinterpret_cast<int (*)()>(dlsym(RTLD_NEXT, "_ZN16QCoreApplication4execEv"));
  Require(realExec != nullptr, "Qt event-loop entry");
  return realExec();
}
