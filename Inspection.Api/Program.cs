using Dapper;
using Inspection;
using Inspection.Api;
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
builder.Services.AddAuthentication(options =>
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
     {

         var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection(key: "Jwt:Key").Value);
         jwt.SaveToken = false;
         jwt.Authority = "http://QCMI.PIDEC.COM";
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
             ValidateIssuer = false,
             ValidateAudience = false,
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
});
//    .AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder()
//.AddAuthenticationSchemes("Bearer")
//.RequireAuthenticatedUser()
//.Build());



builder.Services.AddEndpointsApiExplorer();
  
    builder.Services.AddCors(o =>
    o.AddPolicy("CorsPolicy",
    builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()    
    .AllowAnyHeader()));

// Add services to the container.
var app = builder.Build();
    app.UseDeveloperExceptionPage();
app.UseCors("CorsPolicy");


app.UseRouting();

app.UseAuthentication();


app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

app.MapGroup("api/Identity").MapIdentityApi<SystematicUser>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  

    //Add when i wanted to upload Files
    app.UseStaticFiles();


  

    app.MapControllers();

    app.Run();
