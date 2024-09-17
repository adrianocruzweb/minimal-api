using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.DTO;

namespace MinimalAPI.Infraestrutura.Interfaces
{
    public interface IAdministradorServicos
    {
        List<Administrador> Login(LoginDTO loginDTO);
    }
}