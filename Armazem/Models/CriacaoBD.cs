using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armazem.Models
{
    [Migration(1)]
    public class CriacaoBD : Migration
    {
        public override void Down()
        {
            Delete.Table("categorias");
            Delete.Table("produtos");
        }

        public override void Up()
        {
            Create.Table("categorias")
                 .WithColumn("categoriaid").AsInt64().PrimaryKey().Identity()
                 .WithColumn("nome").AsString().Unique().NotNullable();

            Create.Table("produtos")
                .WithColumn("produtoid").AsInt64().PrimaryKey().Identity()
                .WithColumn("nome").AsString().NotNullable()
                .WithColumn("preco").AsDouble().NotNullable()
                .WithColumn("categoriaid").AsInt64().ForeignKey("categoriaid", "categorias", "categoriaid");
        }
    }
}
