using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Tests;

// One in-memory database per test. Create() returns a fresh DbContext on the same data,
// so a test can "save with one context, read with another", like two requests would.
// (If your InspectionDbContext constructor needs more than the options, adapt Create().)
public sealed class TestDb
{
    private readonly DbContextOptions<InspectionDbContext> _options =
        new DbContextOptionsBuilder<InspectionDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    public InspectionDbContext Create() => new(_options);
}