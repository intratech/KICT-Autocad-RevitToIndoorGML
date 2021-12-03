using GML.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IndoorGML.Core
{
    public class IndoorGMLUtility
    {
        public static void CombineMultipleFile( string output,string building,string description, float tolerance, params string[] input)
        {
            //Data layer 
            IndoorFeatures masterFile = new IndoorFeatures();
            masterFile.Name = building;
            masterFile.Description = description;
            //Logic layer 
            XmlSerializer xmlTool = new XmlSerializer(typeof(IndoorFeatures));

          
            foreach (string f in input)
            {
                if(File.Exists(f))
                {
                    using (FileStream fs = new FileStream(f, FileMode.Open))
                    {
                        var indoor = xmlTool.Deserialize(fs) as IndoorFeatures;
                        masterFile.Append(indoor);
                    }
                }
            }

            //if (types != null)
            //{
            //    foreach (var type in types.Where(x => x.IsVertical))
            //    {
            //        masterFile.ConnectFloors(type.Class, type.Function, type.Usage, tolerance);
            //    }
            //}
            masterFile.ConnectFloors("Vertical", "", "", tolerance);

            masterFile.Save(output);
        }

        public static XmlSerializerNamespaces GetNamespaces()
        {
            XmlSerializerNamespaces xmlNamespaces =
            new XmlSerializerNamespaces();

            // Add three prefix-namespace pairs.
            xmlNamespaces.Add("gml", "http://www.opengis.net/gml/3.2");
            xmlNamespaces.Add("xlink", "http://www.w3.org/1999/xlink");
            xmlNamespaces.Add("core", "http://www.opengis.net/indoorgml/1.0/core");
            //Sorry customer don't want use Navi
            //xmlNamespaces.Add("navi", "http://www.opengis.net/indoorgml/1.0/navigation");          
            xmlNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            
          


            return xmlNamespaces;
        }
    }
}
