namespace Doroti.Editor.Assist;

public sealed record Snapshot(string Uri, int Version, string Text);
public sealed record Request(string Id, string Operation, string? Project = null, string? Uri = null,
    int Version = 0, int Offset = 0, int Length = 0, Snapshot[]? Snapshots = null,
    string? Action = null, string? Name = null);
public sealed record Edit(int Start, int Length, string Text);
public sealed record ParameterInfo(string Name, string Type, bool Optional, string? Default,
    string[]? DelegateParameters = null, string? DelegateReturn = null);
public sealed record TypeInfo(string Name, string Namespace, string FullName, string Kind,
    ParameterInfo[][] Constructors, string[] Values, string[] Factories, string Documentation);
public sealed record ActionInfo(string Id, string Title, bool Preview = false, string? Reason = null);
public sealed record Analysis(bool Semantic, string? Reason, bool Included, bool Code,
    string Context, string[] ExistingMembers, string? StateType, ParameterInfo[] Arguments,
    ActionInfo[] Actions, string[] GlobalImports, string[] Conflicts, string? Parent = null);
public sealed record Transform(Edit[] Edits, bool Preview, string? Reason = null);
