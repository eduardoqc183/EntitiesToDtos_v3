using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EntitiesToDtos.Properties;
using generatedtos.Conector;
using generatedtos.Modelo;

namespace EntitiesToDtos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtBaseDatos.Text = Settings.Default.bd;
            cboTipo.SelectedIndex = Settings.Default.indextipo;
            txtNamespaceDtos.Text = Settings.Default.namespacedto;
            txtNamespacesEntities.Text = Settings.Default.namespaceentities;
            txtOutputAssemblers.Text = Settings.Default.outputassembers;
            txtOutputDtos.Text = Settings.Default.outputdto;
            txtPassword.Text = Settings.Default.password;
            txtPuerto.Text = Settings.Default.port;
            txtServer.Text = Settings.Default.server;
            txtUserID.Text = Settings.Default.userid;
            checkBox1.Checked = Settings.Default.generarassemblers;
        }

        private void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPuerto.Enabled = cboTipo.SelectedIndex == 1;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Enabled = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                JobInfo job = new JobInfo
                {
                    DBName = txtBaseDatos.Text,
                    Password = txtPassword.Text,
                    ServerName = txtServer.Text,
                    UserId = txtUserID.Text,
                    Port = txtPuerto.Enabled ? int.Parse(txtPuerto.Text) : 0
                };

                Console.WriteLine($"Obteniendo datos de {job.ServerName}...");
                ConectorBase dbConnector;

                switch (cboTipo.SelectedIndex)
                {
                    case 0:
                        dbConnector = new SqlSrvConnector(job);
                        break;

                    case 1:
                        dbConnector = new PgSqlConnector(job);
                        break;

                    case 2:
                        dbConnector = new MysqlConnector(job);
                        break;

                    default:
                        dbConnector = new SqlSrvConnector(job);
                        break;
                }

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
                    str.AppendLine($"namespace {txtNamespaceDtos.Text}");
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
                    File.WriteAllText($"{txtOutputDtos.Text}\\{tabla}Dto.cs", str.ToString());
                }

                if (checkBox1.Checked)
                {
                    foreach (var tabla in tablas)
                    {
                        Console.WriteLine($"Generando Assemblers de: {tabla}...");
                        var columnas = dbConnector.ColumasDeTabla(tabla);

                        var str = new StringBuilder();
                        str.AppendLine("using System.Collections.Generic;");
                        str.AppendLine("using System.Linq;");
                        str.AppendLine($"using {txtNamespacesEntities.Text};");
                        str.AppendLine("");
                        str.AppendLine($"namespace {txtNamespaceDtos.Text}");
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
                        File.WriteAllText($"{txtOutputAssemblers.Text}\\{tabla}Assembler.cs", str.ToString());
                    }
                }

                Settings.Default.bd = txtBaseDatos.Text;
                Settings.Default.indextipo = cboTipo.SelectedIndex;
                Settings.Default.namespacedto = txtNamespaceDtos.Text;
                Settings.Default.namespaceentities = txtNamespacesEntities.Text;
                Settings.Default.outputassembers = txtOutputAssemblers.Text;
                Settings.Default.outputdto = txtOutputDtos.Text;
                Settings.Default.password = txtPassword.Text;
                Settings.Default.port = job.Port.ToString();
                Settings.Default.server = txtServer.Text;
                Settings.Default.userid = txtUserID.Text;
                Settings.Default.generarassemblers = checkBox1.Checked;
                Settings.Default.Save();

                MessageBox.Show("Proceso Terminado");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
