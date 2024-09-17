using Microsoft.EntityFrameworkCore;
using MinimalAPI.DTO;
using MinimalAPI.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddDbContext<DbContexto>(
    options =>
    {
        options.UseMySql(
            builder.Configuration.GetConnectionString("mysql"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
        );
    }
);

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (MinimalAPI.DTO.LoginDTO loginDTO) => {
    if(loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
       return Results.Ok("Login com Sucesso");
    else
        return Results.Unauthorized();
});



app.Run();