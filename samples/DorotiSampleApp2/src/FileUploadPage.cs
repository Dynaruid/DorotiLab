using System.Text;
using Doroti.Cupertino;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using TextStyle = Doroti.Framework.Painting.TextStyle;
using WidgetImage = Doroti.Framework.Widgets.Image;

namespace DorotiSampleApp2;

internal sealed class FileUploadPage : StatefulWidget
{
    public override IState createState() => new FileUploadPageState();
}

internal sealed class FileUploadPageState : State<FileUploadPage>
{
    private static readonly FilePickOptions PickerOptions = new(AllowMultiple: true, Extensions: UploadPreviewReader.Extensions);
    private readonly CancellationTokenSource _lifetime = new();
    private readonly List<UploadPreview> _files = [];
    private DorotiView? _view;
    private IFilePickerHostCapability? _picker;
    private IOsDropRegistration? _registration;
    private bool _canDropFiles;
    private bool _active;
    private bool _hovering;
    private bool _busy;
    private bool _probeStarted;
    private string _status = "이미지와 텍스트 파일을 선택해 보세요.";

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        var view = View.of(context);
        if (_view != view)
        {
            _registration?.Dispose();
            _registration = null;
            _view = view;
            _picker = view.registeredCapabilityIds.Contains(DorotiCapabilityIds.FilePicker)
                ? view.RequireCapability<IFilePickerHostCapability>(DorotiCapabilityIds.FilePicker,
                    DorotiUiInvocation.Managed("File upload sample"))
                : null;
            var support = OsDragDrop.support(view);
            _canDropFiles = support.CanReceive && support.Actions.HasFlag(OsDropAction.Copy)
                && support.Formats.Contains(OsDropFormats.Files);
        }

