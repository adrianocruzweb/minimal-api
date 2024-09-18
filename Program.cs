using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.DTO;
using MinimalAPI.Infraestrutura.Db;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.ModelViews;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    )
);

var app = builder.Build();


app.MapGet("/", ()=>Results.Json(new Home()));


app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    //if(loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
    if(administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com Sucesso");
    else
        return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();