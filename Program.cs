using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.DTO;
using MinimalAPI.Infraestrutura.Db;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.ModelViews;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.Dominio.Enuns;

#region Builder

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    )
);

#endregion

#region StartApp
var app = builder.Build();
#endregion

#region CenterApp

#region Home
app.MapGet("/", ()=>Results.Json(new Home())).WithTags("Home");
#endregion

#region Admin
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    if(administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com Sucesso");
    else
        return Results.Unauthorized();
}).WithTags("Admin");

app.MapGet("/administradores", ([FromQuery] int? pagina , IAdministradorServico administradorServico) => {
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach (var admin in administradores)
    {
        adms.Add(new AdministradorModelView{
            Id = admin.Id,
            Email = admin.Email,
            Perfil = admin.Perfil
        });
    }
    return Results.Ok(adms);
}).WithTags("Admin");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) => {
    var administrador = administradorServico.BuscaPorId(id);
    if(administrador == null)
        return Results.NotFound("Não encontrado");
    return Results.Ok(new AdministradorModelView{
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
}).WithTags("Admin");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {
    var validacao = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };

    if(string.IsNullOrEmpty(administradorDTO.Email) || administradorDTO.Email == "string")
        validacao.Mensagens.Add("A informação 'E-MAIL' do administrador deve ser preenchido");
    if(string.IsNullOrEmpty(administradorDTO.Senha) || administradorDTO.Senha == "string")
        validacao.Mensagens.Add("A informação 'Senha' do administrador deve ser preenchida");
    if(administradorDTO.Perfil == null)
        validacao.Mensagens.Add("A informação 'Perfil' do administrador deve ser preenchido");

    if(validacao.Mensagens.Count() > 0)
        return Results.BadRequest(validacao);

    var administrador = new Administrador{
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };
    administradorServico.Incluir(administrador);

    return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView{
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
}).WithTags("Admin");
#endregion

#region Cars

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };

    if(string.IsNullOrEmpty(veiculoDTO.Nome) || veiculoDTO.Nome == "string")
        validacao.Mensagens.Add("A informação 'NOME' do veículo deve ser preenchido");

    if(string.IsNullOrEmpty(veiculoDTO.Marca) || veiculoDTO.Marca == "string")
        validacao.Mensagens.Add("A Informação 'MARCA' do veículo deve ser preenchida");

    if(veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("A Informação 'ANO' deve ser superior a 1950");

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {

    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count() >0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano,
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculo");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) => {
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).WithTags("Veiculo");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null)
        return Results.NotFound("Não encontrado");
    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {

    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count() >0)
        return Results.BadRequest(validacao);

    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null)
        return Results.NotFound("Não encontrado");

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null)
        return Results.NotFound("Não encontrado");

    veiculoServico.Remover(veiculo);

    return Results.NoContent();
}).WithTags("Veiculo");
#endregion

#endregion

#region FinishApp
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion