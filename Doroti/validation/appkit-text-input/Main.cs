using AppKit;
using CoreGraphics;
using Doroti.Host.Maui;
using Doroti.Ui;
using Foundation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platforms.MacOS.Hosting;

NSApplication.Init();
using var app = MauiApp.CreateBuilder().UseMauiAppMacOS<FixtureApplication>().Build();
var context = new MauiContext(app.Services);
var layout = new Grid();
var handler = new DorotiMacOSLayoutHandler();
handler.SetMauiContext(context);
layout.Handler = handler;
using var window = new NSWindow(new CGRect(0, 0, 400, 200), NSWindowStyle.Titled, NSBackingStore.Buffered, false);
window.ContentView = handler.PlatformView;
var bridge = new MauiTextInputBridge(
    () => new Entry { Opacity = 0, WidthRequest = 1, HeightRequest = 1 },
    () => new Editor { Opacity = 0, WidthRequest = 1, HeightRequest = 1 },
    layout, attachOnDemand: true);
var actions = 0;
DorotiTextEditingState? edited = null;
bridge.ActionPerformed += _ => actions++;
bridge.EditingStateChanged += state => edited = state;
var configuration = new DorotiTextInputConfiguration(DorotiTextInputType.text, DorotiTextInputAction.done,
    DorotiTextCapitalization.none, false, false, true, true);
try
{
    bridge.SetClient(configuration, new("seed", new(4, 4), null));
    bridge.ShowTextInput();
    Pump();
    var field = bridge.Inputs[0].Handler?.PlatformView as NSTextField
        ?? throw new Exception("Dynamically attached Entry has no native NSTextField.");
    Require(ReferenceEquals(field.Superview, handler.PlatformView), "Entry was not added to the native layout.");
    var editor = field.CurrentEditor as NSTextView;
    Require(editor is not null && ReferenceEquals(window.FirstResponder, editor), "Field editor did not receive focus.");
    Require(editor!.SelectedRange.Location == 4 && editor.SelectedRange.Length == 0, "Initial selection was lost before native focus.");
    bridge.ShowTextInput();
    bridge.ShowTextInput();
    Pump();
    Require(actions == 0, "Repeated show incorrectly submitted the field.");
    editor.InsertText(new NSString("abc"), new NSRange(NSRange.NotFound, 0));
    Pump();
    Require(edited?.text == "seedabc", "Native insertion did not reach the bridge.");
    // AppKit reports Other for non-submission editing endings (including layout).
    NSNotificationCenter.DefaultCenter.PostNotificationName(NSControl.TextDidEndEditingNotification, field,
        NSDictionary.FromObjectAndKey(NSNumber.FromInt64((long)NSTextMovement.Other), new NSString("NSTextMovement")));
    Pump();
    Require(actions == 0, "Layout editing-end incorrectly submitted the field.");
    NSNotificationCenter.DefaultCenter.PostNotificationName(NSControl.TextDidEndEditingNotification, field,
        NSDictionary.FromObjectAndKey(NSNumber.FromInt64((long)NSTextMovement.Return), new NSString("NSTextMovement")));
    Require(actions == 1, "Return did not submit exactly once.");
    bridge.SetClient(configuration with { inputType = DorotiTextInputType.multiline }, new("line", new(4, 4), null));
    bridge.ShowTextInput();
    Pump();
    Require(field.Superview is null, "Single-line native input survived the multiline switch.");
    var multiline = (bridge.Inputs.Single(input => input is Editor).Handler?.PlatformView as NSScrollView)?.DocumentView as NSTextView;
    Require(multiline is not null && ReferenceEquals(window.FirstResponder, multiline), "Multiline native input did not receive focus.");
    bridge.ShowTextInput();
    bridge.ClearClient();
    Pump();
    Require(layout.Children.Count == 0 && handler.PlatformView.Subviews.Length == 0, "Clear retained hidden native inputs.");
    Console.WriteLine("AppKit text input: dynamic native attachment, focus, selection, insertion, submission and client lifecycle PASS");
}
finally
{
    bridge.Dispose();
    ((IElementHandler)handler).DisconnectHandler();
}

void Pump()
{
    ((IView)layout).Arrange(new Microsoft.Maui.Graphics.Rect(0, 0, 400, 200));
    for (var i = 0; i < 10; i++) NSRunLoop.Main.RunUntil(NSDate.FromTimeIntervalSinceNow(.01));
}
void Require(bool condition, string message) { if (!condition) throw new Exception(message); }

sealed class FixtureApplication : Application { }
