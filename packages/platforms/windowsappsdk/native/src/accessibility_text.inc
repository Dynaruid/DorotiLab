// Included after Provider/State definitions. All ranges retain immutable layout data.
struct __declspec(uuid("c7644a03-ef38-4094-9caa-1d26f884ec9a")) IRangeIdentity : IUnknown {
  virtual HRESULT STDMETHODCALLTYPE Identity(void** owner, int* node, uint64_t* revision, int* start, int* end) = 0;
};

class TextRange final : public ITextRangeProvider, public IRangeIdentity {
 public:
  TextRange(std::shared_ptr<AccessibilityBridge::State> state, int id, AccessibilityNode node,
            uint64_t revision, int start, int end)
      : state_(std::move(state)), id_(id), node_(std::move(node)), revision_(revision) {
    start_ = Snap(start); end_ = std::max(start_, Snap(end, true));
  }
  IFACEMETHODIMP QueryInterface(REFIID iid, void** result) override {
    if (!result) return E_POINTER; *result = nullptr;
    if (iid == IID_IUnknown || iid == __uuidof(ITextRangeProvider)) *result = static_cast<ITextRangeProvider*>(this);
    else if (iid == __uuidof(IRangeIdentity)) *result = static_cast<IRangeIdentity*>(this);
    else return E_NOINTERFACE;
    AddRef(); return S_OK;
  }
  IFACEMETHODIMP_(ULONG) AddRef() override { return ++references_; }
  IFACEMETHODIMP_(ULONG) Release() override { const auto count = --references_; if (!count) delete this; return count; }
  HRESULT Validate() const {
    AccessibilityNode current; uint64_t revision;
    return state_->SnapshotText(id_, current, revision) && revision == revision_ && current.text_revision == node_.text_revision
        ? S_OK : UIA_E_ELEMENTNOTAVAILABLE;
  }
  IFACEMETHODIMP Identity(void** owner, int* node, uint64_t* revision, int* start, int* end) override {
    if (!owner || !node || !revision || !start || !end) return E_POINTER;
    const auto status = Validate(); if (FAILED(status)) return status;
    *owner = state_.get(); *node = id_; *revision = revision_; *start = start_; *end = end_; return S_OK;
  }
  IFACEMETHODIMP Clone(ITextRangeProvider** result) override {
    if (!result) return E_POINTER; *result = nullptr;
    const auto status = Validate(); if (FAILED(status)) return status;
    *result = new (std::nothrow) TextRange(state_, id_, node_, revision_, start_, end_); return *result ? S_OK : E_OUTOFMEMORY;
  }
  IFACEMETHODIMP Compare(ITextRangeProvider* other, BOOL* result) override {
    if (!result) return E_POINTER; *result = FALSE;
    int start, end; const auto status = Other(other, start, end); if (FAILED(status)) return status;
    *result = start_ == start && end_ == end; return S_OK;
  }
  IFACEMETHODIMP CompareEndpoints(TextPatternRangeEndpoint endpoint, ITextRangeProvider* other,
                                 TextPatternRangeEndpoint other_endpoint, int* result) override {
    if (!result || !Endpoint(endpoint) || !Endpoint(other_endpoint)) return E_INVALIDARG;
    int start, end; const auto status = Other(other, start, end); if (FAILED(status)) return status;
    *result = Position(endpoint) - (other_endpoint == TextPatternRangeEndpoint_Start ? start : end); return S_OK;
  }
  IFACEMETHODIMP ExpandToEnclosingUnit(TextUnit unit) override {
    const auto status = Validate(); if (FAILED(status)) return status;
    const auto values = Units(unit); if (values.size() < 2) { start_ = end_ = 0; return S_OK; }
    auto index = Index(values, start_); index = std::min(index, static_cast<int>(values.size()) - 2);
    start_ = values[index]; end_ = values[index + 1]; return S_OK;
  }
  IFACEMETHODIMP FindAttribute(TEXTATTRIBUTEID attribute, VARIANT value, BOOL, ITextRangeProvider** result) override {
    if (!result) return E_POINTER; *result = nullptr;
    const auto status = Validate(); if (FAILED(status)) return status;
    if (attribute == UIA_IsReadOnlyAttributeId && value.vt == VT_BOOL
        && (value.boolVal != VARIANT_FALSE) == (node_.read_only || (node_.actions & (1ll << 21)) == 0)) return Clone(result);
    return S_OK;
  }
  IFACEMETHODIMP FindText(BSTR text, BOOL backward, BOOL ignore_case, ITextRangeProvider** result) override {
    if (!result || !text) return E_INVALIDARG; *result = nullptr;
    const auto status = Validate(); if (FAILED(status)) return status;
    const auto requested_length = SysStringLen(text);
    if (requested_length > static_cast<UINT>(end_ - start_)) return S_OK;
    const int length = static_cast<int>(requested_length);
    int match = -1;
    for (const auto position : node_.text_boundaries) {
      if (position < start_ || position + length > end_) continue;
      if (CompareStringOrdinal(node_.layout_text.data() + position, length, text, length, ignore_case) == CSTR_EQUAL) {
        match = position; if (!backward) break;
      }
    }
    if (match >= 0) *result = new (std::nothrow) TextRange(state_, id_, node_, revision_, match, match + length);
    return match < 0 || *result ? S_OK : E_OUTOFMEMORY;
  }
  IFACEMETHODIMP GetAttributeValue(TEXTATTRIBUTEID attribute, VARIANT* result) override {
    if (!result) return E_POINTER; VariantInit(result);
    const auto status = Validate(); if (FAILED(status)) return status;
    if (attribute == UIA_IsReadOnlyAttributeId) {
      result->vt = VT_BOOL; result->boolVal = node_.read_only || (node_.actions & (1ll << 21)) == 0 ? VARIANT_TRUE : VARIANT_FALSE; return S_OK;
    }
    result->vt = VT_UNKNOWN; return UiaGetReservedNotSupportedValue(&result->punkVal);
  }
  IFACEMETHODIMP GetBoundingRectangles(SAFEARRAY** result) override {
    if (!result) return E_POINTER; *result = nullptr;
    const auto status = Validate(); if (FAILED(status)) return status;
    POINT origin{}; if (!ClientToScreen(state_->Window(), &origin)) return UIA_E_ELEMENTNOTAVAILABLE;
    std::vector<AccessibilityTextRun> lines;
    for (const auto& run : node_.text_runs) {
      if (run.start >= end_ || run.end <= start_ || run.right <= run.left || run.bottom <= run.top) continue;
      if (!lines.empty() && lines.back().line_start == run.line_start && lines.back().line_end == run.line_end) {
        auto& line = lines.back(); line.left = std::min(line.left, run.left); line.top = std::min(line.top, run.top);
        line.right = std::max(line.right, run.right); line.bottom = std::max(line.bottom, run.bottom);
      } else lines.push_back(run);
    }
    *result = SafeArrayCreateVector(VT_R8, 0, static_cast<ULONG>(lines.size() * 4));
    if (!*result) return E_OUTOFMEMORY;
    const auto scale = state_->Scale(); LONG index = 0;
    for (const auto& line : lines) {
      const double values[] = {origin.x + line.left * scale, origin.y + line.top * scale,
          (line.right - line.left) * scale, (line.bottom - line.top) * scale};
      for (auto value : values) { SafeArrayPutElement(*result, &index, &value); ++index; }
    }
    return S_OK;
  }
  IFACEMETHODIMP GetEnclosingElement(IRawElementProviderSimple** result) override {
    if (!result) return E_POINTER; *result = nullptr; const auto status = Validate(); if (FAILED(status)) return status;
    *result = new (std::nothrow) Provider(state_, id_); return *result ? S_OK : E_OUTOFMEMORY;
  }
  IFACEMETHODIMP GetText(int maximum, BSTR* result) override {
    if (!result) return E_POINTER; *result = nullptr; const auto status = Validate(); if (FAILED(status)) return status;
    auto end = maximum < 0 ? end_ : Snap(start_ + std::min(maximum, end_ - start_));
    *result = SysAllocStringLen(node_.layout_text.data() + start_, static_cast<UINT>(std::max(0, end - start_)));
    return *result ? S_OK : E_OUTOFMEMORY;
  }
  IFACEMETHODIMP Move(TextUnit unit, int count, int* moved) override {
    if (!moved) return E_POINTER; *moved = 0; const auto status = Validate(); if (FAILED(status) || !count) return status;
    const auto values = Units(unit); if (values.size() < 2) return S_OK;
    const bool caret = start_ == end_; const int maximum = static_cast<int>(values.size()) - (caret ? 1 : 2);
    const int index = std::min(Index(values, start_), maximum);
    const int next = static_cast<int>(std::clamp<int64_t>(static_cast<int64_t>(index) + count, 0, maximum));
    start_ = values[next]; end_ = caret ? start_ : values[next + 1]; *moved = next - index; return S_OK;
  }
  IFACEMETHODIMP MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count, int* moved) override {
    if (!moved || !Endpoint(endpoint)) return E_INVALIDARG; *moved = 0;
    const auto status = Validate(); if (FAILED(status) || !count) return status;
    const auto values = Units(unit); const int index = Index(values, Position(endpoint));
    const int next = static_cast<int>(std::clamp<int64_t>(static_cast<int64_t>(index) + count, 0, values.size() - 1));
    SetPosition(endpoint, values[next]); *moved = next - index; return S_OK;
  }
  IFACEMETHODIMP MoveEndpointByRange(TextPatternRangeEndpoint endpoint, ITextRangeProvider* other,
                                    TextPatternRangeEndpoint other_endpoint) override {
    if (!Endpoint(endpoint) || !Endpoint(other_endpoint)) return E_INVALIDARG;
    int start, end; const auto status = Other(other, start, end); if (FAILED(status)) return status;
    SetPosition(endpoint, other_endpoint == TextPatternRangeEndpoint_Start ? start : end); return S_OK;
  }
  IFACEMETHODIMP Select() override {
    const auto status = Validate(); if (FAILED(status)) return status;
    if ((node_.actions & (1ll << 11)) == 0) return UIA_E_NOTSUPPORTED;
    state_->Invoke(id_, 1ll << 11, L"{\"base\":" + std::to_wstring(start_) + L",\"extent\":" + std::to_wstring(end_) + L"}"); return S_OK;
  }
  IFACEMETHODIMP AddToSelection() override { return Select(); }
  IFACEMETHODIMP RemoveFromSelection() override { return UIA_E_INVALIDOPERATION; }
  IFACEMETHODIMP ScrollIntoView(BOOL) override {
    const auto status = Validate(); if (FAILED(status)) return status;
    if ((node_.actions & (1ll << 26)) == 0) return UIA_E_NOTSUPPORTED;
    state_->Invoke(id_, 1ll << 26, L"{\"base\":" + std::to_wstring(start_) + L",\"extent\":" + std::to_wstring(end_) + L"}"); return S_OK;
  }
  IFACEMETHODIMP GetChildren(SAFEARRAY** result) override {
    if (!result) return E_POINTER; *result = nullptr; const auto status = Validate(); if (FAILED(status)) return status;
    *result = SafeArrayCreateVector(VT_UNKNOWN, 0, 0); return *result ? S_OK : E_OUTOFMEMORY;
  }

 private:
  static bool Endpoint(TextPatternRangeEndpoint value) { return value == TextPatternRangeEndpoint_Start || value == TextPatternRangeEndpoint_End; }
  int Position(TextPatternRangeEndpoint endpoint) const { return endpoint == TextPatternRangeEndpoint_Start ? start_ : end_; }
  void SetPosition(TextPatternRangeEndpoint endpoint, int position) {
    if (endpoint == TextPatternRangeEndpoint_Start) { start_ = position; end_ = std::max(end_, start_); }
    else { end_ = position; start_ = std::min(start_, end_); }
  }
  int Snap(int value, bool forward = false) const {
    value = std::clamp(value, 0, static_cast<int>(node_.layout_text.size()));
    auto found = std::lower_bound(node_.text_boundaries.begin(), node_.text_boundaries.end(), value);
    if (found == node_.text_boundaries.end()) return static_cast<int>(node_.layout_text.size());
    if (forward || *found == value || found == node_.text_boundaries.begin()) return *found;
    return *std::prev(found);
  }
  std::vector<int> Units(TextUnit unit) const {
    if (unit == TextUnit_Character) return node_.text_boundaries;
    std::vector<int> values{0, static_cast<int>(node_.layout_text.size())};
    if (unit == TextUnit_Line) for (const auto& run : node_.text_runs) values.push_back(run.line_start);
    if (unit == TextUnit_Word || unit == TextUnit_Paragraph) {
      for (const auto boundary : node_.text_boundaries) if (boundary > 0 && boundary < node_.layout_text.size()) {
        const auto previous = node_.layout_text[boundary - 1], current = node_.layout_text[boundary];
        if (unit == TextUnit_Paragraph ? previous == L'\n' : std::iswspace(previous) != std::iswspace(current)) values.push_back(boundary);
      }
    }
    std::sort(values.begin(), values.end()); values.erase(std::unique(values.begin(), values.end()), values.end()); return values;
  }
  static int Index(const std::vector<int>& values, int position) { return std::max(0, static_cast<int>(std::upper_bound(values.begin(), values.end(), position) - values.begin()) - 1); }
  HRESULT Other(ITextRangeProvider* other, int& start, int& end) const {
    const auto status = Validate(); if (FAILED(status)) return status; if (!other) return E_INVALIDARG;
    Microsoft::WRL::ComPtr<IRangeIdentity> identity;
    if (FAILED(other->QueryInterface(__uuidof(IRangeIdentity), reinterpret_cast<void**>(identity.GetAddressOf())))) return E_INVALIDARG;
    void* owner; int id; uint64_t revision;
    const auto other_status = identity->Identity(&owner, &id, &revision, &start, &end);
    if (FAILED(other_status)) return other_status;
    return owner == state_.get() && id == id_ && revision == revision_ ? S_OK : E_INVALIDARG;
  }
  std::atomic<ULONG> references_{1};
  std::shared_ptr<AccessibilityBridge::State> state_;
  int id_, start_{}, end_{};
  AccessibilityNode node_;
  uint64_t revision_;
};

