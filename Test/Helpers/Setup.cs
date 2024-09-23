using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infraestrutura.Db;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.Dominio.Interfaces;
using Test.Mocks;

namespace Test.Helpers
{
    public class Setup
    {
        public const string PORT = "5001";
        public static TestContext testContext = default!;
        public static WebApplicationFactory<Startup> http = default!;
        public static HttpClient client = default!;

       public static void ClassInit(TestContext testContent)
       {
            Setup.testContext = testContent;
            Setup.http = new WebApplicationFactory<Startup>();

            Setup.http = Setup.http.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAdministradorServico, AdministradorServicoMock>();
                    //services.AddScoped<ILogin<Administrador>, AdministradoresServicoMock>();
                });
            });


            /* //==Caso quaira deixar o teste com conexao diferente ==//
            var conexao = "Server=localhost;Database=minimalapitest;User=root;Password=root;"
            services.AddDbContext<DbContexto>(options => {
                options.UseMySql(conexao,ServerVersion.AutoDetect(conexao));
            }); */

            Setup.client = Setup.http.CreateClient();
       }

       public static void ClassCleanup()
       {
            Setup.http.Dispose();
       }

    }
}