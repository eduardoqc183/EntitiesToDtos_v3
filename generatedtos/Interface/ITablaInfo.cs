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
        bool Required { get; }
        bool EsPrimaryKey { get; }
        string GetPropertyDeclaration();
    }
}
