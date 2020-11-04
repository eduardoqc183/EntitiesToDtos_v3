using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generatedtos
{
    public enum DataType
    {
        String,
        Int,
        Long,
        Decimal,
        Datetime,
        Bool,
        ByteArray,
        Unknow,
        SmallInt,
        Guid,
        Short
    }

    public static class Addons
    {
        public static string GetTypeDeclaration(DataType dt)
        {
            switch (dt)
            {
                
                case DataType.String:
                    return "string";
                case DataType.Int:
                case DataType.SmallInt:
                    return "int";
                case DataType.Decimal:
                    return "decimal";
                case DataType.Datetime:
                    return "DateTime";
                case DataType.Bool:
                    return "bool";
                case DataType.ByteArray:
                    return "byte[]";
                case DataType.Guid:
                    return "Guid";
                case DataType.Long:
                    return "long";
                case DataType.Short:
                    return "short";
                default:
                    return "??";
            }
        }

        public static string GetLength(string prop)
        {
            if (!prop.Contains("(") || !prop.Contains(")")) return "0";

            var i = prop.IndexOf("(");
            var ii = prop.IndexOf(")");
            var lenght = prop.Substring(i + 1, ii - i - 1);

            return lenght;
        }
    }
}
