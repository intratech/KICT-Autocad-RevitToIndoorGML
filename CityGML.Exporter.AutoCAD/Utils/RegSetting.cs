using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Utils
{
    public class RegSetting
    {
        public static string RegKey = @"SOFTWARE\Autocad2Indoor";
        private static RegistryKey reg = null;
        private static void InitKey()
        {
            try
            {
                if (reg == null)
                {
                    reg = Registry.CurrentUser.OpenSubKey(RegKey, true);
                    if (reg == null)
                    {
                        reg = Registry.CurrentUser.CreateSubKey(RegKey);
                    }
                    //AppDomain.CurrentDomain.ProcessExit += ProcessExist;
                }
            }
            catch
            {

            }
        }

        private static void ProcessExist(object sender, EventArgs e)
        {
            if (reg != null)
            {
                reg.Close();
            }
        }

        public static string GetSetting(string key)
        {
            InitKey();
            if (reg != null)
            {
                var obj  = reg.GetValue(key);
                return obj != null ? obj.ToString() : "";
            }

            return "";
        }


        public static void SetSetting(string key,string value)
        {
            InitKey();
            if (reg != null)
            {
                reg.SetValue(key, value);               
            }
        }

        internal static int? GetSettingAsNumber(string key)
        {
            string _value = GetSetting(key);
            if(!string.IsNullOrEmpty(_value))
            {
                int rs = 0;
                if (int.TryParse(_value, out rs))
                    return rs;
            }
            return null;
        }

        internal static void Close()
        {
            if(reg != null)
            {
                reg.Close();
            }
        }
    }
}
