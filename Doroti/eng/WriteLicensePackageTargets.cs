using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public sealed class WriteDorotiLicensePackageTargets : Task
{
    [Required] public ITaskItem[] Licenses { get; set; }
    [Required] public string OutputFile { get; set; }
    public string ExistingTargets { get; set; }
    [Output] public ITaskItem[] PackageFiles { get; set; }

    public override bool Execute()
    {
        try
        {
            var document = File.Exists(ExistingTargets) ? XDocument.Load(ExistingTargets) : new XDocument(new XElement("Project"));
            var ns = document.Root.Name.Namespace;
            var items = new XElement(ns + "ItemGroup");
            var stagingRoot = Path.GetDirectoryName(Path.GetFullPath(OutputFile));
            var files = new List<ITaskItem>();
            foreach (var license in Licenses.OrderBy(item => item.GetMetadata("TargetPath"), StringComparer.Ordinal))
            {
                if (!File.Exists(license.ItemSpec)) throw new FileNotFoundException("Registered license is missing.", license.ItemSpec);
                var target = license.GetMetadata("TargetPath").Replace('\\', '/');
                if (!target.StartsWith("licenses/", StringComparison.Ordinal) || target.Split('/').Any(part => part == ".."))
                    throw new InvalidDataException("Registered license must stay under licenses/: " + target);
                // NuGet treats extensionless PackagePath values as directories.
                // Stage the exact target name, then pack into its parent folder.
                var staged = Path.Combine(stagingRoot, target.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(staged));
                File.Copy(license.ItemSpec, staged, true);
                var packaged = new TaskItem(staged);
                packaged.SetMetadata("PackagePath", target.Substring(0, target.LastIndexOf('/') + 1));
                files.Add(packaged);
                items.Add(new XElement(ns + "None",
                    new XAttribute("Include", "$(MSBuildThisFileDirectory)../" + target),
                    new XAttribute("Link", target),
                    new XAttribute("CopyToOutputDirectory", "PreserveNewest"),
                    new XAttribute("CopyToPublishDirectory", "PreserveNewest"),
                    new XAttribute("ExcludeFromSingleFile", "true")));
            }
            document.Root.Add(items);
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(OutputFile)));
            var text = document.ToString();
            if (!File.Exists(OutputFile) || File.ReadAllText(OutputFile) != text) File.WriteAllText(OutputFile, text);
            PackageFiles = files.ToArray();
            return true;
        }
        catch (Exception error)
        {
            Log.LogError("DOROTILICENSE002: " + error.Message);
            return false;
        }
    }
}
