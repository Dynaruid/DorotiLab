namespace Doroti.SourceTools;

public static class FlutterF3ClosureAudit
{
    public static FlutterFrameworkEvidenceAuditReport Run(string repositoryRoot, string evidencePath) =>
        FlutterFrameworkEvidenceAudit.Run(repositoryRoot, evidencePath, "F3");
}
