# Qt provider

Host and Target sources are owned by this independently versioned provider. Package identities are unchanged. Version: 0.4.1-beta; supported core: [0.4.0-alpha.1, 0.5.0). Native/bootstrap adoption and physical platform acceptance are tracked separately in the design-platform execution record.

The default host uses Qt Quick / `QQuickWindow` with Qt-owned Vulkan device and presentation. See the [native host guide](native/README.md) for Qt dependencies, ABI versions, optional WebEngine/GStreamer and the QWindow comparison backend. [Maintained tests](../../../Doroti/tests/README.md) describe Linux smoke, package and development validation; physical input and display timing remain separate gates.
