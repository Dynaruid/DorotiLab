return await Doroti.Tooling.RepositoryCommands.TryRunAsync(args) ?? await Doroti.Tooling.ProviderCli.RunAsync(args);
