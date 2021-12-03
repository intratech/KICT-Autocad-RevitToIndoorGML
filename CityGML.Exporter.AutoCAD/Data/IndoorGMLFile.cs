using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Xml;

namespace CityGML.Exporter.AutoCAD.Data
{
    class IndoorGMLFile
    {
        public string GmlFile { get; set; }
        public string Floor { get; set; }
        public string Building { get; set; }

        public string FileName { get; set; }


        public bool ParseFile(string gmlFile)
        {
            if (!File.Exists(gmlFile))
                return false;

            try
            {
                GmlFile = gmlFile;
                FileName = Path.GetFileNameWithoutExtension(gmlFile);

                XmlDocument doc = new XmlDocument();
                doc.Load(gmlFile);
                var roots = doc.GetElementsByTagName("core:IndoorFeatures");
                if (roots.Count == 0)
                    return false;
                var node = roots[0] as XmlElement;
                var child =  node.GetElementsByTagName("gml:description");
                if(child != null && child.Count>0)
                {
                    Floor = child[0].InnerText;
                }
                else
                {
                    Floor = "None";
                }

                child = node.GetElementsByTagName("gml:name");
                if (child != null && child.Count > 0)
                {
                    Building = child[0].InnerText;
                }
                else
                {
                    Building = "None";
                }

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                return false;
            }
        }
    }
}
