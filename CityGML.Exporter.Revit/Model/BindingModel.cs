using IndoorGML.Exporter.Revit.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Model
{
    public class BindingModel: INotifyPropertyChanged
    {
        private string _outputFile;
        public string OutputFile {
            get { return _outputFile; }
            set { _outputFile = value; OnPropertyChanged("OutputFile"); }
        }

        private IEnumerable<LevelListBoxItemModel> _levels;
        public IEnumerable<LevelListBoxItemModel> Levels
        {
            get
            {
                return _levels;
            }
            set
            {
                _levels = value;
                OnPropertyChanged("Levels");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        
    }
}
