using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoCadPluginRegister
{
    public class Uninstaller
    {
        Dictionary<string, string> autoCadVersionMaping = new Dictionary<string, string>
        {
            { "R19.1","2014" },
            { "R20.0","2015" },
            { "R20.1","2016" },
            { "R21.0","2017" },
            { "R22.0","2018" },
            { "R23.0","2019" }

        };
        const string autoCADRegKey = "Software\\Autodesk\\AutoCAD";
        string pluginRegKey = "CityGML Exporter for AutoCAD";

        public Uninstaller(string _pluginRegKey)
        {
            this.pluginRegKey = _pluginRegKey;
        }

        public void Run()
        {
            var autocadVers = GetAllIntalledAutoCadVersions();
            foreach (var item in autocadVers)
            {
                try
                {
                    var keyRoot = Registry.CurrentUser.OpenSubKey(autoCADRegKey, true); //AutoDesk/AutoCAD
                    var keyVer = keyRoot.OpenSubKey(item.VersionCode, true); //Ex: AutoDesk/AutoCAD/R22.0
                    foreach (var name in keyVer.GetSubKeyNames())
                    {
                        var keySubVer = keyVer.OpenSubKey(name, true); //Ex: AutoDesk/AutoCAD/R22.0/ACAD-1001:409
                        var keyApplication = keySubVer.OpenSubKey("Applications", true); //Ex: AutoDesk/AutoCAD/R22.0/ACAD-1001:409/Applications
                        if (keyApplication == null) continue;

                        RemovePuginInfoFronRegistry(keyApplication);
                    }
                }
                catch
                {

                }
            }
        }

        private List<AutoCADVersion> GetAllIntalledAutoCadVersions()
        {
            var listInstalledAutoCadVersions = new List<AutoCADVersion>();
            try
            {
                var rKey = Registry.CurrentUser.OpenSubKey(autoCADRegKey);
                if (rKey != null)
                {
                    foreach (string sub in rKey.GetSubKeyNames())
                    {
                        if (!Regex.IsMatch(sub, "R\\d+")) continue;
                        if (autoCadVersionMaping.ContainsKey(sub))
                        {

                            listInstalledAutoCadVersions.Add(new AutoCADVersion
                            {
                                Version = autoCadVersionMaping[sub],
                                VersionCode = sub,
                                Display = "AutoCAD " + autoCadVersionMaping[sub]
                            });
                        }
                            //CheckExistAndAddToList(sub, listInstalledAutoCadVersions);
                    }
                }
            }
            catch { }
            return listInstalledAutoCadVersions;
        }
        private void CheckExistAndAddToList(string versionCode, List<AutoCADVersion> list)
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key == null) return;

            foreach (string sub in key.GetSubKeyNames())
            {
                if (!Regex.IsMatch(sub, "AutoCAD\\s" + autoCadVersionMaping[versionCode])) continue;

                var installLocation = key.OpenSubKey(sub).GetValue("InstallLocation").ToString();
                if (!Directory.Exists(installLocation)) continue;

                if (list.Any(v => v.VersionCode == versionCode)) continue;

                list.Add(new AutoCADVersion
                {
                    Version = autoCadVersionMaping[versionCode],
                    VersionCode = versionCode,
                    Display = "AutoCAD " + autoCadVersionMaping[versionCode]
                });
            }
        }
        private void RemovePuginInfoFronRegistry(RegistryKey parentKey)
        {
            try
            {
                parentKey.DeleteSubKeyTree(pluginRegKey);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
