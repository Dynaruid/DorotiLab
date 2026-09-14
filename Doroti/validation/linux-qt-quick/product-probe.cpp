// Test-only interposition around the real product event loop. No production
// input/capture hooks, CPU renderer, or native snapshots are used for composition.
#include <QApplication>
#include <QQuickWindow>
#include <QQuickItem>
#include <QPointer>
#include <QTimer>
#include <QTest>
#include <QFile>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <dlfcn.h>
#include <cstdio>
#include <cstdlib>
#include <cmath>
namespace {
QJsonArray checks;
QString output;
void Check(bool ok,const char* name) {
  checks.append(QJsonObject{{"name",name},{"pass",ok}});
  fprintf(stderr,"QUICK TEST %s: %s\n",ok?"PASS":"FAIL",name);
  if(!ok) { QFile file(output+"/checks.json");if(file.open(QIODevice::WriteOnly))file.write(QJsonDocument(checks).toJson());std::exit(1); }
}
QQuickItem* Item(QQuickWindow* window,const char* name) { return window->contentItem()->findChild<QQuickItem*>(QString::fromLatin1(name)); }
void Pixel(const QImage& image,QPoint point,QColor expected,const char* name) {
  auto actual=image.pixelColor(point*image.devicePixelRatio());
  auto near=[](int a,int b){return std::abs(a-b)<=5;};
  if(!near(actual.red(),expected.red())||!near(actual.green(),expected.green())||!near(actual.blue(),expected.blue()))
    fprintf(stderr,"pixel %d,%d actual=%s expected=%s\n",point.x(),point.y(),qPrintable(actual.name()),qPrintable(expected.name()));
  Check(near(actual.red(),expected.red())&&near(actual.green(),expected.green())&&near(actual.blue(),expected.blue()),name);
}
}
int QCoreApplication::exec() {
  output=qEnvironmentVariable("DOROTI_QUICK_EVIDENCE");
  QTimer timer;
  int step=0,scenario=5,nativePresses=0;
  QPointer<QQuickItem> savedEditor;
  bool sample=qEnvironmentVariable("DOROTI_TESTBED_MODE")=="sample";
  QObject::connect(&timer,&QTimer::timeout,qApp,[&] {
    QQuickWindow* window=nullptr;
    for(auto* w:QGuiApplication::topLevelWindows())if(w->title()=="Doroti Material Testbed")window=qobject_cast<QQuickWindow*>(w);
    Check(window!=nullptr,"real Qt Quick product window");
    auto click=[&](QPoint point){QTest::mouseClick(window,Qt::LeftButton,Qt::NoModifier,point);};
    auto* editor=Item(window,"doroti-quick-editor");
    auto* button=Item(window,"doroti-quick-button");
    auto capture=[&](const QString& name){auto image=window->grabWindow();Check(!image.isNull(),"composed GPU capture available");
      Check(image.save(output+"/"+name+".png"),"capture saved");return image;};
    fprintf(stderr,"QUICK TEST step=%d scenario=%d\n",step,scenario);
    if(sample) {
      switch(step) {
        case 0:click({40,294});break;
        case 1:Check(editor&&button,"sample tab creates Quick controls");capture("sample-tab");savedEditor=editor;
          click({40,window->height()-80});break; // enabled seed menu with live native content
        case 2:Check(savedEditor&&savedEditor->isVisible(),"popup keeps native instance live");capture("sample-popup");click({1000,500});break;
        case 3:Check(Item(window,"doroti-quick-editor")==savedEditor,"popup dismiss preserves native identity");click({40,162});break;
        case 4:Check(!Item(window,"doroti-quick-editor"),"navigation disposes native controls");click({40,294});break;
        case 5:Check(editor&&button,"navigation remounts Quick controls");window->resize(800,700);break;
        case 6:capture("sample-narrow");Check(editor&&editor->isVisible(),"narrow sample keeps native content");step=99;break;
      }
    } else {
      if(step==0) {
        Check(editor&&button,"interleaved fixture creates Quick controls");savedEditor=editor;
        click({40,160});
      } else if(step==1) {
        Check(button->property("clickCount").toInt()==1,"exposed native button receives one click");
        click(editor->mapToScene(QPointF(190,30)).toPoint());
        QTest::keyClick(window,Qt::Key_End);QTest::keyClick(window,Qt::Key_X);
      } else if(step==2) {
        Check(editor->property("text").toString().endsWith('x'),"native keyboard input through Qt window");
        auto image=capture("interleaved");
        Pixel(image,{140,190},QColor(0,170,85),"middle Doroti raster above native A");
        auto color=image.pixelColor(QPoint(200,210)*image.devicePixelRatio());
        Check(color.red()<80&&color.green()<80,"native B above middle Doroti raster");
        auto under=image.pixelColor(QPoint(370,270)*image.devicePixelRatio());
        Pixel(image,{300,270},QColor((255*153+under.red()*102+127)/255,
            (51*153+under.green()*102+127)/255,(under.blue()*102+127)/255),"foreground alpha blends with live native pixels");
        nativePresses=editor->property("pressCount").toInt();click({260,250});
      } else if(step==3) {
        Check(editor->property("pressCount").toInt()==nativePresses,"foreground shield blocks native press");click({240,104});scenario=6;
      } else if(step==4) {
        capture("translucent");click({240,104});scenario=7;
      } else if(step==5) {
        click({260,250});
      } else if(step==6) {
        Check(editor->property("pressCount").toInt()==nativePresses+1,"shield-off delivers exactly one native press");click({240,104});scenario=8;
      } else if(step>=7&&step<=13) {
        Check(editor==savedEditor&&editor->property("text").toString().endsWith('x'),"scenario keeps same edited native instance");
        auto image=capture(QString("scenario-%1").arg(scenario));
        if(scenario==9)Check(std::abs(editor->mapToScene(QPointF()).x()-210)<1,"same native editor moves with widget layout");
        if(scenario==2) {
          Pixel(image,{60,160},QColor(255,51,0),"full Doroti cover above native A");
          Pixel(image,{200,240},QColor(255,51,0),"full Doroti cover above native B");
          Check(editor->isVisible()&&button->isVisible(),"full cover does not hide native controls");
        }
        click({240,104});scenario=(scenario+1)%10;
      } else if(step==14) {
        click({380,104}); // Dispose controls.
      } else if(step==15) {
        Check(savedEditor.isNull()&&!editor&&!button,"dispose releases Quick native instances");click({380,104});
      } else if(step==16) {
        Check(editor&&button,"recreate controls after disposal");savedEditor=editor;window->resize(800,700);
      } else if(step==17) {
        Check(editor==savedEditor,"resize keeps native identity");capture("resize");
        click({500,104}); // Doroti modal above live native content.
      } else if(step==18) {
        Check(editor==savedEditor&&editor->isVisible(),"modal keeps native content alive");capture("modal");
        click({10,400});
      } else if(step==19) { Check(editor==savedEditor,"modal dismissal keeps identity");step=99; }
    }
    if(step==99) {
      QFile file(output+"/checks.json");Check(file.open(QIODevice::WriteOnly),"checks file");file.write(QJsonDocument(checks).toJson());file.close();
      fprintf(stderr,"QUICK PRODUCT PASS\n");timer.stop();window->close();
    }
    ++step;
  });
  timer.start(1200);
  auto real=reinterpret_cast<int(*)()>(dlsym(RTLD_NEXT,"_ZN16QCoreApplication4execEv"));
  Check(real!=nullptr,"Qt event loop entry");return real();
}
