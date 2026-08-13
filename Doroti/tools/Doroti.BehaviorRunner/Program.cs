using Doroti.BehaviorRunner;

if (args.Length is not 2)
{
    Console.Error.WriteLine("Usage: Doroti.BehaviorRunner <fixture.json> <output.json>");
    return 64;
}

BehaviorFixtureRunner.Run(args[0], args[1]);
return 0;
