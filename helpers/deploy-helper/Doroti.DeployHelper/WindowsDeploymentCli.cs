using System.Reflection.PortableExecutable;

namespace Doroti.DeployHelper;

public sealed class WindowsDeploymentCli(string repositoryRoot, ICommandRunner runner, IUserInterface ui)
{
    private const string Rid = "win-x64";

    public async Task RunAsync(Options options, CancellationToken ct)
    {
        options.ValidateForPlatform("windows");
        if (options.List)
        {
            ui.WriteLine("\nWindows App SDK samples (.NET 10 / Release / Native AOT / win-x64)");
            foreach (var item in SampleApp.All) ui.WriteLine($"  {item.Key}: {item.WindowsProject(repositoryRoot)}");
            return;
        }
        if (!OperatingSystem.IsWindows() && !options.DryRun)
            throw new DeployException("Windows Native AOT publishing requires Windows and the Visual Studio C++ toolchain.");

        var app = await new Selector(ui).SelectAsync("Sample app", SampleApp.All, item => item.Folder, "--app / -App", ct,
            options.App, (item, value) => item.Key.Equals(value, StringComparison.OrdinalIgnoreCase));
        var project = app.WindowsProject(repositoryRoot);
        if (!File.Exists(project)) throw new DeployException($"Windows App SDK project not found: {project}");

        var artifacts = Path.Combine(repositoryRoot, "Doroti", "artifacts", "windows-aot-net10-release-x64");
        var publish = Path.Combine(artifacts, "publish", app.ArtifactSlug, Guid.NewGuid().ToString("N"));
        var executable = Path.Combine(publish, Path.GetFileNameWithoutExtension(project) + ".exe");
        var command = new Command(Executables.Resolve(options.DotnetPath),
            ["publish", project, "-c", "Release", "-r", Rid, "--self-contained", "--disable-build-servers", "-nr:false",
                "--artifacts-path", Path.Combine(artifacts, "build"), "-o", publish,
                "-p:DorotiCompilationMode=NativeAot", "-p:DorotiBuildNativeFromSource=true"],
            Path.Combine(repositoryRoot, "Doroti"), Capture: false, TimeoutSeconds: 1200);

        ui.WriteLine($"\n.NET 10 / Release / Native AOT / {Rid}\nPublished executable: {executable}");
        if (options.DryRun)
        {
            ui.WriteLine("Dry run: " + command.Display);
            if (!options.NoLaunch) ui.WriteLine("After a successful publish, launch the EXE and wait for it to close.");
            return;
        }

        // The build cache and the native host's source-build output are shared. Each launch uses its own publish directory.
        Directory.CreateDirectory(artifacts);
        using (AcquireBuildLock(artifacts))
            await runner.RunAsync(command, ct);
        ValidatePublish(executable);
        ui.WriteLine($"Native AOT publish complete: {executable}");
        if (options.NoLaunch) return;

        ui.WriteLine("Close the sample window to exit deploy-helper.");
        await runner.RunAsync(new(executable, [], publish, Capture: false, TimeoutSeconds: null, Environment: options.Environment), ct);
    }

    private static FileStream AcquireBuildLock(string artifacts)
    {
        try
        {
            return File.Open(Path.Combine(artifacts, "build.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException error) when ((error.HResult & 0xffff) is 32 or 33)
        {
            throw new DeployException("Another deploy-helper Windows Native AOT publish is running. Retry when it finishes.");
        }
    }

    private static void ValidatePublish(string executable)
    {
        var publish = Path.GetDirectoryName(executable)!;
        if (!File.Exists(executable) || !File.Exists(Path.Combine(publish, "doroti_windows_appsdk_host_v1.dll")))
            throw new DeployException($"Publish did not produce the expected executable and Windows host in {publish}.");
        try
        {
            using var pe = new PEReader(File.OpenRead(executable));
            if (pe.PEHeaders.CoffHeader.Machine == Machine.Amd64 && pe.PEHeaders.CorHeader is null &&
                !File.Exists(Path.ChangeExtension(executable, ".dll")) && !File.Exists(Path.Combine(publish, "coreclr.dll"))) return;
        }
        catch (BadImageFormatException) { }
        throw new DeployException($"Published executable is not a win-x64 Native AOT image: {executable}");
    }
}
