using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infraestrutura.Db;

namespace Test.Helpers
{
    public class Setup
    {
        public const string PORT = "5001";
        public static TestContext testContext = default!;
        public static WebApplicationFactory<Startup> http = default!;
        public static HttpClient client = default!;

        /* public static async Task ExecutaComandoSqlAsync(string sql)
        {
            await new DbContexto().Database.ExecuteSqlRawAsync(sql);
        } */

        /* public static async Task<int> ExecutaEntityCountAsync(int id, string nome)
        {
            return await new DbContexto().Clientes.Where(c=>c.Id==id && c.Nome==nome).FirstOrDefaultAsync();
        } */

        /* public static async Task FakeClienteAsync()
        {
            await new DbContexto().Database.ExecuteSqlRawAsync("""
            insert lientes(Nome,Telefone,Email,DataCriacao)
            values('Danilo', '(11)11111-1111', 'email@teste.com', '2022-12-15 06:09:0000')
            """);
        } */
    }
}