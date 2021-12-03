using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IndoorGML.Exporter.Revit.Entities
{
    public class RoomScheduleItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public bool HighLight { get; set; }
        public string Area { get; set; }
        public string Location { get; set; }

        private string type;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Type 
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
            }
        }

        private string function;
        public string Function
        {
            get
            {
                return function;
            }
            set
            {
                function = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Function)));
            }
        }

        public RoomScheduleItem()
        {

        }
        public RoomScheduleItem(int id, string level, string name, int number, bool highLight = false)
        {
            this.Id = id;
            Level = level;
            Name = name;
            Number = number;
            HighLight = highLight;
        }
    }
    public class DoorScheduleItem
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public int? Mark { get; set; }
        public string Type { get; set; }
        public string FromRoom { get; set; }
        public string ToRoom { get; set; }
        public bool HighLight { get; set; }

        public DoorScheduleItem()
        {

        }
    }
    public class CheckBoxModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }

        public bool IsVisible { get; set; } = true;

        public Visibility Visibility
        {
            get
            {
                if (IsVisible)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
        }

        
        public CheckBoxModel()
        {

        }
        public CheckBoxModel(string id, string text, bool isChecked = false)
        {
            this.Id = id;
            this.Text = text;
            this.IsChecked = isChecked;
        }
    }

}
