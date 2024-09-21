using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.Infraestrutura.Db;


namespace Test.Domain.Servicos
{
    [TestClass]
    public class AdministradorServicoTest
    {
        private DbContexto CriarContextoDeTeste()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

            //Configurar o ConfigurationBuilder
            var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

            var configuration = builder.Build();

            //Obter a string de conexão
            //var connectionString = configuration.GetConnectionString("Mysql");

            //Configurar o DbContextOptionsBuilder
            /* var options = new DbContextOptionsBuilder<DbContexto>()
            .UseMySql(connectionString,ServerVersion.AutoDetect(connectionString))
            .Options; */

            return new DbContexto(configuration);
        }

        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            //Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");


            var administrador = new Administrador();
            administrador.Email = "test@example.com";
            administrador.Senha = "teste";
            administrador.Perfil = "Adm";
            var administradorServico = new AdministradorServico(context);

            //Act
            administradorServico.Incluir(administrador);


            //Assert
            Assert.AreEqual(1,administradorServico.Todos(1).Count());
            /* Assert.AreEqual("test@example.com",administrador.Email);
            Assert.AreEqual("teste",administrador.Senha);
            Assert.AreEqual("Adm",administrador.Perfil); */
        }

        [TestMethod]
        public void TestandoBuscaPorIDAdministrador()
        {
            //Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");


            var administrador = new Administrador();
            administrador.Email = "test@example.com";
            administrador.Senha = "teste";
            administrador.Perfil = "Adm";
            var administradorServico = new AdministradorServico(context);

            //Act
            administradorServico.Incluir(administrador);
            var adm = administradorServico.BuscaPorId(administrador.Id);


            //Assert
            Assert.AreEqual(1,adm.Id);
            /* Assert.AreEqual("test@example.com",administrador.Email);
            Assert.AreEqual("teste",administrador.Senha);
            Assert.AreEqual("Adm",administrador.Perfil); */
        }
    }
}