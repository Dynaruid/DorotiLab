int reorder_events;

void RecordSemanticsEvent(QAccessibleEvent* event) {
  if (event->type() == QAccessible::ObjectReorder) ++reorder_events;
}

void RequireSemantics(bool condition, const char* message) {
  if (!condition) throw std::runtime_error(message);
}

void CheckSemanticsRegression() {
  QAccessible::installFactory(&AccessibleFactory);
  QAccessible::installUpdateHandler(&RecordSemanticsEvent);
  QAccessible::setActive(true);
  doroti_qt_callbacks_v2 callbacks{};
  callbacks.lifecycle_changed = [](void*, void*, std::uint32_t, std::int64_t) {};
  callbacks.closed = [](void*, void*) {};
  callbacks.metrics_changed = [](void*, void*, const doroti_qt_metrics_v2*) {};
  callbacks.configuration_changed = [](void*, void*, doroti_qt_utf8_v2, std::uint32_t, std::uint32_t, std::uint32_t) {};
  callbacks.frame_terminal = [](void*, void*, std::uint64_t, std::uint32_t, std::uint64_t, std::int64_t) {};
  callbacks.focus = [](void*, void*, std::uint32_t, std::int64_t) {};
  callbacks.close_requested = [](void* count, void*) { ++*static_cast<int*>(count); };

  for (const bool unified : {false, true}) {
#ifdef DOROTI_QT_QUICK
    QVulkanInstance vulkan;
#endif
    int close_requests = 0;
    DorotiSurface surface(&close_requests, callbacks, DOROTI_QT_BACKDROP_ACRYLIC, 0,
                          unified ? DOROTI_QT_TITLEBAR_UNIFIED : DOROTI_QT_TITLEBAR_SOLID
#ifdef DOROTI_QT_QUICK
                          , vulkan
#endif
    );
    accessible_surfaces.insert(&surface);
    const auto set_tree = [&surface](const char* json, bool patch = false) {
      surface.ApplySemantics(QJsonDocument::fromJson(json), patch);
    };
    set_tree(R"({"nodes":[]})");
    auto* root = surface.Accessible(0);
    const int caption_count = unified ? 3 : 0;
    RequireSemantics(root && root->childCount() == caption_count, "window root without an app tree");
    auto* close = unified ? root->child(2) : nullptr;
    const auto close_id = close ? QAccessible::uniqueId(close) : 0;
    if (unified) {
      RequireSemantics(root->child(0)->text(QAccessible::Name) == QStringLiteral("Minimize"), "minimize is exposed");
      RequireSemantics(root->child(1)->text(QAccessible::Name) == QStringLiteral("Maximize"), "maximize is exposed");
      RequireSemantics(close && close->text(QAccessible::Name) == QStringLiteral("Close"), "close is exposed");
      RequireSemantics(close->actionInterface()->actionNames().contains(QAccessibleActionInterface::pressAction()),
                       "caption action is available");
    }
    set_tree(R"({"nodes":[{"id":0,"children":[1]},{"id":1,"children":[],"rect":[0,40,30,60],"label":"Field",
       "value":"a","flags":{"textField":true},"textGeometry":{"text":"a","boundaries":[0,1],
       "runs":[{"start":0,"end":1,"left":2,"top":43,"right":8,"bottom":56}]}}]})");
    RequireSemantics(root->childCount() == caption_count + 1, "app and caption controls share the window root");
    auto* field = root->child(0);
    const auto field_rect = field->rect();
    auto* text = field->textInterface();
    RequireSemantics(text != nullptr, "field exposes text");
    const auto glyph_rect = text->characterRect(0);
    reorder_events = 0;
    set_tree(R"({"nodes":[{"id":1,"rect":[0,45,30,65],"textBounds":[[0,2,48,8,61]]}]})", true);
    RequireSemantics(root->child(0) == field && field->rect().y() == field_rect.y() + 5,
                     "sparse position patch retains the field interface");
    RequireSemantics(text->characterRect(0).y() == glyph_rect.y() + 5, "retained text interface reads new coordinates");
    set_tree(R"({"nodes":[{"id":0,"children":[1]},{"id":1,"children":[],"rect":[0,48,30,68],"label":"Field",
       "value":"a","flags":{"textField":true},"textGeometry":{"text":"a","boundaries":[0,1],
       "runs":[{"start":0,"end":1,"left":2,"top":51,"right":8,"bottom":64}]}}]})");
    RequireSemantics(reorder_events == 0 && root->child(0) == field, "identical hierarchy does not announce a reorder");
    if (unified) RequireSemantics(QAccessible::uniqueId(root->child(3)) == close_id, "position updates preserve the caption peer");
    set_tree(R"({"nodes":[]})");
    RequireSemantics(root->childCount() == caption_count && reorder_events == 1, "clear removes only the app subtree");
    if (unified) RequireSemantics(QAccessible::uniqueId(root->child(2)) == close_id, "caption peer survives clear");
    set_tree(R"({"nodes":[{"id":5,"indexInParent":1,"rect":[0,40,10,50],"label":"Second"},
                          {"id":6,"indexInParent":0,"rect":[10,40,20,50],"label":"First"}]})");
    RequireSemantics(root->child(0)->text(QAccessible::Name) == QStringLiteral("First"), "flat nodes keep reading order");
    if (unified) {
      const auto press = QAccessibleActionInterface::pressAction();
      auto* maximize = root->child(3);
      maximize->actionInterface()->doAction(press);
      RequireSemantics(surface.windowState() == Qt::WindowMaximized &&
                           maximize->text(QAccessible::Name) == QStringLiteral("Restore"),
                       "maximize exposes restore on the same interface");
      maximize->actionInterface()->doAction(press);
      RequireSemantics(surface.windowState() == Qt::WindowNoState, "restore exits maximize");
      root->child(2)->actionInterface()->doAction(press);
      RequireSemantics(surface.windowState() == Qt::WindowMinimized, "minimize changes window state");
      close->actionInterface()->doAction(press);
      RequireSemantics(close_requests == 1, "close dispatches the window request");
    }
  }
  QAccessible::installUpdateHandler(nullptr);
  QAccessible::removeFactory(&AccessibleFactory);
}
