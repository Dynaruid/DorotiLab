import android.os.SystemClock;
import android.view.InputDevice;
import android.view.InputEvent;
import android.view.MotionEvent;
import java.lang.reflect.Method;

/** Shell-only test driver: one DOWN, timed MOVE samples, one UP.
 * Runs as adb shell with app_process, never inside or installed with the app.
 * Waypoints are x,y,elapsed-ms; all samples preserve the same downTime.
 */
public final class ScrollGesture {
    public static void main(String[] args) throws Exception {
        if (args.length < 2) throw new IllegalArgumentException("Supply at least two x,y,elapsed-ms waypoints");
        float[] x = new float[args.length], y = new float[args.length];
        long[] t = new long[args.length];
        for (int i = 0; i < args.length; i++) {
            String[] p = args[i].split(",");
            x[i] = Float.parseFloat(p[0]); y[i] = Float.parseFloat(p[1]); t[i] = Long.parseLong(p[2]);
            if (t[i] < 0 || t[i] > 15000 || (i > 0 && t[i] <= t[i-1]))
                throw new IllegalArgumentException("Waypoint times must increase within 15 seconds");
        }
        if (t[0] != 0) throw new IllegalArgumentException("First waypoint must have elapsed-ms 0");
        Class<?> type = Class.forName("android.hardware.input.InputManagerGlobal");
        Object manager = type.getMethod("getInstance").invoke(null);
        Method inject = type.getMethod("injectInputEvent", InputEvent.class, int.class);
        long down = SystemClock.uptimeMillis();
        boolean ended = false;
        int samples = 0, segment = 1;
        float lastX = x[0], lastY = y[0];
        try {
            send(manager, inject, down, down, MotionEvent.ACTION_DOWN, lastX, lastY);
            for (int sample = 1; ; sample++) {
                long target = down + Math.round(sample * 1000.0 / 120.0);
                long wait = target - SystemClock.uptimeMillis();
                if (wait > 0) SystemClock.sleep(wait);
                long now = SystemClock.uptimeMillis(), elapsed = now - down;
                if (elapsed >= t[t.length-1]) break;
                while (segment < t.length-1 && elapsed > t[segment]) segment++;
                float f = (float)(elapsed - t[segment-1]) / (t[segment] - t[segment-1]);
                lastX = x[segment-1] + (x[segment] - x[segment-1]) * f;
                lastY = y[segment-1] + (y[segment] - y[segment-1]) * f;
                send(manager, inject, down, now, MotionEvent.ACTION_MOVE, lastX, lastY);
                samples++;
            }
            send(manager, inject, down, SystemClock.uptimeMillis(), MotionEvent.ACTION_UP, x[x.length-1], y[y.length-1]);
            ended = true;
            System.out.println("continuous-down-ms=" + down + " moves=" + samples + " duration-ms=" + (SystemClock.uptimeMillis()-down));
        } finally {
            if (!ended) send(manager, inject, down, SystemClock.uptimeMillis(), MotionEvent.ACTION_CANCEL, lastX, lastY);
        }
    }

    private static void send(Object manager, Method inject, long down, long now, int action, float x, float y) throws Exception {
        MotionEvent event = MotionEvent.obtain(down, now, action, x, y, action == MotionEvent.ACTION_UP ? 0 : 1,
            1, 0, 1, 1, -1, 0);
        event.setSource(InputDevice.SOURCE_TOUCHSCREEN);
        try {
            if (!Boolean.TRUE.equals(inject.invoke(manager, event, 0)))
                throw new IllegalStateException("Input injection rejected");
        } finally { event.recycle(); }
    }
}
