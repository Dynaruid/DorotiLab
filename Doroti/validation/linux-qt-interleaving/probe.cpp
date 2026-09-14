// Topology experiment, not the Doroti product presenter. Raster inputs are synthetic
// premultiplied images; live Qt controls/WebEngine never use snapshots or hide-on-cover.
#include <QApplication>
#include <QColorSpace>
#include <QDir>
#include <QElapsedTimer>
#include <QFile>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QLineEdit>
#include <QMouseEvent>
#include <QPainter>
#include <QPushButton>
#include <QScreen>
#include <QTest>
#include <QTimer>
#include <QWindow>
#ifdef DOROTI_PROBE_WEBENGINE
#include <QWebEngineView>
#endif
#include <algorithm>
#include <bit>
#include <cstdio>
#include <functional>
#include <stdexcept>
#include <X11/Xlib.h>
#include <X11/Xutil.h>
#include <X11/extensions/Xcomposite.h>

namespace {
const QColor Background(12,24,48), Blue(32,96,224), Green(16,176,64), Yellow(240,192,32), Red(240,32,48);
const QRect A(40,80,300,160), B(220,150,300,160), Middle(120,120,240,110), Front(300,190,180,120);

// Read the compositor's redirected window buffer. QScreen::grabWindow reads black
// for ordinary redirected QWidget windows on this XWayland compositor.
// This never invokes QWidget::render/grab or captures native controls separately.
QImage RedirectedCapture(WId window) {
  auto* display = XOpenDisplay(nullptr);
  if (!display) return {};
  static int error = 0;
  XSync(display, False);
  error = 0;
  auto oldHandler = XSetErrorHandler([](Display*, XErrorEvent*) { error = 1; return 0; });
  auto pixmap = XCompositeNameWindowPixmap(display, window);
  XSync(display, False);
  QImage result;
  if (!error) {
    XWindowAttributes attributes{};
    XGetWindowAttributes(display,window,&attributes);
    Window root; int x,y; unsigned width,height,border,depth;
    if (XGetGeometry(display,pixmap,&root,&x,&y,&width,&height,&border,&depth)) {
      auto* image = XGetImage(display,pixmap,0,0,width,height,AllPlanes,ZPixmap);
      if (image) {
        result = QImage(width,height,QImage::Format_RGB32);
        const auto* visual = attributes.visual;
        auto channel = [](unsigned long pixel, unsigned long mask) {
          if (!mask) return 0;
          const auto shift = std::countr_zero(mask);
          return int(((pixel & mask) >> shift) * 255 / (mask >> shift));
        };
        for (unsigned row=0;row<height;++row) for (unsigned column=0;column<width;++column) {
          auto pixel=XGetPixel(image,column,row);
          result.setPixel(column,row,qRgb(channel(pixel,visual->red_mask),
              channel(pixel,visual->green_mask),channel(pixel,visual->blue_mask)));
        }
        XDestroyImage(image);
      }
    }
    XFreePixmap(display,pixmap);
  }
  XSync(display,False);
  XSetErrorHandler(oldHandler);
  XCloseDisplay(display);
  return result;
}

// One ordinary QWidget per raster segment. In a product bridge these immutable
// pixels would arrive after successful Graphite readback, not from QPainter.
class RasterLayer final : public QWidget {
 public:
  QImage image;
  explicit RasterLayer(QWidget* parent) : QWidget(parent) {
    setAttribute(Qt::WA_TransparentForMouseEvents);
    setAttribute(Qt::WA_NoSystemBackground);
  }
  void Set(QSize size, QRect rect, QColor color) {
    image = QImage(size, QImage::Format_ARGB32_Premultiplied);
    image.fill(Qt::transparent);
    QPainter painter(&image);
    painter.fillRect(rect, color);
    painter.end();
    resize(size);
    update();
  }
 protected:
  void paintEvent(QPaintEvent*) override { QPainter(this).drawImage(QPoint(), image); }
};
class Shield final : public QWidget {
 public:
  int clicks = 0;
  explicit Shield(QWidget* parent) : QWidget(parent) { setAttribute(Qt::WA_NoSystemBackground); }
 protected:
  void paintEvent(QPaintEvent*) override {}
  void mousePressEvent(QMouseEvent* e) override { e->accept(); }
  void mouseReleaseEvent(QMouseEvent* e) override { ++clicks; e->accept(); }
};
struct Probe {
  QWidget shell;
  RasterLayer background{&shell}, middle{&shell}, foreground{&shell};
  Shield shield{&shell};
  QWidget *a, *b;
#ifndef DOROTI_PROBE_WEBENGINE
  QPushButton button{"Live QPushButton", &shell};
  QLineEdit editor{"live-state", &shell};
#else
  QWebEngineView first{&shell}, second{&shell};
  int loads = 0, jsVerified = 0;
#endif
  QJsonArray checks, captures;
  QString output;
  QTimer timer;
  bool valid = true;
  int stage = 0, buttonClicks = 0, pixelChecks = 0, pixelFailures = 0;
  QElapsedTimer elapsed;
  explicit Probe(QString directory) : output(std::move(directory)) {
    shell.setWindowTitle("Doroti Qt interleaving topology spike");
    shell.setWindowFlag(Qt::FramelessWindowHint);
    shell.resize(640,400);
#ifndef DOROTI_PROBE_WEBENGINE
    a = &button; b = &editor;
    button.setStyleSheet("QPushButton {background:rgb(32,96,224);color:white;border:0;border-radius:0;}");
    editor.setStyleSheet("QLineEdit {background:rgb(240,192,32);color:black;border:0;border-radius:0;}");
    QObject::connect(&button, &QPushButton::clicked, [&] { ++buttonClicks; });
#else
    a = &first; b = &second;
    auto setup = [&](QWebEngineView& web, const char* color) {
      QObject::connect(&web, &QWebEngineView::loadFinished, &shell, [&](bool ok) {
        Check(ok, "WebEngine load");
        ++loads;
      });
      web.setHtml(QString("<style>html,body{margin:0;background:%1}input{position:absolute;left:8px;top:8px;width:120px}</style>"
                          "<input id='editor' value='live-state'><script>window.ticks=0;window.clicks=0;"
                          "setInterval(()=>++window.ticks,50);document.onclick=()=>++window.clicks;</script>").arg(color));
    };
    setup(first, "rgb(32,96,224)"); setup(second, "rgb(240,192,32)");
#endif
    QObject::connect(&timer, &QTimer::timeout, &shell, [&] {
      try { Tick(); } catch (const std::exception& error) { Check(false, error.what()); Finish(); }
    });
    Baseline();
    shell.show();
    elapsed.start();
    timer.start(600);
  }
  void Check(bool ok, const char* name) {
    valid &= ok;
    checks.append(QJsonObject{{"name", name}, {"pass", ok}, {"stage", stage}});
    std::printf("%s: %s\n", ok ? "PASS" : "FAIL", name);
  }
  void Baseline() {
    background.Set(shell.size(), shell.rect(), Background);
    middle.Set(shell.size(), Middle, Green);
    auto alpha = Red; alpha.setAlpha(128);
    foreground.Set(shell.size(), Front, alpha);
    a->setGeometry(A); b->setGeometry(B);
    // Every part is a sibling. No WA_NativeWindow, winId(), QWindow container,
    // WA_AlwaysStackOnTop or reparenting on any child.
    for (auto* layer : {static_cast<QWidget*>(&background), a, static_cast<QWidget*>(&middle), b,
                        static_cast<QWidget*>(&foreground), static_cast<QWidget*>(&shield)}) {
      layer->show(); layer->raise();
    }
    shield.setGeometry(Front);
  }
  QImage Capture(const char* name) {
    auto image = QGuiApplication::platformName()=="xcb" ? RedirectedCapture(shell.winId())
        : shell.screen()->grabWindow(shell.winId()).toImage();
    bool available = !image.isNull();
    auto file = QString::fromLatin1(name) + ".png";
    if (available) available = image.save(output + "/" + file);
    captures.append(QJsonObject{{"name", name}, {"available", available}, {"file", available ? file : QString()}});
    if (image.colorSpace().isValid()) image.convertToColorSpace(QColorSpace::SRgb);
    return image;
  }
  void Pixel(const QImage& image, QPoint point, QColor expected, const char* name) {
    if (image.isNull()) return; // Recorded as unverified, not a pixel pass.
    ++pixelChecks;
    auto actual = image.pixelColor(point * shell.devicePixelRatioF());
    bool ok = std::abs(actual.red()-expected.red()) <= 5 && std::abs(actual.green()-expected.green()) <= 5 &&
              std::abs(actual.blue()-expected.blue()) <= 5;
    if (!ok) std::printf("pixel (%d,%d): actual=%s expected=%s\n", point.x(),point.y(),qPrintable(actual.name()),qPrintable(expected.name()));
    if (!ok) ++pixelFailures;
    Check(ok, name);
  }
  QColor Blend(QColor under) {
    return QColor((Red.red()*128+under.red()*127+127)/255,
                  (Red.green()*128+under.green()*127+127)/255,
                  (Red.blue()*128+under.blue()*127+127)/255);
  }
  void Click(QPoint point, QWidget* expected, const char* label) {
    auto* target = shell.childAt(point);
    Check(target && (target == expected || expected->isAncestorOf(target)), label);
    // Resolve through Qt hit testing, not a direct event to a preselected native control.
    if (target) QTest::mouseClick(target, Qt::LeftButton, Qt::NoModifier, target->mapFrom(&shell,point));
  }
  void Tick() {
#ifdef DOROTI_PROBE_WEBENGINE
    if (loads < 2) { if (elapsed.elapsed() > 30000) { Check(false,"WebEngine load timeout"); Finish(); } return; }
#endif
    switch (stage) {
      case 0: {
        auto image=Capture("interleaved");
        Check(a->isVisible() && b->isVisible(), "native controls remain live");
        Check(!a->testAttribute(Qt::WA_NativeWindow) && !b->testAttribute(Qt::WA_NativeWindow), "no native child windows");
        Pixel(image,{20,20},Background,"R0 background");
        Pixel(image,{60,140},Blue,"N0 above R0");
        Pixel(image,{140,140},Green,"R1 above N0");
        Pixel(image,{240,210},Yellow,"N1 above R1");
        Pixel(image,{400,250},Blend(Yellow),"R2 alpha above N1");
        Click({400,250},&shield,"shield selects foreground");
        Check(shield.clicks==1,"foreground receives exactly one click");
        shield.hide();
        break;
      }
      case 1:
        Click({400,250},b,"shield-off selects native through raster");
        Click({60,140},a,"exposed native receives input");
#ifndef DOROTI_PROBE_WEBENGINE
        Check(buttonClicks==1,"native button receives exactly one click");
        editor.setFocus(); editor.selectAll(); QTest::keyClicks(&editor,"edited-live-state");
        Check(editor.text()=="edited-live-state","native editor text input");
#else
        for (auto* web : {&first,&second})
          web->page()->runJavaScript("document.getElementById('editor').value='edited-live-state'");
#endif
        foreground.Set(shell.size(),shell.rect(),Red); foreground.raise();
        break;
      case 2: {
        auto image=Capture("full-cover");
        Pixel(image,{60,140},Red,"C1 full cover native A");
        Pixel(image,{240,210},Red,"C1 full cover native B");
        Check(a->isVisible() && b->isVisible(),"full cover does not hide native instances");
#ifdef DOROTI_PROBE_WEBENGINE
        for (auto* web : {&first,&second}) web->page()->runJavaScript(
            "[document.getElementById('editor').value,window.ticks,window.clicks]", [&] (const QVariant& value) {
              auto values=value.toList();
              Check(values.size()==3 && values[0].toString()=="edited-live-state" && values[1].toInt()>10 && values[2].toInt()==1,
                    "covered WebEngine retains DOM, runs timers, received one click"); ++jsVerified;
            });
#endif
        Baseline();
        break;
      }
      case 3: {
        auto image=Capture("restored");
        Pixel(image,{240,210},Yellow,"C1 restore same native instance");
#ifndef DOROTI_PROBE_WEBENGINE
        Check(editor.text()=="edited-live-state","cover/restore preserves edited text");
#else
        if(jsVerified<2) return;
#endif
        middle.raise(); // Dynamically put the same raster above both native views.
        foreground.raise(); shield.raise();
        break;
      }
      case 4: {
        auto image=Capture("reverse-order");
        Pixel(image,{240,210},Green,"C5 reverse raster/native order");
        Baseline();
        b->move(300,150);
        shell.resize(700,440);
        background.Set(shell.size(),shell.rect(),Background);
        middle.Set(shell.size(),Middle,Green);
        { auto alpha=Red; alpha.setAlpha(128); foreground.Set(shell.size(),Front,alpha); }
        break;
      }
      case 5: {
        auto image=Capture("move-resize");
        Pixel(image,{240,210},Green,"C5 moved editor uncovers middle raster");
        Pixel(image,{500,210},Yellow,"C5 same editor moved");
        // Rectangular clipping without recreating native content.
        b->setMask(QRegion(QRect(0,0,100,160)));
        break;
      }
      case 6: {
        auto image=Capture("clip");
        Pixel(image,{500,210},Background,"rect clip reveals underlying background");
        b->clearMask();
        Check(a->parentWidget()==&shell && b->parentWidget()==&shell,"same owner after all mutations");
        Finish(); return;
      }
    }
    ++stage;
  }
  void Finish() {
    timer.stop();
    QJsonObject report{{"schema", "doroti.qt-interleaving-topology/v1"}, {"automatedChecksPassed",valid},
        {"qt",qVersion()}, {"qpa",QGuiApplication::platformName()}, {"dpr",shell.devicePixelRatioF()},
        {"pixelChecks",pixelChecks}, {"pixelFailures",pixelFailures},
        {"visualStatus",pixelFailures ? "FAIL" : pixelChecks==12 ? "PASS" : "notVerified"},
        {"captureSource",QGuiApplication::platformName()=="xcb" ? "XComposite redirected window pixmap" : "QScreen window capture"},
#ifdef DOROTI_PROBE_WEBENGINE
        {"nativeKind","QWebEngineView x2"},
#else
        {"nativeKind","QPushButton/QLineEdit"},
#endif
        {"rasterProducer","synthetic-QImage; not Doroti/Graphite"}, {"productIntegrated",false},
        {"physicalInputVerified",false}, {"scanoutVerified",false}, {"checks",checks}, {"captures",captures}};
    QFile file(output+"/result.json");
    if(file.open(QIODevice::WriteOnly)) file.write(QJsonDocument(report).toJson());
    shell.close();
    qApp->exit(valid?0:1);
  }
};
}
int main(int argc,char** argv) {
  setvbuf(stdout,nullptr,_IONBF,0);
  if(argc!=2) return 2;
  QApplication app(argc,argv);
  QDir().mkpath(argv[1]);
  Probe probe(QString::fromLocal8Bit(argv[1]));
  return app.exec();
}
