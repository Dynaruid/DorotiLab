using System.Globalization;
using Doroti.Intl;
using Doroti.Intl.Tests;

if (args is ["--inspect-profile"])
{
    Console.WriteLine(NativeProfile.Inspect());
    return;
}

if (args is ["--expect-missing-data"])
{
    try
    {
        _ = new IntlFormatter("en-US");
        throw new Exception("Invariant/missing globalization data was accepted.");
    }
    catch (InvalidOperationException error) when (error.Message.Contains("globalization", StringComparison.Ordinal))
    {
        Console.WriteLine($"PASS: explicit data failure: {error.Message}");
    }
    return;
}

if (args is ["--profile"])
{
    Console.WriteLine(GlobalizationContract.Run());
    return;
}

Console.WriteLine(GlobalizationContract.Run());
Console.WriteLine("PASS: .NET formatting, strict Gregorian input, CLDR cardinal fixtures and culture isolation.");
