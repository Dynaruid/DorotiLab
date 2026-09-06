package dev.doroti.bridge;

import static org.junit.Assert.assertEquals;
import org.junit.Test;

public final class DorotiNativeCoreTest {
    @Test
    public void echoPreservesTheValue() {
        assertEquals("native", DorotiNativeCore.echo("native"));
    }
}
