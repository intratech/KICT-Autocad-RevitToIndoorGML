using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CityGML.Exporter.AutoCAD.Entities;
using Autodesk.AutoCAD.DatabaseServices;
using CityGML.Exporter.AutoCAD.Data;
using GML.Core.DTO.Json;
using Autodesk.AutoCAD.Geometry;
using CityGML.Exporter.AutoCAD.IndoorData;
using IndoorGML.Adapter;
using CityGML.Exporter.AutoCAD.Utils;
using System.IO;
using IndoorGML.Core;
using Pos = GML.Core.DTO.Json.Pos;
using System.Diagnostics;
using IndoorGML.Exporter.AutoCAD.CAD;
using AppCad = Autodesk.AutoCAD.ApplicationServices.Application;
using Microsoft.WindowsAPICodePack.Dialogs;
using Region = System.Drawing.Region;
using IndoorGML.Exporter.AutoCAD.Utils;
using GML.Core;
using IndoorGML.Exporter.AutoCAD.Entities;

namespace CityGML.Exporter.AutoCAD.UI
{
    public partial class IndoorGMLControl : UserControl
    {

        public string[] DoorLayers { get; set; }
        public string[] RoomSeperators { get; set; }

        private BindingList<DataInfo> dataSource = new BindingList<DataInfo>();

        private BindingList<IndoorGMLFile> sourceCombines = new BindingList<IndoorGMLFile>();

        public List<JDoor> Doors = new List<JDoor>();
        private UnitsValue currentUnit =  UnitsValue.Millimeters;

        private Dictionary<string, DataInfo[]> memoryCachingSpaces = new Dictionary<string, DataInfo[]>();
        private string currentDocument = "";

        private UnitSpaceFunctionExcel unitSpaceFunction;
        public IndoorGMLControl()
        {
            InitializeComponent();
            

            dataSource.AllowNew = false;

            dataGridView_Model.AutoGenerateColumns = false;
            dataGridView_Model.DataSource = dataSource;

            dataGridView_CombineFiles.AutoGenerateColumns = false;
            sourceCombines.AllowEdit = false;
            sourceCombines.AllowNew = false;
            dataGridView_CombineFiles.DataSource = sourceCombines;
            dataGridView_CombineFiles.ColumnSortModeChanged += DataGridView_CombineFiles_ColumnSortModeChanged;

            unitSpaceFunction = new UnitSpaceFunctionExcel();
            unitSpaceFunction.ReadFile();

            ColumnType.DataSource = unitSpaceFunction.UnitSpaceClasses;
            ColumnType.DisplayMember = "Name";

            ColumnFunction.DataSource = unitSpaceFunction.FunctionCodes;
            ColumnFunction.DisplayMember = "Name";

            comboBox_DefaultClass.Items.AddRange(unitSpaceFunction.UnitSpaceClasses.Select(x => x).ToArray());
            comboBox_DefaultClass.DisplayMember = "Name";
            
            comboBox_DefaultClass.SelectedItem = unitSpaceFunction.UnitSpaceClasses.Where(x => x.Name == RegSetting.GetSetting(comboBox_DefaultClass.Name)).FirstOrDefault();
            if (comboBox_DefaultClass.SelectedItem == null)
                comboBox_DefaultClass.SelectedItem = unitSpaceFunction.UnitSpaceClasses.FirstOrDefault();

            comboBox_DefaultClass.SelectedIndexChanged += (a, b) =>
            {
                var _item = comboBox_DefaultClass.SelectedItem as UnitSpaceClass;
                RegSetting.SetSetting(comboBox_DefaultClass.Name, _item.Name);
            };

            comboBox_DefaultFunction.Items.AddRange(unitSpaceFunction.FunctionCodes.ToArray());
            comboBox_DefaultFunction.DisplayMember = "Name";
            comboBox_DefaultFunction.SelectedItem = unitSpaceFunction.FunctionCodes.Where(x => x.Name == RegSetting.GetSetting(comboBox_DefaultFunction.Name)).FirstOrDefault();
            if (comboBox_DefaultFunction.SelectedItem == null)
                comboBox_DefaultFunction.SelectedItem = unitSpaceFunction.FunctionCodes.FirstOrDefault();

            comboBox_DefaultFunction.SelectedIndexChanged += (a, b) =>
            {
                var _item = comboBox_DefaultFunction.SelectedItem as FunctionCode;
                RegSetting.SetSetting(comboBox_DefaultFunction.Name, _item.Name);
            };


            //ColumnFunc

            dataGridView_Model.DataError += dataGridView_Model_DataError;

            if (API.ActiveDoc != null)
            {
                textBox_Output.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }
            else
            {
                textBox_Output.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }

            textBox_CombineOutput.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Master.gml");

            comboBox_Unit.DataSource = Enum.GetValues(typeof(UnitsValue));
            int? idx = RegSetting.GetSettingAsNumber("UnitCBB1");
            if (idx != null)
            {
                comboBox_Unit.SelectedIndex = idx.Value;
                currentUnit = (UnitsValue)comboBox_Unit.SelectedItem;
            }
            else
            {
                comboBox_Unit.SelectedIndex = (int)UnitsValue.Millimeters;
                currentUnit = UnitsValue.Millimeters;
            }

            try
            {
                label_unit.Text = comboBox_Unit.SelectedItem.ToString();
                label_unit_1.Text = comboBox_Unit.SelectedItem.ToString();
                label_unit_2.Text = comboBox_Unit.SelectedItem.ToString();
            }
            catch { }

            comboBox_Unit.SelectedIndexChanged += ComboBox_Unit_SelectedIndexChanged;

            AppCad.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;
            
        }

       

