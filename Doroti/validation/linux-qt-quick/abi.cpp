#include "doroti_qt_host_v2.h"
#include "doroti_qt_quick.h"
#include <QApplication>
#include <QQuickWindow>
#include <cstdio>
#include <stdexcept>
void Check(bool value,const char* name) { if(!value)throw std::runtime_error(name);std::printf("PASS: %s\n",name); }
int main(int argc,char** argv) {
  doroti_qt_configuration_v2 config{};config.abi_version=4;config.struct_size=sizeof(config);
  doroti_qt_callbacks_v2 callbacks{};callbacks.abi_version=4;callbacks.struct_size=sizeof(callbacks);
  Check(doroti_qt_run_v2(&config,&callbacks)==DOROTI_QT_ERROR_UNSUPPORTED_FEATURE,"old managed Quick negotiation rejected before startup");
  callbacks.feature_bits=DOROTI_QT_FEATURE_QUICK_COMPOSITION;
  Check(doroti_qt_run_v2(&config,&callbacks)==DOROTI_QT_ERROR_REQUIRED_CALLBACK,"Quick opt-in still requires callback table");
  QApplication app(argc,argv);
  QQuickWindow window;window.resize(400,300);DorotiQtRegisterPlatformOwner(&window);
  doroti_qt_pv_api api{};std::uint64_t owner=0,a=0,b=0;
  Check(doroti_qt_get_platform_views(&window,1,sizeof(api),&owner,&api)==0&&api.feature_bits==3,"Quick attachment ABI");
  Check(api.create(owner,0,{},nullptr,nullptr,&a)==0&&api.create(owner,1,{},nullptr,nullptr,&b)==0,"live QML factories");
  Check(api.adopt_widget(owner,nullptr,nullptr,nullptr,&a)==DOROTI_QT_PV_UNSUPPORTED,"arbitrary QWidget adoption rejected");
  doroti_qt_quick_part parts[2]{};
  parts[0]={sizeof(parts[0]),1,a,0,0,0,{20,20,100,100},{20,20,100,100}};
  parts[1]={sizeof(parts[1]),1,b,0,0,0,{50,50,100,100},{50,50,100,100}};
  Check(doroti_qt_quick_commit(&window,parts,2,0)==0,"overlapping live native parts accepted");
  parts[1].id=a;
  Check(doroti_qt_quick_commit(&window,parts,2,0)==DOROTI_QT_PV_STALE,"duplicate attachment rejected");
  parts[1].id=b;parts[1].bounds.width=-1;
  Check(doroti_qt_quick_commit(&window,parts,2,0)==DOROTI_QT_ERROR_INVALID_ARGUMENT,"invalid geometry rejected");
  parts[1].bounds.width=100;parts[1].size--;
  Check(doroti_qt_quick_commit(&window,parts,2,0)==DOROTI_QT_ERROR_INVALID_ARGUMENT,"part ABI size rejected");
  int cancelled=0;
  for(int i=0;i<20;i++)Check(api.post(owner,[](void* p,int status){if(status==DOROTI_QT_PV_CLOSED)++*static_cast<int*>(p);},&cancelled)==0,"queued owner work accepted");
  DorotiQtClosePlatformOwner(&window);
  Check(cancelled==20,"close terminates every accepted queued callback");
  Check(api.remove(owner,b)==DOROTI_QT_PV_CLOSED,"late owner use rejected");
}
