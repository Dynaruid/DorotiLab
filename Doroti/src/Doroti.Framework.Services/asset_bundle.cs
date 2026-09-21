// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/asset_bundle.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Services;

public abstract class AssetBundle
{
    public abstract Future<ByteData> load(string key);

    public virtual async Future<ImmutableBuffer> loadBuffer(string key)
    {
        ByteData data = await load(key);
        return await DorotiUiLibrary.ImmutableBuffer.fromUint8List(new Uint8List(data));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual async Future<string> loadString(string key, bool cache = true)
    {
        ByteData data = await load(key);
        if ((data.lengthInBytes < (50L * 1024L)) || ConstantsLibrary.kIsWeb)
        {
            return Dart_convertLibrary.utf8.decode(new Uint8List(data));
        }
        return await IsolatesLibrary.compute(
            _utf8decode,
            data,
            debugLabel: $"UTF8 decode for \"{key}\""
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static string _utf8decode(ByteData data)
    {
        return Dart_convertLibrary.utf8.decode(new Uint8List(data));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual async Future<T> loadStructuredData<T>(string key, Func<string, object> parser)
    {
        return await DartAsyncRuntime.AwaitFutureOrValue<T>(parser(await loadString(key)));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual async Future<T> loadStructuredBinaryData<T>(
        string key,
        Func<ByteData, object> parser
    )
    {
        return await DartAsyncRuntime.AwaitFutureOrValue<T>(parser(await load(key)));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void evict(string key) { }

    public virtual void clear() { }

    public override string ToString() => $"{DiagnosticsLibrary.describeIdentity(this)}()";
}

public class NetworkAssetBundle : AssetBundle
{
    internal virtual DartUri _baseUrl { get; private set; } = default!;
    internal virtual Runtime.HttpClient _httpClient { get; private set; } = default!;

    public NetworkAssetBundle(DartUri baseUrl)
    {
        _baseUrl = baseUrl;
        _httpClient = new Runtime.HttpClient();
    }

    internal virtual DartUri _urlFromKey(string key) => _baseUrl.resolve(key);

    public override async Future<ByteData> load(string key)
    {
        HttpClientRequest request = await _httpClient.getUrl(_urlFromKey(key));
        HttpClientResponse response = await request.close();
        if (response.statusCode != HttpStatus.ok)
        {
            throw new FlutterError(
                new List<DiagnosticsNode>
                {
                    Asset_bundleLibrary._errorSummaryWithKey(key),
                    new IntProperty("HTTP status code", response.statusCode),
                }
            );
        }
        Uint8List bytes = await Consolidate_responseLibrary.consolidateHttpClientResponseBytes(
            response
        );
        return new ByteData(bytes);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() => $"{DiagnosticsLibrary.describeIdentity(this)}({_baseUrl})";
}

public abstract class CachingAssetBundle : AssetBundle
{
    internal virtual DartMap<string, Future<string>> _stringCache { get; private set; } =
        new DartMap<string, Future<string>>();
    internal virtual DartMap<string, Future> _structuredDataCache { get; private set; } =
        new DartMap<string, Future>();
    internal virtual DartMap<string, Future> _structuredBinaryDataCache { get; private set; } =
        new DartMap<string, Future>();

    public override Future<string> loadString(string key, bool cache = true)
    {
        if (cache)
        {
            return _stringCache.putIfAbsent(key, () => base.loadString(key));
        }
        return base.loadString(key);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Future<T> loadStructuredData<T>(string key, Func<string, object> parser)
    {
        if (_structuredDataCache.ContainsKey(key))
        {
            return ((Future<T>?)_structuredDataCache.GetValueOrDefault(key)!)!;
        }
        Completer<T>? completer = default!;
        Future<T>? synchronousResult = default!;
        _ = loadString(key, cache: false)
            .then<T>(parser)
            .then(
                (value) =>
                {
                    synchronousResult = new SynchronousFuture<T>(value);
                    _ = _structuredDataCache[key] = synchronousResult!;
                    completer?.complete(value);
                },
                onError: (error, stack) =>
                {
                    DartRuntimePrimitives.Assert(() => completer is not null);
                    _ = _structuredDataCache.remove(key);
                    completer!.completeError(error, stack);
                }
            );
        if (synchronousResult is not null)
        {
            return synchronousResult!;
        }
        completer = new Completer<T>();
        _ = _structuredDataCache[key] = completer.future;
        return completer.future;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Future<T> loadStructuredBinaryData<T>(string key, Func<ByteData, object> parser)
    {
        if (_structuredBinaryDataCache.ContainsKey(key))
        {
            return ((Future<T>?)_structuredBinaryDataCache.GetValueOrDefault(key)!)!;
        }
        Completer<T>? completer = default!;
        Future<T>? synchronousResult = default!;
        _ = load(key)
            .then<T>(parser)
            .then(
                (value) =>
                {
                    synchronousResult = new SynchronousFuture<T>(value);
                    _ = _structuredBinaryDataCache[key] = synchronousResult!;
                    completer?.complete(value);
                },
                onError: (error, stack) =>
                {
                    DartRuntimePrimitives.Assert(() => completer is not null);
                    _ = _structuredBinaryDataCache.remove(key);
                    completer!.completeError(error, stack);
                }
            );
        if (synchronousResult is not null)
        {
            return synchronousResult!;
        }
        completer = new Completer<T>();
        _ = _structuredBinaryDataCache[key] = completer.future;
        return completer.future;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void evict(string key)
    {
        _ = _stringCache.remove(key);
        _ = _structuredDataCache.remove(key);
        _ = _structuredBinaryDataCache.remove(key);
    }

    public override void clear()
    {
        _stringCache.Clear();
        _structuredDataCache.Clear();
        _structuredBinaryDataCache.Clear();
    }

    public override async Future<ImmutableBuffer> loadBuffer(string key)
    {
        ByteData data = await load(key);
        return await DorotiUiLibrary.ImmutableBuffer.fromUint8List(new Uint8List(data));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class PlatformAssetBundle : CachingAssetBundle
{
    public override Future<ByteData> load(string key)
    {
        Uint8List encoded = Dart_convertLibrary.utf8.encode(
            new DartUri(path: DartUri.encodeFull(key)).path
        );
        Future<ByteData>? future = ServicesBinding
            .instance.defaultBinaryMessenger.send("flutter/assets", new ByteData(encoded))
            ?.then(
                (asset) =>
                {
                    if (asset is null)
                    {
                        throw new FlutterError(
                            new List<DiagnosticsNode>
                            {
                                Asset_bundleLibrary._errorSummaryWithKey(key),
                                new ErrorDescription("The asset does not exist or has empty data."),
                            }
                        );
                    }
                    return asset;
                }
            );
        if (future is null)
        {
            throw new FlutterError(
                new List<DiagnosticsNode>
                {
                    Asset_bundleLibrary._errorSummaryWithKey(key),
                    new ErrorDescription("The asset does not exist or has empty data."),
                }
            );
        }
        return future;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override async Future<ImmutableBuffer> loadBuffer(string key)
    {
        if (ConstantsLibrary.kIsWeb)
        {
            ByteData bytes = await load(key);
            return await DorotiUiLibrary.ImmutableBuffer.fromUint8List(new Uint8List(bytes));
        }
        var debugUsePlatformChannel = false;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Platform.environment.ContainsKey("UNIT_TEST_ASSETS"))
            {
                debugUsePlatformChannel = true;
            }
            return true;
        });
        if (debugUsePlatformChannel)
        {
            ByteData bytesLocal = await load(key);
            return await DorotiUiLibrary.ImmutableBuffer.fromUint8List(new Uint8List(bytesLocal));
        }
        try
        {
            return await DorotiUiLibrary.ImmutableBuffer.fromAsset(key);
        }
        catch (Exception e)
        {
            throw new FlutterError(
                new List<DiagnosticsNode>
                {
                    Asset_bundleLibrary._errorSummaryWithKey(key),
                    new ErrorDescription(e.ToString()),
                }
            );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Asset_bundleLibrary
{
    internal static AssetBundle _initRootBundle()
    {
        return new PlatformAssetBundle();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Asset_bundleLibrary
{
    internal static ErrorSummary _errorSummaryWithKey(string key)
    {
        return new ErrorSummary($"Unable to load asset: \"{key}\".");
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Asset_bundleLibrary
{
    public static AssetBundle rootBundle = _initRootBundle();
}
