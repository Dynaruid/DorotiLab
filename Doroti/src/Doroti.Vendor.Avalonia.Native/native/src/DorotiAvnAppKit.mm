// Source-port adaptation of Avalonia.Native src/OSX AvnWindow/AvnView/AvnTextInputMethod,
// PlatformRenderTimer and native/Avalonia.Native render-target contracts at pinned revision
// f159423f691946e713f454447a780d4677d8a0d2. See g7-macos-source-port-provenance.json.
#import <AppKit/AppKit.h>
#import <OpenGL/OpenGL.h>
#import <OpenGL/gl3.h>
#import "doroti-avalonia-native.h"
#include <atomic>
#include <cstdlib>
#include <cstring>
#include <cmath>

enum { EvActivated=1, EvDeactivated=2, EvMetrics=3, EvCloseRequested=4, EvClosed=5,
       EvPointer=6, EvKey=7, EvText=8, EvCompositionStart=9, EvCompositionUpdate=10,
       EvCompositionEnd=11, EvAccessibilityPress=12 };

static std::atomic<uint64_t> s_nextId{0};

@interface DorotiAvnView : NSView <NSTextInputClient, NSAccessibility>
@property(nonatomic, assign) doroti_avn_event_callback callback;
@property(nonatomic, assign) void* callbackContext;
@property(nonatomic, assign) uint64_t windowId;
@property(nonatomic, assign) NSInteger semanticsNodeId;
@property(nonatomic, copy) NSString* semanticsLabel;
@property(nonatomic, assign) BOOL semanticsCanPress;
@property(nonatomic, strong) NSTrackingArea* tracking;
@property(nonatomic, assign) NSRect caretRect;
@end

