using Dapper;
using generatedtos.Interface;
using generatedtos.Modelo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generatedtos.Conector
{
    public class SqlSrvConnector : ConectorBase
    {
        public SqlSrvConnector(JobInfo jobInfo)
        {
            try
            {
                if (jobInfo == null) throw new Exception("Se necesita un parametro para la incializacion de la clase");

                _jobInfo = jobInfo;

                var cnnString = new SqlConnectionStringBuilder
                {
                    DataSource = jobInfo.ServerName,
                    InitialCatalog = jobInfo.DBName,
                    UserID = jobInfo.UserId,
                    Password = jobInfo.Password
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
                using (var cnx = new SqlConnection(_conectionString))
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var q = @"SELECT 
                            name as Field,
                            system_type_id as TypeId,
                            max_length as MaxLenth,
                            precision as Precision,
                            is_nullable as IsNullable,
                            is_identity as IsIdentity " +
                            $"FROM sys.columns WHERE object_id = OBJECT_ID('dbo.{nombreTabla}') ";
                    var data = cnx.Query<TablaInfoSqlSrv>(q);
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
                using (var cnx = new SqlConnection(_conectionString))
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    var qtablas = $"SELECT name FROM SYSOBJECTS WHERE xtype = 'U'; ";

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
