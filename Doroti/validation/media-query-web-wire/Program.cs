using System.Text.Json;
using Doroti.Host.Web;
using Doroti.Ui;

var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
const string json = """
{"canvasId":"canvas","logicalWidth":360,"logicalHeight":800,"devicePixelRatio":0.75,
 "visible":true,"focused":true,"languageTag":"en-US","brightness":"light","operatingSystem":"web",
 "generation":5,"surfaceGeneration":1,"inputSequence":2,"environmentGeneration":3,
 "gpu":{"api":"webgpu","vendor":"fixture","renderer":"fixture","hardware":true,"softwareFallbackUsed":false},
 "resizeEpoch":{"generation":1,"logicalWidth":360,"logicalHeight":800,"physicalWidth":270,"physicalHeight":600,"devicePixelRatio":0.75,"timestampMicroseconds":0},
 "viewPadding":{"left":0,"top":24,"right":0,"bottom":20},
 "viewInsets":{"left":0,"top":0,"right":0,"bottom":300},
 "systemGestureInsets":{"left":0,"top":0,"right":0,"bottom":0},
 "displayFeatures":[{"bounds":{"left":179,"top":0,"right":181,"bottom":800},"type":2,"state":1}],
 "reduceMotion":true,"highContrast":false,"invertColors":false}
""";
var snapshot = JsonSerializer.Deserialize<BrowserHostSnapshot>(json, options)!;
var feature = snapshot.DisplayFeatures!.Single().ToDisplayFeature();
if (feature.bounds != new Rect(179, 0, 181, 800) || feature.type != DisplayFeatureType.hinge ||
    snapshot.ViewInsets.bottom != 300 || snapshot.DevicePixelRatio != .75 || !snapshot.ReduceMotion)
    throw new Exception("Browser environment wire conversion changed.");
var roundTrip = JsonSerializer.Deserialize<BrowserHostSnapshot>(JsonSerializer.Serialize(snapshot, options), options)!;
if (roundTrip.DisplayFeatures!.Single().ToDisplayFeature() != feature)
    throw new Exception("Browser environment JSON round trip failed.");
JsonSerializer.Deserialize<BrowserHostSnapshot>(json.Replace("[{\"bounds\":{\"left\":179,\"top\":0,\"right\":181,\"bottom\":800},\"type\":2,\"state\":1}]", "[]"), options);
Console.WriteLine("Browser environment JSON wire DTOs PASS: feature/empty list, round-trip, fractional DPR and keyboard");
