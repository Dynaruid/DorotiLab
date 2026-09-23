# HTTP loading contracts

Run with the repository's 20-minute process-tree timeout:

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/http-loading/HttpLoading.csproj -c Release
```

This regression harness uses an in-memory `HttpMessageHandler` and observable
response streams; it makes no external network requests.

- Both request paths (default and explicit `TimeProvider`) forward authentication,
  custom multi-value headers and client defaults. Request credentials do not
  escape into the shared client or a subsequent request.
- Reading preserves bytes across multiple chunks and releases the response on
  completion, early exit, read failure and cancellation.
- Error-body draining preserves its result, uses a stream instead of buffering
  the entire body, propagates read failures and releases the response.
- Explicit disposal releases unread responses, including rejected asset loads.

## Review on 2026-09-23

The active Painting project excludes `_network_image_web.cs`; its compiled
`NetworkImageIo` and Services' `NetworkAssetBundle` use the Runtime HTTP adapter.
The adapter previously recorded headers but called `GetAsync` without applying
them. Response enumeration/draining also left `HttpResponseMessage` undisposed,
and `drain` buffered the complete error body. These are separate from the frame
cost and renderer memory work in the [archived summary](../../../history/26-09-22/web-frame-cost-and-memory-summary.md).

The original eight regression cases failed before the fix and passed afterward.
An additional explicit unread-response disposal case covers the ownership API
used by the two consumers. The fix keeps per-request headers on a disposable
`HttpRequestMessage`, releases responses on stream termination, and scopes both
consumer responses so rejection before enumeration also releases resources.

Final verification: all nine contracts passed in Release, and the affected
`Doroti.Framework.Painting` project (including Runtime and Services dependencies)
built in Release with zero warnings and zero errors. `git diff --check` passed.

These contracts verify managed HTTP behavior. Browser CORS, real network servers,
physical devices and rendered images remain `notVerified`; no frame-rate or
device-memory improvement is claimed.
