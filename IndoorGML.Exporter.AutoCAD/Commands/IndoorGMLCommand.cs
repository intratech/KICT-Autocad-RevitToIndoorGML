using Autodesk.AutoCAD.ApplicationServices;
using CityGML.Exporter.AutoCAD.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IndoorGML.Exporter.AutoCAD.Command
{
    class IndoorGMLCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Plugin.tvp.Show();
        }
    }
}
