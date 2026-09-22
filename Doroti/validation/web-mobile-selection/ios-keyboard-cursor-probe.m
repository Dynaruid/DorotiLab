#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>

// Test-only WebKit SPI: the same point-selection path used by UIKit text interaction.
@interface UIView (SelectionProbe)
- (void)selectPositionAtPoint:(CGPoint)point completionHandler:(void (^)(void))completion;
- (BOOL)hasHiddenContentEditable;
@end
@interface Probe : UIResponder <UIApplicationDelegate, WKNavigationDelegate>
@property (nonatomic,strong) UIWindow *window;
@property WKWebView *web;
@property NSMutableArray *results;
@property NSInteger index;
@end
@implementation Probe
- (BOOL)application:(UIApplication *)app didFinishLaunchingWithOptions:(NSDictionary *)options {
    self.results = [NSMutableArray new];
    self.window = [[UIWindow alloc] initWithFrame:UIScreen.mainScreen.bounds];
    UIViewController *vc = [UIViewController new]; self.window.rootViewController = vc;
    self.web = [[WKWebView alloc] initWithFrame:self.window.bounds];
    self.web.navigationDelegate = self;
    self.web.scrollView.contentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentNever;
    [vc.view addSubview:self.web]; [self.window makeKeyAndVisible];
    [self.web loadHTMLString:@"<meta name='viewport' content='width=device-width,initial-scale=1'><div style='position:fixed;inset:0;background:#eee'></div><textarea id='e' style='position:absolute;left:20px;top:180px;width:300px;height:40px;font:20px monospace;padding:0;border:0;opacity:0;pointer-events:none;color:transparent;caret-color:transparent'>abcdef</textarea>" baseURL:nil];
    return YES;
}
- (UIView *)inputViewIn:(UIView *)view {
    if ([view respondsToSelector:@selector(selectPositionAtPoint:completionHandler:)]) return view;
    for (UIView *child in view.subviews) { UIView *found = [self inputViewIn:child]; if(found)return found; }
    return nil;
}
- (void)webView:(WKWebView *)web didFinishNavigation:(WKNavigation *)navigation { [self runCase]; }
- (void)runCase {
    NSArray *modes=@[@"none",@"none",@"auto",@"auto"];
    if(self.index == modes.count) {
        NSString *file=[NSSearchPathForDirectoriesInDomains(NSDocumentDirectory,NSUserDomainMask,YES).firstObject stringByAppendingPathComponent:@"result.json"];
        [[NSJSONSerialization dataWithJSONObject:self.results options:NSJSONWritingPrettyPrinted error:nil] writeToFile:file atomically:YES];
        exit(0);
    }
    NSString *script=[NSString stringWithFormat:@"e.style.pointerEvents='%@';e.focus();e.setSelectionRange(6,6);true",modes[self.index]];
    [self.web evaluateJavaScript:script completionHandler:^(id value,NSError *error) {
        dispatch_after(dispatch_time(DISPATCH_TIME_NOW,NSEC_PER_SEC),dispatch_get_main_queue(),^{
            UIView *input=[self inputViewIn:self.web];
            BOOL hidden=[input respondsToSelector:@selector(hasHiddenContentEditable)] && [input hasHiddenContentEditable];
            if(!input) { [self.results addObject:@{@"error":@"No WebKit text interaction view"}]; self.index=modes.count; [self runCase];return; }
            CGFloat x=self.index%2==0 ? 45 : 80;
            [input selectPositionAtPoint:CGPointMake(x,200) completionHandler:^{
                [self.web evaluateJavaScript:@"({start:e.selectionStart,end:e.selectionEnd,active:document.activeElement.id,hit:document.elementFromPoint(45,200).id})" completionHandler:^(id state,NSError *error) {
                    [self.results addObject:@{@"pointerEvents":modes[self.index],@"x":@(x),@"selectionAssistantSuppressed":@(hidden),@"state":state?:error.description?:@"missing"}];
                    self.index++; [self runCase];
                }];
            }];
        });
    }];
}
@end
int main(int argc,char **argv) { @autoreleasepool {return UIApplicationMain(argc,argv,nil,NSStringFromClass(Probe.class));} }
