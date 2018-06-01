using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Settings
{
    public class SettingsLoader
    {
        public RestApiUserSetting LoadSettings()
        {
            
            RestApiUserSetting setting = new RestApiUserSetting();
            string configpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\serverconfig.ini";
            if (File.Exists(configpath))
            {


                Dictionary<string, string> dictionary = File.ReadLines(configpath)
                    .Select(line => line.Split('='))
                    .ToDictionary(line => line[0], line => line[1]);

                

                if (dictionary.ContainsKey("MySQL"))
                {
                    setting.DatabaseAddress = dictionary["MySQL"];
                }

                setting.DatabaseCredential = new System.Net.NetworkCredential();

                if (dictionary.ContainsKey("Username"))
                {
                    setting.DatabaseCredential.UserName = dictionary["Username"];
                }
                if (dictionary.ContainsKey("Password"))
                {
                    setting.DatabaseCredential.Password = dictionary["Password"];
                }
            }
            
            return setting;
        }

        public void SaveSettings(RestApiUserSetting setting)
        {
            string configpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\serverconfig.ini";
            try
            {
                StreamWriter w = new StreamWriter(configpath);
                if (!File.Exists(configpath)
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\");
                    File.Create(configpath);
                }
                w = new StreamWriter(configpath);
                w.WriteLine("MySQL=" + setting.DatabaseAddress);
                w.WriteLine("Username=" + setting.DatabaseCredential.UserName);
                w.WriteLine("Password=" + setting.DatabaseCredential.Password);
                w.Close();
                w.Dispose();
            }
            catch (Exception)
            {
                
            }
        }

    }
}