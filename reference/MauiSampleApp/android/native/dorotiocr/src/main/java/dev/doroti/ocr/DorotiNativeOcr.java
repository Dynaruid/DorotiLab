package dev.doroti.ocr;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

import com.google.android.gms.tasks.Tasks;
import com.google.mlkit.vision.common.InputImage;
import com.google.mlkit.vision.text.Text;
import com.google.mlkit.vision.text.TextRecognition;
import com.google.mlkit.vision.text.TextRecognizer;
import com.google.mlkit.vision.text.korean.KoreanTextRecognizerOptions;
import com.google.mlkit.vision.text.latin.TextRecognizerOptions;

import java.util.concurrent.TimeUnit;

public final class DorotiNativeOcr {
    private DorotiNativeOcr() {
    }

    public interface Callback {
        void onSuccess(String text);

        void onFailure(String message);
    }

    public static void recognize(byte[] imageBytes, String script, Callback callback) {
        Thread worker = new Thread(() -> {
            try {
                callback.onSuccess(recognizeBlocking(imageBytes, script));
            } catch (Exception ex) {
                String message = ex.getMessage();
                callback.onFailure(message == null || message.isEmpty() ? ex.toString() : message);
            }
        }, "doroti-ocr");
        worker.setDaemon(true);
        worker.start();
    }

    static String recognizeBlocking(byte[] imageBytes, String script) throws Exception {
        if (imageBytes == null || imageBytes.length == 0) {
            throw new IllegalArgumentException("Image bytes were empty.");
        }

        Bitmap bitmap = BitmapFactory.decodeByteArray(imageBytes, 0, imageBytes.length);
        if (bitmap == null) {
            throw new IllegalArgumentException("Unable to decode image bytes.");
        }

        InputImage image = InputImage.fromBitmap(bitmap, 0);
        String mode = script == null ? "auto" : script.trim().toLowerCase();
        String latin = "";
        String korean = "";

        if (mode.equals("latin") || mode.equals("auto") || mode.isEmpty()) {
            latin = recognizeWith(image, TextRecognition.getClient(TextRecognizerOptions.DEFAULT_OPTIONS));
        }

        if (mode.equals("korean") || mode.equals("auto") || mode.isEmpty()) {
            korean = recognizeWith(
                image,
                TextRecognition.getClient(new KoreanTextRecognizerOptions.Builder().build()));
        }

        if (!mode.equals("latin") && !mode.equals("korean") && !mode.equals("auto") && !mode.isEmpty()) {
            throw new IllegalArgumentException("Unsupported script '" + script + "'. Use latin, korean, or auto.");
        }

        return merge(latin, korean);
    }

    private static String recognizeWith(InputImage image, TextRecognizer recognizer) throws Exception {
        try {
            Text text = Tasks.await(recognizer.process(image), 30, TimeUnit.SECONDS);
            String value = text.getText();
            return value == null ? "" : value.trim();
        } finally {
            recognizer.close();
        }
    }

    private static String merge(String first, String second) {
        if (first == null || first.isEmpty()) {
            return second == null ? "" : second;
        }

        if (second == null || second.isEmpty() || first.equals(second) || first.contains(second)) {
            return first;
        }

        if (second.contains(first)) {
            return second;
        }

        return first + "\n" + second;
    }
}
