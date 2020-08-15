using Dapper;
using generatedtos.Conector;
using generatedtos.Modelo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generatedtos
{
    class Program
    {
        public const string dbName = "tisolsoportebd";
        public const string serverName = "localhost";
        public const string userid = "postgres";
        public const string psswrd = "123456";
        public const string outputfiles = @"C:\Users\eduar\source\repos\TISolucionesSoporte\TISolucionesSoporteEntities\Dtos\";

        static void Main(string[] args)
        {
            try
            {
                JobInfo job = new JobInfo
                {
                    DBName = dbName,
                    OutPutFiles = outputfiles,
                    Password = psswrd,
                    ServerName = serverName,
                    UserId = userid
                };

                Console.WriteLine($"Obteniendo datos de {serverName}...");
                var dbConnector = new PgSqlConnector(job);

                var tablas = dbConnector.ObtenerTablas();

                foreach (var tabla in tablas)
                {
                    Console.WriteLine($"Generando {tabla}...");
                    var columnas = dbConnector.ColumasDeTabla(tabla);

                    var str = new StringBuilder();
                    str.AppendLine("using Dapper.Contrib.Extensions;");
                    str.AppendLine("using System;");
                    str.AppendLine("using System.ComponentModel.DataAnnotations;");
                    //str.AppendLine("using NetInvoice.BussinesEntities.Atributos;");
                    str.AppendLine("");
                    str.AppendLine("namespace TISolucionesSoporteEntities");
                    str.AppendLine("{");
                    str.AppendLine("    [Table(\"" + tabla + "\")]");
                    str.AppendLine($"    public partial class {tabla}Dto");
                    str.AppendLine("    {");
                    foreach (var col in columnas)
                    {
                        str.AppendLine(col.GetPropertyDeclaration());
                    }
                    str.AppendLine("    }");
                    str.AppendLine("}");
                    File.WriteAllText($"{outputfiles}{tabla}Dto.cs", str.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadKey();
            }
        }
    }
}
