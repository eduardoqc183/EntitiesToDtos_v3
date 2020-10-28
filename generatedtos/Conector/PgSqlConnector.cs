using Dapper;
using generatedtos.Interface;
using generatedtos.Modelo;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generatedtos.Conector
{
    public class PgSqlConnector : ConectorBase
    {
        public PgSqlConnector(JobInfo jobInfo)
        {
            try
            {
                if (jobInfo == null) throw new Exception("Se necesita un parametro para la incializacion de la clase");

                _jobInfo = jobInfo;

                var cnnString = new NpgsqlConnectionStringBuilder
                {
                    Host = jobInfo.ServerName,
                    Database = jobInfo.DBName,
                    Username = jobInfo.UserId,
                    Password = jobInfo.Password,
                    Port = jobInfo.Port
                };

                _conectionString = cnnString.ConnectionString;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override IEnumerable<ITablaInfo> ColumasDeTabla(string nombreTabla)
        {
            try
            {
                using (var cnx = new NpgsqlConnection(_conectionString))
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    var q = "select  distinct c.table_name as \"TableName\", " +
                            "c.column_name as \"Field\",  " +
                            "c.is_nullable as \"IsNUllable\",  " +
                            "c.is_identity as \"IsIdentity\", " +
                            "c.udt_name as \"DataType\",  " +
                            "c.ordinal_position,  " +
                            "c.character_maximum_length as \"MaxLength\",   " +
                            "tc.constraint_type as \"ConstraintType\", " +
                            "pgd.description as \"Description\" " +
                            "from information_schema.columns c  " +
                            "left join information_schema.key_column_usage cu on c.table_name = cu.table_name and c.column_name = cu.column_name  " +
                            "left join information_schema.table_constraints tc on cu.constraint_name = tc.constraint_name and  " +
                            "cu.table_name = tc.table_name and tc.constraint_type = 'PRIMARY KEY'  " +
                            "join pg_catalog.pg_statio_all_tables st on c.table_name = st.relname " +
                            "left join pg_catalog.pg_description pgd on st.relid = pgd.objoid and pgd.objsubid=c.ordinal_position and c.table_schema=st.schemaname and c.table_name=st.relname " +
                            $"where c.table_name = '{nombreTabla}'" +
                            "order by c.ordinal_position asc;";

                    var data = cnx.Query<TablaInfoPgsql>(q);
                    return data.Select(s => (ITablaInfo)s);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override IEnumerable<string> ObtenerTablas()
        {
            try
            {
                using (var cnx = new NpgsqlConnection(_conectionString))
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    var qtablas = $"SELECT table_name FROM information_schema.tables WHERE table_schema='public'";

                    return cnx.Query<string>(qtablas); ;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
