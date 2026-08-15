// <doroti-reviewed-framework-source />
#nullable enable
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/foundation/annotations.dart
using System;
using Doroti.Runtime;

namespace Doroti.Generated.Framework.Foundation;

public class Category
{
    public IReadOnlyList<string> sections { get; }

    public Category(IReadOnlyList<string> sections)
    {
        this.sections = sections;
    }

}

public class DocumentationIcon
{
    public string url { get; }

    public DocumentationIcon(string url)
    {
        this.url = url;
    }

}

public class Summary
{
    public string text { get; }

    public Summary(string text)
    {
        this.text = text;
    }

}