        private void DataGridView_CombineFiles_ColumnSortModeChanged(object sender, DataGridViewColumnEventArgs e)
        {
            switch (e.Column.Name)
            {
                case "FileName":
                    {
                        var items = sourceCombines.OrderBy(x => x.FileName);
                        sourceCombines.Clear();
                        foreach (var item in items)
                            sourceCombines.Add(item);
                    }
                    break;
            }

        }

        private void dataGridView_Model_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
          
        }

     

     

        private void DocumentManager_DocumentActivated(object sender, Autodesk.AutoCAD.ApplicationServices.DocumentCollectionEventArgs e)
        {
            if(!string.IsNullOrEmpty(currentDocument))
            {
                LogUtils.Info("Model changed " + currentDocument);
                memoryCachingSpaces[currentDocument] = dataSource.ToArray();               
            }


            try
            {
                var key = API.ActiveDoc?.Database?.Filename + "_" + API.ActiveDoc?.Name;
                dataSource?.Clear();
                currentDocument = key;
                if (!string.IsNullOrEmpty(key))
                {
                    LogUtils.Info("Model actived " + key);
                    if (memoryCachingSpaces.ContainsKey(key))
                    {
                        var datas = memoryCachingSpaces[key];
                        if (datas != null)
                        {
                            foreach (var item in datas)
                            {
                                item.UnSelect();
                                dataSource?.Add(item);
                            }
                        }
                    }
                }

                if (textBox_FileName != null && API.ActiveDoc != null)
                {
                    textBox_FileName.Text = Path.GetFileNameWithoutExtension(API.ActiveDoc.Name) + ".gml";
                }
            }
            catch(Exception ex)
            {
                LogUtils.Error(ex.Message, ex);
            }
        }

       

        private void ComboBox_Unit_SelectedIndexChanged(object sender, EventArgs e)
        {
            var changed = (UnitsValue)comboBox_Unit.SelectedItem;
            var scale = (float)UnitsConverter.GetConversionFactor(currentUnit, changed);

            try
            {
                //int? idx = RegSetting.GetSettingAsNumber("UnitCBB1");
                RegSetting.SetSetting("UnitCBB1", ((int)changed).ToString());
            }
            catch { }
            try
            {
                textBox_Elevation.Text = (scale * float.Parse(textBox_Elevation.Text)).ToString("0.0");
            }
            catch { }

            try
            {
                textBox_Height.Text = (scale * float.Parse(textBox_Height.Text)).ToString("0.0");
            }
            catch { }

            try
            {
                textBox_VerticalTransitionTolerance.Text = (scale * float.Parse(textBox_VerticalTransitionTolerance.Text)).ToString("0.0");
            }
            catch { }
            currentUnit = changed;

            try
            {
                label_unit.Text = comboBox_Unit.SelectedItem.ToString();
                label_unit_1.Text = comboBox_Unit.SelectedItem.ToString();
                label_unit_2.Text = comboBox_Unit.SelectedItem.ToString();
            }
            catch { }

            //RegSetting.SetSetting("UnitCBB", comboBox_Unit.SelectedIndex.ToString());
        }

