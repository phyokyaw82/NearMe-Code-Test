using BMS.Core.Data;
using BMS.Core.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
DaoConfig.Configure(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("BMS"))
);

builder.Services.AddBms(options =>
{
    options.AssemblyNames.Add("BMS.Core");
    options.AssemblyNames.Add("BMS.Domain");
    options.AssemblyNames.Add("BMS.Api");
});

builder.Services.AddControllers()
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new DefaultNamingStrategy() // PascalCase
    };
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

// Add Swagger
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

// Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at root
});

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
