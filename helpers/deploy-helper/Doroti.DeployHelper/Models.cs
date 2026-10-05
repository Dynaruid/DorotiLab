namespace Doroti.DeployHelper;

public sealed record SampleApp(string Key, string Folder, string BundleId, string ArtifactSlug)
{
    public string Project(string root) => Path.Combine(root, "samples", Folder, "ios", Folder + ".iOS.csproj");
    public string AndroidProject(string root) => Path.Combine(root, "samples", Folder, "android", Folder + ".Android.csproj");
    public static readonly SampleApp[] All =
    [
        new("Sample2", "DorotiSampleApp2", "dev.doroti.sample2", "sample2"),
        new("Testbed", "DorotiTestbedApp", "dev.doroti.testbed", "testbed"),
    ];
}

public record BuildPlan(Command Command, string Artifacts, string Sdk, string Mode, string Configuration, string Rid);

public sealed record AndroidTarget(string Serial, string State, string Name, string Kind)
{
    public string Label => $"[{Kind}] {Name} / {State} / {Serial}";
}
