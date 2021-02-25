using System;
using System.Linq;
using CursoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CursoEFCore.Data
{
    public class ApplicationContext: DbContext
    {
        private static readonly ILoggerFactory _logger = LoggerFactory.Create(l => l.AddConsole());
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string strConection = "Data source=(localdb)\\mssqllocaldb;Initial Catalog=CursoEFCore;Integrated Security=true";

            //optionsBuilder.UseSqlServer("Data source=(localdb)\\mssqllocaldb;Initial Catalog=CursoEFCore;Integrated Security=true");            
            optionsBuilder
                .UseLoggerFactory(_logger)
                .EnableSensitiveDataLogging()
                .UseSqlServer(strConection,
                                p => p.EnableRetryOnFailure(
                                    maxRetryCount: 2, 
                                    maxRetryDelay: TimeSpan.FromSeconds(5), 
                                    errorNumbersToAdd: null)
                                .MigrationsHistoryTable("curso_ef_core")
                );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);   
            MapeaarPropriedadeEsquecidas(modelBuilder);
        }

        private void MapeaarPropriedadeEsquecidas(ModelBuilder modelBuilder)
        {
            foreach( var entity in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entity.GetProperties().Where(p => p.ClrType == typeof(string));

                foreach(var property in properties)
                {
                    if (string.IsNullOrEmpty(property.GetColumnType())
                        && !property.GetMaxLength().HasValue)
                    {
                        property.SetColumnType("VARCHAR(100)");
                    }

                }
            }
        }
    }   
}