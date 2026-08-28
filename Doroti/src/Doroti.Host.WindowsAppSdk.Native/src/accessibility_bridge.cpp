#include "accessibility_bridge.h"

#include <oleauto.h>
#include <wrl/client.h>

#include <algorithm>
#include <atomic>
#include <cmath>
#include <cwchar>
#include <mutex>
#include <unordered_map>
#include <unordered_set>
#include <utility>

namespace doroti::windows {
namespace {

constexpr int kRootId = -1;
constexpr int64_t kTapAction = 1ll << 0;
constexpr int64_t kIncreaseAction = 1ll << 6;
constexpr int64_t kDecreaseAction = 1ll << 7;
constexpr int64_t kSetTextAction = 1ll << 21;
constexpr int64_t kFocusAction = 1ll << 22;
constexpr int64_t kGainAccessibilityFocusAction = 1ll << 15;

VARIANT EmptyVariant() noexcept {
  VARIANT value{};
  VariantInit(&value);
  return value;
}

HRESULT StringVariant(const std::wstring& text, VARIANT* result) noexcept {
  result->vt = VT_BSTR;
  result->bstrVal = SysAllocStringLen(text.data(), static_cast<UINT>(text.size()));
  return result->bstrVal != nullptr || text.empty() ? S_OK : E_OUTOFMEMORY;
}

}  // namespace

struct AccessibilityBridge::State {
  std::mutex mutex;
  HWND window{};
  uint64_t generation{};
  double scale{1.0};
  std::unordered_map<int, AccessibilityNode> nodes;
  std::unordered_map<int, int> parents;
  std::vector<int> roots;
  ActionCallback action;
  bool alive{true};

  bool Snapshot(int id, AccessibilityNode& node) {
    std::lock_guard lock(mutex);
    const auto found = nodes.find(id);
    if (!alive || found == nodes.end()) return false;
    node = found->second;
    return true;
  }

  std::vector<int> Children(int id) {
    std::lock_guard lock(mutex);
    if (!alive) return {};
    if (id == kRootId) return roots;
    const auto found = nodes.find(id);
    return found == nodes.end() ? std::vector<int>{} : found->second.children;
  }

  int Parent(int id) {
    std::lock_guard lock(mutex);
    const auto found = parents.find(id);
    return found == parents.end() ? kRootId : found->second;
  }

  int Focused() {
    std::lock_guard lock(mutex);
    for (const auto& [id, node] : nodes)
      if (node.focused && !node.hidden) return id;
    return kRootId;
  }

  HWND Window() {
    std::lock_guard lock(mutex);
    return alive ? window : nullptr;
  }

  double Scale() {
    std::lock_guard lock(mutex);
    return scale;
  }

