#pragma once

#include <windows.h>
#include <dwmapi.h>
#include <algorithm>
#include <atomic>
#include <cstdint>
#include <cstdio>
#include <iterator>
#include <mutex>
#include <vector>

// Opt-in J0 observation only. No file IO on the sizing/render path and no
// compositor wait. qpcVBlank is a timing observation, not a display receipt.
namespace doroti::resize_trace {
struct Key {
  uint64_t epoch{};
  uint64_t generation{};
  uint32_t edge{};
  RECT outer{};
};
struct Event {
  const char* stage{};
  int64_t qpc{};
  DWORD thread{};
  Key key{};
  UINT flags{};
  int64_t vblank{};
  HRESULT timing_status{E_PENDING};
};
inline thread_local Key render_key;
inline wchar_t output_directory[32768]{};
inline const bool enabled = [] {
  const auto count = GetEnvironmentVariableW(L"DOROTI_WINDOWS_RESIZE_ORDER_TRACE",
      output_directory, static_cast<DWORD>(std::size(output_directory)));
  return count > 0 && count < std::size(output_directory);
}();
inline std::mutex gate;
inline std::vector<Event> events;
inline uint64_t dropped{};
inline constexpr size_t capacity = 32768;

inline void Record(const char* stage, Key key, UINT flags = 0,
                   bool observe_clock = false) {
  if (!enabled) return;
  LARGE_INTEGER now{};
  QueryPerformanceCounter(&now);
  Event event{stage, now.QuadPart, GetCurrentThreadId(), key, flags};
  if (observe_clock) {
    DWM_TIMING_INFO timing{};
    timing.cbSize = sizeof(timing);
    event.timing_status = DwmGetCompositionTimingInfo(nullptr, &timing);
    if (SUCCEEDED(event.timing_status)) event.vblank = timing.qpcVBlank;
  }
  std::lock_guard lock(gate);
  if (events.size() < capacity) events.push_back(event);
  else ++dropped;
}

inline void Initialize() {
  if (enabled) events.reserve(capacity);
}

struct MessageScope {
  const char* end{};
  Key key{};
  const uint64_t& generation;
  UINT flags{};
  ~MessageScope() {
    if (end != nullptr) {
      key.generation = generation;
      Record(end, key, flags);
    }
  }
};

inline void Flush() {
  if (!enabled) return;
  wchar_t path[32768]{};
  if (swprintf_s(path, L"%s\\resize-order-%lu.csv", output_directory,
                 GetCurrentProcessId()) < 0) return;
  FILE* file{};
  if (_wfopen_s(&file, path, L"w") != 0) return;
  LARGE_INTEGER frequency{};
  QueryPerformanceFrequency(&frequency);
  std::lock_guard lock(gate);
  std::sort(events.begin(), events.end(), [](const auto& a, const auto& b) {
    return a.qpc < b.qpc;
  });
  fprintf(file, "stage,qpc,frequency,thread,epoch,generation,edge,left,top,right,bottom,flags,vblank,timingStatus,dropped\n");
  for (const auto& e : events)
    fprintf(file, "%s,%lld,%lld,%lu,%llu,%llu,%u,%ld,%ld,%ld,%ld,%u,%lld,%ld,%llu\n",
        e.stage, e.qpc, frequency.QuadPart, e.thread, e.key.epoch,
        e.key.generation, e.key.edge, e.key.outer.left, e.key.outer.top,
        e.key.outer.right, e.key.outer.bottom, e.flags, e.vblank,
        e.timing_status, dropped);
  fclose(file);
}
}  // namespace doroti::resize_trace
