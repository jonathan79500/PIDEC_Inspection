using Dapper;
using Inspection;
using Inspection.Api;
using Inspection.Api.Middlewares;
using Inspection.Application;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.ValueObjects;
using Inspection.Infrastructure;
using Inspection.Persistence;
using Inspection.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

// Register your type handlers before building the host
SqlMapper.AddTypeHandler(new ProjectIdTypeHandler());
SqlMapper.AddTypeHandler(new AttachmentIdTypeHandler());
SqlMapper.AddTypeHandler(new OrderIdTypeHandler());
SqlMapper.AddTypeHandler(new ContactIdTypeHandler());
SqlMapper.AddTypeHandler(new InspectionSubjectIdTypeHandler());
SqlMapper.AddTypeHandler(new InspectionNotificationIdTypeHandler());
SqlMapper.AddTypeHandler(new InspectionTypeIdTypeHandler());
SqlMapper.AddTypeHandler(new InspectorIdTypeHandler());
SqlMapper.AddTypeHandler(new InspectionReportIdTypeHandler());
SqlMapper.AddTypeHandler(new InspectionReportAttachmentIdTypeHandler());
SqlMapper.SetTypeMap(typeof(InspectionNotificationId), new CustomPropertyTypeMap(typeof(InspectionNotificationId), (type, columnName) => {
    if (columnName == "InspectionNotificationId") return type.GetProperty("Id"); // map to base Id
    return type.GetProperty(columnName);
}));

var builder = WebApplication.CreateBuilder(args);   

    builder.Services.ConfigureApplicationServices();
    builder.Services.ConfigureInfrastructureServices(builder.Configuration);
    builder.Services.ConfigurePersistenceServices(builder.Configuration);

    builder.Services.
    AddApplication().
    AddInfrastructure();


builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;//System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
//        options => builder.Configuration.Bind("JwtSettings", options))
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
//        options => builder.Configuration.Bind("CookieSettings", options));

//builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme)
//                                    .AddBearerToken(IdentityConstants.BearerScheme);


//builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(key: "Jwt"));




//------------------------------------------------------------------------------------------------------

// Get the server URL from configuration
var serverUrl = builder.Configuration["BaseUrl"] ?? "https://localhost:5000";

builder.Services.AddAuthentication(options =>
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
     {

         var key = Encoding.ASCII.GetBytes("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e");
         jwt.SaveToken = false;
         jwt.Authority = serverUrl; //"http://QCMI.PIDEC.COM";
         jwt.RequireHttpsMetadata = false;
         
         jwt.TokenValidationParameters = new TokenValidationParameters()
         {
             ValidateIssuerSigningKey = false,
             //SignatureValidator = delegate (string token, TokenValidationParameters parameters)
             //{
             //    var jwt = new JwtSecurityToken(token);

             //    return jwt;
             //},
             IssuerSigningKey = new SymmetricSecurityKey(key),
             ValidateIssuer = true,
             ValidIssuer=serverUrl,
             ValidateAudience = true,
             ValidAudience = serverUrl,
             RequireExpirationTime = false,
             ValidateLifetime = false
         };
     });
//------------------------------------------------------------------------------------------------------

//----------------------check file size to upload in my system------------------------------------------
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500 * 1024 * 1024; // 500MB
});

//builder.Services.AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder()
//.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
//.RequireAuthenticatedUser()
//.Build());

//------------------------------------------------------------------------------------------------------


//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options=> {
//        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });


builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
    c.AddSecurityDefinition("bearerAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Fill in the JWT Token",
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{ Type= ReferenceType.SecurityScheme,Id="bearerAuth"}
            },[]
        }
    });
    // ** ADD THIS: Helps Swagger find all controllers **
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
//    .AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder()
//.AddAuthenticationSchemes("Bearer")
//.RequireAuthenticatedUser()
//.Build());



builder.Services.AddEndpointsApiExplorer();
  
    builder.Services.AddCors(o =>
    o.AddPolicy("CorsPolicy",
    builder => builder
     //.WithOrigins(
     //       "https://pims.pidec.com",      // Your server domain
     //       "https://pims.pidec.com/pims",      // Your server domain
     //       "https://pims.pidec.com/pims/*",      // Your server domain
     //       "https://localhost:4200",      // Local development
     //       "http://localhost:4200",
     //       "*"
     //   )
    .AllowAnyOrigin()
    .AllowAnyMethod()    
    .AllowAnyHeader()));

// Add services to the container.
var app = builder.Build();
    app.UseDeveloperExceptionPage();
app.UseCors("CorsPolicy");

//app.UseExceptionHandlingMiddleware();
app.UseRouting();

app.UseAuthentication();


app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

app.MapGroup("Identity").MapIdentityApi<SystematicUser>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();  

   


  

    app.MapControllers();

//app.Use(async (context, next) =>
//{
//    // Get the request path
//    var path = context.Request.Path.Value ?? "/";

//    // List of your API controller base paths (without 'api/' prefix)
//    // Add all your controller names here
//    var apiPaths = new[]
//    {
//        "BasicInfo",
//        "/Identity",     // Identity routes
//        "/Users",        // Your Users controller       
//        "/InspectionNotification",
//        "/InspectionReport",
//        "/Attachments",
//        // Add all your controller names here
//        "/swagger",      // Swagger
//        "/_framework",   // .NET framework
//        "/manifest"      // Web manifest
//    };

//    // Check if the request is for an API endpoint
//    var isApiRequest = apiPaths.Any(apiPath =>
//        path.StartsWith(apiPath, StringComparison.OrdinalIgnoreCase)) ||
//        path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);

//    // Check if it's a static file (has extension)
//    var hasExtension = Path.HasExtension(path);

//    // Check if the file exists in wwwroot
//    var filePath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", path.TrimStart('/'));
//    var isStaticFile = File.Exists(filePath);

//    // If it's not an API request, not a static file, and not a file with extension
//    if (!isApiRequest && !isStaticFile && !hasExtension)
//    {
//        // Serve Angular's index.html
//        context.Request.Path = "/index.html";
//    }

//    await next();
//});

//Add when i wanted to upload Files
app.UseStaticFiles();

app.MapFallbackToFile("index.html");


app.Run();
