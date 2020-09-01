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
                                    sc.name as Field,
                                    sc.system_type_id as TypeId,
                                    sc.max_length as MaxLenth,
                                    sc.precision as Precision,
                                    sc.is_nullable as IsNullable,
                                    sc.is_identity as IsIdentity ,
                                    [Description] = s.value
                                    FROM sys.columns sc
                                    JOIN INFORMATION_SCHEMA.COLUMNS i_s 
                                    ON 
                                      sc.object_id = OBJECT_ID(i_s.TABLE_SCHEMA+'.'+i_s.TABLE_NAME) 
                                      AND i_s.COLUMN_NAME = sc.name
                                    LEFT JOIN 
                                      sys.extended_properties s 
                                    ON 
                                      s.major_id = OBJECT_ID(i_s.TABLE_SCHEMA+'.'+i_s.TABLE_NAME) 
                                      AND s.minor_id = i_s.ORDINAL_POSITION 
                                      AND s.name = 'MS_Description' " +
                                    $"WHERE sc.object_id = OBJECT_ID('dbo.{nombreTabla}') ";
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
