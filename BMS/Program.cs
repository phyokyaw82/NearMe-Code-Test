using BMS.Api.Middleware;
using BMS.Core.Data;
using BMS.Core.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<GlobalExceptionMiddleware>();

// DAO config
DaoConfig.Configure(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("BMS"))
);

// BMS services
builder.Services.AddBms(options =>
{
    options.AssemblyNames.Add("BMS.Core");
    options.AssemblyNames.Add("BMS.Domain");
    options.AssemblyNames.Add("BMS.Api");
});

// Controllers + Newtonsoft JSON
builder.Services.AddControllers()
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new DefaultNamingStrategy()
    };
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BMS",
        Version = "v1",
        Description = "Project sample of Phyo Kyaw Thu Lin"
    });
});

var app = builder.Build();

// Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Interview Code Test for NearMe co.ltd.");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
