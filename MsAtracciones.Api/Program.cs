using Atracciones.Shared.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using MsAtracciones.Api.Data;
using MsAtracciones.Api.GrpcServices;
using MsAtracciones.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(listenOptions =>
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2);
});

builder.Services.AddAtraccionesApiDefaults(builder.Configuration, "ms-atracciones");
builder.Services.AddDbContext<AtraccionesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AtraccionesDb")));
builder.Services.AddScoped<AtraccionesService>();
builder.Services.AddScoped<CatalogosService>();
builder.Services.AddGrpc();

var app = builder.Build();

app.UseAtraccionesApiDefaults();
app.MapGrpcService<AtraccionesGrpcService>();

app.Run();
