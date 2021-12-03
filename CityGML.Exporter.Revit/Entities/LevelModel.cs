using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Entities
{
    public class LevelListBoxItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }

        public LevelListBoxItemModel() { }
        public LevelListBoxItemModel(int id, string name, bool isChecked = false) {
            this.Id = id;
            this.Name = name;
            this.Checked = isChecked;
        }
    }
}
