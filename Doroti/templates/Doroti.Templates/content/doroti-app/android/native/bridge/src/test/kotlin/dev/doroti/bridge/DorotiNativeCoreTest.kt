package dev.doroti.bridge

import org.junit.Assert.assertEquals
import org.junit.Test

class DorotiNativeCoreTest {
    @Test
    fun echoPreservesTheValue() = assertEquals("native", DorotiNativeCore.echo("native"))
}
