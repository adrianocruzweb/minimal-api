using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Dominio.Entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class AdministradorTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            //Arrange
            var administrador = new Administrador();

            //Act
            administrador.Id = 1;
            administrador.Email = "test@example.com";
            administrador.Senha = "teste";
            administrador.Perfil = "Adm";

            //Assert
            Assert.AreEqual(1,administrador.Id);
            Assert.AreEqual("test@example.com",administrador.Email);
            Assert.AreEqual("teste",administrador.Senha);
            Assert.AreEqual("Adm",administrador.Perfil);
        }
    }
}