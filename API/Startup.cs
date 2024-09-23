using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.Dominio.Enuns;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.ModelViews;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.DTO;
using MinimalAPI.Infraestrutura.Db;

using MinimalAPI;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Key = Configuration?.GetSection("Jwt")?.ToString()??"";
    }

    private string Key;

    public IConfiguration Configuration{ get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option => {
            option.TokenValidationParameters = new TokenValidationParameters{
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        services.AddControllers();
        services.AddSwaggerGen();
        services.AddSwaggerGen(options =>{
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT aqui"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        services.AddDbContext<DbContexto>(options =>
            options.UseMySql(
                Configuration.GetConnectionString("MySQL"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySQL"))
            )
        );

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                }
            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints =>{
            #region Home
            endpoints.MapGet("/", ()=>Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Admin
            string GerarTokenJwt(Administrador administrador)
            {
                if(string.IsNullOrEmpty(Key)) return string.Empty;
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrador.Email),
                    new Claim("Perfil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil),
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
                var adm = administradorServico.Login(loginDTO);
                if(adm != null)
                {
                    string token = GerarTokenJwt(adm);
                    return Results.Ok(new AdministradorLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            }).AllowAnonymous().WithTags("Admin");

            endpoints.MapGet("/administradores", ([FromQuery] int? pagina , IAdministradorServico administradorServico) => {
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
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Admin");

            endpoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) => {
                var administrador = administradorServico.BuscaPorId(id);
                if(administrador == null)
                    return Results.NotFound("Não encontrado");
                return Results.Ok(new AdministradorModelView{
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Admin");

            endpoints.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {
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
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Admin");
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

            endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {

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
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"})
            .WithTags("Veiculo");

            endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) => {
                var veiculos = veiculoServico.Todos(pagina);
                return Results.Ok(veiculos);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"})
            .WithTags("Veiculo");

            endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => {
                var veiculo = veiculoServico.BuscaPorId(id);
                if(veiculo == null)
                    return Results.NotFound("Não encontrado");
                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"})
            .WithTags("Veiculo");

            endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {

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
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Veiculo");

            endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => {
                var veiculo = veiculoServico.BuscaPorId(id);
                if(veiculo == null)
                    return Results.NotFound("Não encontrado");

                veiculoServico.Remover(veiculo);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Veiculo");
            #endregion
        });
    }

}