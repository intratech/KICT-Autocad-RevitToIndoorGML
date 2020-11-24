using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using IndoorGML.Exporter.Revit.Model;
using Microsoft.Win32;
using IndoorGML.Exporter.Revit.Config;
using IndoorGML.Exporter.Revit.Entities;
using IndoorGML.Exporter.Revit.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Environment = IndoorGML.Exporter.Revit.Config.Environment;
using System.Text.RegularExpressions;
using IndoorGML.Exporter.Revit.Exporter;
using System.Diagnostics;

namespace IndoorGML.Exporter.Revit.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        UIApplication m_uiApp;
        Document m_doc;

        Process publisherProcess;

        public BindingModel _model { get; set; }
        private AppSettings _settings;

        IEnumerable<WallType> m_wallTypes;
        IEnumerable<FamilySymbol> m_doorTypes;
        bool m_csWallExisted = false;
        bool m_vdDoorExisted = false;

        const string csName = "cs";
        const string vdName = "vd";

        #region Bindings
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private RoomScheduleBindingList _roomsSchedule;
        public RoomScheduleBindingList RoomSchedule
        {
            get { return _roomsSchedule; }
            set { _roomsSchedule = value; OnPropertyChanged("RoomSchedule"); }
        }

        public DoorScheduleBindingList _doorsSchedule { get; set; }
        public DoorScheduleBindingList DoorSchedule
        {
            get { return _doorsSchedule; }
            set { _doorsSchedule = value; OnPropertyChanged("DoorSchedule"); }
        }

        private string _statusMsg;
        public string StatusMsg {
            get { return _statusMsg; }
            set { _statusMsg = value; OnPropertyChanged("StatusMsg"); }
        }

        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged("Progress"); }
        }

        private double _configWallThickness;
        public double ConfigWallThickness
        {
            get { return _configWallThickness; }
            set { _configWallThickness = value; OnPropertyChanged("ConfigWallThickness"); }
        }
        private string _configElevatorLabels;
        public string ConfigElevatorLabels
        {
            get { return _configElevatorLabels; }
            set { _configElevatorLabels = value; OnPropertyChanged("ConfigElevatorLabels"); }
        }
        private string _configEscaplatorLabels;
        public string ConfigEscaplatorLabels
        {
            get { return _configEscaplatorLabels; }
            set { _configEscaplatorLabels = value; OnPropertyChanged("ConfigEscaplatorLabels"); }
        }
        private string configStairLabels;
        public string ConfigStairLabels
        {
            get { return configStairLabels; }
            set { configStairLabels = value; OnPropertyChanged("ConfigStairLabels"); }
        }

        private string logPath;
        public string LogPath
        {
            get {
                if (string.IsNullOrEmpty(logPath))
                {
                    logPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\RevitToIndoorGML.log";
                }
                return logPath; 
            }
            set { logPath = value; OnPropertyChanged("LogPath"); }
        }

        #endregion

        public MainWindow(UIApplication uiapp)
        {
            this.m_uiApp = uiapp;
            this.m_doc = uiapp.ActiveUIDocument.Document;
            this._model = new BindingModel();
            InitializeComponent();

            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRooms();
            LoadDoors();
            LoadLevels();
            LoadSettings();

            m_wallTypes = GetAllBasicWallTypes();
            m_doorTypes = GetAllDoorTypes();

            m_csWallExisted = m_wallTypes.Any(w => w.Name == csName);

            var rvFileName = System.IO.Path.GetFileNameWithoutExtension(m_doc.PathName);
            var defaultOuputFileName = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), rvFileName + ".gml");

            _model.OutputFile = defaultOuputFileName;

            
        }
        private void ZoomToElement(int elementId)
        {
            try
            {
                this.m_uiApp.ActiveUIDocument.ShowElements(new ElementId(elementId));
            }
            catch (System.Exception ex)
            {

            }
        }
        
        private IEnumerable<WallType> GetAllBasicWallTypes()
        {
            return new FilteredElementCollector(m_doc)
            .OfClass(typeof(WallType)).Cast<WallType>().Where(w => w.Kind == WallKind.Basic);
        }
        private IEnumerable<FamilySymbol> GetAllDoorTypes()
        {
            var doorTypes = new FilteredElementCollector(m_doc)
            .OfCategory(BuiltInCategory.OST_Doors).WhereElementIsElementType().Cast<FamilySymbol>();

            return doorTypes;
        }
        private void LoadSettings()
        {
            this._settings = AppSettings.Instance;
            this._settings.Load();

            ConfigWallThickness = _settings.WallThickness;
            ConfigElevatorLabels = string.Join(";", _settings.ElevatorLabels);
            ConfigEscaplatorLabels = string.Join(";", _settings.EscaplatorLabels);
            ConfigStairLabels = string.Join(";", _settings.StairLabels);
        }
        private void LoadLevels()
        {
            _model.Levels = LevelUtils.GetLevelsByRooms(m_doc).Where(i=>i!=null).Select(l => new LevelListBoxItemModel(l.Id.IntegerValue, l.Name, true));
            //_model.Levels = LevelUtils.GetLevelsByRooms(m_doc).Select(l => new LevelListBoxItemModel(l.Id.IntegerValue, l.Name, true));
        }
        private void LoadRooms()
        {
            Task.Run(() => {
                var rooms = new FilteredElementCollector(m_doc).OfCategory(BuiltInCategory.OST_Rooms).Cast<Room>();
                var rows = new RoomScheduleBindingList();
                rows.AddRange(rooms.Select(r => {
                    var isNotPlaced = r.Area <= 0 && r.Location == null;
                    int number = 0;
                    int.TryParse(r.Number, out number);

                    return new RoomScheduleItem
                    {
                        Id = r.Id.IntegerValue,
                        Level = isNotPlaced ? Environment.CONST_NOT_PLACE : r.Level?.Name,
                        Name = r.Name,
                        Number = number,
                        Area = r.Area.ToString(),
                        Location = r.Location?.ToString(),
                        HighLight = isNotPlaced
                    };
                }));

                RoomSchedule = rows;
                RoomSchedule.Filters.Clear();
                RoomSchedule.Filters.AddRange(RoomSchedule.GroupBy(r => r.Level).Select(g => new ColumnFilter(g.Key, "Level")));
                RoomSchedule.Filters.AddRange(RoomSchedule.GroupBy(r => r.Name).Select(g => new ColumnFilter(g.Key, "Name")));
                RoomSchedule.Filters.AddRange(RoomSchedule.GroupBy(r => r.Number).Select(g => new ColumnFilter(g.Key.ToString(), "Number")));

                Log.Info($"Load rooms: {rows.Count}");
            });
        }
        private void LoadDoors()
        {
            Task.Run(() => {
                var doors = new FilteredElementCollector(m_doc).OfCategory(BuiltInCategory.OST_Doors).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>();
                var rows = new DoorScheduleBindingList();
                rows.AddRange(doors.Select(d => {
                    var isNotPlaced = d.FromRoom == null && d.ToRoom == null;
                    var level = "";
                    if (d.LevelId != ElementId.InvalidElementId)
                    {
                        var _lv = m_doc.GetElement(d.LevelId);
                        if (_lv != null)
                        {
                            level = _lv.Name;
                        }
                    }
                    var mark = "";
                    var markNo = 0;
                    var parameters = d.GetParameters("Mark");
                    if (parameters != null && parameters.Count > 0)
                    {
                        var _mark = parameters.FirstOrDefault();
                        if (_mark != null)
                        {
                            mark = _mark.AsString();
                            if (string.IsNullOrEmpty(mark))
                            {
                                mark = _mark.AsValueString();
                            }
                        }
                    }

                    int.TryParse(mark, out markNo);

                    var type = "";
                    if (d.GetTypeId() != ElementId.InvalidElementId)
                    {
                        var _type = m_doc.GetElement(d.GetTypeId());
                        if (_type.Name != null)
                        {
                            type = _type.Name;
                        }
                    }

                    return new DoorScheduleItem
                    {
                        Id = d.Id.IntegerValue,
                        Level = level,
                        Mark = markNo,
                        Type = type,
                        FromRoom = d.FromRoom?.Name,
                        ToRoom = d.ToRoom?.Name,
                        HighLight = isNotPlaced
                    };
                }));

                DoorSchedule = rows;

                DoorSchedule.Filters.Clear();
                DoorSchedule.Filters.AddRange(DoorSchedule.GroupBy(r => r.Level).Select(g => new ColumnFilter(g.Key, Environment.COLUMN_LEVEL)));
                DoorSchedule.Filters.AddRange(DoorSchedule.GroupBy(r => r.Mark).Select(g => new ColumnFilter(g.Key?.ToString(), Environment.COLUMN_MARK)));
                DoorSchedule.Filters.AddRange(DoorSchedule.GroupBy(r => r.Type).Select(g => new ColumnFilter(g.Key, Environment.COLUMN_TYPE)));
                DoorSchedule.Filters.AddRange(DoorSchedule.GroupBy(r => r.FromRoom).Select(g => new ColumnFilter(g.Key, Environment.COLUMN_FROM_ROOM)));
                DoorSchedule.Filters.AddRange(DoorSchedule.GroupBy(r => r.ToRoom).Select(g => new ColumnFilter(g.Key, Environment.COLUMN_TO_ROOM)));

                Log.Info($"Load doors: {rows.Count}");
            });
        }
        private void FilterRoomSchedule()
        {
            var filteredLevel = RoomSchedule.Filters.Where(c => c.Active && c.ColumnName == Environment.COLUMN_LEVEL).Select(c => c.Text);
            var filteredName = RoomSchedule.Filters.Where(c => c.Active && c.ColumnName == Environment.COLUMN_NAME).Select(c => c.Text);
            var filteredNumber = RoomSchedule.Filters.Where(c => c.Active && c.ColumnName == Environment.COLUMN_NUMBER).Select(c => c.Text);

            IEnumerable<RoomScheduleItem> filteredRooms = new List<RoomScheduleItem>(this.RoomSchedule);
            filteredRooms = filteredRooms.Where(r => filteredLevel.Contains(r.Level));
            filteredRooms = filteredRooms.Where(r => filteredName.Contains(r.Name));
            filteredRooms = filteredRooms.Where(r => filteredNumber.Contains(r.Number.ToString()));

            tbRoom.ItemsSource = filteredRooms;
        }
        private void FilterDoorSchedule()
        {
            var filteredLevel = DoorSchedule.Filters.Where(c => c.Active && c.ColumnName == Environment.COLUMN_LEVEL).Select(c => c.Text);
            var filteredMark = DoorSchedule.Filters.Where(c => c.Active && c.ColumnName == Environment.COLUMN_MARK).Select(c => c.Text);
            var filteredType = DoorSchedule.Filters.Where(c => c.Active && c.ColumnName == Environment.COLUMN_TYPE).Select(c => c.Text);
            var filteredFromRoom = DoorSchedule.Filters.Where(c => c.Active && c.ColumnName == Environment.COLUMN_FROM_ROOM).Select(c => c.Text);
            var filteredToRoom = DoorSchedule.Filters.Where(c => c.Active && c.ColumnName == Environment.COLUMN_TO_ROOM).Select(c => c.Text);

            IEnumerable<DoorScheduleItem> filteredDoors = new List<DoorScheduleItem>(this.DoorSchedule);
            filteredDoors = filteredDoors.Where(r => filteredLevel.Contains(r.Level));
            filteredDoors = filteredDoors.Where(r => filteredMark.Contains(r.Mark?.ToString()));
            filteredDoors = filteredDoors.Where(r => filteredType.Contains(r.Type));
            filteredDoors = filteredDoors.Where(r => filteredFromRoom.Contains(r.FromRoom));
            filteredDoors = filteredDoors.Where(r => filteredToRoom.Contains(r.ToRoom));


            tbDoor.ItemsSource = filteredDoors;
        }

        #region Event Handlers
        private void btnExportSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Log.Setup(LogPath, "Revit to IndoorGML");

                this.Progress = 0;
                var outputDir = IOUtils.ShowSelectFolderDialog();
                if (string.IsNullOrEmpty(outputDir))
                    return;

                var rvFileName = System.IO.Path.GetFileNameWithoutExtension(m_doc.PathName);

                var roomOutputFile = System.IO.Path.Combine(outputDir, rvFileName + "_room_schedule.csv");
                var doorOutputFile = System.IO.Path.Combine(outputDir, rvFileName + "_door_schedule.csv");
                
                var excelExporter = new ExcelExporter();
                excelExporter.ExportRoomSchedule(this.RoomSchedule, roomOutputFile);
                excelExporter.ExportDoorSchedule(this.DoorSchedule, doorOutputFile);

                this.Progress = 100;
                this.StatusMsg = "Export schedule completed.";
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                MessageBox.Show(ex.Message);
            }
        }
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(_model.OutputFile)))
            {
                MessageBox.Show("Invalid output file", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Log.Setup(LogPath, "Revit to IndoorGML");

            try
            {
                Log.Info("***************************************");
                Log.Info("Start convert model " + m_uiApp.ActiveUIDocument.Document.PathName);
                Log.Info("Output " + _model.OutputFile);
                Stopwatch sw = new Stopwatch();
                sw.Start();

                var levelsToExport = _model.Levels.Where(l => l.Checked).Select(l => l.Id);
                if(levelsToExport.Count() == 0)
                {
                    MessageBox.Show("Please select at least 1 level to export", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Log.Info("Levels to export " + string.Join(";", _model.Levels.Where(l => l.Checked).Select(l=>l.Name)));
                Log.Info("Default wall thickness : " + ConfigWallThickness);

                this.Progress = 0;
                this.StatusMsg = "Exporting";

                var ex = new TwoDExporter();
                ex.Export(m_doc, _model.OutputFile, levelsToExport, ConfigWallThickness);

                this.Progress = 100;
                this.StatusMsg = "Export completed.";

                sw.Stop();
                Log.Info($"Total exporting time {sw.Elapsed.TotalSeconds} seconds");
                Log.Info("***************************************");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LaunchPublisher()
        {
            try
            {
                if (publisherProcess != null && !publisherProcess.HasExited)
                    return;

                var pi = new ProcessStartInfo
                {
                    FileName = System.IO.Path.Combine(IOUtils.GetCurrentExecutingAssemplyLocation(), "CityGMLPublisher", "CityGMLPublisher.exe")
                    //Arguments = @"D:\Projects\Project_Net\CityGML\CityGMLPublisher\main.js"
                };
                publisherProcess = Process.Start(pi);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadRooms();
            LoadDoors();
        }
        private void tbRoom_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    var room = dgr.DataContext as RoomScheduleItem;
                    ZoomToElement(room.Id);
                }
            }
        }
        private void tbDoor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    var door = dgr.DataContext as DoorScheduleItem;
                    ZoomToElement(door.Id);
                }
            }
        }
     

        //Table Room
        private void GridRoom_ColLevel_Click(object sender, RoutedEventArgs e)
        {
            listBoxFilter.ItemsSource = this.RoomSchedule.Filters.Where(f => f.ColumnName == Environment.COLUMN_LEVEL).Select(f => new CheckBoxModel(f.Id, f.Text, f.Active));
            tbRoomPopupFilterBtnSelectAll.Text = listBoxFilter.ItemsSource.Cast<CheckBoxModel>().Any(i => i.IsChecked == false) ? "Select all" : "Unselect all";
            myPopup.IsOpen = true;
        }
        private void GridRoom_ColName_Click(object sender, RoutedEventArgs e)
        {
            listBoxFilter.ItemsSource = this.RoomSchedule.Filters.Where(f => f.ColumnName == Environment.COLUMN_NAME).Select(f => new CheckBoxModel(f.Id, f.Text, f.Active));
            tbRoomPopupFilterBtnSelectAll.Text = listBoxFilter.ItemsSource.Cast<CheckBoxModel>().Any(i => i.IsChecked == false) ? "Select all" : "Unselect all";
            myPopup.IsOpen = true;
        }
        private void GridRoom_ColNumber_Click(object sender, RoutedEventArgs e)
        {
            listBoxFilter.ItemsSource = this.RoomSchedule.Filters.Where(f => f.ColumnName == Environment.COLUMN_NUMBER).Select(f => new CheckBoxModel(f.Id, f.Text, f.Active));
            tbRoomPopupFilterBtnSelectAll.Text = listBoxFilter.ItemsSource.Cast<CheckBoxModel>().Any(i => i.IsChecked == false) ? "Select all" : "Unselect all";
            myPopup.IsOpen = true;
        }


        private void tbDoor_ColLevel_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopupForTbDoor(Environment.COLUMN_LEVEL);
        }
        private void tbDoor_ColMark_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopupForTbDoor(Environment.COLUMN_MARK);
        }
        private void tbDoor_ColType_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopupForTbDoor(Environment.COLUMN_TYPE);
        }
        private void tbDoor_ColFromRoom_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopupForTbDoor(Environment.COLUMN_FROM_ROOM);
        }
        private void tbDoor_ColToRoom_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterPopupForTbDoor(Environment.COLUMN_TO_ROOM);
        }

        private void ShowFilterPopupForTbDoor(string colname)
        {
            tbDoorFilterPopupListBox.ItemsSource = this.DoorSchedule.Filters.Where(f => f.ColumnName == colname).Select(f => new CheckBoxModel(f.Id, f.Text, f.Active));
            tbDoorFilterPopupBtnSelectAll.Text = tbDoorFilterPopupListBox.ItemsSource.Cast<CheckBoxModel>().Any(i => i.IsChecked == false) ? "Select all" : "Unselect all";
            tbDoorFilterPopup.IsOpen = true;
        }


        private void tbRoomFilterPopup_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = e.Source as CheckBox;
            var item = checkBox.DataContext as CheckBoxModel;
            var filterItem = this.RoomSchedule.Filters.Where(f => f.Id == item.Id).FirstOrDefault();
            filterItem.Active = checkBox.IsChecked != null ? checkBox.IsChecked.Value : false;

            FilterRoomSchedule();
        }
        private void tbDoorFilterPopup_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = e.Source as CheckBox;
            var item = checkBox.DataContext as CheckBoxModel;
            var filterItem = this.DoorSchedule.Filters.Where(f => f.Id == item.Id).FirstOrDefault();
            filterItem.Active = checkBox.IsChecked != null ? checkBox.IsChecked.Value : false;

            FilterDoorSchedule();
        }
        private void tbRoomContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            tbRoomContextMenuItemDelete.IsEnabled = true;
            var selectedItems = tbRoom.SelectedItems.Cast<RoomScheduleItem>();
            foreach (var item in selectedItems)
            {
                if (item.Level != Environment.CONST_NOT_PLACE)
                {
                    tbRoomContextMenuItemDelete.IsEnabled = false;
                    break;
                }
            }
        }
        private void tbRoomContextMenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = tbRoom.SelectedItems.Cast<RoomScheduleItem>();
            if (selectedItems.Count() == 0)
                return;

            //var selectedRoomNumbers = selectedItems.Select(i => i.Number);
            //var rooms = new FilteredElementCollector(m_doc).OfCategory(BuiltInCategory.OST_Rooms).Cast<Room>().Where(r => selectedRoomNumbers.Contains(r.Number));
            //if (rooms == null || rooms.Count() == 0)
            //    return;

            ElementUtils.DeleteElements(m_doc, selectedItems.Select(r => new ElementId(r.Id)));

            LoadRooms();
        }
        private void btnBrowseOutput_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog() { Filter = "CityGML (*.xml)|*.xml| IndoorGML (*.gml)|*.gml" };
            if(sfd != null && sfd.ShowDialog().Value)
            {
                _model.OutputFile = sfd.FileName;
            }
        }
        private void lbLevel_ItemChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = e.OriginalSource as CheckBox;
            var itemModel = checkBox.DataContext as LevelListBoxItemModel;
            var _levels = new List<LevelListBoxItemModel>(_model.Levels);
            foreach (var lv in _levels)
            {
                if(lv.Id == itemModel.Id)
                    lv.Checked = checkBox.IsChecked.Value;
                break;
            }
            _model.Levels = _levels;
        }
        private void ListBoxLevelSelectAll_Click(object sender, RoutedEventArgs e)
        {
            btnSelectAll.Text = btnSelectAll.Text.ToString() == "Select all" ? "Unselect all" : "Select all";
            var state = btnSelectAll.Text.ToString() != "Select all";
            _model.Levels = _model.Levels.Select(l => new LevelListBoxItemModel(l.Id, l.Name, state));
        }

        private void txtWallThickness_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._settings == null)
                return;

            double value = 0;
            if(double.TryParse((e.OriginalSource as System.Windows.Controls.TextBox).Text, out value))
            {
                ConfigWallThickness = value;
                this._settings.WallThickness = ConfigWallThickness;
                this._settings.Save();
            }
        }
        private void txtElevatorLabels_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._settings == null)
                return;

            ConfigElevatorLabels = (e.OriginalSource as System.Windows.Controls.TextBox).Text;
            this._settings.ElevatorLabels = ConfigElevatorLabels.Split(';');
            this._settings.Save();
        }
        private void txtEscaplatorLabels_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._settings == null)
                return;

            ConfigEscaplatorLabels = (e.OriginalSource as System.Windows.Controls.TextBox).Text;
            this._settings.EscaplatorLabels = ConfigEscaplatorLabels.Split(';');
            this._settings.Save();
        }
        private void txtStairLabels_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._settings == null)
                return;

            ConfigStairLabels = (e.OriginalSource as System.Windows.Controls.TextBox).Text;
            this._settings.StairLabels = ConfigStairLabels.Split(';');
            this._settings.Save();
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        private void TbRoomPopupFilterBtnSelectAll_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var isNotChecked = tbRoomPopupFilterBtnSelectAll.Text == "Select all";
            tbRoomPopupFilterBtnSelectAll.Text = isNotChecked ? "Unselect all" :  "Select all";

            listBoxFilter.ItemsSource = listBoxFilter.ItemsSource.Cast<CheckBoxModel>().Select(i => new CheckBoxModel(i.Id, i.Text, isNotChecked));

            foreach (CheckBoxModel item in listBoxFilter.ItemsSource)
            {
                var filterItem = this.RoomSchedule.Filters.Where(f => f.Id == item.Id).FirstOrDefault();
                filterItem.Active = isNotChecked;
            }

            FilterRoomSchedule();
        }
        private void TbDoorPopupFilterBtnSelectAll_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var isNotChecked = tbDoorFilterPopupBtnSelectAll.Text == "Select all";
            tbDoorFilterPopupBtnSelectAll.Text = isNotChecked ? "Unselect all" : "Select all";

            tbDoorFilterPopupListBox.ItemsSource = tbDoorFilterPopupListBox.ItemsSource.Cast<CheckBoxModel>().Select(i => new CheckBoxModel(i.Id, i.Text, isNotChecked));

            foreach (CheckBoxModel item in tbDoorFilterPopupListBox.ItemsSource)
            {
                var filterItem = this.DoorSchedule.Filters.Where(f => f.Id == item.Id).FirstOrDefault();
                filterItem.Active = isNotChecked;
            }

            FilterDoorSchedule();
        }

        private void TbRoom_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void TbRoom_Sorting(object sender, DataGridSortingEventArgs e)
        {
            //e.Column.
        }

        private void BrowseLogFileClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Log files (*.log)|*.log";
            if(saveFile.ShowDialog() == true)
            {
                LogPath = saveFile.FileName;
            }
        }
    }
}
