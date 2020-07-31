using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generatedtos
{
    public class TablaInfo
    {
        public string Field { get; set; }
        public string Type { get; set; }
        public string Null { get; set; }
        public string Key { get; set; }

        public DataType TipoDato
        {
            get
            {
                if (Type.Contains("int")) return DataType.Int;
                if (Type.Contains("varchar")) return DataType.String;
                if (Type.Contains("datetime")) return DataType.Datetime;
                if (Type.Contains("tinyint")) return DataType.SmallInt;
                if (Type.Contains("decimal")) return DataType.Decimal;
                if (Type.Contains("blob")) return DataType.ByteArray;

                return DataType.Unknow;
            }
        }

        public bool Required { get { return Null == "NO"; } }

        public bool EsPrimaryKey { get { return (Key ?? "") == "PRI"; } }

        public string GetPropertyDeclaration()
        {
            var sb = new StringBuilder();
            if (EsPrimaryKey)
            {
                sb.AppendLine("         [Dapper.Contrib.Extensions.Key]");
            }

            if (Required && !EsPrimaryKey)
            {
                sb.AppendLine("         [Required(ErrorMessage = \"Campo requerido\", AllowEmptyStrings = false)]");
            }

            if (TipoDato == DataType.String)
            {
                var maxLenght = Addons.GetLength(Type);
                sb.AppendLine($"         [MaxLength({maxLenght}, ErrorMessage = \"Máximo de {maxLenght} caracteres\")]");
            }

            if (Field.ToLower().Contains("mail"))
            {
                sb.AppendLine("         [EsEmail]");
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

            if (TipoDato == DataType.Datetime)
            {
                sb.AppendLine("         [DataType(DataType.Date)]");
            }

            if (TipoDato == DataType.Int && !EsPrimaryKey)
            {
                sb.AppendLine("         [Range(0, int.MaxValue)]");
            }

            if (TipoDato == DataType.SmallInt && !EsPrimaryKey)
            {
                sb.AppendLine("         [Range(0, 1)]");
            }

            var propertyType = Addons.GetTypeDeclaration(TipoDato);

            if (TipoDato != DataType.String && TipoDato != DataType.ByteArray && !Required && !EsPrimaryKey)
            {
                propertyType += "?";
            }

            sb.AppendLine($"         public {propertyType} {Field} " + "{ get; set; }");
            return sb.ToString();
        }
    }
}