static HRESULT MakeTextRanges(std::shared_ptr<AccessibilityBridge::State> state, int id, AccessibilityNode node,
                              uint64_t revision, std::vector<std::pair<int, int>> spans, SAFEARRAY** result) {
  if (!result) return E_POINTER;
  *result = SafeArrayCreateVector(VT_UNKNOWN, 0, static_cast<ULONG>(spans.size())); if (!*result) return E_OUTOFMEMORY;
  LONG index = 0;
  for (const auto [start, end] : spans) {
    auto* range = new (std::nothrow) TextRange(state, id, node, revision, start, end);
    if (!range) { SafeArrayDestroy(*result); *result = nullptr; return E_OUTOFMEMORY; }
    const auto status = SafeArrayPutElement(*result, &index, static_cast<ITextRangeProvider*>(range)); range->Release();
    if (FAILED(status)) { SafeArrayDestroy(*result); *result = nullptr; return status; } ++index;
  }
  return S_OK;
}
HRESULT Provider::get_DocumentRange(ITextRangeProvider** result) {
  if (!result) return E_POINTER; *result = nullptr; AccessibilityNode node; uint64_t revision;
  if (!state_->SnapshotText(id_, node, revision)) return UIA_E_ELEMENTNOTAVAILABLE;
  const auto size = static_cast<int>(node.layout_text.size());
  *result = new (std::nothrow) TextRange(state_, id_, std::move(node), revision, 0, size); return *result ? S_OK : E_OUTOFMEMORY;
}
HRESULT Provider::get_SupportedTextSelection(SupportedTextSelection* result) {
  if (!result) return E_POINTER; AccessibilityNode node; if (!state_->Snapshot(id_, node)) return UIA_E_ELEMENTNOTAVAILABLE;
  *result = (node.actions & (1ll << 11)) != 0 ? SupportedTextSelection_Single : SupportedTextSelection_None; return S_OK;
}
HRESULT Provider::GetSelection(SAFEARRAY** result) {
  if (!result) return E_POINTER; *result = nullptr;
  AccessibilityNode node; uint64_t revision; if (!state_->SnapshotText(id_, node, revision)) return UIA_E_ELEMENTNOTAVAILABLE;
  std::vector<std::pair<int, int>> spans;
  if (node.selection_base >= 0 && node.selection_extent >= 0) spans.emplace_back(
      std::min(node.selection_base, node.selection_extent), std::max(node.selection_base, node.selection_extent));
  return MakeTextRanges(state_, id_, node, revision, std::move(spans), result);
}
HRESULT Provider::GetVisibleRanges(SAFEARRAY** result) {
  if (!result) return E_POINTER; *result = nullptr;
  AccessibilityNode node; uint64_t revision; if (!state_->SnapshotText(id_, node, revision)) return UIA_E_ELEMENTNOTAVAILABLE;
  std::vector<std::pair<int, int>> spans;
  for (const auto& run : node.text_runs) if (run.right > run.left && run.bottom > run.top) {
    if (!spans.empty() && spans.back().second >= run.start) spans.back().second = std::max(spans.back().second, run.end);
    else spans.emplace_back(run.start, run.end);
  }
  return MakeTextRanges(state_, id_, node, revision, std::move(spans), result);
}
HRESULT Provider::RangeFromChild(IRawElementProviderSimple*, ITextRangeProvider** result) {
  if (!result) return E_POINTER; *result = nullptr; return UIA_E_NOTSUPPORTED;
}
HRESULT Provider::RangeFromPoint(UiaPoint point, ITextRangeProvider** result) {
  if (!result) return E_POINTER; *result = nullptr; AccessibilityNode node; uint64_t revision;
  if (!state_->SnapshotText(id_, node, revision)) return UIA_E_ELEMENTNOTAVAILABLE;
  POINT origin{}; if (!ClientToScreen(state_->Window(), &origin)) return UIA_E_ELEMENTNOTAVAILABLE;
  const auto scale = state_->Scale(); const double x = (point.x - origin.x) / scale, y = (point.y - origin.y) / scale;
  double distance = std::numeric_limits<double>::infinity(); int offset = 0;
  for (const auto& run : node.text_runs) {
    if (run.right <= run.left || run.bottom <= run.top) continue;
    const double dx = x - std::clamp(x, run.left, run.right), dy = y - std::clamp(y, run.top, run.bottom);
    const auto current = dx * dx + dy * dy;
    if (current < distance) { distance = current; const bool trailing = (x >= (run.left + run.right) / 2) != run.rtl; offset = trailing ? run.end : run.start; }
  }
  *result = new (std::nothrow) TextRange(state_, id_, std::move(node), revision, offset, offset); return *result ? S_OK : E_OUTOFMEMORY;
}

