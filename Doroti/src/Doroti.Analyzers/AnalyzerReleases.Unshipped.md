### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
DOTARCH001 | Architecture | Error | Framework layer uses a backend dependency
DOTARCH002 | Architecture | Error | Public API exposes a backend type
DOTARCH003 | Architecture | Error | Production assembly references migration tooling
DOTARCH004 | Architecture | Error | Backend references Widgets
DOTARCH005 | Architecture | Error | Production JSON code must use System.Text.Json
DOTARCH006 | Architecture | Error | Only the matching backend adapter may reference an internal vendor assembly
DOTARCH007 | Architecture | Error | Vendor assemblies must not reference owned framework or Avalonia UI assemblies
DOTARCH008 | Architecture | Error | Official Avalonia assemblies may be referenced only by Doroti.Host.Avalonia
DOTARCH009 | Architecture | Error | Flutter runtime, UI, framework, and hosting assemblies must follow the G4 ownership boundary
