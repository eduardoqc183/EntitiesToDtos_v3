using Dapper;
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
        public const string dbName = "[DATABASE NAME]";
        public const string serverName = "[SERVER NAME]";
        public const string userid = "[USER ID]";
        public const string psswrd = "[YOUR PASSWORD]";
        public const string outputfiles = @"C:\Users\eduar\source\repos\NetInvoice\NetInvoice.BussinesEntities\Dto\";

        static void Main(string[] args)
        {
            try
            {
                var cnnString = new MySqlConnectionStringBuilder
                {
                    Server = serverName,
                    Database = dbName,
                    UserID = userid,
                    Password = psswrd
                };

                Console.WriteLine($"Conectando a {serverName}...");
                using (var cnx = new MySqlConnection(cnnString.ConnectionString))
                {
                    Console.WriteLine($"Obteniendo datos...");
                    var qtablas = $"SELECT table_name FROM information_schema.tables WHERE table_schema = '{dbName}'";
                    var tablas = cnx.Query<string>(qtablas);

                    foreach (var tabla in tablas)
                    {
                        Console.WriteLine($"Generando {tabla}...");
                        var columnas = cnx.Query<TablaInfo>($"SHOW COLUMNS FROM {tabla}");

                        var str = new StringBuilder();
                        str.AppendLine("using Dapper.Contrib.Extensions;");
                        str.AppendLine("using System;");
                        str.AppendLine("using System.ComponentModel.DataAnnotations;");
                        str.AppendLine("using NetInvoice.BussinesEntities.Atributos;");
                        str.AppendLine("");
                        str.AppendLine("namespace NetInvoice.BussinesEntities.Dto");
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