        // Cupertino tabs retain their states offstage. Only the visible tab owns the receiver.
        _active = TickerMode.of(context);
        if (_active && _canDropFiles && _registration is null)
            _registration = OsDragDrop.register(view, new(OsDropAction.Copy, [OsDropFormats.Files]), Receive);
        else if (!_active)
        {
            _registration?.Dispose();
            _registration = null;
            _hovering = false;
        }
        if (_active && !_probeStarted && Environment.GetEnvironmentVariable("DOROTI_UPLOAD_PROBE") is { Length: > 0 })
        {
            _probeStarted = true;
            WidgetsBinding.instance.addPostFrameCallback(_ => PickFiles());
        }
    }

    private async void PickFiles()
    {
        if (_busy || _picker is null) return;
        setState(() => { _busy = true; _status = "파일을 선택하는 중…"; });
        FilePickResult? result = null;
        try
        {
            result = await _picker.PickFilesAsync(PickerOptions, _lifetime.Token);
            if (!mounted) return;
            if (result.Status == FilePickStatus.selected)
                await ImportFiles(result.Files, _lifetime.Token);
            else
                setState(() => _status = result.Status switch
                {
                    FilePickStatus.cancelled => "파일 선택을 취소했습니다.",
                    FilePickStatus.denied => "파일을 읽을 권한이 없습니다.",
                    FilePickStatus.unsupported => "이 환경에서는 파일 선택을 지원하지 않습니다.",
                    _ => $"파일 선택 실패: {result.Message ?? "다시 시도해 주세요."}",
                });
        }
        catch (OperationCanceledException) { }
        catch (Exception error) { if (mounted) setState(() => _status = $"파일 읽기 실패: {error.Message}"); }
        finally
        {
            if (result is not null)
                foreach (var file in result.Files) file.Dispose();
            if (mounted) setState(() => _busy = false);
            if (Environment.GetEnvironmentVariable("DOROTI_UPLOAD_PROBE") is { Length: > 0 } path)
                File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(
                    new UploadProbeResult(
                        result?.Status == FilePickStatus.selected && _files.Count > 0 ? "PASS" : "FAIL",
                        result?.Status.ToString(),
                        _files.Select(file => new UploadPreviewProbe(file.Name, file.Length, file.Text)).ToArray(),
                        result is not null, _status), UploadProbeJsonContext.Default.UploadProbeResult));
        }
    }

    private async void Receive(OsDropEvent item)
    {
        if (!mounted || !_active) { item.Data?.Dispose(); return; }
        var hovering = !_busy && item.Action == OsDropAction.Copy
            && item.Phase is OsDropPhase.Enter or OsDropPhase.Over;
        if (_hovering != hovering) setState(() => _hovering = hovering);
        if (item.Phase == OsDropPhase.Error)
        {
            setState(() => _status = $"드롭 실패: {item.Message ?? item.Failure.ToString()}");
            return;
        }
        if (item.Data is not { } data) return;
        using (data)
        {
            if (_busy) { setState(() => _status = "파일을 읽고 있습니다. 완료 후 다시 놓아 주세요."); return; }
            setState(() => { _busy = true; _status = "드롭한 파일을 읽는 중…"; });
            using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token, data.Lifetime);
            try { await ImportFiles(data.Files, cancellation.Token); }
            catch (OperationCanceledException)
            {
                if (mounted) setState(() => _status = "파일 읽기를 취소했습니다.");
            }
            catch (Exception error) { if (mounted) setState(() => _status = $"파일 읽기 실패: {error.Message}"); }
            finally { if (mounted) setState(() => _busy = false); }
        }
    }

    private async Task ImportFiles(IReadOnlyList<IPickedFile> files, CancellationToken token)
    {
        var added = 0;
        var errors = new List<string>();
        foreach (var file in files)
        {
            token.ThrowIfCancellationRequested();
            if (_files.Count >= UploadPreviewReader.MaxFiles)
            {
                errors.Add($"최대 {UploadPreviewReader.MaxFiles}개까지 추가할 수 있습니다. 목록에서 파일을 지운 뒤 다시 시도하세요.");
                break;
            }
            try
            {
                var preview = await UploadPreviewReader.ReadAsync(file, token);
                token.ThrowIfCancellationRequested();
                if (!mounted) return;
                setState(() => _files.Add(preview));
                added++;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception error) { errors.Add($"{file.Name}: {error.Message}"); }
        }
        if (mounted) setState(() => _status = string.Join("\n",
            new[] { $"{added}개 파일 추가 · 현재 {_files.Count}개" }.Concat(errors)));
    }

    public override Widget build(BuildContext context)
    {
        var blue = CupertinoColors.systemBlue.resolveFrom(context);
        var secondary = CupertinoColors.secondaryLabel.resolveFrom(context);
        return new CupertinoPageScaffold(
            navigationBar: new CupertinoNavigationBar(middle: new Text("File Upload")),
            backgroundColor: CupertinoColors.systemGroupedBackground.resolveFrom(context),
            child: new SafeArea(child: new SingleChildScrollView(child: new Center(
                child: new ConstrainedBox(constraints: new BoxConstraints(maxWidth: 680),
                    child: new Padding(padding: EdgeInsets.CreateAll(20), child: new Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch, children:
                        [
                            new Text("파일 업로드 예제", style: new TextStyle(fontSize: 26, fontWeight: FontWeight.bold)),
                            new SizedBox(height: 8),
                            new Text("파일을 읽어 이 페이지에서 미리 봅니다. 서버로 전송하지 않으며 앱을 닫으면 목록이 초기화됩니다.",
                                style: new TextStyle(color: secondary)),
                            new SizedBox(height: 20),
                            new Container(padding: EdgeInsets.CreateAll(24), decoration: new BoxDecoration(
                                color: _hovering ? CupertinoColors.systemBlue.withValues(alpha: 0.12)
                                    : CupertinoColors.secondarySystemGroupedBackground.resolveFrom(context),
                                border: Border.CreateAll(color: _hovering ? blue : CupertinoColors.separator.resolveFrom(context),
                                    width: _hovering ? 2 : 1), borderRadius: BorderRadius.CreateCircular(16)),
                                child: new Column(children:
                                [
                                    new Icon(CupertinoIcons.tray_arrow_up, size: 44, color: blue),
                                    new SizedBox(height: 12),
                                    new Text(_hovering ? "파일을 놓아 주세요" : "이미지 또는 텍스트 파일 추가",
                                        textAlign: TextAlign.center, style: new TextStyle(fontSize: 20, fontWeight: FontWeight.w600)),
                                    new SizedBox(height: 8),
                                    new Text(_canDropFiles ? "Upload 탭의 페이지 안에 파일을 드래그해서 놓거나 아래 버튼을 누르세요."
                                        : "이 환경에서는 파일 드롭을 지원하지 않습니다. 아래 버튼으로 파일을 선택하세요.",
                                        textAlign: TextAlign.center, style: new TextStyle(color: secondary)),
                                    new SizedBox(height: 16),
                                    new FilePickerActivation(options: PickerOptions,
                                        child: CupertinoButton.CreateFilled(child: new Text("파일 선택"),
                                            onPressed: _busy || _picker is null ? null : PickFiles)),
                                    .. (_picker is null ? new Widget[] { new Text("이 환경에서는 파일 선택을 지원하지 않습니다.") } : []),
                                    .. (_busy ? new Widget[] { new Padding(padding: EdgeInsets.CreateOnly(top: 12), child: new CupertinoActivityIndicator()) } : []),
                                ])),
                            new SizedBox(height: 12),
                            new Text("PNG · JPG · JPEG · GIF · WEBP · BMP / TXT · MD · CSV · JSON · LOG\n이미지 최대 10 MiB · 텍스트 최대 1 MiB · 총 8개",
                                style: new TextStyle(fontSize: 13, color: secondary)),
                            new SizedBox(height: 12),
                            new Text(_status, style: new TextStyle(fontSize: 14)),
                            new SizedBox(height: 16),
                            new Row(children:
                            [
                                new Expanded(child: new Text($"추가한 파일 ({_files.Count})", style: new TextStyle(fontWeight: FontWeight.w600))),
                                new CupertinoButton(child: new Text("모두 지우기"),
                                    onPressed: _busy || _files.Count == 0 ? null : () => setState(() =>
                                    { _files.Clear(); _status = "목록을 비웠습니다."; })),
                            ]),
                            .. (_files.Count == 0 ? new Widget[] { new Padding(padding: EdgeInsets.CreateSymmetric(vertical: 24),
                                child: new Text("추가한 파일이 여기에 표시됩니다.", textAlign: TextAlign.center,
                                    style: new TextStyle(color: secondary))) } : []),
                            .. _files.Select(file => PreviewCard(context, file)),
                            new SizedBox(height: 24),
                        ])))))));
    }

    private Widget PreviewCard(BuildContext context, UploadPreview file) => new Padding(
        padding: EdgeInsets.CreateOnly(bottom: 12), child: new Container(padding: EdgeInsets.CreateAll(16),
            decoration: new BoxDecoration(color: CupertinoColors.secondarySystemGroupedBackground.resolveFrom(context),
                borderRadius: BorderRadius.CreateCircular(12)), child: new Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch, children:
                    [
                        new Row(children:
                        [
                            new Icon(file.ImageBytes is null ? CupertinoIcons.doc_text : CupertinoIcons.photo,
                                color: CupertinoColors.systemBlue.resolveFrom(context)),
                            new SizedBox(width: 10),
                            new Expanded(child: new Column(crossAxisAlignment: CrossAxisAlignment.start, children:
                            [
                                new Text(file.Name, maxLines: 2, overflow: TextOverflow.ellipsis,
                                    style: new TextStyle(fontWeight: FontWeight.w600)),
                                new Text($"{file.Length:N0} bytes · {(file.ImageBytes is null ? "텍스트" : "이미지")}",
                                    style: new TextStyle(fontSize: 13, color: CupertinoColors.secondaryLabel.resolveFrom(context))),
                            ])),
                            new CupertinoButton(child: new Text("삭제"), onPressed: _busy ? null
                                : () => setState(() => _files.Remove(file))),
                        ]),
                        new SizedBox(height: 12),
                        file.ImageBytes is { } bytes
                            ? WidgetImage.CreateMemory(new Uint8List(bytes), height: 220, fit: BoxFit.contain,
                                cacheWidth: 1024, semanticLabel: file.Name,
                                errorBuilder: (_, _, _) => new Padding(padding: EdgeInsets.CreateAll(16),
                                    child: new Text("이미지 미리보기 실패: 손상된 파일이거나 지원하지 않는 이미지 형식입니다.")))
                            : new Text(file.Text!, style: new TextStyle(fontSize: 14)),
                        .. (file.TextTruncated ? new Widget[] { new Padding(padding: EdgeInsets.CreateOnly(top: 8),
                            child: new Text("미리보기는 처음 4,000자까지 표시합니다.",
                                style: new TextStyle(fontSize: 12, color: CupertinoColors.secondaryLabel.resolveFrom(context)))) } : []),
                    ])));

    public override void dispose()
    {
        _lifetime.Cancel();
        _registration?.Dispose();
        _lifetime.Dispose();
        base.dispose();
    }
}

