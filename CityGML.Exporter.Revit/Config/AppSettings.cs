using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace IndoorGML.Exporter.Revit.Config
{
    [Serializable]
    public sealed class AppSettings
    {
        public double WallThickness { get; set; } = 350;
        public string[] ElevatorLabels { get; set; } = new string[] { "엘레베이터", "E.V", "E/V", "ELEV" };
        public string[] EscaplatorLabels { get; set; } = new string[] { "에스컬레이터" };
        public string[] StairLabels { get; set; } = new string[] { "에스컬레이터","계단실" };

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static AppSettings _instance;
        public static AppSettings Instance {
            get {
                if(_instance == null)
                {
                    _instance = new AppSettings();
                }
                return _instance;
            }
        }

        private AppSettings() { }

        
        public AppSettings Load()
        {
            try
            {
                using (var wt = new StreamReader(Environment.AppSettingFile))
                {
                    var serializer = new XmlSerializer(typeof(AppSettings));
                    var settings = (AppSettings)serializer.Deserialize(wt);
                    this.WallThickness = settings.WallThickness;
                    this.ElevatorLabels = settings.ElevatorLabels;
                    this.EscaplatorLabels = settings.EscaplatorLabels;
                    this.StairLabels = settings.StairLabels;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return this;
        }
        public bool Save()
        {
            try
            {
                using (var wt = new StreamWriter(Environment.AppSettingFile))
                {
                    var serializer = new XmlSerializer(typeof(AppSettings));
                    var settings = new AppSettings() {
                        WallThickness = this.WallThickness,
                        ElevatorLabels = this.ElevatorLabels,
                        EscaplatorLabels = this.EscaplatorLabels,
                        StairLabels = this.StairLabels
                    };
                    serializer.Serialize(wt, settings);
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return false;
            }
        }
    }
}
