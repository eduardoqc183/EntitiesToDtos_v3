using generatedtos.Interface;
using System.Text;

namespace generatedtos.Modelo
{
    public class TablaInfoSqlSrv : ITablaInfo
    {
        public string Field { get; set; }
        public int TypeId { get; set; }
        public int MaxLenth { get; set; }
        public int Precision { get; set; }
        public int IsNullable { get; set; }
        public int IsIdentity { get; set; }

        public DataType TipoDato
        {
            get
            {
                switch (TypeId)
                {
                    case 36: return DataType.Guid;
                    case 56: return DataType.Int;
                    case 61: return DataType.Datetime;
                    case 104: return DataType.Bool;
                    case 231:
                    case 167:
                    case 239:
                    case 175:
                        return DataType.String;
                    case 108: return DataType.Decimal;
                    case 52: return DataType.Decimal;
                    case 165: return DataType.ByteArray;
                    case 40: return DataType.Datetime;
                    default: return DataType.String;
                }
            }
        }


        public bool Required => IsNullable == 0;

        public bool EsPrimaryKey => IsIdentity == 1;

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
                sb.AppendLine($"         [MaxLength({MaxLenth}, ErrorMessage = \"Máximo de {MaxLenth} caracteres\")]");
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
