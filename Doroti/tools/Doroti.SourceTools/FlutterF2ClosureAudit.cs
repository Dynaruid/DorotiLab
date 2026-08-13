namespace Doroti.SourceTools;

public static class FlutterF2ClosureAudit
{
    public static FlutterFrameworkEvidenceAuditReport Run(string repositoryRoot, string evidencePath) =>
        FlutterFrameworkEvidenceAudit.Run(repositoryRoot, evidencePath, "F2");
}
