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
        public const string DbName = "empresa";
        public const string ServerName = "localhost";
        public const string Userid = "postgres";
        public const string Psswrd = "123456";
        public const int Port = 5434;
        public const string NamespaceDTOs = "EF_Postgres.Entities";
        public const string NamespaceEntities = "EF_Postgres.DataModel.Models";
        public const string OutputDTOfiles = @"C:\Users\eduar\source\repos\EF_Postgres\EF_Postgres.Entities\DTOs\";
        public const string OutputAssemblersfiles = @"C:\Users\eduar\source\repos\EF_Postgres\EF_Postgres.DataModel\Assemblers\";
        public const bool GenerarAssemblers = true;

        static void Main(string[] args)
        {
            try
            {
                JobInfo job = new JobInfo
                {
                    DBName = DbName,
                    Password = Psswrd,
                    ServerName = ServerName,
                    UserId = Userid,
                    Port = Port
                };

                Console.WriteLine($"Obteniendo datos de {ServerName}...");
                var dbConnector = new PgSqlConnector(job); //aca cambiar segun el motor de bd

                var tablas = dbConnector.ObtenerTablas();

                foreach (var tabla in tablas)
                {
                    Console.WriteLine($"Generando DTOs de: {tabla}...");
                    var columnas = dbConnector.ColumasDeTabla(tabla);

                    var str = new StringBuilder();
                    //str.AppendLine("using Dapper.Contrib.Extensions;");
                    str.AppendLine("using System;");
                    str.AppendLine("using System.ComponentModel.DataAnnotations;");
                    str.AppendLine("");
                    str.AppendLine($"namespace {NamespaceDTOs}");
                    str.AppendLine("{");
                    //str.AppendLine("    [Table(\"" + tabla + "\")]");
                    str.AppendLine($"    public partial class {tabla}Dto");
                    str.AppendLine("    {");
                    foreach (var col in columnas)
                    {
                        str.AppendLine(col.GetPropertyDeclaration());
                    }
                    str.AppendLine("    }");
                    str.AppendLine("}");
                    File.WriteAllText($"{OutputDTOfiles}{tabla}Dto.cs", str.ToString());
                }

                if (GenerarAssemblers)
                {
                    foreach (var tabla in tablas)
                    {
                        Console.WriteLine($"Generando Assemblers de: {tabla}...");
                        var columnas = dbConnector.ColumasDeTabla(tabla);

                        var str = new StringBuilder();
                        str.AppendLine("using System.Collections.Generic;");
                        str.AppendLine("using System.Linq;");
                        str.AppendLine($"using {NamespaceEntities};");
                        str.AppendLine("");
                        str.AppendLine($"namespace {NamespaceDTOs}");
                        str.AppendLine("{");
                        str.AppendLine($"    public static partial class {tabla}Assembler");
                        str.AppendLine("    {");
                        str.AppendLine($"        static partial void OnDTO(this {tabla} entity, {tabla}Dto dto);");
                        str.AppendLine($"        static partial void OnEntity(this {tabla}Dto entity, {tabla} dto);");
                        str.AppendLine($"        public static {tabla} ToEntity(this {tabla}Dto dto)");
                        str.AppendLine("        {");
                        str.AppendLine("            if (dto == null) return null;");
                        str.AppendLine($"            var entity = new {tabla}");
                        str.AppendLine("            {");
                        foreach (var col in columnas)
                        {
                            str.AppendLine($"                {col.Field} = dto.{col.Field},");
                        }
                        str.AppendLine("            };");
                        str.AppendLine("            dto.OnEntity(entity);");
                        str.AppendLine("            return entity;");
                        str.AppendLine("        }");
                        str.AppendLine("");
                        str.AppendLine($"        public static {tabla}Dto ToDTO(this {tabla} entity)");
                        str.AppendLine("        {");
                        str.AppendLine("            if (entity == null) return null;");
                        str.AppendLine($"            var dto = new {tabla}Dto");
                        str.AppendLine("            {");
                        foreach (var col in columnas)
                        {
                            str.AppendLine($"                {col.Field} = entity.{col.Field},");
                        }
                        str.AppendLine("            };");
                        str.AppendLine("            entity.OnDTO(dto);");
                        str.AppendLine("            return dto;");
                        str.AppendLine("        }");
                        str.AppendLine("");
                        str.AppendLine($"        public static List<{tabla}> ToEntities(this IEnumerable<{tabla}Dto> dtos)");
                        str.AppendLine("        {");
                        str.AppendLine("            if (dtos == null) return null;");
                        str.AppendLine("            return dtos.Select(e => e.ToEntity()).ToList();");
                        str.AppendLine("        }");
                        str.AppendLine("");
                        str.AppendLine($"        public static List<{tabla}Dto> ToDTOs(this IEnumerable<{tabla}> entities)");
                        str.AppendLine("        {");
                        str.AppendLine("            if (entities == null) return null;");
                        str.AppendLine("            return entities.Select(e => e.ToDTO()).ToList();");
                        str.AppendLine("        }");
                        str.AppendLine("    }");
                        str.AppendLine("}");
                        File.WriteAllText($"{OutputAssemblersfiles}{tabla}Assembler.cs", str.ToString());
                    }
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
