using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.DTO;
using MinimalAPI.Infraestrutura.Db;
using MinimalAPI.Dominio.Interfaces;

namespace MinimalAPI.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;
        }
        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a=>a.Email==loginDTO.Email && a.Senha==loginDTO.Senha).FirstOrDefault();
            return adm;
        }


    }
}