        private bool UpdateRoomLayerName()
        {
            if(string.IsNullOrEmpty(textBox_RoomLayerName.Text))
            {
                MessageBox.Show("Please input the room layer name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Config.RoomLayer = textBox_RoomLayerName.Text;
            return true;
        }
        private void button_CreateRoom_Click(object sender, EventArgs e)
        {
            if (!UpdateRoomLayerName())
                return;
            var texts = API.GetMTextObjs().ToList();
            GetBoundary(texts);
        }

        private void GetBoundary(List<CadText> texts)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            DBObjectCollection roomBoundary;
            if (API.RoomPicker(out roomBoundary, null))
            {
                if (roomBoundary == null || roomBoundary.Count == 0)
                    return;
                Polyline polyline = null;
                foreach (var obj in roomBoundary)
                {
                    if (obj is Polyline)
                    {
                        polyline = obj as Polyline;
                        break;
                    }
                }

                if (polyline != null)
                {
                    API.ActiveDoc?.Editor.WriteMessage("\nCreate space sucessful");

                    API.AddRegAppTableRecord(Config.AppName);
                    var oid = API.CreateOutLine(polyline, Config.RoomLayer);
                    var name = GetRoomName(polyline, texts);
                    if(string.IsNullOrEmpty(name))
                    {
                        name = "Room " + (dataSource.Count + 1);
                    }
                    var data = new DataInfo()
                    {
                        Elevation = float.Parse(textBox_Elevation.Text),
                        FloorHeight = float.Parse(textBox_Height.Text),
                        Name =name,
                        Type = GetDefaultClass(),
                        Function = GetDefaultFunction(),
                        ID = polyline.Handle.Value,
                        Boundary = polyline,
                        ObjectId = oid
                    };
                    data.UpdateProperty();

                    dataSource.Add(data);

                    //Continous getting room
                    GetBoundary(texts);


                }

            }
        }

        private string GetDefaultFunction()
        {
            var _class = comboBox_DefaultFunction.SelectedItem as FunctionCode;
            if (_class != null)
                return _class.Name;
            return "";
        }

        private string GetDefaultClass()
        {
            var _class = comboBox_DefaultClass.SelectedItem as UnitSpaceClass;
            if (_class != null)
                return _class.Name;
            return "";
        }

        private string GetRoomName(Polyline polyline, List<CadText> texts)
        {
            if (texts == null || texts.Count == 0)
                return "";
            string rs = "";
            foreach(var text in texts)
            {
                if(PolyUtility.PointInPolygon(text.Position.X, text.Position.Y, polyline))
                {
                    rs += text.Text + Environment.NewLine;
                }
            }
            return rs;
        }

        private void button_Export_Click(object sender, EventArgs e)
        {
            if (!UpdateRoomLayerName())
                return;

            if (string.IsNullOrEmpty(textBox_Output.Text) || string.IsNullOrEmpty(textBox_FileName.Text))
            {
                MessageBox.Show("Please input the output file path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                LogUtils.Setup(textBox_LogFile.Text, "Autocad to IndoorGML");
                LogUtils.Info("******************************");
                LogUtils.Info("Start Exporting IndoorGML");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                string path = Path.Combine(textBox_Output.Text,textBox_FileName.Text);
                if (!path.EndsWith(".gml", StringComparison.OrdinalIgnoreCase))
                    path += ".gml";

                LogUtils.Info("Creating " + path);
                double.TryParse(textBox_VerticalTransitionTolerance.Text, out GML.Core.Config.ToleranceSameArea);
                //GML.Core.Config.DefaultThickness=0.05;
               // double.TryParse(textBox_DefaultThickness.Text, out GML.Core.Config.DefaultThickness);
                //LogUtils.Info("Default thickness :" + GML.Core.Config.DefaultThickness);
                LogUtils.Info("Door layers :" + textBox_LayerVirtualDoor.Text);
                LogUtils.Info("Room sperators :" + textBox_RoomSeperate.Text);
                LogUtils.Info("Building :" +textBox_Building.Text);
                LogUtils.Info("Floor name :" + textBox_FloorName.Text);
                LogUtils.Info("Floor height:" + textBox_Height.Text);
                LogUtils.Info("Elevation:" + textBox_Elevation.Text);

                LogUtils.Info("Default space :" + comboBox_DefaultClass.Text);
                LogUtils.Info("Default Function:" + comboBox_DefaultFunction.Text);

                API.ApplyScaleUnit((UnitsValue)comboBox_Unit.SelectedItem);

                DoorLayers = textBox_LayerVirtualDoor.Text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                RoomSeperators = textBox_RoomSeperate.Text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                Doors.Clear();

                LogUtils.Info("Get all door entity on " + textBox_LayerVirtualDoor.Text);
                var doorEntity = GetAllEntityOnLayer(DoorLayers);
                var sperateRoom = GetAllEntityOnLayer(RoomSeperators);

                foreach (DataInfo x in dataSource)
                {
                    GetDoor(x, doorEntity);
                    GetSeperateLine(x, sperateRoom);
                }

                doorEntity.Clear();

                LogUtils.Info($"Room count {dataSource.Count}");
                LogUtils.Info($"Door count {Doors.Count}");

                JModel model = new JModel();
                model.doors = Doors;
                var defaultCass = unitSpaceFunction.UnitSpaceClasses.Where(x => x.DomainCode == comboBox_DefaultClass.Text).FirstOrDefault();
                if(defaultCass== null)
                {
                    defaultCass = unitSpaceFunction.UnitSpaceClasses[0];
                }

                var defaultFunction = unitSpaceFunction.FunctionCodes.Where(x => x.DomainCode == comboBox_DefaultFunction.Text).FirstOrDefault();
                if (defaultFunction == null)
                {
                    defaultFunction = unitSpaceFunction.FunctionCodes[0];
                }
                foreach (var door in model.doors)
                {
                    door.type = defaultCass.Name;
                    //door.usage = defaultFunction.Description;
                    door.function = defaultFunction.Name;
                }
                model.model_name = textBox_Building.Text;
                model.description = textBox_FloorName.Text;
                string level = textBox_FloorName.Text;
                model.rooms = dataSource.Select(x => ConvertToJRoom(x)).ToList();
                foreach(var room in model.rooms)
                {
                    room.level = level;
                    if (string.IsNullOrEmpty(room.type))
                    {
                        room.type = defaultCass.Name;
                    }
                    if(string.IsNullOrEmpty(room.function))
                    {
                        room.function = defaultFunction.Name;
                    }
                    //if(string.IsNullOrEmpty(room.usage))
                    //{
                    //    //room.usage = defaultFunction.Description;
                    //}
                }
                foreach(var door in model.doors)
                {
                    door.level = level;
                }
                model.scale = 1;

                JModel2Indoor jConverter = new JModel2Indoor();

                LogUtils.Info("Save as " + path);
                jConverter.Convert(model).Save(path);

                LogUtils.Info("View model " + path);
                IndoorView.ViewIndoorGML(path);

                stopWatch.Stop();
                LogUtils.Info($"Total export time: {stopWatch.Elapsed.TotalSeconds} seconds");
                LogUtils.Info("******************************");

                MessageBox.Show("Export finished!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogUtils.Error(ex.Message,ex);
            }
        }

        private List<Entity> GetAllEntityOnLayer(string[] layers)
        {
            List<Entity> ids = new List<Entity>();
            foreach (string layer in layers)
            {
                ids.AddRange(API.GetEntitiesOnLayer(layer));
            }
            return ids;
        }

        private JRoom ConvertToJRoom(DataInfo info)
        {
            JRoom room = new JRoom();
            room.name = info.Name;
            room.id = info.ID;
            room.type = info.Type;
            room.function = info.Function;
            var roomType = unitSpaceFunction.UnitSpaceClasses.Where(x => x.Name == room.type).FirstOrDefault();
            if (roomType != null)
            {
                room.code = roomType.DomainCode;
            }

            var function = unitSpaceFunction.FunctionCodes.Where(x => x.Name == room.function).FirstOrDefault();
            if(function != null)
            {
                //room.usage = function.Description;
            }
           // room.SpaceType = GetSpaceType(room.type);
            room.geometry = new JRoomGeometry();
            room.geometry.boundary = new List<JLineSegment>();

            int numVertex = info.Boundary.NumberOfVertices;
            for (int i=0;i< numVertex; i++)
            {
                JLineSegment segment = new JLineSegment();
                segment.points = new List<Point3DIntra.Point3D>();
                var start = info.Boundary.GetPoint3dAt(i).ToIntraPoint(API.Scale);
                var end = info.Boundary.GetPoint3dAt((i + 1) % numVertex).ToIntraPoint(API.Scale);
                start.z += info.Elevation;
                end.z += info.Elevation;
                segment.points.Add(start);
                segment.points.Add(end);
                room.geometry.boundary.Add(segment);
            }
         
            room.type = info.Type;
            room.geometry.CaculatorBox( info.FloorHeight);

            return room;
        }

       

        private void GetSeperateLine(DataInfo info, List<Entity> sperateRoom)
        {
            foreach (var entity in sperateRoom)
            {
                Point3dCollection collection = new Point3dCollection();
                //info.Boundary.IntersectWith(entity, Intersect.OnBothOperands, collection, IntPtr.Zero, IntPtr.Zero);
                var line = info.Boundary.GetLineIntersect(entity);
                if (line != null )
                {
                    collection.Add(line.EndPoint);
                    collection.Add(line.StartPoint);

                    var door = Doors.Where(x => x.id == entity.Handle.Value).FirstOrDefault();
                    if (door == null)
                    {
                        var id = entity.Handle.Value;
                        door = new JDoor();
                        door.name = "Room Seperator " + id.ToString();
                        door.id = id;
                        door.type = "Room Seperator";

                        door.roomIds = new List<long>();
                        door.roomIds.Add(info.ID);
                        door.geometry = GetVirtualDoorGeometry(entity, collection, info);
                        
                        Doors.Add(door);

                        LogUtils.Info($"Connect room sperator {door.id} to {info.ID}");
                    }
                    else
                    {
                        door.roomIds.Add(info.ID);
                        InitPos(door.geometry, collection, info,1);
                        LogUtils.Info($"Connect room sperator {door.id} to {info.ID}");
                    }
                }

            }
        }
        private void GetDoor(DataInfo info, List<Entity> doorEntity)
        {
            foreach (var entity in doorEntity)
            {
                Point3dCollection collection = new Point3dCollection();
                info.Boundary.IntersectWith(entity, Intersect.OnBothOperands, collection, IntPtr.Zero, IntPtr.Zero);

                if (collection != null && collection.Count >= 2)
                {
                    var door = Doors.Where(x => x.id == entity.Handle.Value).FirstOrDefault();
                    if (door == null)
                    {
                        var id = entity.Handle.Value;
                        door = new JDoor();
                        door.name = "Door " + id.ToString();
                        door.type = "";
                        door.id = id;
                        door.roomIds = new List<long>();
                        door.roomIds.Add(info.ID);
                        door.geometry = GetDoorGeometry(entity, collection, info);
                        Doors.Add(door);

                        LogUtils.Info($"Connect door {door.id} to {info.ID}");
                    }
                    else
                    {
                        door.roomIds.Add(info.ID);
                        InitPos(door.geometry, collection, info,1);
                        LogUtils.Info($"Connect door {door.id} to {info.ID}");
                    }
                }
            }
        }


        private JDoorGeometry GetDoorGeometry(Entity entity,Point3dCollection collection, DataInfo info)
        {
            //using 80% percent height of floor
           var geo =new  JDoorGeometry() { height = info.FloorHeight *0.8 };
            int last = collection.Count - 1;
            geo.width = (collection[last] - collection[0]).Length*API.Scale;
           
            if(geo.positions == null)
            {
                geo.positions = new List<Pos>();
            }

            InitPos(geo, collection, info, last);
            return geo;
        }

        private JDoorGeometry GetVirtualDoorGeometry(Entity entity, Point3dCollection collection, DataInfo info)
        {
            //using 80% percent height of floor
            var geo = new JDoorGeometry() { height = info.FloorHeight * 0.8 };

            int last = 1;
            
            geo.width = (collection[last] - collection[0]).Length * API.Scale;

            if (geo.positions == null)
            {
                geo.positions = new List<Pos>();
            }

            InitPos(geo, collection, info, last);
            return geo;
        }

        private bool IsStraightLine(Point3dCollection collection)
        {
            if (collection.Count == 2)
                return true;

            Vector3d? dir = null;
            for(int i=0;i<collection.Count-1;i++)
            {
                if(dir == null)
                {
                    dir = (collection[i + 1] - collection[i]).GetNormal();
                }
                else
                {
                    var nextDir = (collection[i + 1] - collection[i]).GetNormal();
                    if (dir.Value.DotProduct(nextDir) <0.95f)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void InitPos(JDoorGeometry geo, Point3dCollection collection,DataInfo info,int last)
        {
            var dir = (collection[last] - collection[0]);            
            Pos pos = new Pos();
            pos.width = dir.Length * API.Scale;
            dir = dir.GetNormal();

            pos.position = (collection[last].ToIntraPoint(API.Scale) + collection[0].ToIntraPoint(API.Scale)) / 2;
            pos.position.z += info.Elevation;
            pos.direction = Point3DIntra.Point3D.CrossProduct(Point3DIntra.Point3D.Zaxis,dir.ToIntraPoint());
            geo.positions.Add(pos);
        }

        private void textBox_NotChar_Validating(object sender, CancelEventArgs e)
        {
            if(sender is TextBox txt)
            {
                if (txt.Text.Any(c => char.IsLetter(c)))
                    e.Cancel = true;
            }
        }

        private void button_BrowseFile_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox_Output.Text = dialog.FileName;
            }
        }



        private void dataGridView_Model_SelectionChanged(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dataGridView_Model.Rows)
            {
                if (row.Selected)
                {
                    dataSource[row.Index].Select();
                }
                else
                {
                    dataSource[row.Index].UnSelect();
                }
            }

            API.ActiveDoc?.Editor.UpdateScreen();
        }

        private void button_AddCombineFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            openFile.Filter = "IndoorGML (*.gml)|*.gml";
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                foreach(var file in openFile.FileNames)
                {
                    if(!sourceCombines.Any(x => x.GmlFile == file))
                    {
                        IndoorGMLFile indoorFile = new IndoorGMLFile();
                        if (indoorFile.ParseFile(file))
                        {
                            sourceCombines.Add(indoorFile);
                        }
                    }
                   
                }
            }
        }

        private void button_Browse_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "IndoorGML (*.gml)|*.gml";
            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                textBox_CombineOutput.Text = saveFile.FileName;
            }
        }

        private void button_Combines_Click(object sender, EventArgs e)
        {
            if(sourceCombines.Count>0)
            {
                API.ApplyScaleUnit((UnitsValue)comboBox_Unit.SelectedItem);

                string output = textBox_CombineOutput.Text;

                var files = sourceCombines.Select(x => x.GmlFile).ToArray();
                float tolerance = 0;
                if(!float.TryParse(textBox_VerticalTransitionTolerance.Text,out tolerance))
                {
                    tolerance = 0.1f * API.ScaleTolerance;
                }
                IndoorGMLUtility.CombineMultipleFile(output,sourceCombines.First().Building,
                    "Master",
                    tolerance,
                    files);

                if(File.Exists(output))
                {
                    IndoorView.ViewIndoorGML(output);
                }
                else
                {
                    MessageBox.Show("Can't combine gml files!", "Error");
                }
            }
        }

        private void button_FixGap_Click(object sender, EventArgs e)
        {
            try
            {
                LogUtils.Setup(textBox_LogFile.Text, "Autocad to IndoorGML");
                float setting = 0;
                if (float.TryParse(textBox_GapTolerance.Text, out setting))
                {
                    string layer = textBox_Gap.Text;
                    if (!string.IsNullOrEmpty(layer))
                    {
                        API.FixGap(setting, layer);
                    }
                    else
                    {
                        MessageBox.Show("Please input the gap-layer", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("The gap tolerance is not a number", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch(Exception ex)
            {
                LogUtils.Error(ex.Message,ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                  
        }

        private void dataGridView_Model_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                dataSource[e.Row.Index].RemoveBoundary();
                API.ActiveDoc?.Editor.UpdateScreen();
            }
            catch(Exception ex)
            {

            }
        }

        private void IndoorGMLControl_Load(object sender, EventArgs e)
        {

            foreach(Control control in this.Controls)
            {
                GetDefaultValueOfControl(control);
            }

            if (textBox_LogFile.Text == "")
            {
                textBox_LogFile.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AutocadToIndoorGML.log";
            }



        }

        private void GetDefaultValueOfControl(Control control)
        {
            if (control is TextBox textBox)
            {
                var setting = RegSetting.GetSetting(textBox.Name);
                if (!string.IsNullOrEmpty(setting))
                {
                    textBox.Text = setting;
                }
            }
            else
            {
                foreach (Control child in control.Controls)
                {
                    GetDefaultValueOfControl(child);
                }
            }

           
        }

        private void updateSettingTextBox(object sender, EventArgs e)
        {
            if(sender is TextBox textBox)
            {
                RegSetting.SetSetting(textBox.Name, textBox.Text);
            }
        }

        private void button_OpenLog_Click(object sender, EventArgs e)
        {
            if(File.Exists(textBox_LogFile.Text))
            {
                Process.Start(textBox_LogFile.Text);
            }
        }

        private void button_BrowseLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Log files (*.log)|*.log";
            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                textBox_LogFile.Text = saveFile.FileName;
            }
        }

     

        private void button_RoomList_Click(object sender, EventArgs e)
        {
            if (!UpdateRoomLayerName())
                return;

            var objectIds =  API.GetObjectIdsOnLayer(Config.RoomLayer);
            float elevation = 0;
            float.TryParse(textBox_Elevation.Text, out elevation);

            float height = 0;
            float.TryParse(textBox_Height.Text, out height);
            var _type = GetDefaultClass();
            var _function = GetDefaultFunction();
            bool hasChanged = false;
            foreach (var oid in objectIds)
            {
                var data = API.GetBoundaryObject(oid, elevation, height,"Room "+(dataSource.Count+1), _type,_function);
                if(!unitSpaceFunction.UnitSpaceClasses.Exists(x=>x.Name == data.Type))
                {
                    hasChanged = true;
                    data.Type = _type;
                }

                if (!unitSpaceFunction.FunctionCodes.Exists(x => x.Name == data.Function))
                {
                    hasChanged = true;
                    data.Function = _function;
                }

                if (!dataSource.Any(x => x.ObjectId == data.ObjectId))
                {
                    dataSource.Add(data);
                    if (hasChanged)
                        data.UpdateProperty();
                }

               
            }
        }

        private void button_ClearRooms_Click(object sender, EventArgs e)
        {
            //Clear rooms 
            dataSource.Clear();
        }

        private void dataGridView_Model_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView_Model.CurrentCell == null)
            {
                return;
            }
            foreach (DataGridViewRow row in dataGridView_Model.Rows)
            {
                if (row.Index == dataGridView_Model.CurrentCell.RowIndex)
                {
                    dataSource[row.Index].Select();
                }
                else
                {
                    dataSource[row.Index].UnSelect();
                }
            }

            API.ActiveDoc?.Editor.UpdateScreen();
        }

        private void button_DeleteSpace_Click(object sender, EventArgs e)
        {
            if(dataGridView_Model.SelectedRows != null && dataGridView_Model.SelectedRows.Count>0)
            {
                for(int i=dataGridView_Model.SelectedRows.Count-1;i>=0;i--)
                {
                    dataSource.Remove((DataInfo)dataGridView_Model.SelectedRows[i].DataBoundItem);
                }
            }
            else
            {
                MessageBox.Show("Please the space to delete!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_Create_Click(object sender, EventArgs e)
        {
            var layers = new List<LayerInfo>();
            foreach (var layer in textBox_LayerVirtualDoor.Text.Split(';'))
            {
                if (string.IsNullOrEmpty(layer))
                    continue;
                layers.Add(new LayerInfo(layer, Enums.Colors.Blue, Autodesk.AutoCAD.GraphicsInterface.Linetype.Solid, LineWeight.LineWeight020));
            }

            foreach (var layer in textBox_RoomSeperate.Text.Split(';'))
            {
                if (string.IsNullOrEmpty(layer))
                    continue;
                layers.Add(new LayerInfo(layer, Enums.Colors.Blue, Autodesk.AutoCAD.GraphicsInterface.Linetype.Solid, LineWeight.LineWeight020));
            }

            foreach (var layer in textBox_RoomLayerName.Text.Split(';'))
            {
                if (string.IsNullOrEmpty(layer))
                    continue;
                layers.Add(new LayerInfo(layer, Enums.Colors.Blue, Autodesk.AutoCAD.GraphicsInterface.Linetype.Solid, LineWeight.LineWeight020));
            }

            API.CreateLayerIfNotExist(layers);

            API.ActiveDoc?.Editor.UpdateScreen();
        }

        private void dataGridView_Model_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView_Model.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                dataGridView_Model.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
