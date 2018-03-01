using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.MySqlCom
{
    public class MySqlAdmin
    {
        public void RegisterToken(string UserName, Int64 Password, string Token)
        {
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT `Password` FROM `tbAdminAccounts` WHERE `Username` = @Username", connection))
            {
                int? DatabasePassword = null;
                connection.Open();
                command.Parameters.AddWithValue("@Username", UserName);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabasePassword = (int)reader["Password"];
                    }
                    reader.Close();
                }
                if (DatabasePassword == Password)
                {
                    command.CommandText = @"UPDATE `tbAdminAccounts` SET `Token`= @Token WHERE `Username` = @Username";
                    command.Parameters.AddWithValue("@Token", Token);
                    command.Parameters.AddWithValue("@Username", UserName);
                    command.ExecuteNonQuery();

                }
            }
        }
        public bool Authorized(string Username, string Token)
        {
            bool result;
            string _token = JsonConvert.DeserializeObject<string>(Token);
            using (MySqlConnection connection = WebApiConfig.Connection())
            using (MySqlCommand command = new MySqlCommand(@"SELECT * FROM `tbAdminAccounts` WHERE `Username` = @Username and `Token` = @Token", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Username", Username);
                command.Parameters.AddWithValue("@Token", _token);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        result = true;
                    else
                        result = false;
                    reader.Close();
                }
                connection.Close();
            }
            return result;
            
        }
    }
}