import Foundation
#if targetEnvironment(macCatalyst)
import UIKit
#else
import AppKit
#endif

@objc(DorotiNativeInterop)
public final class DorotiNativeInterop: NSObject {
    @objc(platformInfo)
    public static func platformInfo() -> String {
#if targetEnvironment(macCatalyst)
        let platform = "MacCatalyst"
        let osVersion = UIDevice.current.systemVersion
#else
        let platform = "macOS"
        let osVersion = ProcessInfo.processInfo.operatingSystemVersionString
#endif
        let payload: [String: String] = [
            "platform": platform,
            "osVersion": osVersion,
            "bridgeVersion": "1.0.0"
        ]
        let data = try! JSONSerialization.data(withJSONObject: payload, options: [.sortedKeys])
        return String(data: data, encoding: .utf8)!
    }

    @objc(echo:)
    public static func echo(_ value: String) -> String { value }

    @objc(echoOnMainThreadWithValue:completion:)
    public static func echoOnMainThread(value: String, completion: @escaping (String) -> Void) {
        DispatchQueue.main.async { completion(echo(value)) }
    }
}
