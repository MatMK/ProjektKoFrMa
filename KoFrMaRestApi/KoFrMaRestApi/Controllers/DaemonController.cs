using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using KoFrMaRestApi.Models;
using System.Data.SqlClient;

namespace KoFrMaRestApi.Controllers
{
    /// <summary>
    /// Slouží k komunikaci s Daemony a Serverem
    /// </summary>
    public class DaemonController : ApiController
    {
        /// <summary>
        /// Vrací instrukce pro daemon a registruje daemony do databáze.
        /// </summary>
        /// <param name="daemon"></param>
        /// <returns>Obsahuje informace o deamonu zasílajícím informaci.</returns>
        [HttpPost]
        public string GetInstructions(DaemonInfo daemon)
        {
            //Zjistí zda je Daemon už zaregistrovaný, pokud ne, přidá ho do databáze
            string DaemonId = "";
            MySqlConnection connection = WebApiConfig.Connection();
            connection.Open();
            MySqlDataReader reader = SelectFromTableByPcId(connection,daemon);
            
            if (reader.Read())
            {
                DaemonId = reader.GetString(0);
                reader.Close();
            }
            else
            {
                reader.Close();
                string SqlInsert = "insert into tbDaemons values(null, @version, @os, @pc_unique, 1)";
                MySqlCommand command = new MySqlCommand(SqlInsert, connection);
                command.Parameters.AddWithValue("@version", daemon.Version);
                command.Parameters.AddWithValue("@os", daemon.OS);
                command.Parameters.AddWithValue("@pc_unique", daemon.PC_Unique);
                command.ExecuteNonQuery();
                GetInstructions(daemon);
            }
            // Vybere task určený pro daemona. - nedodelane
            
            MySqlCommand sqlCommand = new MySqlCommand(@"SELECT Task FROM `tbTasks` WHERE `IdDaemon` = @Id", connection);
            sqlCommand.Parameters.AddWithValue("@Id", DaemonId);
            MySqlDataReader result = sqlCommand.ExecuteReader();
            if (result.Read())
            {
                return result.GetString(0);
            }
            else
            {
                return "";
            }
        }
        private MySqlDataReader SelectFromTableByPcId(MySqlConnection connection,DaemonInfo daemon)
        {
            string SqlCommand = "SELECT id FROM `tbDaemons` WHERE PC_Unique = @PC_ID";
            MySqlCommand query = new MySqlCommand(SqlCommand, connection);
            query.Parameters.AddWithValue("@PC_ID", daemon.PC_Unique);
            return query.ExecuteReader();
        }
    }
}