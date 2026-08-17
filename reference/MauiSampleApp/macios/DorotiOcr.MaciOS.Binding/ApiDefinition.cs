using System;
using Foundation;

namespace DorotiOcrMaciOS
{
    // @interface DorotiNativeOcr : NSObject
    [BaseType(typeof(NSObject))]
    interface DorotiNativeOcr
    {
        // +(void)recognize:(NSData *)imageData script:(NSString *)script completion:(void (^)(NSString * _Nullable, NSError * _Nullable))completion;
        [Static]
        [Export("recognize:script:completion:")]
        [Async]
        void Recognize(NSData imageData, string script, Action<NSString?, NSError?> completion);
    }
}