@implementation DorotiAvnView {
    NSMutableAttributedString* _markedText;
}
- (BOOL)acceptsFirstResponder { return YES; }
- (BOOL)isFlipped { return YES; }
- (void)updateTrackingAreas {
    if (self.tracking) [self removeTrackingArea:self.tracking];
    self.tracking = [[NSTrackingArea alloc] initWithRect:self.bounds
        options:NSTrackingMouseEnteredAndExited|NSTrackingMouseMoved|NSTrackingActiveInKeyWindow|NSTrackingInVisibleRect
        owner:self userInfo:nil];
    [self addTrackingArea:self.tracking];
    [super updateTrackingAreas];
}
- (uint64_t)mods:(NSEvent*)event {
    uint64_t m=0; NSEventModifierFlags f=event.modifierFlags;
    if (f&NSEventModifierFlagShift) m|=1; if(f&NSEventModifierFlagControl)m|=2;
    if(f&NSEventModifierFlagOption)m|=4; if(f&NSEventModifierFlagCommand)m|=8; return m;
}
- (void)pointer:(NSEvent*)e phase:(int)p dx:(double)dx dy:(double)dy {
    NSPoint q=[self convertPoint:e.locationInWindow fromView:nil];
    uint64_t buttons=(uint64_t)[NSEvent pressedMouseButtons];
    self.callback(self.callbackContext,EvPointer,p,self.windowId,q.x,q.y,dx,dy,buttons,[self mods:e],nullptr);
}
- (void)mouseEntered:(NSEvent*)e {[self pointer:e phase:0 dx:0 dy:0];}
- (void)mouseMoved:(NSEvent*)e {[self pointer:e phase:1 dx:0 dy:0];}
- (void)mouseDragged:(NSEvent*)e {[self pointer:e phase:3 dx:0 dy:0];}
- (void)rightMouseDragged:(NSEvent*)e {[self pointer:e phase:3 dx:0 dy:0];}
- (void)otherMouseDragged:(NSEvent*)e {[self pointer:e phase:3 dx:0 dy:0];}
- (void)mouseDown:(NSEvent*)e {[self.window makeFirstResponder:self];[self pointer:e phase:2 dx:0 dy:0];}
- (void)rightMouseDown:(NSEvent*)e {[self mouseDown:e];}
- (void)otherMouseDown:(NSEvent*)e {[self mouseDown:e];}
- (void)mouseUp:(NSEvent*)e {[self pointer:e phase:4 dx:0 dy:0];}
- (void)rightMouseUp:(NSEvent*)e {[self mouseUp:e];}
- (void)otherMouseUp:(NSEvent*)e {[self mouseUp:e];}
- (void)mouseExited:(NSEvent*)e {[self pointer:e phase:5 dx:0 dy:0];}
- (void)scrollWheel:(NSEvent*)e {
    double multiplier=e.hasPreciseScrollingDeltas?1.0:40.0;
    [self pointer:e phase:1 dx:e.scrollingDeltaX*multiplier dy:e.scrollingDeltaY*multiplier];
}
- (void)keyDown:(NSEvent*)e {
    self.callback(self.callbackContext,EvKey,e.isARepeat?1:0,self.windowId,0,0,0,0,e.keyCode,[self mods:e],nullptr);
    [self interpretKeyEvents:@[e]];
}
- (void)keyUp:(NSEvent*)e { self.callback(self.callbackContext,EvKey,2,self.windowId,0,0,0,0,e.keyCode,[self mods:e],nullptr); }
- (void)insertText:(id)value replacementRange:(NSRange)range {
    NSString* s=[value isKindOfClass:[NSAttributedString class]]?[value string]:value;
    self.callback(self.callbackContext,EvText,0,self.windowId,0,0,0,0,0,0,s.UTF8String);
    if (_markedText.length) { [_markedText setAttributedString:[[NSAttributedString alloc]initWithString:@""]]; self.callback(self.callbackContext,EvCompositionEnd,0,self.windowId,0,0,0,0,0,0,nullptr); }
}
- (void)setMarkedText:(id)value selectedRange:(NSRange)selected replacementRange:(NSRange)replacement {
    NSString* s=[value isKindOfClass:[NSAttributedString class]]?[value string]:value;
    if(!_markedText) _markedText=[NSMutableAttributedString new];
    if(!_markedText.length) self.callback(self.callbackContext,EvCompositionStart,0,self.windowId,0,0,0,0,0,0,nullptr);
    [_markedText setAttributedString:[[NSAttributedString alloc]initWithString:s]];
    self.callback(self.callbackContext,EvCompositionUpdate,0,self.windowId,0,0,0,0,0,0,s.UTF8String);
}
- (void)unmarkText { if(_markedText.length){[_markedText setAttributedString:[[NSAttributedString alloc]initWithString:@""]];self.callback(self.callbackContext,EvCompositionEnd,0,self.windowId,0,0,0,0,0,0,nullptr);} }
- (NSRange)selectedRange { return NSMakeRange(NSNotFound,0); }
- (NSRange)markedRange { return _markedText.length?NSMakeRange(0,_markedText.length):NSMakeRange(NSNotFound,0); }
- (BOOL)hasMarkedText { return _markedText.length>0; }
- (NSArray<NSAttributedStringKey>*)validAttributesForMarkedText { return @[]; }
- (NSAttributedString*)attributedSubstringForProposedRange:(NSRange)range actualRange:(NSRangePointer)actual { return nil; }
- (NSUInteger)characterIndexForPoint:(NSPoint)point { return 0; }
- (NSRect)firstRectForCharacterRange:(NSRange)range actualRange:(NSRangePointer)actual { return [self.window convertRectToScreen:self.caretRect]; }
- (void)doCommandBySelector:(SEL)selector {}
- (BOOL)isAccessibilityElement { return YES; }
- (NSString*)accessibilityRole { return self.semanticsCanPress?NSAccessibilityButtonRole:NSAccessibilityGroupRole; }
- (NSString*)accessibilityLabel { return self.semanticsLabel?:@"Doroti"; }
- (BOOL)accessibilityPerformPress { if(!self.semanticsCanPress)return NO; self.callback(self.callbackContext,EvAccessibilityPress,0,self.windowId,0,0,0,0,self.semanticsNodeId,0,nullptr); return YES; }
@end

@interface DorotiAvnHost : NSObject <NSWindowDelegate>
@property(nonatomic,strong) NSWindow* window;
@property(nonatomic,strong) DorotiAvnView* view;
@property(nonatomic,assign) doroti_avn_event_callback callback;
@property(nonatomic,assign) void* callbackContext;
@property(nonatomic,assign) uint64_t windowId;
@property(nonatomic,strong) NSOpenGLContext* renderContext;
@end

@implementation DorotiAvnHost
- (void)emit:(int)kind { self.callback(self.callbackContext,kind,0,self.windowId,0,0,0,0,0,0,nullptr); }
- (void)emitMetrics {
    NSRect b=self.view.bounds; CGFloat s=self.window.backingScaleFactor;
    self.callback(self.callbackContext,EvMetrics,self.window.isMiniaturized?1:0,self.windowId,b.size.width,b.size.height,b.size.width*s,b.size.height*s,(uint64_t)llround(s*1000.0),0,nullptr);
}
- (void)windowDidBecomeKey:(NSNotification*)n {[self emit:EvActivated];}
- (void)windowDidResignKey:(NSNotification*)n {[self emit:EvDeactivated];}
- (void)windowDidResize:(NSNotification*)n {[self emitMetrics];}
- (void)windowDidChangeBackingProperties:(NSNotification*)n {[self emitMetrics];}
- (void)windowDidMiniaturize:(NSNotification*)n {[self emitMetrics];}
- (void)windowDidDeminiaturize:(NSNotification*)n {[self emitMetrics];}
- (BOOL)windowShouldClose:(NSWindow*)sender {[self emit:EvCloseRequested];return YES;}
- (void)windowWillClose:(NSNotification*)n {[self emit:EvClosed];}
@end

