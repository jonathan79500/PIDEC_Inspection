using System.Runtime.CompilerServices;
using Inspection.Application.Features.Documents;
using Mapster;

namespace Inspection.Application.Tests;

// The handlers call .Adapt<>() (static, global config), so the mapping configs of the Application
// assembly are registered once, before any test runs.
internal static class MapsterSetup
{
    [ModuleInitializer]
    public static void Init() =>
        TypeAdapterConfig.GlobalSettings.Scan(typeof(DocumentRules).Assembly);
}