internal sealed record UploadPreview(string Name, long Length, byte[]? ImageBytes = null,
    string? Text = null, bool TextTruncated = false);

internal static class UploadPreviewReader
{
    internal const int MaxFiles = 8;
    // Windows and Qt require dotted extensions; the browser and mobile hosts also accept them.
    internal static readonly string[] Extensions = [".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".txt", ".md", ".csv", ".json", ".log"];
    private const int TextPreviewLength = 4000;

    internal static async Task<UploadPreview> ReadAsync(IPickedFile file, CancellationToken token)
    {
        var extension = System.IO.Path.GetExtension(file.Name).ToLowerInvariant();
        if (!Extensions.Contains(extension)) throw new InvalidDataException("지원하지 않는 파일 형식입니다.");
        var image = extension is ".png" or ".jpg" or ".jpeg" or ".gif" or ".webp" or ".bmp";
        var limit = (image ? 10 : 1) * 1024 * 1024;
        if (file.Length < 0 || file.Length > limit)
            throw new InvalidDataException($"파일 크기를 확인할 수 없거나 최대 {(image ? 10 : 1)} MiB를 초과했습니다.");
        var bytes = new byte[(int)file.Length];
        var offset = 0;
        while (offset < bytes.Length)
        {
            token.ThrowIfCancellationRequested();
            var count = await file.ReadAsync(offset, bytes.AsMemory(offset, Math.Min(65536, bytes.Length - offset)), token);
            if (count <= 0) throw new EndOfStreamException("파일을 끝까지 읽지 못했습니다.");
            offset += count;
        }
        token.ThrowIfCancellationRequested();
        if (image) return new(file.Name, file.Length, ImageBytes: bytes);
        using var stream = new MemoryStream(bytes);
        using var reader = new StreamReader(stream, new UTF8Encoding(false, true), detectEncodingFromByteOrderMarks: true);
        string text;
        try { text = await reader.ReadToEndAsync(token); }
        catch (DecoderFallbackException) { throw new InvalidDataException("UTF-8 또는 BOM이 있는 UTF-16 텍스트 파일을 선택하세요."); }
        if (text.Contains('\0')) throw new InvalidDataException("텍스트 파일에 바이너리 데이터가 포함되어 있습니다.");
        var truncated = text.Length > TextPreviewLength;
        // Keep a UTF-16 surrogate pair together at the preview boundary.
        var length = truncated && char.IsHighSurrogate(text[TextPreviewLength - 1]) ? TextPreviewLength - 1 : TextPreviewLength;
        return new(file.Name, file.Length, Text: truncated ? text[..length] : text, TextTruncated: truncated);
    }
}
