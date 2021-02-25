using generatedtos.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generatedtos.Modelo
{
    public class TablaInfoPgsql : ITablaInfo
    {
        public string TableName { get; set; }
        public string Field { get; set; }
        public string IsNUllable { get; set; }
        public string IsIdentity { get; set; }
        public string DataType { get; set; }
        public int MaxLength { get; set; }
        public string Description { get; set; }
        public string ConstraintType { get; set; }

        public DataType TipoDato
        {
            get
            {
                switch (DataType)
                {
                    case "bpchar":
                    case "varchar":
                        return generatedtos.DataType.String;

                    case "timestamp":
                    case "date":
                    case "timestamptz":
                        return generatedtos.DataType.Datetime;
                    case "int4":
                        return generatedtos.DataType.Int;
                    case "int2":
                        return generatedtos.DataType.Short;
                    case "int8":
                        return generatedtos.DataType.Long;
                    case "numeric": return generatedtos.DataType.Decimal;                       
                    case "bytea":
                        return generatedtos.DataType.ByteArray;
                    case "bool": 
                        return generatedtos.DataType.Bool;
                    default:
                        return generatedtos.DataType.Unknow;
                }
            }
        }

        public bool Required => IsNUllable == "NO";

        public bool EsPrimaryKey => ConstraintType == "PRIMARY KEY";

        public string GetPropertyDeclaration()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Description))
            {
                sb.AppendLine("         /// <summary>");
                sb.AppendLine($"         /// {Description}");
                sb.AppendLine("         /// </summary>");
            }

            //if (EsPrimaryKey)
            //{
            //    if (TipoDato == generatedtos.DataType.Int)
            //        sb.AppendLine("         [Dapper.Contrib.Extensions.Key]");
            //    else
            //        sb.AppendLine("         [Dapper.Contrib.Extensions.ExplicitKey]");
            //}

            if (Required && !EsPrimaryKey)
            {
                sb.AppendLine("         [Required(ErrorMessage = \"Campo requerido\", AllowEmptyStrings = false)]");
            }

            if (TipoDato == generatedtos.DataType.String)
            {
                sb.AppendLine($"         [MaxLength({MaxLength}, ErrorMessage = \"Máximo de {MaxLength} caracteres\")]");
            }

            if (Field.ToLower().Contains("mail"))
            {
                //sb.AppendLine("         [EsEmail]");
                sb.AppendLine("         [EmailAddress]");
            }

            if (Field.ToLower().Contains("password"))
            {
                sb.AppendLine("         [DataType(DataType.Password)]");
            }

            if (Field.ToLower().Contains("ruc"))
            {
                sb.AppendLine("         [EsRUC]");
            }

            if (TipoDato == generatedtos.DataType.Datetime)
            {
                sb.AppendLine("         [DataType(DataType.Date)]");
            }

            if (TipoDato == generatedtos.DataType.Int && !EsPrimaryKey && !Field.ToLower().Contains("id"))
            {
                sb.AppendLine("         [Range(0, int.MaxValue)]");
            }

            if (TipoDato == generatedtos.DataType.SmallInt && !EsPrimaryKey)
            {
                sb.AppendLine("         [Range(0, 1)]");
            }

            var propertyType = Addons.GetTypeDeclaration(TipoDato);

            if (TipoDato != generatedtos.DataType.String && TipoDato != generatedtos.DataType.ByteArray && !Required && !EsPrimaryKey)
            {
                propertyType += "?";
            }

            sb.AppendLine($"         public {propertyType} {Field} " + "{ get; set; }");
            return sb.ToString();
        }
    }
}
