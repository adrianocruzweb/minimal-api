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
    public class VeiculoServico : IVeiculoServico
    {
        private readonly DbContexto _contexto;
        public VeiculoServico(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public void Atualizar(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }

        public Veiculo? BuscaPorId(int id)
        {
            return _contexto.Veiculos.Where(v=>v.Id == id).FirstOrDefault();//lembrar de testar Find aqui.
        }

        public void Incluir(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }

        public void Remover(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public List<Veiculo> Todos(int pagina = 1, string nome = null, string marca = null)
        {
            var veiculoQuery = _contexto.Veiculos.AsQueryable();
            var lista = new List<VeiculoServico>();
            if(!string.IsNullOrEmpty(nome))
            {
                veiculoQuery = veiculoQuery.Where(x => x.Nome.Contains(nome));
            }

            int itensPorPagina = 10;

            veiculoQuery = veiculoQuery.Skip((pagina-1)*itensPorPagina).Take(itensPorPagina);

            return veiculoQuery.ToList();

            /* if(lista.Any() && lista.Count() > 0)
                lista = lista.Add(veiculoQuery); */
        }
    }
}