  void Invoke(int id, int64_t action_id, const std::wstring& arguments = {}) {
    ActionCallback callback;
    {
      std::lock_guard lock(mutex);
      if (!alive) return;
      callback = action;
    }
    if (callback) callback(id, action_id, arguments);
  }
};

class Provider final : public IRawElementProviderSimple,
                       public IRawElementProviderFragment,
                       public IRawElementProviderFragmentRoot,
                       public IInvokeProvider,
                       public IValueProvider,
                       public IToggleProvider,
                       public ISelectionItemProvider,
                       public IRangeValueProvider {
 public:
  Provider(std::shared_ptr<AccessibilityBridge::State> state, int id)
      : state_(std::move(state)), id_(id) {}

  IFACEMETHODIMP QueryInterface(REFIID iid, void** result) override {
    if (result == nullptr) return E_POINTER;
    *result = nullptr;
    if (iid == __uuidof(IUnknown) || iid == __uuidof(IRawElementProviderSimple))
      *result = static_cast<IRawElementProviderSimple*>(this);
    else if (iid == __uuidof(IRawElementProviderFragment))
      *result = static_cast<IRawElementProviderFragment*>(this);
    else if (iid == __uuidof(IRawElementProviderFragmentRoot) && id_ == kRootId)
      *result = static_cast<IRawElementProviderFragmentRoot*>(this);
    else if (iid == __uuidof(IInvokeProvider) && Supports(kTapAction))
      *result = static_cast<IInvokeProvider*>(this);
    else if (iid == __uuidof(IValueProvider) && IsTextField())
      *result = static_cast<IValueProvider*>(this);
    else if (iid == __uuidof(IToggleProvider) && IsToggle())
      *result = static_cast<IToggleProvider*>(this);
    else if (iid == __uuidof(ISelectionItemProvider) && IsRadio())
      *result = static_cast<ISelectionItemProvider*>(this);
    else if (iid == __uuidof(IRangeValueProvider) && IsSlider())
      *result = static_cast<IRangeValueProvider*>(this);
    else
      return E_NOINTERFACE;
    AddRef();
    return S_OK;
  }

  IFACEMETHODIMP_(ULONG) AddRef() override { return ++references_; }
  IFACEMETHODIMP_(ULONG) Release() override {
    const auto count = --references_;
    if (count == 0) delete this;
    return count;
  }

  IFACEMETHODIMP get_ProviderOptions(ProviderOptions* result) override {
    if (result == nullptr) return E_POINTER;
    *result = ProviderOptions_ServerSideProvider;
    return S_OK;
  }

  IFACEMETHODIMP GetPatternProvider(PATTERNID pattern, IUnknown** result) override {
    if (result == nullptr) return E_POINTER;
    *result = nullptr;
    if (pattern == UIA_InvokePatternId && Supports(kTapAction))
      return QueryInterface(__uuidof(IInvokeProvider), reinterpret_cast<void**>(result));
    if (pattern == UIA_ValuePatternId && IsTextField())
      return QueryInterface(__uuidof(IValueProvider), reinterpret_cast<void**>(result));
    if (pattern == UIA_TogglePatternId && IsToggle())
      return QueryInterface(__uuidof(IToggleProvider), reinterpret_cast<void**>(result));
    if (pattern == UIA_SelectionItemPatternId && IsRadio())
      return QueryInterface(__uuidof(ISelectionItemProvider), reinterpret_cast<void**>(result));
    if (pattern == UIA_RangeValuePatternId && IsSlider())
      return QueryInterface(__uuidof(IRangeValueProvider), reinterpret_cast<void**>(result));
    return S_OK;
  }

  IFACEMETHODIMP GetPropertyValue(PROPERTYID property, VARIANT* result) override {
    if (result == nullptr) return E_POINTER;
    *result = EmptyVariant();
    AccessibilityNode node;
    if (id_ != kRootId && !state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    switch (property) {
      case UIA_ControlTypePropertyId:
        result->vt = VT_I4;
        result->lVal = ControlType(node);
        return S_OK;
      case UIA_NamePropertyId:
        return StringVariant(id_ == kRootId ? L"Doroti" :
            (!node.label.empty() ? node.label : (node.text_field ? L"" : node.value)), result);
      case UIA_AutomationIdPropertyId:
        return StringVariant(id_ == kRootId ? L"DorotiRoot" :
            (!node.identifier.empty() ? node.identifier : L"DorotiNode" + std::to_wstring(id_)), result);
      case UIA_FrameworkIdPropertyId:
        return StringVariant(L"Doroti", result);
      case UIA_IsEnabledPropertyId:
        result->vt = VT_BOOL;
        result->boolVal = id_ == kRootId || node.enabled ? VARIANT_TRUE : VARIANT_FALSE;
        return S_OK;
      case UIA_IsKeyboardFocusablePropertyId:
        result->vt = VT_BOOL;
        result->boolVal = id_ != kRootId && node.focusable ? VARIANT_TRUE : VARIANT_FALSE;
        return S_OK;
      case UIA_HasKeyboardFocusPropertyId:
        result->vt = VT_BOOL;
        result->boolVal = id_ != kRootId && node.focused ? VARIANT_TRUE : VARIANT_FALSE;
        return S_OK;
      case UIA_IsOffscreenPropertyId:
        result->vt = VT_BOOL;
        result->boolVal = id_ != kRootId && node.hidden ? VARIANT_TRUE : VARIANT_FALSE;
        return S_OK;
      case UIA_IsControlElementPropertyId:
      case UIA_IsContentElementPropertyId:
        result->vt = VT_BOOL;
        result->boolVal = id_ == kRootId || IsControl(node) ? VARIANT_TRUE : VARIANT_FALSE;
        return S_OK;
      case UIA_HelpTextPropertyId:
        return StringVariant(node.hint.empty() ? node.tooltip : node.hint, result);
      case UIA_IsPasswordPropertyId:
        result->vt = VT_BOOL;
        result->boolVal = id_ != kRootId && node.obscured ? VARIANT_TRUE : VARIANT_FALSE;
        return S_OK;
      case UIA_IsRequiredForFormPropertyId:
        result->vt = VT_BOOL;
        result->boolVal = id_ != kRootId && node.required ? VARIANT_TRUE : VARIANT_FALSE;
        return S_OK;
      case UIA_LiveSettingPropertyId:
        result->vt = VT_I4;
        result->lVal = node.live_region ? 1 : 0;
        return S_OK;
      case UIA_HeadingLevelPropertyId:
        result->vt = VT_I4;
        result->lVal = HeadingLevelValue(node.heading_level);
        return S_OK;
      default:
        return S_OK;
    }
  }

  IFACEMETHODIMP get_HostRawElementProvider(IRawElementProviderSimple** result) override {
    if (result == nullptr) return E_POINTER;
    *result = nullptr;
    return id_ == kRootId ? UiaHostProviderFromHwnd(state_->Window(), result) : S_OK;
  }

  IFACEMETHODIMP Navigate(NavigateDirection direction,
                          IRawElementProviderFragment** result) override {
    if (result == nullptr) return E_POINTER;
    *result = nullptr;
    int target = kRootId;
    if (direction == NavigateDirection_Parent) {
      if (id_ == kRootId) return S_OK;
      target = state_->Parent(id_);
    } else if (direction == NavigateDirection_FirstChild ||
               direction == NavigateDirection_LastChild) {
      auto children = state_->Children(id_);
      if (children.empty()) return S_OK;
      target = direction == NavigateDirection_FirstChild ? children.front() : children.back();
    } else if (direction == NavigateDirection_NextSibling ||
               direction == NavigateDirection_PreviousSibling) {
      if (id_ == kRootId) return S_OK;
      const auto parent = state_->Parent(id_);
      auto siblings = state_->Children(parent);
      const auto found = std::find(siblings.begin(), siblings.end(), id_);
      if (found == siblings.end()) return S_OK;
      if (direction == NavigateDirection_NextSibling) {
        if (std::next(found) == siblings.end()) return S_OK;
        target = *std::next(found);
      } else {
        if (found == siblings.begin()) return S_OK;
        target = *std::prev(found);
      }
    } else {
      return S_OK;
    }
    *result = static_cast<IRawElementProviderFragment*>(new Provider(state_, target));
    return S_OK;
  }

  IFACEMETHODIMP GetRuntimeId(SAFEARRAY** result) override {
    if (result == nullptr) return E_POINTER;
    *result = nullptr;
    if (id_ == kRootId) return S_OK;
    int values[] = {UiaAppendRuntimeId, id_};
    auto array = SafeArrayCreateVector(VT_I4, 0, 2);
    if (array == nullptr) return E_OUTOFMEMORY;
    for (LONG index = 0; index < 2; ++index)
      if (FAILED(SafeArrayPutElement(array, &index, &values[index]))) {
        SafeArrayDestroy(array);
        return E_FAIL;
      }
    *result = array;
    return S_OK;
  }

  IFACEMETHODIMP get_BoundingRectangle(UiaRect* result) override {
    if (result == nullptr) return E_POINTER;
    RECT client{};
    const auto window = state_->Window();
    if (window == nullptr || !GetClientRect(window, &client))
      return UIA_E_ELEMENTNOTAVAILABLE;
    POINT origin{};
    if (!ClientToScreen(window, &origin)) return UIA_E_ELEMENTNOTAVAILABLE;
    if (id_ == kRootId) {
      result->left = static_cast<double>(origin.x);
      result->top = static_cast<double>(origin.y);
      result->width = static_cast<double>(client.right - client.left);
      result->height = static_cast<double>(client.bottom - client.top);
      return S_OK;
    }
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    const auto scale = state_->Scale();
    result->left = origin.x + node.left * scale;
    result->top = origin.y + node.top * scale;
    result->width = std::max(0.0, node.right - node.left) * scale;
    result->height = std::max(0.0, node.bottom - node.top) * scale;
    return S_OK;
  }

  IFACEMETHODIMP GetEmbeddedFragmentRoots(SAFEARRAY** result) override {
    if (result == nullptr) return E_POINTER;
    *result = nullptr;
    return S_OK;
  }

  IFACEMETHODIMP SetFocus() override {
    if (id_ == kRootId) return S_OK;
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    if (!node.focusable) return UIA_E_NOTSUPPORTED;
    const auto window = state_->Window();
    if (window == nullptr) return UIA_E_ELEMENTNOTAVAILABLE;
    ::SetFocus(window);
    if (Supports(node, kFocusAction)) state_->Invoke(id_, kFocusAction);
    if (Supports(node, kGainAccessibilityFocusAction))
      state_->Invoke(id_, kGainAccessibilityFocusAction);
    return S_OK;
  }

  IFACEMETHODIMP get_FragmentRoot(IRawElementProviderFragmentRoot** result) override {
    if (result == nullptr) return E_POINTER;
    *result = static_cast<IRawElementProviderFragmentRoot*>(new Provider(state_, kRootId));
    return S_OK;
  }

  IFACEMETHODIMP ElementProviderFromPoint(double x, double y,
      IRawElementProviderFragment** result) override {
    if (result == nullptr) return E_POINTER;
    *result = nullptr;
    std::vector<int> pending = state_->Children(kRootId);
    int best = kRootId;
    while (!pending.empty()) {
      const auto id = pending.back();
      pending.pop_back();
      AccessibilityNode node;
      if (!state_->Snapshot(id, node) || node.hidden) continue;
      UiaRect bounds{};
      Provider probe(state_, id);
      probe.references_ = 1000;
      if (SUCCEEDED(probe.get_BoundingRectangle(&bounds)) && x >= bounds.left &&
          y >= bounds.top && x <= bounds.left + bounds.width &&
          y <= bounds.top + bounds.height) {
        best = id;
        const auto children = state_->Children(id);
        pending.insert(pending.end(), children.begin(), children.end());
      }
    }
    *result = static_cast<IRawElementProviderFragment*>(new Provider(state_, best));
    return S_OK;
  }

  IFACEMETHODIMP GetFocus(IRawElementProviderFragment** result) override {
    if (result == nullptr) return E_POINTER;
    *result = nullptr;
    const auto focused = state_->Focused();
    if (focused != kRootId)
      *result = static_cast<IRawElementProviderFragment*>(new Provider(state_, focused));
    return S_OK;
  }

  IFACEMETHODIMP Invoke() override {
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    if (!node.enabled || !Supports(node, kTapAction)) return UIA_E_NOTSUPPORTED;
    state_->Invoke(id_, kTapAction);
    return S_OK;
  }

  IFACEMETHODIMP SetValue(LPCWSTR value) override {
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    if (!node.enabled || node.read_only || !Supports(node, kSetTextAction))
      return UIA_E_NOTSUPPORTED;
    std::wstring json = L"\"";
    for (const auto character : std::wstring(value == nullptr ? L"" : value)) {
      if (character == L'\\' || character == L'\"') json.push_back(L'\\');
      json.push_back(character);
    }
    json.push_back(L'\"');
    state_->Invoke(id_, kSetTextAction, json);
    return S_OK;
  }

  IFACEMETHODIMP get_Value(BSTR* result) override {
    if (result == nullptr) return E_POINTER;
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    *result = SysAllocStringLen(node.value.data(), static_cast<UINT>(node.value.size()));
    return *result != nullptr || node.value.empty() ? S_OK : E_OUTOFMEMORY;
  }

  IFACEMETHODIMP get_IsReadOnly(BOOL* result) override {
    if (result == nullptr) return E_POINTER;
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    *result = node.slider
                  ? (node.enabled &&
                             (Supports(node, kIncreaseAction) || Supports(node, kDecreaseAction))
                         ? FALSE
                         : TRUE)
                  : (node.read_only || !node.enabled ? TRUE : FALSE);
    return S_OK;
  }

  IFACEMETHODIMP Toggle() override {
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    if (!node.enabled || !IsToggle(node) || !Supports(node, kTapAction))
      return UIA_E_NOTSUPPORTED;
    state_->Invoke(id_, kTapAction);
    return S_OK;
  }

  IFACEMETHODIMP get_ToggleState(ToggleState* result) override {
    if (result == nullptr) return E_POINTER;
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    const auto state = node.checked >= 0 ? node.checked : node.toggled;
    *result = state == 2 ? ToggleState_Indeterminate :
              state == 1 ? ToggleState_On : ToggleState_Off;
    return S_OK;
  }

  IFACEMETHODIMP Select() override {
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    if (!node.enabled || !IsRadio(node) || !Supports(node, kTapAction))
      return UIA_E_NOTSUPPORTED;
    const auto selected = node.selected >= 0 ? node.selected : node.checked;
    if (selected != 1) state_->Invoke(id_, kTapAction);
    return S_OK;
  }

  IFACEMETHODIMP AddToSelection() override { return Select(); }

  IFACEMETHODIMP RemoveFromSelection() override {
    return UIA_E_INVALIDOPERATION;
  }

  IFACEMETHODIMP get_IsSelected(BOOL* result) override {
    if (result == nullptr) return E_POINTER;
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    const auto selected = node.selected >= 0 ? node.selected : node.checked;
    *result = selected == 1 ? TRUE : FALSE;
    return S_OK;
  }

  IFACEMETHODIMP get_SelectionContainer(
      IRawElementProviderSimple** result) override {
    if (result == nullptr) return E_POINTER;
    *result = nullptr;
    if (!IsRadio()) return UIA_E_NOTSUPPORTED;
    *result = static_cast<IRawElementProviderSimple*>(
        new Provider(state_, state_->Parent(id_)));
    return S_OK;
  }

  IFACEMETHODIMP SetValue(double value) override {
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    if (!node.enabled || !IsSlider(node)) return UIA_E_NOTSUPPORTED;
    double current{};
    if (!TryParseNumber(node.value, current)) return UIA_E_NOTSUPPORTED;
    double minimum{};
    double maximum{};
    if (TryParseNumber(node.min_value, minimum) && value < minimum) return E_INVALIDARG;
    if (TryParseNumber(node.max_value, maximum) && value > maximum) return E_INVALIDARG;
    if (value > current && Supports(node, kIncreaseAction)) {
      state_->Invoke(id_, kIncreaseAction);
      return S_OK;
    }
    if (value < current && Supports(node, kDecreaseAction)) {
      state_->Invoke(id_, kDecreaseAction);
      return S_OK;
    }
    return value == current ? S_OK : UIA_E_NOTSUPPORTED;
  }

  IFACEMETHODIMP get_Value(double* result) override {
    if (result == nullptr) return E_POINTER;
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    return TryParseNumber(node.value, *result) ? S_OK : UIA_E_NOTSUPPORTED;
  }

  IFACEMETHODIMP get_Maximum(double* result) override {
    return RangeEndpoint(result, false);
  }

  IFACEMETHODIMP get_Minimum(double* result) override {
    return RangeEndpoint(result, true);
  }

  IFACEMETHODIMP get_LargeChange(double* result) override {
    if (result == nullptr) return E_POINTER;
    *result = 0;
    return S_OK;
  }

  IFACEMETHODIMP get_SmallChange(double* result) override {
    if (result == nullptr) return E_POINTER;
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    double current{};
    double adjacent{};
    if (TryParseNumber(node.value, current) &&
        ((TryParseNumber(node.increased_value, adjacent) && adjacent > current) ||
         (TryParseNumber(node.decreased_value, adjacent) && adjacent < current))) {
      *result = std::abs(adjacent - current);
      return S_OK;
    }
    *result = 0;
    return S_OK;
  }

 private:
  ~Provider() = default;

  bool Supports(int64_t action) {
    AccessibilityNode node;
    return state_->Snapshot(id_, node) && Supports(node, action);
  }

  static bool Supports(const AccessibilityNode& node, int64_t action) {
    return (node.actions & action) != 0;
  }

  bool IsTextField() {
    AccessibilityNode node;
    return state_->Snapshot(id_, node) && node.text_field;
  }

  bool IsToggle() {
    AccessibilityNode node;
    return state_->Snapshot(id_, node) && IsToggle(node);
  }

  static bool IsToggle(const AccessibilityNode& node) {
    return !node.mutually_exclusive && (node.checked >= 0 || node.toggled >= 0);
  }

  bool IsRadio() {
    AccessibilityNode node;
    return state_->Snapshot(id_, node) && IsRadio(node);
  }

  static bool IsRadio(const AccessibilityNode& node) {
    return node.mutually_exclusive && (node.checked >= 0 || node.selected >= 0);
  }

  bool IsSlider() {
    AccessibilityNode node;
    return state_->Snapshot(id_, node) && IsSlider(node);
  }

  static bool IsSlider(const AccessibilityNode& node) {
    return node.slider &&
        (Supports(node, kIncreaseAction) || Supports(node, kDecreaseAction));
  }

  HRESULT RangeEndpoint(double* result, bool minimum) {
    if (result == nullptr) return E_POINTER;
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    const auto& text = minimum ? node.min_value : node.max_value;
    if (TryParseNumber(text, *result)) return S_OK;
    *result = minimum ? 0 : 100;
    return S_OK;
  }

  static bool TryParseNumber(const std::wstring& text, double& result) {
    if (text.empty()) return false;
    wchar_t* end{};
    result = std::wcstod(text.c_str(), &end);
    return end != text.c_str() && *end == L'\0' && std::isfinite(result);
  }

  static bool IsControl(const AccessibilityNode& node) {
    return !node.hidden && (!node.label.empty() || !node.value.empty() ||
        node.actions != 0 || node.role != L"none" || node.button || node.text_field ||
        node.slider || node.image || node.link || node.header || IsToggle(node));
  }

  static int HeadingLevelValue(int level) {
    return level >= 1 && level <= 9 ? HeadingLevel_None + level : HeadingLevel_None;
  }

  static CONTROLTYPEID ControlType(const AccessibilityNode& node) {
    if (node.text_field) return UIA_EditControlTypeId;
    if (IsRadio(node)) return UIA_RadioButtonControlTypeId;
    if (IsToggle(node)) return node.toggled >= 0 ? UIA_ButtonControlTypeId : UIA_CheckBoxControlTypeId;
    if (node.link || node.role == L"link") return UIA_HyperlinkControlTypeId;
    if (node.image) return UIA_ImageControlTypeId;
    if (node.button || Supports(node, kTapAction)) return UIA_ButtonControlTypeId;
    if (node.slider) return UIA_SliderControlTypeId;
    if (node.role == L"tab") return UIA_TabItemControlTypeId;
    if (node.role == L"tabBar") return UIA_TabControlTypeId;
    if (node.role == L"table") return UIA_TableControlTypeId;
    if (node.role == L"cell") return UIA_DataItemControlTypeId;
    if (node.role == L"row") return UIA_DataItemControlTypeId;
    if (node.role == L"columnHeader") return UIA_HeaderItemControlTypeId;
    if (node.role == L"menu") return UIA_MenuControlTypeId;
    if (node.role == L"menuBar") return UIA_MenuBarControlTypeId;
    if (node.role == L"menuItem" || node.role == L"menuItemCheckbox" ||
        node.role == L"menuItemRadio") return UIA_MenuItemControlTypeId;
    if (node.role == L"progressBar" || node.role == L"loadingSpinner")
      return UIA_ProgressBarControlTypeId;
    if (node.role == L"list") return UIA_ListControlTypeId;
    if (node.role == L"listItem") return UIA_ListItemControlTypeId;
    if (node.role == L"dialog" || node.role == L"alertDialog") return UIA_WindowControlTypeId;
    return UIA_TextControlTypeId;
  }

  std::atomic<ULONG> references_{1};
  std::shared_ptr<AccessibilityBridge::State> state_;
  int id_{};

  friend class AccessibilityBridge;
};

AccessibilityBridge::AccessibilityBridge() : state_(std::make_shared<State>()) {}

AccessibilityBridge::~AccessibilityBridge() {
  Clear();
  std::lock_guard lock(state_->mutex);
  state_->alive = false;
  state_->action = {};
  state_->window = nullptr;
}

void AccessibilityBridge::Attach(HWND window, ActionCallback callback) {
  std::lock_guard lock(state_->mutex);
  state_->window = window;
  state_->action = std::move(callback);
}

void AccessibilityBridge::Update(uint64_t generation,
                                 std::vector<AccessibilityNode> nodes,
                                 double scale) {
  std::unordered_set<int> hidden_ids;
  for (const auto& node : nodes)
    if (node.hidden) hidden_ids.insert(node.id);
  bool hidden_changed = true;
  while (hidden_changed) {
    hidden_changed = false;
    for (const auto& node : nodes) {
      if (!hidden_ids.contains(node.id)) continue;
      for (const auto child : node.children)
        hidden_changed = hidden_ids.insert(child).second || hidden_changed;
    }
  }
  std::unordered_set<int> visible_ids;
  for (const auto& node : nodes)
    if (!hidden_ids.contains(node.id)) visible_ids.insert(node.id);
  std::vector<AccessibilityNode> visible_nodes;
  visible_nodes.reserve(visible_ids.size());
  for (auto& node : nodes) {
    if (!visible_ids.contains(node.id)) continue;
    std::erase_if(node.children, [&](int id) { return !visible_ids.contains(id); });
    visible_nodes.push_back(std::move(node));
  }

  HWND window{};
  bool topology_changed{};
  int previous_focused{kRootId};
  int focused{kRootId};
  std::unordered_map<int, AccessibilityNode> previous_nodes;
  std::unordered_map<int, AccessibilityNode> current_nodes;
  {
    std::lock_guard lock(state_->mutex);
    if (!state_->alive || generation < state_->generation) return;
    previous_nodes = state_->nodes;
    for (const auto& [id, node] : state_->nodes)
      if (node.focused) previous_focused = id;
    const auto previous_roots = state_->roots;
    std::unordered_map<int, std::vector<int>> previous_children;
    for (const auto& [id, node] : state_->nodes)
      previous_children.emplace(id, node.children);
    state_->generation = generation;
    state_->scale = scale > 0 ? scale : 1.0;
    state_->nodes.clear();
    state_->parents.clear();
    std::unordered_set<int> child_ids;
    for (auto& node : visible_nodes) {
      for (const auto child : node.children) {
        state_->parents[child] = node.id;
        child_ids.insert(child);
      }
      state_->nodes.emplace(node.id, std::move(node));
    }
    state_->roots.clear();
    for (const auto& [id, node] : state_->nodes)
      if (!child_ids.contains(id)) state_->roots.push_back(id);
    std::sort(state_->roots.begin(), state_->roots.end());
    topology_changed = previous_roots != state_->roots ||
        previous_children.size() != state_->nodes.size();
    if (!topology_changed) {
      for (const auto& [id, node] : state_->nodes) {
        const auto previous = previous_children.find(id);
        const auto old_node = previous_nodes.find(id);
        if (previous == previous_children.end() || previous->second != node.children ||
            old_node == previous_nodes.end() || old_node->second.role != node.role ||
            old_node->second.button != node.button || old_node->second.text_field != node.text_field ||
            old_node->second.slider != node.slider || old_node->second.link != node.link ||
            old_node->second.image != node.image ||
            old_node->second.mutually_exclusive != node.mutually_exclusive ||
            (old_node->second.checked >= 0) != (node.checked >= 0) ||
            (old_node->second.toggled >= 0) != (node.toggled >= 0)) {
          topology_changed = true;
          break;
        }
      }
    }
    for (const auto& [id, node] : state_->nodes)
      if (node.focused) focused = id;
    current_nodes = state_->nodes;
    window = state_->window;
  }
  if (window != nullptr && topology_changed) {
    auto* provider = new Provider(state_, kRootId);
    UiaRaiseStructureChangedEvent(provider, StructureChangeType_ChildrenInvalidated,
                                  nullptr, 0);
    provider->Release();
  }
  if (window != nullptr && focused != kRootId && focused != previous_focused) {
    auto* provider = new Provider(state_, focused);
    UiaRaiseAutomationEvent(provider, UIA_AutomationFocusChangedEventId);
    provider->Release();
  }
  if (window != nullptr) {
    const auto raise_string = [&](int id, PROPERTYID property,
                                  const std::wstring& previous,
                                  const std::wstring& current) {
      if (previous == current) return;
      auto old_value = EmptyVariant();
      auto new_value = EmptyVariant();
      StringVariant(previous, &old_value);
      StringVariant(current, &new_value);
      auto* provider = new Provider(state_, id);
      UiaRaiseAutomationPropertyChangedEvent(provider, property, old_value, new_value);
      provider->Release();
      VariantClear(&old_value);
      VariantClear(&new_value);
    };
    const auto raise_bool = [&](int id, PROPERTYID property, bool previous,
                                bool current) {
      if (previous == current) return;
      auto old_value = EmptyVariant();
      auto new_value = EmptyVariant();
      old_value.vt = new_value.vt = VT_BOOL;
      old_value.boolVal = previous ? VARIANT_TRUE : VARIANT_FALSE;
      new_value.boolVal = current ? VARIANT_TRUE : VARIANT_FALSE;
      auto* provider = new Provider(state_, id);
      UiaRaiseAutomationPropertyChangedEvent(provider, property, old_value, new_value);
      provider->Release();
    };
    const auto raise_int = [&](int id, PROPERTYID property, int previous,
                               int current) {
      if (previous == current) return;
      auto old_value = EmptyVariant();
      auto new_value = EmptyVariant();
      old_value.vt = new_value.vt = VT_I4;
      old_value.lVal = previous;
      new_value.lVal = current;
      auto* provider = new Provider(state_, id);
      UiaRaiseAutomationPropertyChangedEvent(provider, property, old_value, new_value);
      provider->Release();
    };
    const auto raise_double = [&](int id, PROPERTYID property, double previous,
                                  double current) {
      if (previous == current) return;
      auto old_value = EmptyVariant();
      auto new_value = EmptyVariant();
      old_value.vt = new_value.vt = VT_R8;
      old_value.dblVal = previous;
      new_value.dblVal = current;
      auto* provider = new Provider(state_, id);
      UiaRaiseAutomationPropertyChangedEvent(provider, property, old_value, new_value);
      provider->Release();
    };
    const auto accessible_name = [](const AccessibilityNode& node) {
      return !node.label.empty() ? node.label : (node.text_field ? L"" : node.value);
    };
    const auto toggle_state = [](const AccessibilityNode& node) {
      const auto state = node.checked >= 0 ? node.checked : node.toggled;
      return state == 2 ? static_cast<int>(ToggleState_Indeterminate) :
             state == 1 ? static_cast<int>(ToggleState_On) :
                          static_cast<int>(ToggleState_Off);
    };
    const auto is_toggle = [](const AccessibilityNode& node) {
      return !node.mutually_exclusive && (node.checked >= 0 || node.toggled >= 0);
    };
    const auto is_radio = [](const AccessibilityNode& node) {
      return node.mutually_exclusive && (node.checked >= 0 || node.selected >= 0);
    };
    const auto parse_number = [](const std::wstring& text, double& result) {
      if (text.empty()) return false;
      wchar_t* end{};
      result = std::wcstod(text.c_str(), &end);
      return end != text.c_str() && *end == L'\0' && std::isfinite(result);
    };
    for (const auto& [id, current] : current_nodes) {
      const auto found = previous_nodes.find(id);
      if (found == previous_nodes.end()) continue;
      const auto& previous = found->second;
      raise_string(id, UIA_NamePropertyId, accessible_name(previous),
                   accessible_name(current));
      raise_bool(id, UIA_IsEnabledPropertyId, previous.enabled, current.enabled);
      if (current.text_field)
        raise_string(id, UIA_ValueValuePropertyId, previous.value, current.value);
      if (current.slider) {
        double previous_value{};
        double current_value{};
        if (parse_number(previous.value, previous_value) &&
            parse_number(current.value, current_value))
          raise_double(id, UIA_RangeValueValuePropertyId, previous_value, current_value);
      }
      if (is_toggle(current))
        raise_int(id, UIA_ToggleToggleStatePropertyId, toggle_state(previous),
                  toggle_state(current));
      if (is_radio(current))
        raise_bool(id, UIA_SelectionItemIsSelectedPropertyId,
                   (previous.selected >= 0 ? previous.selected : previous.checked) == 1,
                   (current.selected >= 0 ? current.selected : current.checked) == 1);
    }
  }
}

void AccessibilityBridge::Clear() {
  std::lock_guard lock(state_->mutex);
  state_->generation = 0;
  state_->nodes.clear();
  state_->parents.clear();
  state_->roots.clear();
}

void AccessibilityBridge::SetScale(double scale) {
  std::lock_guard lock(state_->mutex);
  state_->scale = scale > 0 ? scale : 1.0;
}

bool AccessibilityBridge::ValidateAndInvokeForTest() {
  auto* root = new Provider(state_, kRootId);
  IRawElementProviderFragment* child = nullptr;
  const auto navigation = root->Navigate(NavigateDirection_FirstChild, &child);
  root->Release();
  if (FAILED(navigation) || child == nullptr) return false;
  UiaRect bounds{};
  const auto bounds_status = child->get_BoundingRectangle(&bounds);
  Microsoft::WRL::ComPtr<IRawElementProviderSimple> simple;
  const auto simple_status = child->QueryInterface(IID_PPV_ARGS(&simple));
  if (FAILED(bounds_status) || bounds.width < 0 || bounds.height < 0 ||
      FAILED(simple_status)) {
    child->Release();
    return false;
  }
  Microsoft::WRL::ComPtr<IUnknown> invoke;
  if (FAILED(simple->GetPatternProvider(UIA_InvokePatternId, &invoke)) || !invoke) {
    child->Release();
    return false;
  }
  Microsoft::WRL::ComPtr<IInvokeProvider> provider;
  if (FAILED(invoke.As(&provider)) || FAILED(provider->Invoke())) {
    child->Release();
    return false;
  }

  IRawElementProviderFragment* toggle_fragment = nullptr;
  const auto toggle_navigation = child->Navigate(
      NavigateDirection_FirstChild, &toggle_fragment);
  child->Release();
  if (FAILED(toggle_navigation) || toggle_fragment == nullptr) return false;
  Microsoft::WRL::ComPtr<IRawElementProviderSimple> toggle_simple;
  if (FAILED(toggle_fragment->QueryInterface(IID_PPV_ARGS(&toggle_simple)))) {
    toggle_fragment->Release();
    return false;
  }
  IRawElementProviderFragment* radio_fragment = nullptr;
  const auto sibling_navigation = toggle_fragment->Navigate(
      NavigateDirection_NextSibling, &radio_fragment);
  toggle_fragment->Release();
  if (FAILED(sibling_navigation) || radio_fragment == nullptr) return false;
  Microsoft::WRL::ComPtr<IUnknown> toggle_unknown;
  if (FAILED(toggle_simple->GetPatternProvider(UIA_TogglePatternId,
                                               &toggle_unknown)) ||
      !toggle_unknown) {
    radio_fragment->Release();
    return false;
  }
  Microsoft::WRL::ComPtr<IToggleProvider> toggle;
  ToggleState state{};
  if (FAILED(toggle_unknown.As(&toggle)) ||
      FAILED(toggle->get_ToggleState(&state)) ||
      state != ToggleState_Off || FAILED(toggle->Toggle())) {
    radio_fragment->Release();
    return false;
  }

  Microsoft::WRL::ComPtr<IRawElementProviderSimple> radio_simple;
  if (FAILED(radio_fragment->QueryInterface(IID_PPV_ARGS(&radio_simple)))) {
    radio_fragment->Release();
    return false;
  }
  IRawElementProviderFragment* slider_fragment = nullptr;
  const auto slider_navigation = radio_fragment->Navigate(
      NavigateDirection_NextSibling, &slider_fragment);
  radio_fragment->Release();
  if (FAILED(slider_navigation) || slider_fragment == nullptr) return false;
  Microsoft::WRL::ComPtr<IUnknown> selection_unknown;
  if (FAILED(radio_simple->GetPatternProvider(UIA_SelectionItemPatternId,
                                              &selection_unknown)) ||
      !selection_unknown) {
    slider_fragment->Release();
    return false;
  }
  Microsoft::WRL::ComPtr<ISelectionItemProvider> selection;
  BOOL selected{};
  if (FAILED(selection_unknown.As(&selection)) ||
      FAILED(selection->get_IsSelected(&selected)) || selected ||
      FAILED(selection->Select())) {
    slider_fragment->Release();
    return false;
  }

  Microsoft::WRL::ComPtr<IRawElementProviderSimple> slider_simple;
  if (FAILED(slider_fragment->QueryInterface(IID_PPV_ARGS(&slider_simple)))) {
    slider_fragment->Release();
    return false;
  }
  IRawElementProviderFragment* trailing_sibling = nullptr;
  const auto trailing_navigation = slider_fragment->Navigate(
      NavigateDirection_NextSibling, &trailing_sibling);
  slider_fragment->Release();
  if (FAILED(trailing_navigation) || trailing_sibling != nullptr) {
    if (trailing_sibling != nullptr) trailing_sibling->Release();
    return false;
  }
  Microsoft::WRL::ComPtr<IUnknown> range_unknown;
  if (FAILED(slider_simple->GetPatternProvider(UIA_RangeValuePatternId,
                                               &range_unknown)) ||
      !range_unknown)
    return false;
  Microsoft::WRL::ComPtr<IRangeValueProvider> range;
  double value{};
  double minimum{};
  double maximum{};
  return SUCCEEDED(range_unknown.As(&range)) &&
         SUCCEEDED(range->get_Value(&value)) && value == 0.2 &&
         SUCCEEDED(range->get_Minimum(&minimum)) && minimum == 0 &&
         SUCCEEDED(range->get_Maximum(&maximum)) && maximum == 1 &&
         SUCCEEDED(range->SetValue(0.3));
}

LRESULT AccessibilityBridge::HandleGetObject(WPARAM wparam, LPARAM lparam) {
  if (static_cast<LONG>(lparam) != UiaRootObjectId) return 0;
  const auto window = state_->Window();
  if (window == nullptr) return 0;
  auto* provider = new Provider(state_, kRootId);
  const auto result = UiaReturnRawElementProvider(window, wparam, lparam, provider);
  provider->Release();
  return result;
}

}  // namespace doroti::windows
