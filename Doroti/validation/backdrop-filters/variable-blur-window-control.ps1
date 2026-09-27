param([ValidateSet('observe','size','click','scroll','close')][string]$Action='observe',
    [int]$X=0,[int]$Y=0,[int]$Width=2560,[int]$Height=1600,[int]$Delta=-120,
    [int]$Count=1,[int]$IntervalMs=16,[string]$Name='native-window',
    [string]$ArtifactDirectory=(Join-Path $PSScriptRoot '../../artifacts/variable-blur-performance'))
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class BlurWindowProbe {
 public delegate bool EnumProc(IntPtr hwnd,IntPtr state);
 [DllImport("user32.dll")] public static extern bool EnumWindows(EnumProc fn,IntPtr state);
 [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr hwnd,out uint pid);
 [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern int GetWindowText(IntPtr hwnd,System.Text.StringBuilder text,int count);
 [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hwnd,int command);
 public static IntPtr Find(uint pid) { IntPtr found=IntPtr.Zero; EnumWindows((h,s)=>{uint p; GetWindowThreadProcessId(h,out p); if(p==pid){ var t=new System.Text.StringBuilder(512); GetWindowText(h,t,512); if(t.ToString()=="Doroti Cupertino Sample") { found=h; return false; }} return true;},IntPtr.Zero); return found; }
 [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left,Top,Right,Bottom; }
 [StructLayout(LayoutKind.Sequential)] public struct POINT { public int X,Y; }
 [StructLayout(LayoutKind.Sequential)] public struct MOUSEINPUT { public int dx,dy; public uint mouseData,dwFlags,time; public IntPtr dwExtraInfo; }
 [StructLayout(LayoutKind.Sequential)] public struct INPUT { public uint type; public MOUSEINPUT mi; }
 [DllImport("user32.dll")] public static extern bool SetProcessDpiAwarenessContext(IntPtr value);
 [DllImport("user32.dll")] public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr value);
 [DllImport("user32.dll")] public static extern bool GetCursorPos(out POINT point);
 [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
 [DllImport("user32.dll")] public static extern IntPtr WindowFromPoint(POINT point);
 public static bool OwnsPoint(POINT point,uint pid) { uint owner; GetWindowThreadProcessId(WindowFromPoint(point),out owner); return owner==pid; }
 [DllImport("user32.dll")] public static extern uint GetDpiForWindow(IntPtr hwnd);
 public static POINT MoveClient(IntPtr hwnd,int x,int y) { SetThreadDpiAwarenessContext(new IntPtr(-4)); var p=new POINT { X=x,Y=y }; ClientToScreen(hwnd,ref p); SetCursorPos(p.X,p.Y); GetCursorPos(out p); return p; }
 [DllImport("winmm.dll")] public static extern uint timeBeginPeriod(uint ms);
 [DllImport("winmm.dll")] public static extern uint timeEndPeriod(uint ms);
 public static double[] Wheel(IntPtr hwnd,int count,int interval,int delta) {
  var result=new double[count]; var timer=System.Diagnostics.Stopwatch.StartNew();
  timeBeginPeriod(1);
  try { for(int i=0;i<count;i++) { while(timer.Elapsed.TotalMilliseconds<i*interval) System.Threading.Thread.Sleep(1);
    if(GetForegroundWindow()!=hwnd) throw new Exception("Sample lost foreground; input aborted"); Mouse(0x800,delta); result[i]=(double)System.Diagnostics.Stopwatch.GetTimestamp()/System.Diagnostics.Stopwatch.Frequency; }
  } finally {timeEndPeriod(1);} return result;
 }
 [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr hwnd,out RECT rect);
 [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hwnd,out RECT rect);
 [DllImport("user32.dll")] public static extern bool ClientToScreen(IntPtr hwnd,ref POINT point);
 [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr hwnd,IntPtr after,int x,int y,int cx,int cy,uint flags);
 [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hwnd);
 [DllImport("user32.dll")] public static extern bool SetCursorPos(int x,int y);
 [DllImport("user32.dll")] public static extern uint SendInput(uint count,INPUT[] input,int size);
 [DllImport("user32.dll")] public static extern bool PostMessage(IntPtr hwnd,uint msg,IntPtr wp,IntPtr lp);
 public static void Mouse(uint flags,int delta=0) { var input=new INPUT { mi=new MOUSEINPUT { dwFlags=flags, mouseData=unchecked((uint)delta) } }; if(SendInput(1,new[]{input},Marshal.SizeOf<INPUT>())!=1) throw new Exception("SendInput failed"); }
}
'@
[BlurWindowProbe]::SetProcessDpiAwarenessContext([IntPtr](-4)) | Out-Null
[BlurWindowProbe]::SetThreadDpiAwarenessContext([IntPtr](-4)) | Out-Null
$artifact=(Resolve-Path $ArtifactDirectory).Path
$samplePid=[int](Get-Content (Join-Path $artifact 'native-pid.txt'))
$sample=Get-Process -Id $samplePid
if($sample.ProcessName -ne 'DorotiSampleApp2.WindowsAppSdk'){throw 'Unexpected process'}
$hwnd=$sample.MainWindowHandle
if($hwnd -eq 0){$hwnd=[BlurWindowProbe]::Find($samplePid)}
if($hwnd -eq 0){throw 'Sample has not created a titled window'}
[BlurWindowProbe]::ShowWindow($hwnd,5) | Out-Null
[BlurWindowProbe]::SetForegroundWindow($hwnd) | Out-Null
if($Action -in @('click','scroll') -and [BlurWindowProbe]::GetForegroundWindow() -ne $hwnd){
 $shell=New-Object -ComObject WScript.Shell
 try {$shell.AppActivate($samplePid) | Out-Null} finally {[Runtime.InteropServices.Marshal]::ReleaseComObject($shell) | Out-Null}
 Start-Sleep -Milliseconds 100
}
$client=New-Object BlurWindowProbe+RECT
$outer=New-Object BlurWindowProbe+RECT
[BlurWindowProbe]::GetClientRect($hwnd,[ref]$client) | Out-Null
[BlurWindowProbe]::GetWindowRect($hwnd,[ref]$outer) | Out-Null
if($Action -eq 'size'){
 [BlurWindowProbe]::SetWindowPos($hwnd,[IntPtr]::Zero,20,20,$Width+$outer.Right-$outer.Left-$client.Right,$Height+$outer.Bottom-$outer.Top-$client.Bottom,0x40) | Out-Null
 Start-Sleep -Milliseconds 700
}
if($Action -in @('click','scroll')){
 $point=[BlurWindowProbe]::MoveClient($hwnd,$X,$Y)
 if(-not [BlurWindowProbe]::OwnsPoint($point,$samplePid)){throw 'Input point is not owned by the sample; input aborted'}
 Write-Output ('Input screen position: '+$point.X+','+$point.Y)
 if($Action -eq 'click'){[BlurWindowProbe]::Mouse(2); [BlurWindowProbe]::Mouse(4)}
 else {
  $inputTimes=[BlurWindowProbe]::Wheel($hwnd,$Count,$IntervalMs,$Delta)
  $inputTimes | ConvertTo-Json | Set-Content (Join-Path $artifact ($Name+'-input.json'))
 }
 Start-Sleep -Milliseconds 400
}
if($Action -eq 'close'){[BlurWindowProbe]::PostMessage($hwnd,0x10,[IntPtr]::Zero,[IntPtr]::Zero) | Out-Null; return}
[BlurWindowProbe]::GetClientRect($hwnd,[ref]$client) | Out-Null
$point=New-Object BlurWindowProbe+POINT
[BlurWindowProbe]::ClientToScreen($hwnd,[ref]$point) | Out-Null
$bitmap=[Drawing.Bitmap]::new($client.Right,$client.Bottom)
$graphics=[Drawing.Graphics]::FromImage($bitmap)
try { $graphics.CopyFromScreen($point.X,$point.Y,0,0,$bitmap.Size); $bitmap.Save((Join-Path $artifact ($Name+'.png'))) }
finally {$graphics.Dispose();$bitmap.Dispose()}
$state=[ordered]@{pid=$samplePid;hwnd=$hwnd.ToInt64();dpi=[BlurWindowProbe]::GetDpiForWindow($hwnd);clientWidth=$client.Right;clientHeight=$client.Bottom;originX=$point.X;originY=$point.Y;screens=@([Windows.Forms.Screen]::AllScreens | ForEach-Object { $_.Bounds.ToString() })}
$state | ConvertTo-Json | Set-Content (Join-Path $artifact "native-state.json")
$state | ConvertTo-Json
