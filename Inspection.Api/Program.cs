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
   
    builder.Services.AddControllers().
    AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);

    builder.Services.AddEndpointsApiExplorer();
  //  builder.Services.AddSwaggerGen();
    builder.Services.AddCors(o =>
    o.AddPolicy("CorsPolicy",
    builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()    
    .AllowAnyHeader()));

// Add services to the container.
var app = builder.Build();

    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseHttpsRedirection();  

    app.UseCors("CorsPolicy");

    app.MapControllers();

    app.Run();