bool AccessibilityBridge::ValidateTextForTest() {
  AccessibilityNode node;
  {
    std::lock_guard lock(state_->mutex);
    for (const auto& [id, candidate] : state_->nodes)
      if (candidate.text_geometry && !candidate.layout_text.empty() && !candidate.text_runs.empty()) { node = candidate; break; }
  }
  if (!node.text_geometry) return false;
  auto snapshot = std::make_shared<State>();
  snapshot->window = state_->Window(); snapshot->scale = state_->Scale(); snapshot->generation = 1;
  snapshot->nodes.emplace(node.id, node);
  Microsoft::WRL::ComPtr<Provider> provider; provider.Attach(new Provider(snapshot, node.id));
  Microsoft::WRL::ComPtr<ITextRangeProvider> range, clone;
  if (FAILED(provider->get_DocumentRange(range.GetAddressOf())) || FAILED(range->Clone(clone.GetAddressOf()))) return false;
  BOOL equal{}; if (FAILED(range->Compare(clone.Get(), &equal)) || !equal) return false;
  BSTR text{};
  if (FAILED(range->GetText(-1, &text))) return false;
  const bool matches = std::wstring(text, SysStringLen(text)) == node.layout_text; SysFreeString(text); if (!matches) return false;
  SAFEARRAY* rectangles{};
  if (FAILED(range->GetBoundingRectangles(&rectangles)) || !rectangles) return false;
  SafeArrayDestroy(rectangles);
  int moved{};
  if (FAILED(clone->MoveEndpointByUnit(TextPatternRangeEndpoint_Start, TextUnit_Character, 1, &moved)) || moved != 1) return false;
  Microsoft::WRL::ComPtr<IRangeIdentity> identity; if (FAILED(clone.As(&identity))) return false;
  void* owner{}; int id{}, start{}, end{}; uint64_t revision{};
  if (FAILED(identity->Identity(&owner, &id, &revision, &start, &end)) || start != node.text_boundaries[1]) return false;
  auto second = std::make_shared<State>(); second->nodes.emplace(node.id, node); second->generation = 1;
  Microsoft::WRL::ComPtr<Provider> second_provider; second_provider.Attach(new Provider(second, node.id));
  Microsoft::WRL::ComPtr<ITextRangeProvider> foreign; if (FAILED(second_provider->get_DocumentRange(foreign.GetAddressOf()))) return false;
  if (range->Compare(foreign.Get(), &equal) != E_INVALIDARG) return false;
  snapshot->generation++;
  if (range->GetText(-1, &text) != UIA_E_ELEMENTNOTAVAILABLE) return false;
  snapshot->alive = false;
  if (clone->GetText(-1, &text) != UIA_E_ELEMENTNOTAVAILABLE) return false;
  second->nodes[node.id].obscured = true;
  Microsoft::WRL::ComPtr<ITextProvider> password;
  return second_provider->QueryInterface(__uuidof(ITextProvider), reinterpret_cast<void**>(password.GetAddressOf())) == E_NOINTERFACE;
}
