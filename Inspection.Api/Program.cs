using Inspection;
using Inspection.Application;
using Inspection.Infrastructure;
using Inspection.Persistence;

var builder = WebApplication.CreateBuilder(args);

    builder.Services.ConfigureApplicationServices();
    builder.Services.ConfigureInfrastructureServices(builder.Configuration);
    builder.Services.ConfigurePersistenceServices(builder.Configuration);
    builder.Services.
    AddApplication().
    AddInfrastructure();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddCors(o =>
    o.AddPolicy("CorsPolicy",
    builder => builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));

// Add services to the container.
var app = builder.Build();


    app.UseHttpsRedirection();  

    app.UseCors("CorsPolicy");

    app.MapControllers();

    app.Run();
