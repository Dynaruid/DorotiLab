import Foundation
import UIKit
import Vision

@objc(DorotiNativeOcr)
public class DorotiNativeOcr : NSObject
{
    @objc(recognize:script:completion:)
    public static func recognize(imageData: NSData, script: String, completion: @escaping (NSString?, NSError?) -> Void) {
        DispatchQueue.global(qos: .userInitiated).async {
            do {
                let text = try recognizeBlocking(imageData: imageData as Data, script: script)
                completion(text as NSString, nil)
            } catch {
                completion(nil, error as NSError)
            }
        }
    }

    static func recognizeBlocking(imageData: Data, script: String) throws -> String {
        guard let image = UIImage(data: imageData), let cgImage = image.cgImage else {
            throw NSError(
                domain: "dev.doroti.ocr",
                code: 1,
                userInfo: [NSLocalizedDescriptionKey: "Unable to decode image bytes."])
        }

        let request = VNRecognizeTextRequest()
        request.recognitionLevel = .accurate
        request.usesLanguageCorrection = true
        request.recognitionLanguages = languages(for: script)

        let handler = VNImageRequestHandler(cgImage: cgImage, options: [:])
        try handler.perform([request])

        let observations = request.results ?? []
        return observations
            .compactMap { $0.topCandidates(1).first?.string }
            .joined(separator: "\n")
            .trimmingCharacters(in: .whitespacesAndNewlines)
    }

    static func languages(for script: String) -> [String] {
        switch script.trimmingCharacters(in: .whitespacesAndNewlines).lowercased() {
        case "latin":
            return ["en-US"]
        case "korean":
            return ["ko-KR"]
        default:
            return ["ko-KR", "en-US"]
        }
    }
}
