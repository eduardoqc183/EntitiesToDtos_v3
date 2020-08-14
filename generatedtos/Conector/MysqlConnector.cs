using Dapper;
using generatedtos.Interface;
using generatedtos.Modelo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace generatedtos.Conector
{
    public class MysqlConnector : ConectorBase
    {
        public MysqlConnector(JobInfo jobInfo)
        {
            try
            {
                if (jobInfo == null) throw new Exception("Se necesita un parametro para la incializacion de la clase");

                _jobInfo = jobInfo;

                var cnnString = new MySqlConnectionStringBuilder
                {
                    Server = jobInfo.ServerName,
                    Database = jobInfo.DBName,
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
                using (var cnx = new MySqlConnection(_conectionString))
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    var data = cnx.Query<TablaInfoMysql>($"SHOW COLUMNS FROM {nombreTabla}");
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
                using (var cnx = new MySqlConnection(_conectionString))
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    var qtablas = $"SELECT table_name FROM information_schema.tables WHERE table_schema = '{_jobInfo.DBName}'";

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
