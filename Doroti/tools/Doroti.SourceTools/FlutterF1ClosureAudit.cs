namespace Doroti.SourceTools;

public static class FlutterF1ClosureAudit
{
    public static FlutterFrameworkEvidenceAuditReport Run(string repositoryRoot, string evidencePath) =>
        FlutterFrameworkEvidenceAudit.Run(repositoryRoot, evidencePath, "F1");
}
