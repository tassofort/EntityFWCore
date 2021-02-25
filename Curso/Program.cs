using System;
using System.Collections.Generic;
using System.Linq;
using CursoEFCore.Domain;
using CursoEFCore.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CursoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //using var db = new Data.ApplicationContext();

            //db.Database.Migrate();
            
            //var existe = db.Database.GetPendingMigrations().Any();
            //if (existe) {/**/}

            //InserirDados();
            //InserirDadosEmMassa();
            //ConsultarDados();
            //ConsultarPedidoCarregamentoAdiantado();
            // AtualizarDados01();
            AtualizarDados02();

            //Console.WriteLine("Hello World!");
        }

        private static void RemoverRegistroDesconectado()
        {
            using var db = new Data.ApplicationContext();
            var cliente = new Cliente {Id = 3};

            db.Entry(cliente).State = EntityState.Deleted;
            db.SaveChanges();
        }
        private static void RemoverRegistro()
        {
            using var db = new Data.ApplicationContext();
            var cliente = db.Clientes.Find(2);

            // db.Clientes.Remove(cliente);
            // db.Remove(cliente);
            db.Entry(cliente).State = EntityState.Deleted;
            db.SaveChanges();
        }

        private static void AtualizarDados01()
        {
            using var db = new Data.ApplicationContext();
            var cliente = db.Clientes.Find(1);

            // db.Clientes.Update(cliente); //no banco o set referencia todas as colunas
            db.SaveChanges();
        }

        private static void AtualizarDados02()
        {
            using var db = new Data.ApplicationContext();
            
            var cliente = new Cliente
            {
                Id= 1
            };

            var clienteDesconectado = new
            {
                Nome = "Cliente Desconectado Passo 3",
                Telefone = "991541163"
            };           

            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            db.SaveChanges();
        }

        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();
            var pedidos = db
                .Pedidos
                .Include(p => p.Itens)
                .ThenInclude(p => p.Produto)
                .ToList();

            if (pedidos.Count == 0) CadastrarPedido();

            Console.WriteLine(pedidos.Count);
        }

        private static void CadastrarPedido()
        {
            using var dbcontext = new Data.ApplicationContext();

            var cliente = dbcontext.Clientes.FirstOrDefault();
            var produto = dbcontext.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10
                    }
                }
            };

            dbcontext.Pedidos.Add(pedido);
            dbcontext.SaveChanges();

        }

        private static void ConsultarDados()
        {
            using var db = new Data.ApplicationContext();
            //var consultaPorSintaxe = (from c in db.Clientes where c.Id>0 select c).ToList();
            var consultaPorMetodo = db.Clientes
                .Where(c => c.Id>0)
                .OrderBy(c => c.Id)
                .ToList();

            foreach(var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: {cliente.Id}");
                // db.Clientes.Find(cliente.Id);
                db.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
            }
        }
        private static void InserirDados()
        {
            using var db = new Data.ApplicationContext();

            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda,
                Ativo = true

            };

            db.Produtos.Add(produto);
            db.Set<Produto>().Add(produto);
            db.Entry(produto).State = EntityState.Added;
            db.Add(produto);

            var registros= db.SaveChanges();
            Console.WriteLine($"Total Registros(s): {registros}");
        }

        private static void InserirDadosEmMassa()
        {
            using var db = new Data.ApplicationContext();

            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda,
                Ativo = true

            };

            var cliente = new Cliente
            {
                Nome = "Rafael Almeida",
                CEP = "99999000",
                Cidade = "Itabaiana",
                Estado = "SE",
                Telefone = "99000001111"
            };

            var listaClientes = new[]
            {
                new Cliente
                {
                    Nome = "Teste 1",
                    CEP = "99999000",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    Telefone = "99000001115"
                },
                new Cliente
                {
                    Nome = "Teste 2",
                    CEP = "99999000",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    Telefone = "99000001116"
                }
            };

            //db.AddRange(produto, cliente);
            db.AddRange(listaClientes);
            // db.Set<Cliente>().AddRange(listaClientes);

            var registros= db.SaveChanges();
            Console.WriteLine($"Total Registros(s): {registros}");
        }
    }
}
