using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AutoCadPluginRegister
{
    public partial class SelectVersionForm : Form
    {
        //------Registry infomation--------
        string m_currentFolder;
        Dictionary<string, string> autoCadVersionMaping = new Dictionary<string, string>
        {
            { "R19.1","2014" },
            { "R20.0","2015" },
            { "R20.1","2016" },
            { "R21.0","2017" },
            { "R22.0","2018" },
            { "R23.0","2019" },
            { "R24.0","2021" }
        };
        const string autoCADRegKey = "Software\\Autodesk\\AutoCAD";
        string pluginRegKey = string.Empty; //"CityGML Exporter for AutoCAD";
        string dllName = string.Empty; //"CityGML.Exporter.AutoCAD.dll"


        List<PluginRegistryInfoValue> pluginRegInfo;

        public SelectVersionForm(string _pluginRegKey, string _dllName)
        {
            InitializeComponent();

            this.pluginRegKey = _pluginRegKey;
            this.dllName = _dllName;

            m_currentFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            pluginRegInfo = new List<PluginRegistryInfoValue>
            {
                new PluginRegistryInfoValue("DESCRIPTION", pluginRegKey, RegistryValueKind.String),
                new PluginRegistryInfoValue("LOADCTRLS", 2, RegistryValueKind.DWord),
                new PluginRegistryInfoValue("LOADER", Path.Combine(m_currentFolder, dllName), RegistryValueKind.String),
                new PluginRegistryInfoValue("MANAGED", 1, RegistryValueKind.DWord),
            };
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var autocadVers = GetAllIntalledAutoCadVersions();
            
            foreach (var ver in autocadVers)
            {
                var idx = checkedListBox.Items.Add(ver);
                checkedListBox.SetItemChecked(idx, true);
            }
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
                        if(autoCadVersionMaping.ContainsKey(sub))
                            CheckExistAndAddToList(sub, listInstalledAutoCadVersions);
                    }
                }
            }
            catch{}
            return listInstalledAutoCadVersions;
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            foreach (var item in checkedListBox.CheckedItems)
            {
                var i = item as AutoCADVersion;
                try
                {
                    if (i == null) continue;

                    var keyRoot = Registry.CurrentUser.OpenSubKey(autoCADRegKey, true); //AutoDesk/AutoCAD
                    var keyVer = keyRoot.OpenSubKey(i.VersionCode, true); //Ex: AutoDesk/AutoCAD/R22.0
                    foreach (var name in keyVer.GetSubKeyNames())
                    {
                        var keySubVer = keyVer.OpenSubKey(name, true); //Ex: AutoDesk/AutoCAD/R22.0/ACAD-1001:409
                        var keyApplication = keySubVer.OpenSubKey("Applications", true); //Ex: AutoDesk/AutoCAD/R22.0/ACAD-1001:409/Applications
                        if (keyApplication == null) continue;

                        WritePuginInfoToRegistry(keyApplication);
                    }
                }
                catch
                {
                    MessageBox.Show("Install plugin for " + i.Display + " failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Application.Exit();
        }
        private void WritePuginInfoToRegistry(RegistryKey parentKey)
        {
            try
            {
                var pluginKey = parentKey.CreateSubKey(pluginRegKey);
                foreach (var value in pluginRegInfo)
                {
                    pluginKey.SetValue(value.Name, value.Value, value.Type);
                }
            }
            catch (Exception e)
            {
                throw;
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
        private void btnUninstall_Click(object sender, EventArgs e)
        {
            foreach (var item in checkedListBox.CheckedItems)
            {
                var i = item as AutoCADVersion;
                try
                {
                    if (i == null) continue;

                    var keyRoot = Registry.CurrentUser.OpenSubKey(autoCADRegKey, true); //AutoDesk/AutoCAD
                    var keyVer = keyRoot.OpenSubKey(i.VersionCode, true); //Ex: AutoDesk/AutoCAD/R22.0
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
                    MessageBox.Show("Install plugin for " + i.Display + " failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
    class AutoCADVersion
    {
        public string Version { get; set; }
        public string VersionCode { get; set; }
        public string Display { get; set; }
        public AutoCADVersion()
        {

        }
        public AutoCADVersion(string version, string code, string display)
        {
            this.Version = version;
            this.VersionCode = code;
            this.Display = display;
        }
        public override string ToString()
        {
            return this.Display;
        }
    }
    class PluginRegistryInfoValue
    {
        public string Name { get; set; }
        public RegistryValueKind Type { get; set; }
        public Object Value { get; set; }
        public PluginRegistryInfoValue(string name, Object value, RegistryValueKind type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }
    }
}
