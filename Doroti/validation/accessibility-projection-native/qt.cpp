// Compile the production native provider in this test translation unit. This
// exercises its actual QAccessible interfaces without a managed app or GPU draw.
#include "../../../DorotiTestbedApp/linux/native/src/doroti_qt_host.cpp"
#include <iostream>
#include <stdexcept>

int main(int argc, char** argv) {
  QGuiApplication app(argc, argv);
  std::vector<std::pair<int64_t, int64_t>> actions;
  doroti_qt_callbacks_v2 callbacks{};
  callbacks.semantics_action = [](void* context, void*, int64_t id, int64_t action, doroti_qt_utf8_v2) {
    static_cast<decltype(actions)*>(context)->emplace_back(id, action);
  };
  callbacks.lifecycle_changed = [](void*, void*, uint32_t, int64_t) {};
  callbacks.closed = [](void*, void*) {};
  DorotiSurface surface(&actions, callbacks, DOROTI_QT_BACKDROP_SOLID, DOROTI_QT_BACKDROP_FALLBACK_SOLID);
  const auto project = [&](bool enabled, bool readOnly, bool include = true) {
    const auto node = QString(R"({"id":1,"rect":[0,0,200,40],"label":"Action","value":"5","actions":65,"flags":{"enabled":%1,"readOnly":%2,"slider":true}})")
        .arg(enabled ? "true" : "false", readOnly ? "true" : "false");
    surface.ApplySemantics((QString("{\"nodes\":[") + (include ? node : QString()) + "]}").toUtf8());
  };
  project(true, false); project(true, false);
  if (!actions.empty()) throw std::runtime_error("Qt projection emitted input");
  DorotiAccessibleNode provider(&surface, 1);
  provider.doAction(QAccessibleActionInterface::pressAction());
  provider.doAction(QAccessibleActionInterface::increaseAction());
  if (actions != decltype(actions){{1, 1}, {1, 1ll << 6}}) throw std::runtime_error("Qt live accessibility actions failed");
  actions.clear();
  provider.doAction(QAccessibleActionInterface::decreaseAction());
  project(false, false); provider.doAction(QAccessibleActionInterface::pressAction());
  project(true, true); provider.doAction(QAccessibleActionInterface::increaseAction());
  project(true, false, false); provider.doAction(QAccessibleActionInterface::pressAction());
  if (!actions.empty()) throw std::runtime_error("Qt accepted unsupported/disabled/read-only/removed action");
  surface.ClearSemanticsTree();
  std::cout << "Qt: projection emits zero actions; live press/increase; unsupported/disabled/read-only/removed actions rejected PASS\n";
}
