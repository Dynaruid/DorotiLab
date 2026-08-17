using Microsoft.Maui.Controls.Shapes;

namespace MauiSampleApp;

public sealed class DorotiOcrPage : ContentPage
{
    private readonly Image _preview = new()
    {
        Aspect = Aspect.AspectFit,
        BackgroundColor = Color.FromArgb("#2B2930"),
        HeightRequest = 280,
    };

    private readonly Editor _result = new()
    {
        IsReadOnly = true,
        AutoSize = EditorAutoSizeOption.TextChanges,
        Placeholder = "Recognized text will appear here.",
        FontFamily = "OpenSansRegular",
        FontSize = 16,
    };

    private readonly Label _status = new()
    {
        FontSize = 13,
        TextColor = Color.FromArgb("#CAC4D0"),
        LineBreakMode = LineBreakMode.WordWrap,
    };

    private readonly ActivityIndicator _busy = new()
    {
        IsRunning = false,
        IsVisible = false,
        Color = Color.FromArgb("#D0BCFF"),
        HeightRequest = 24,
        WidthRequest = 24,
        HorizontalOptions = LayoutOptions.Start,
    };

    private byte[]? _imageBytes;

    public DorotiOcrPage()
    {
        Title = "Doroti OCR";
        BackgroundColor = Color.FromArgb("#1D1B20");
        _status.Text = "Load the bundled sample, pick a photo, or capture one. Android uses ML Kit; iOS/Mac Catalyst use Vision.";

        var sampleButton = CreateActionButton("Sample");
        sampleButton.Clicked += async (_, _) => await LoadSampleAsync();
        var galleryButton = CreateActionButton("Gallery");
        galleryButton.Clicked += async (_, _) => await PickPhotoAsync();
        var cameraButton = CreateActionButton("Camera");
        cameraButton.Clicked += async (_, _) => await CapturePhotoAsync();
        var recognizeButton = new Button
        {
            Text = "Recognize text",
            BackgroundColor = Color.FromArgb("#D0BCFF"),
            TextColor = Color.FromArgb("#381E72"),
            CornerRadius = 12,
            HeightRequest = 44,
        };
        recognizeButton.Clicked += async (_, _) => await RecognizeAsync();

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 16,
                Children =
                {
                    new Label
                    {
                        Text = "Native Library Interop OCR",
                        FontFamily = "OpenSansSemibold",
                        FontSize = 24,
                        TextColor = Colors.White,
                    },
                    new Label
                    {
                        Text = "Android binds a slim Java wrapper around com.google.mlkit:text-recognition and text-recognition-korean. iOS/Mac Catalyst wrap Vision. Windows uses Windows.Media.Ocr.",
                        FontSize = 14,
                        TextColor = Color.FromArgb("#CAC4D0"),
                        LineBreakMode = LineBreakMode.WordWrap,
                    },
                    new Border
                    {
                        Stroke = Color.FromArgb("#49454F"),
                        StrokeThickness = 1,
                        StrokeShape = new RoundRectangle { CornerRadius = 16 },
                        BackgroundColor = Color.FromArgb("#2B2930"),
                        Padding = 8,
                        Content = _preview,
                    },
                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            sampleButton,
                            galleryButton,
                            cameraButton,
                        },
                    },
                    recognizeButton,
                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            _busy,
                            _status,
                        },
                    },
                    _result,
                },
            },
        };
    }

    private static Button CreateActionButton(string text) => new()
    {
        Text = text,
        BackgroundColor = Color.FromArgb("#4A4458"),
        TextColor = Colors.White,
        CornerRadius = 12,
        HeightRequest = 40,
        Padding = new Thickness(16, 0),
    };

    private async Task LoadSampleAsync()
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync("ocr-sample.png");
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        ShowImage(memory.ToArray());
        _status.Text = "Loaded bundled Korean/English sample image.";
    }

    private async Task PickPhotoAsync()
    {
        var photos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions { SelectionLimit = 1 });
        var photo = photos?.FirstOrDefault();
        if (photo is null)
        {
            return;
        }

        ShowImage(await ReadAllBytesAsync(photo));
        _status.Text = "Photo selected. Tap Recognize text.";
    }

    private async Task CapturePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            _status.Text = "Camera capture is not supported on this device.";
            return;
        }

        var photo = await MediaPicker.Default.CapturePhotoAsync();
        if (photo is null)
        {
            return;
        }

        ShowImage(await ReadAllBytesAsync(photo));
        _status.Text = "Photo captured. Tap Recognize text.";
    }

    private async Task RecognizeAsync()
    {
        if (_imageBytes is null)
        {
            await LoadSampleAsync();
        }

        SetBusy(true, "Running on-device OCR...");
        try
        {
            var text = await DorotiNativeOcrInterop.RecognizeAsync(_imageBytes!, "auto");
            _result.Text = string.IsNullOrWhiteSpace(text) ? "(no text found)" : text;
            _status.Text = $"Done via {DorotiNativeOcrInterop.EngineName}.";
        }
        catch (Exception ex)
        {
            _result.Text = string.Empty;
            _status.Text = ex.Message;
        }
        finally
        {
            SetBusy(false, _status.Text);
        }
    }

    private void ShowImage(byte[] bytes)
    {
        _imageBytes = bytes;
        _preview.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
    }

    private void SetBusy(bool busy, string status)
    {
        _busy.IsRunning = busy;
        _busy.IsVisible = busy;
        _status.Text = status;
    }

    private static async Task<byte[]> ReadAllBytesAsync(FileResult file)
    {
        await using var stream = await file.OpenReadAsync();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        return memory.ToArray();
    }
}
