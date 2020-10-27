using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generatedtos.Interface
{
    public interface ITablaInfo
    {
        DataType TipoDato { get; }
        string Field { get; set; }
        bool Required { get; }
        bool EsPrimaryKey { get; }
        string Description { get; set; }
        string GetPropertyDeclaration();
    }
}
