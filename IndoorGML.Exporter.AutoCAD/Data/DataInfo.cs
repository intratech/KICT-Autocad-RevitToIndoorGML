using Autodesk.AutoCAD.DatabaseServices;
using CityGML.Exporter.AutoCAD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Data
{
    public class DataInfo
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Function { get; set; }

        public double Elevation { get; set; }
        public double FloorHeight { get; set; }

        public Polyline Boundary { get; set; }
        public ObjectId ObjectId { get; internal set; }

        private int BackupColor = -1;
        internal void Select()
        {
            if (BackupColor == -1)
            {
                BackupColor = Boundary.ColorIndex;
            }
            
            API.ChangeColor(ObjectId, 2);
        }

        public void UnSelect()
        {
            if (BackupColor != -1)
            {
                API.ChangeColor(ObjectId, BackupColor);
            }
            else if (Boundary.ColorIndex == 2)
            {
                API.ChangeColor(ObjectId, (short)Colors.Red);
            }

        }

        internal void RemoveBoundary()
        {
            API.RemoveEntity(ObjectId);
        }

        internal void UpdateProperty()
        {
            API.UpdateProperty(this);
        }
    }
}
