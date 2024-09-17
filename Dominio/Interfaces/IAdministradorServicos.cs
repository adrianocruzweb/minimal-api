using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.DTO;

namespace MinimalAPI.Dominio.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);
    }
}