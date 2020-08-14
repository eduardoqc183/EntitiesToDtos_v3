using generatedtos.Interface;
using generatedtos.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generatedtos.Conector
{
    public abstract class ConectorBase
    {
        internal string _conectionString;
        internal JobInfo _jobInfo;

        public DbConnection Conector { get; set; }

        public abstract IEnumerable<string> ObtenerTablas();

        public abstract IEnumerable<ITablaInfo> ColumasDeTabla(string nombreTabla);
    }
}
