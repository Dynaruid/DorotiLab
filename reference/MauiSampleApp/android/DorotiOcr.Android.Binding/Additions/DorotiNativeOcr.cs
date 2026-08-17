using System.Threading.Tasks;

namespace DorotiOcrAndroid;

public partial class DorotiNativeOcr
{
    public static Task<string> RecognizeAsync(byte[] imageBytes, string script = "auto")
    {
        var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        Recognize(imageBytes, script, new RecognizeCallback(completion));
        return completion.Task;
    }

    sealed class RecognizeCallback : Java.Lang.Object, ICallback
    {
        private readonly TaskCompletionSource<string> _completion;

        public RecognizeCallback(TaskCompletionSource<string> completion)
        {
            _completion = completion;
        }

        public void OnSuccess(string? text) => _completion.TrySetResult(text ?? string.Empty);

        public void OnFailure(string? message) =>
            _completion.TrySetException(new InvalidOperationException(message ?? "OCR failed."));
    }
}