static DorotiAvnHost* host(void* p){return (__bridge DorotiAvnHost*)p;}
static NSOpenGLContext* glctx(void* p){return (__bridge NSOpenGLContext*)p;}

extern "C" void doroti_avn_app_init(){
    [NSApplication sharedApplication]; [NSApp setActivationPolicy:NSApplicationActivationPolicyRegular]; [NSApp finishLaunching];
}
extern "C" void doroti_avn_app_wake(){CFRunLoopWakeUp(CFRunLoopGetMain());}
extern "C" int32_t doroti_avn_app_pump(int32_t wait){
    NSDate* until=wait?[NSDate dateWithTimeIntervalSinceNow:0.01]:[NSDate distantPast];
    NSEvent* e=[NSApp nextEventMatchingMask:NSEventMaskAny untilDate:until inMode:NSDefaultRunLoopMode dequeue:YES];
    if(e){[NSApp sendEvent:e];return 1;} return 0;
}
extern "C" void* doroti_avn_window_create(const char* title,double w,double h,doroti_avn_event_callback cb,void* context){
    DorotiAvnHost* x=[DorotiAvnHost new]; x.callback=cb;x.callbackContext=context;x.windowId=++s_nextId;
    NSRect r=NSMakeRect(0,0,w,h); NSUInteger style=NSWindowStyleMaskTitled|NSWindowStyleMaskClosable|NSWindowStyleMaskMiniaturizable|NSWindowStyleMaskResizable;
    x.window=[[NSWindow alloc]initWithContentRect:r styleMask:style backing:NSBackingStoreBuffered defer:NO];
    x.view=[[DorotiAvnView alloc]initWithFrame:r];x.view.callback=cb;x.view.callbackContext=context;x.view.windowId=x.windowId;
    x.window.contentView=x.view;x.window.delegate=x;x.window.title=[NSString stringWithUTF8String:title?:"Doroti"];[x.window center];
    NSOpenGLPixelFormatAttribute attrs[]={NSOpenGLPFAOpenGLProfile,NSOpenGLProfileVersion3_2Core,NSOpenGLPFAAccelerated,NSOpenGLPFADoubleBuffer,NSOpenGLPFAColorSize,24,NSOpenGLPFAAlphaSize,8,0};
    NSOpenGLPixelFormat* pf=[[NSOpenGLPixelFormat alloc]initWithAttributes:attrs];
    x.renderContext=[[NSOpenGLContext alloc]initWithFormat:pf shareContext:nil];
    [x.renderContext setView:x.view]; [x.renderContext update];
    [x emitMetrics]; return (void*)CFBridgingRetain(x);
}
extern "C" void doroti_avn_window_show(void* p){[host(p).window makeKeyAndOrderFront:nil];[NSApp activateIgnoringOtherApps:YES];[host(p) emitMetrics];}
extern "C" void doroti_avn_window_resize(void* p,double w,double h){[host(p).window setContentSize:NSMakeSize(w,h)];}
extern "C" void doroti_avn_window_minimize(void* p,int32_t m){m?[host(p).window miniaturize:nil]:[host(p).window deminiaturize:nil];}
extern "C" void doroti_avn_window_focus(void* p,int32_t f){f?[host(p).window makeKeyWindow]:[host(p).window resignKeyWindow];}
extern "C" void doroti_avn_window_close(void* p){[host(p).window performClose:nil];}
extern "C" void doroti_avn_window_destroy(void* p){if(!p)return;[host(p).window orderOut:nil];CFBridgingRelease(p);}
extern "C" void doroti_avn_window_move_to_screen(void* p,uint64_t screenId){
    for(NSScreen* s in NSScreen.screens){NSNumber* n=s.deviceDescription[@"NSScreenNumber"];if(n.unsignedLongLongValue==screenId){NSRect f=s.visibleFrame;NSRect w=host(p).window.frame;w.origin=NSMakePoint(f.origin.x+(f.size.width-w.size.width)/2,f.origin.y+(f.size.height-w.size.height)/2);[host(p).window setFrame:w display:YES];return;}}
}
extern "C" void doroti_avn_window_metrics(void* p,double* w,double* h,double* pw,double* ph,double* s){NSRect b=host(p).view.bounds;*s=host(p).window.backingScaleFactor;*w=b.size.width;*h=b.size.height;*pw=*w**s;*ph=*h**s;}
extern "C" int32_t doroti_avn_screen_primary(uint64_t* id,double* x,double* y,double* w,double* h,double* scale){NSScreen* s=NSScreen.mainScreen;if(!s)return 0;NSRect f=s.visibleFrame;NSNumber* n=s.deviceDescription[@"NSScreenNumber"];*id=n.unsignedLongLongValue;*x=f.origin.x;*y=f.origin.y;*w=f.size.width;*h=f.size.height;*scale=s.backingScaleFactor;return 1;}
extern "C" void* doroti_avn_window_nswindow(void* p){return (__bridge void*)host(p).window;}
extern "C" void doroti_avn_cursor_set(int32_t k){NSCursor* c=k==1?[NSCursor pointingHandCursor]:k==2?[NSCursor IBeamCursor]:k==3?[NSCursor crosshairCursor]:k==4?[NSCursor closedHandCursor]:k==5?[NSCursor operationNotAllowedCursor]:[NSCursor arrowCursor];[c set];}
extern "C" char* doroti_avn_clipboard_get(){NSString* s=[[NSPasteboard generalPasteboard]stringForType:NSPasteboardTypeString];return s?strdup(s.UTF8String):nullptr;}
extern "C" int32_t doroti_avn_clipboard_set(const char* t){NSPasteboard* p=[NSPasteboard generalPasteboard];[p clearContents];return [p setString:[NSString stringWithUTF8String:t?:""] forType:NSPasteboardTypeString]?1:0;}
extern "C" void doroti_avn_string_free(char* p){free(p);}
extern "C" void doroti_avn_accessibility_set(void* p,int32_t id,const char* label,int32_t press){host(p).view.semanticsNodeId=id;host(p).view.semanticsLabel=[NSString stringWithUTF8String:label?:""];host(p).view.semanticsCanPress=press;NSAccessibilityPostNotification(host(p).view,NSAccessibilityValueChangedNotification);}
extern "C" void doroti_avn_text_caret(void* p,double x,double y,double w,double h){host(p).view.caretRect=NSMakeRect(x,y,w,h);}
extern "C" void doroti_avn_test_pointer(void* p,int32_t phase,double x,double y,double dx,double dy){DorotiAvnHost* h=host(p);h.callback(h.callbackContext,EvPointer,phase,h.windowId,x,y,dx,dy,phase==2?1:0,0,nullptr);}
extern "C" void doroti_avn_test_key(void* p,int32_t phase,uint32_t key){DorotiAvnHost* h=host(p);h.callback(h.callbackContext,EvKey,phase,h.windowId,0,0,0,0,key,0,nullptr);}
extern "C" void doroti_avn_test_text(void* p,int32_t phase,const char* text){DorotiAvnHost* h=host(p);int kind=phase==0?EvText:phase==1?EvCompositionStart:phase==2?EvCompositionUpdate:EvCompositionEnd;h.callback(h.callbackContext,kind,0,h.windowId,0,0,0,0,0,0,text);}
extern "C" void* doroti_avn_gl_create(void* p){
    NSOpenGLContext* c=host(p).renderContext; return c?(void*)CFBridgingRetain(c):nullptr;
}
extern "C" void* doroti_avn_gl_make_current(void* p){NSOpenGLContext* old=[NSOpenGLContext currentContext];[glctx(p) makeCurrentContext];return old?(void*)CFBridgingRetain(old):nullptr;}
extern "C" void doroti_avn_gl_restore(void* p){if(p){[glctx(p) makeCurrentContext];CFBridgingRelease(p);}else [NSOpenGLContext clearCurrentContext];}
extern "C" void doroti_avn_gl_present(void* p){[glctx(p) flushBuffer];}
extern "C" const char* doroti_avn_gl_renderer(void* p){[glctx(p) makeCurrentContext];const GLubyte* s=glGetString(GL_RENDERER);return s?(const char*)s:"unknown";}
extern "C" const char* doroti_avn_gl_version(void* p){[glctx(p) makeCurrentContext];const GLubyte* s=glGetString(GL_VERSION);return s?(const char*)s:"unknown";}
extern "C" void doroti_avn_gl_destroy(void* p){if(!p)return;if([NSOpenGLContext currentContext]==glctx(p))[NSOpenGLContext clearCurrentContext];[glctx(p) clearDrawable];CFBridgingRelease(p);}
