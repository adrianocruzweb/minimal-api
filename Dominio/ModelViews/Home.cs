using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalAPI.Dominio.ModelViews
{
    public struct Home
    {
        public string Mensagem { get => "Bem vindo a API de Veículos - Minimal API";}
        public string Documentacao { get=>"/swagger";}
    }
}