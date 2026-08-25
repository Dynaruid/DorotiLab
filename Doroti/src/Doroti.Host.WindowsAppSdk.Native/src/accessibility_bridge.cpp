#include "accessibility_bridge.h"

#include <oleauto.h>
#include <wrl/client.h>

#include <algorithm>
#include <atomic>
#include <mutex>
#include <unordered_map>
#include <unordered_set>
#include <utility>

namespace doroti::windows {
namespace {

constexpr int kRootId = -1;
constexpr int64_t kTapAction = 1ll << 0;
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
                       public IValueProvider {
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
        return StringVariant(id_ == kRootId ? L"Doroti" : node.label, result);
      case UIA_AutomationIdPropertyId:
        return StringVariant(id_ == kRootId ? L"DorotiRoot" : L"DorotiNode" + std::to_wstring(id_), result);
      case UIA_FrameworkIdPropertyId:
        return StringVariant(L"Doroti", result);
      case UIA_IsEnabledPropertyId:
        result->vt = VT_BOOL;
        result->boolVal = id_ == kRootId || node.enabled ? VARIANT_TRUE : VARIANT_FALSE;
        return S_OK;
      case UIA_IsKeyboardFocusablePropertyId:
        result->vt = VT_BOOL;
        result->boolVal = id_ != kRootId &&
                                  (node.text_field || Supports(node, kFocusAction))
                              ? VARIANT_TRUE
                              : VARIANT_FALSE;
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
        result->boolVal = id_ == kRootId || !node.hidden ? VARIANT_TRUE : VARIANT_FALSE;
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
    const auto window = state_->Window();
    if (window == nullptr) return UIA_E_ELEMENTNOTAVAILABLE;
    ::SetFocus(window);
    state_->Invoke(id_, kFocusAction);
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
    *result = static_cast<IRawElementProviderFragment*>(new Provider(state_, state_->Focused()));
    return S_OK;
  }

  IFACEMETHODIMP Invoke() override {
    if (!Supports(kTapAction)) return UIA_E_NOTSUPPORTED;
    state_->Invoke(id_, kTapAction);
    return S_OK;
  }

  IFACEMETHODIMP SetValue(LPCWSTR value) override {
    AccessibilityNode node;
    if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
    if (node.read_only || !Supports(node, kSetTextAction)) return UIA_E_NOTSUPPORTED;
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
    *result = node.read_only ? TRUE : FALSE;
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

  static CONTROLTYPEID ControlType(const AccessibilityNode& node) {
    if (node.text_field) return UIA_EditControlTypeId;
    if (node.button || Supports(node, kTapAction)) return UIA_ButtonControlTypeId;
    if (node.slider) return UIA_SliderControlTypeId;
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
  HWND window{};
  {
    std::lock_guard lock(state_->mutex);
    state_->generation = generation;
    state_->scale = scale > 0 ? scale : 1.0;
    state_->nodes.clear();
    state_->parents.clear();
    std::unordered_set<int> child_ids;
    for (auto& node : nodes) {
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
    window = state_->window;
  }
  if (window != nullptr) {
    auto* provider = new Provider(state_, kRootId);
    UiaRaiseStructureChangedEvent(provider, StructureChangeType_ChildrenInvalidated,
                                  nullptr, 0);
    provider->Release();
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
  child->Release();
  if (FAILED(bounds_status) || bounds.width < 0 || bounds.height < 0 ||
      FAILED(simple_status))
    return false;
  Microsoft::WRL::ComPtr<IUnknown> invoke;
  if (FAILED(simple->GetPatternProvider(UIA_InvokePatternId, &invoke)) || !invoke)
    return false;
  Microsoft::WRL::ComPtr<IInvokeProvider> provider;
  if (FAILED(invoke.As(&provider))) return false;
  return SUCCEEDED(provider->Invoke());
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
