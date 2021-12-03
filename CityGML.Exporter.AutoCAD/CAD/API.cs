using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CityGML.Exporter.AutoCAD.Data;
using CityGML.Exporter.AutoCAD.Entities;
using CityGML.Exporter.AutoCAD.Enums;
using CityGML.Exporter.AutoCAD.Utils;
using IndoorGML.Exporter.AutoCAD.Entities;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Exception = System.Exception;

namespace CityGML.Exporter.AutoCAD
{
    public class API
    {
        private static int indexColor = 1;
        private static double gapTolerance = 3;
        private static double diffTolerance = 3;
        public static Tolerance Tolerance = new Tolerance(diffTolerance, diffTolerance);

        public static float Scale = 1;
        public static float ScaleTolerance = 1;

        public static Document ActiveDoc
        {
            get
            {
                return Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            }
        }

        public static string DocName
        {
            get
            {
                return ActiveDoc?.Database?.ProjectName + "-" + Path.GetFileNameWithoutExtension(ActiveDoc?.Name);
            }
        }

        internal static ObjectId CreateOutLine(Entity boundary, string layer = "OutLine")
        {
            if (boundary == null)
                return ObjectId.Null;

            var doc = ActiveDoc;
            using (doc.LockDocument())
            using (var tr = doc.TransactionManager.StartTransaction())
            {
                doc.Database.LineWeightDisplay = true;
                BlockTable bt = (BlockTable)tr.GetObject(doc.Database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);


                CreateLineLayer(layer, tr, Color.FromColorIndex(ColorMethod.ByAci, (short)Colors.Red));

                //boundary.LineWeight = LineWeight.LineWeight020;
                boundary.Layer = layer;
                boundary.LineWeight = LineWeight.LineWeight080;
                var newObjectID = btr.AppendEntity(boundary);
                tr.AddNewlyCreatedDBObject(boundary, true);

                tr.Commit();
                return newObjectID;
            }
        }




        internal static void ApplyScaleUnit(UnitsValue selectedUnit)
        {
            if (ActiveDoc.Database.Insunits == UnitsValue.Undefined)
            {
                Scale = (float)UnitsConverter.GetConversionFactor(UnitsValue.Millimeters, selectedUnit);
            }
            else
            {
                Scale = (float)UnitsConverter.GetConversionFactor(ActiveDoc.Database.Insunits, selectedUnit);
            }

            ScaleTolerance = (float)UnitsConverter.GetConversionFactor(UnitsValue.Meters, selectedUnit);
        }

        public static bool RoomPicker(out DBObjectCollection roomBoundary, System.Windows.Forms.Form control = null)
        {
            roomBoundary = null;

            var ed = ActiveDoc.Editor;
            PromptPointResult ppr = ed.GetPoint("\nSelect internal point: ");

            if (ppr.Status == PromptStatus.Cancel)
                return false;

            if (ppr.Status != PromptStatus.OK)
                return false;

            if (TraceBox(ppr.Value, out roomBoundary))
            {
                if (roomBoundary.Count > 0)
                {
                    return true;
                }
            }

            ed.WriteMessage("\nSpace is not closed");
            return false;
        }

        public static bool TraceBox(Point3d point, out DBObjectCollection roomBoundary)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            try
            {
                DBObjectCollection objs = ed.TraceBoundary(point, false);
                roomBoundary = objs;
                return objs.Count > 0;
            }
            catch (Exception ex)
            {
                ed.WriteMessage("\n" + ex.Message);
            }
            roomBoundary = null;
            return false;

        }

     

        public static void AddRegAppTableRecord(string regAppName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            using (doc.LockDocument())
            using (var tr = doc.TransactionManager.StartTransaction())
            {
                RegAppTable rat = (RegAppTable)tr.GetObject(doc.Database.RegAppTableId, OpenMode.ForRead, false);

                if (!rat.Has(regAppName))
                {
                    rat.UpgradeOpen();
                    RegAppTableRecord ratr = new RegAppTableRecord();
                    ratr.Name = regAppName;
                    rat.Add(ratr);
                    tr.AddNewlyCreatedDBObject(ratr, true);
                }
                tr.Commit();
            }
        }




        public static void FixGap(double toleren, string gapLayer, List<LayerInfo> wallLayers = null)
        {

            Transaction tr = ActiveDoc.TransactionManager.StartTransaction();
            List<Point3d> points = new List<Point3d>();

            var fillGaplayer = new List<LayerInfo> {
                new LayerInfo(gapLayer, Enums.Colors.Red, Autodesk.AutoCAD.GraphicsInterface.Linetype.Solid, LineWeight.LineWeight020)
            };

            CreateLayerIfNotExist(fillGaplayer);

            using (ActiveDoc.LockDocument())
            using (tr)
            {
                BlockTable acBlkTbl;
                acBlkTbl = tr.GetObject(ActiveDoc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;


                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;


                var layers = wallLayers;
                if (layers == null)
                    layers = GetAllLayer(true);

                foreach (var entityID in acBlkTblRec)
                {
                    var obj = tr.GetObject(entityID, OpenMode.ForRead);

                    if (obj == null)
                        continue;

                    var ent = obj as Entity;

                    if (ent == null)
                        continue;

                    if (layers.Any(l => l.Name == ent.Layer))
                    {
                        points.AddRange(API.ExtractStartEndPoint(tr, entityID, Matrix3d.Identity));
                    }
                }

                for (int i = 0; i < points.Count; i += 2)
                {
                    for (int j = i + 2; j < points.Count; j++)
                    {

                        double distance = points[i].DistanceTo(points[j]);
                        if (distance <= toleren && distance > 0)
                        {
                            //fixed it
                            //add line
                            using (Line line = new Line(points[i], points[j]))
                            {
                                line.ColorIndex = 1;
                                line.LineWeight = LineWeight.LineWeight005;
                                line.Layer = gapLayer;
                                acBlkTblRec.AppendEntity(line);
                                tr.AddNewlyCreatedDBObject(line, true);
                            }
                        }
                    }
                }
                tr.Commit();
            }

            ActiveDoc.Editor.UpdateScreen();
        }

        public static List<LayerInfo> GetAllLayer(bool isVisible, Transaction tr = null)
        {
            var lstlay = new List<LayerInfo>();

            LayerTableRecord layer;

            Document doc = Application.DocumentManager.MdiActiveDocument;

            if (tr != null)
            {
                Process(tr);
            }
            else
            {
                using (doc.LockDocument())
                {
                    using (var trans = doc.TransactionManager.StartOpenCloseTransaction())
                    {
                        Process(trans);
                    }
                }
            }
            return lstlay;

            void Process(Transaction trans)
            {
                LayerTable lt = trans.GetObject(doc.Database.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (var layerId in lt)
                {
                    layer = trans.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                    if (isVisible)
                    {
                        if (!layer.IsOff)
                        {
                            lstlay.Add(new LayerInfo(layer.Id, layer.Name, layer.IsOff));
                        }
                    }
                    else
                    {
                        lstlay.Add(new LayerInfo(layer.Id, layer.Name, layer.IsOff));
                    }
                }
            }
        }
        public static bool IsLayersExist(string[] layerNames)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            using (doc.LockDocument())
            using (Transaction acTrans = doc.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl = acTrans.GetObject(doc.Database.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (var layer in layerNames)
                {
                    if (!acLyrTbl.Has(layer))
                        return false;
                }
            }

            return true;
        }

        public static void CreateLineLayer(string layer, Transaction transaction, Color color)
        {
            var layerTable = transaction.GetObject(ActiveDoc.Database.LayerTableId,
                                               OpenMode.ForRead) as LayerTable;

            if (layerTable.Has(layer))
                return;

            using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
            {
                // Assign the layer the ACI color 3 and a name
                acLyrTblRec.Color = color;
                acLyrTblRec.Name = layer;
                acLyrTblRec.LineWeight = LineWeight.LineWeight020;

                var lineTypeTb = transaction.GetObject(ActiveDoc.Database.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                foreach (var id in lineTypeTb)
                {
                    var lineTypeTbRec = transaction.GetObject(id, OpenMode.ForRead) as LinetypeTableRecord;
                    if (lineTypeTbRec.Name == "_CONTINOUS")
                        acLyrTblRec.LinetypeObjectId = id;
                }


                // Upgrade the Layer table for write
                layerTable.UpgradeOpen();

                // Append the new layer to the Layer table and the transaction
                layerTable.Add(acLyrTblRec);
                transaction.AddNewlyCreatedDBObject(acLyrTblRec, true);
            }

        }
        public static void CreateLayerIfNotExist(List<LayerInfo> layers)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (acDoc.LockDocument())
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                                OpenMode.ForRead) as LayerTable;

                var dicLineType = new Dictionary<string, ObjectId>();
                //GETLINETYPE
                //BYBLOCK
                //BYLAYER
                //CONTINUOUS
                //CENTER
                //DASHDOT
                //HIDDEN
                //PHANTOM
                var lineTypeTb = acTrans.GetObject(acDoc.Database.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                foreach (var id in lineTypeTb)
                {
                    var lineTypeTbRec = acTrans.GetObject(id, OpenMode.ForRead) as LinetypeTableRecord;
                    //acDoc.Editor.WriteMessage(lineTypeTbRec.Name + "\n");
                    dicLineType.Add(lineTypeTbRec.Name, id);
                }

                foreach (var layer in layers)
                {
                    if (acLyrTbl.Has(layer.Name))
                        continue;

                    using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                    {
                        // Assign the layer the ACI color 3 and a name
                        acLyrTblRec.Color = layer.Color;
                        acLyrTblRec.Name = layer.Name;
                        acLyrTblRec.LineWeight = layer.LineWeight;

                        var lineTypeId = GetLineTypeId(layer.LineType);
                        if (lineTypeId != ObjectId.Null)
                            acLyrTblRec.LinetypeObjectId = lineTypeId;


                        // Upgrade the Layer table for write
                        acLyrTbl.UpgradeOpen();

                        // Append the new layer to the Layer table and the transaction
                        acLyrTbl.Add(acLyrTblRec);
                        acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    }
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();

                ObjectId GetLineTypeId(Autodesk.AutoCAD.GraphicsInterface.Linetype lineType)
                {
                    ObjectId _lineTypeId = ObjectId.Null;
                    switch (lineType)
                    {
                        case Autodesk.AutoCAD.GraphicsInterface.Linetype.Dashed:
                            dicLineType.TryGetValue("HIDDEN", out _lineTypeId);
                            break;
                        case Autodesk.AutoCAD.GraphicsInterface.Linetype.Dotted:
                            dicLineType.TryGetValue("PHANTOM", out _lineTypeId);
                            break;
                        case Autodesk.AutoCAD.GraphicsInterface.Linetype.DashDot:
                            dicLineType.TryGetValue("DASHDOT", out _lineTypeId);
                            break;

                        default:
                            dicLineType.TryGetValue("CONTINUOUS", out _lineTypeId); //solid
                            break;
                    }

                    return _lineTypeId;
                }
            }
        }

      

        public static IEnumerable<CadText> GetMTextObjs()
        {
            if (ActiveDoc == null)
                yield break;
            var ed = ActiveDoc.Editor;
            if (ed == null)
                yield break;

            using (ActiveDoc.LockDocument())
            {
                var selMText = new TypedValue[1] { new TypedValue((int)DxfCode.Start, "TEXT,MTEXT") };

                var MTextObjs = ed.SelectAll(new SelectionFilter(selMText));
                var layers = API.GetAllLayer(true);

                using (var Transaction = ActiveDoc.Database.TransactionManager.StartOpenCloseTransaction())
                {
                    if (MTextObjs.Value != null)
                    {
                        foreach (ObjectId MTextObjId in MTextObjs.Value.GetObjectIds())
                        {
                            var current_MTextObj = Transaction.GetObject(MTextObjId, OpenMode.ForWrite) ;
                            if (current_MTextObj != null)
                            {
                                if (current_MTextObj is MText mtext && mtext.Visible)
                                {
                                    if (layers.Any(x => x.Name == mtext.Layer))
                                    {

                                        yield return new CadText(mtext.Text, mtext.Location);
                                    }
                                }
                                else if (current_MTextObj is DBText text && text.Visible)
                                {
                                    
                                    if (layers.Any(x => x.Name == text.Layer))
                                    {
                                        yield return new CadText(text.TextString, text.Position);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Transaction.Commit(); // if you change something.
        }

        public static void GetXData()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            // Ask the user to select an entity
            // for which to retrieve XData
            PromptEntityOptions opt = new PromptEntityOptions("\nSelect entity: " );
            PromptEntityResult res =
             ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                Transaction tr =
                 doc.TransactionManager.StartTransaction();
                using (tr)
                {
                    DBObject obj =
                     tr.GetObject(
                      res.ObjectId,
                      OpenMode.ForRead
                     );
                    ResultBuffer rb = obj.XData;
                    if (rb == null)
                    {
                        ed.WriteMessage(
                         "\nEntity does not have XData attached."
                        );
                    }
                    else
                    {
                        int n = 0;
                        foreach (TypedValue tv in rb)
                        {
                            ed.WriteMessage(
                             "\nTypedValue {0} - type: {1}, value: {2}",
                             n++,
                             tv.TypeCode,
                             tv.Value
                            );
                        }
                        rb.Dispose();
                    }
                }
            }
        }

        internal static void UpdateProperty(DataInfo dataInfo)
        {
            //LogUtils.
            Transaction tr = ActiveDoc.TransactionManager.StartTransaction();
            using (ActiveDoc.LockDocument())
            {
                using (tr)
                {
                    DBObject obj = tr.GetObject(dataInfo.ObjectId, OpenMode.ForWrite);
                    AddRegAppTableRecord(Config.AppName);
                    var data = new ResultBuffer(
                        new TypedValue((int)DxfCode.ExtendedDataRegAppName, Config.AppName),
                        new TypedValue((int)DxfCode.ExtendedDataAsciiString, dataInfo.Name),
                        new TypedValue((int)DxfCode.ExtendedDataReal, dataInfo.FloorHeight),
                           new TypedValue( (int)DxfCode.ExtendedDataReal, dataInfo.Elevation),
                            new TypedValue((int)DxfCode.ExtendedDataAsciiString, dataInfo.Type),
                        new TypedValue((int)DxfCode.ExtendedDataReal, dataInfo.ID),
                        new TypedValue((int)DxfCode.ExtendedDataAsciiString, dataInfo.Function));

                    obj.XData = data;
                    tr.Commit();
                }
            }
        }

       

        internal static DataInfo GetBoundaryObject(ObjectId id, float elevation, float height, string name, string type,string function)
        {
            DataInfo data = new DataInfo();
            data.ObjectId = id;
            data.Elevation = elevation;
            data.FloorHeight = height;
            data.Name = name;
            data.Type = type;
            data.Function = function;

            Transaction tr = ActiveDoc.TransactionManager.StartTransaction();
            using (tr)
            {

                var boundary = tr.GetObject(id, OpenMode.ForRead);
                if (boundary == null)
                    return null;

                data.ID = boundary.Handle.Value;
                data.Boundary = boundary as Polyline;
                if(boundary.XData != null)
                {
                    var property = boundary.XData;
                    if (property != null)
                    {
                        var info = property.AsArray();
                        for (int i = 0; i < info.Length; i++)
                        {
                            if (info[i].Value.ToString() == Config.AppName)
                            {
                                try
                                {
                                    data.Name = info[1 + i].Value.ToString();
                                    data.FloorHeight = (double)info[2 + i].Value;
                                    data.Elevation = (double)info[3 + i].Value;
                                    data.Type = info[4 + i].Value.ToString();

                                    if (info.Length > 6 + i)
                                    {
                                        data.Function = info[6 + i].Value.ToString();
                                    }
                                }catch(Exception ex)
                                {
                                    LogUtils.Warning(ex.Message, ex);
                                }
                               
                                //data.ID = (long)(info[5 + i].Value);
                                break;
                            }
                        }
                    }
                }
            }

            return data;
        }


        internal static void IsIntersect()
        {
            Document doc =
          Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            var firstObj = ed.GetEntity("\nSelect first entity");
            if (firstObj.Status == PromptStatus.Cancel)
                return;

            var secondObj = ed.GetEntity("\nSelect second entity:");
            if (secondObj.Status == PromptStatus.Cancel)
                return;

            var tr = doc.TransactionManager.StartTransaction();
            Point3dCollection collection = new Point3dCollection();
            using (tr)
            {
                var obj1 = tr.GetObject(firstObj.ObjectId, OpenMode.ForRead) as Entity;
                var obj2 = tr.GetObject(secondObj.ObjectId, OpenMode.ForRead) as Entity;
                obj1.IntersectWith(obj2, Intersect.ExtendBoth, collection, IntPtr.Zero,IntPtr.Zero);
                var points = API.ExtractStartEndPoint(tr, obj1.ObjectId, Matrix3d.Identity).ToList();
                
                if(collection.Count>0)
                {
                    API.IsContain(points, collection[0]);
                    ed.WriteMessage("Intersected :" + collection[0].ToString());
                }
                else
                {
                    ed.WriteMessage("none");
                };
            }
        }

        public static void GetIntersect(ObjectId[] ids, Point3dCollection collection, Polyline boundary)
        {
            var doc = ActiveDoc;
            using (var trans = doc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                foreach (var id in ids)
                {
                    var entity = trans.GetObject(id, OpenMode.ForRead) as Entity;
                    if(entity != null)
                    {
                        boundary.IntersectWith(entity, Intersect.OnBothOperands, collection, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
                
        }

        public  static void SetXData(string appName,string data)
        {
            Document doc =
             Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            // Ask the user to select an entity
            // for which to set XData
            PromptEntityOptions opt =
             new PromptEntityOptions(
              "\nSelect entity: "
             );
            PromptEntityResult res =
             ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                Transaction tr =
                 doc.TransactionManager.StartTransaction();
                using (tr)
                {
                    DBObject obj =
                     tr.GetObject(
                      res.ObjectId,
                      OpenMode.ForWrite
                     );
                    AddRegAppTableRecord(appName);
                    ResultBuffer rb =
                     new ResultBuffer(
                      new TypedValue(1001, appName),
                      new TypedValue(1000, data)
                     );
                    obj.XData = rb;
                    rb.Dispose();
                    tr.Commit();
                }
            }
        }

        internal static bool IsPolylineOnBoundary(Point3dCollection collection, Polyline boundary)
        {
            int count = 0;
           foreach (Point3d p in collection)
            {
                if (IsPointOnPolyline(p, boundary))
                    count++;
            }

            return (count >= 2);
        }

     

        internal static bool IsLineOnBoundary(Line polyline, Polyline boundary)
        {
            return IsPointOnPolyline(polyline.StartPoint, boundary) && IsPointOnPolyline(polyline.EndPoint, boundary);

        }

        static bool IsPointOnPolyline( Point3d pt, Polyline pl)
        {
            bool isOn = false;
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {

                Curve3d seg = null;
                SegmentType segType = pl.GetSegmentType(i);
                if (segType == SegmentType.Arc)
                    seg = pl.GetArcSegmentAt(i);
                else if (segType == SegmentType.Line)
                    seg = pl.GetLineSegmentAt(i);



                if (seg != null)
                {
                    
                    isOn = seg.IsOn(pt);
                    if (isOn)
                        break;
                }
            }

            return isOn;

        }
    internal static Entity GetObject(ObjectId id)
        {
            using (var trans = ActiveDoc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                return trans.GetObject(id, OpenMode.ForRead) as Entity;

            }
        }

        
        public static IEnumerable<Entity> GetEntitiesOnLayer(string layerName)
        {
            Document doc = ActiveDoc;
            Editor ed = doc.Editor;

            TypedValue[] tvs = new TypedValue[1] { new TypedValue((int)DxfCode.LayerName, layerName) };

            SelectionFilter sf = new SelectionFilter(tvs);
            PromptSelectionResult psr = ed.SelectAll(sf);

            var trans = doc.Database.TransactionManager.StartOpenCloseTransaction();
            if (psr.Status == PromptStatus.OK)
            {
                foreach (var objId in psr.Value.GetObjectIds())
                {
                    var entity = trans.GetObject(objId, OpenMode.ForRead) as Entity;
                    if (entity != null)
                        yield return entity;
                }
            }
            trans.Dispose();
        }

        public static void RemoveEntity(ObjectId id)
        {
            try
            {
                Document doc = ActiveDoc;
                Editor ed = doc.Editor;
                using (doc.LockDocument())
                {
                    using (var trans = doc.Database.TransactionManager.StartTransaction())
                    {
                        var entityWriteable = trans.GetObject(id, OpenMode.ForWrite) as Entity;
                        if (entityWriteable != null)
                            entityWriteable.Erase();

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IndoorGML",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        public static void RemoveEntitiesOnLayer(string layerName)
        {
            Document doc = ActiveDoc;
            Editor ed = doc.Editor;

            TypedValue[] tvs = new TypedValue[1] { new TypedValue((int)DxfCode.LayerName, layerName) };

            SelectionFilter sf = new SelectionFilter(tvs);
            PromptSelectionResult psr = ed.SelectAll(sf);


            if (psr.Status == PromptStatus.OK)
            {
                using (doc.LockDocument())
                {
                    using (var trans = doc.Database.TransactionManager.StartTransaction())
                    {
                        foreach (var objId in psr.Value.GetObjectIds())
                        {
                            try
                            {
                                var entity = trans.GetObject(objId, OpenMode.ForWrite) as Entity;
                                if (entity != null)
                                    entity.Erase();
                            }
                            catch (Exception ex)
                            {
                                ed.WriteMessage(ex.Message + "\n");
                            }
                        }
                        trans.Commit();
                    }

                }

            }
        }

        public static IEnumerable<ObjectId> GetObjectIdsOnLayer(string layerName)
        {
            Document doc =Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            TypedValue[] tvs = new TypedValue[1] {new TypedValue((int)DxfCode.LayerName,layerName)};

            SelectionFilter sf =new SelectionFilter(tvs);
            PromptSelectionResult psr = ed.SelectAll(sf);

            if (psr.Status == PromptStatus.OK)
                foreach (var objId in psr.Value.GetObjectIds())
                    yield return objId;
        }

        public static IEnumerable<string> IterateLayers(Transaction acTrans)
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            LayerTable acLyrTbl;
            acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                         OpenMode.ForRead) as LayerTable;

            // Step through the Layer table and print each layer name
            foreach (ObjectId acObjId in acLyrTbl)
            {
                LayerTableRecord acLyrTblRec;
                acLyrTblRec = acTrans.GetObject(acObjId,
                                                OpenMode.ForRead) as LayerTableRecord;

                yield return acLyrTblRec.Name;
            }
        }

        

        public static void ExecuteCommand(string globalName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            doc.SendStringToExecute(globalName, true, false, false);
        }

        public static IEnumerable<Point3d> ExtractStartEndPoint(Transaction tr, ObjectId id,Matrix3d matrix )
        {
            var obj = tr.GetObject(id, OpenMode.ForRead);
            if (obj is Line)
            {
                Line line = obj as Line;
                yield return line.StartPoint.TransformBy(matrix);
                yield return line.EndPoint.TransformBy(matrix); ;
            }
            else if (obj is Polyline)
            {
                Polyline polyline = obj as Polyline;
                //yield return polyline.StartPoint.TransformBy(matrix); ;
                //yield return polyline.EndPoint.TransformBy(matrix); ;
                for (int i = 0; i < polyline.NumberOfVertices; i++)
                {
                    yield return polyline.GetPoint3dAt(i);
                }
            }
            else if (obj is Curve)
            {
                Curve curve = obj as Curve;
                yield return curve.StartPoint.TransformBy(matrix); ;
                yield return curve.EndPoint.TransformBy(matrix); ;
            }
            else if(obj is BlockReference)
            {
                BlockReference blockRef = obj as BlockReference;
                
                var btr = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId entId in btr)
                {
                    foreach(var p in ExtractStartEndPoint(tr, entId,blockRef.BlockTransform))
                    {
                        yield return p;
                    }
                }
            }
        }

        private static Entity GetNearestIntersect(Point3d center, Transaction tr, Dictionary<Entity, List<Point3d>> objectList, Entity target,List<Point3d> segment)
        {
            Point3dCollection intersectPoints = new Point3dCollection();
            double length = int.MaxValue;
            Entity nearest = null;
            foreach(var obj in objectList)
            {
                target.IntersectWith(obj.Key, Intersect.ExtendBoth, intersectPoints, IntPtr.Zero, IntPtr.Zero);
                if(intersectPoints.Count>0)
                {
                    if(!IsContain(segment, intersectPoints[0]))
                    {
                        continue;
                    }
                    double distance = intersectPoints[0].DistanceTo(center);
                   
                    intersectPoints.Clear();
                    if (length > distance)
                    {
                        length = distance;
                        nearest = obj.Key;
                    }
                }
                else
                {
                    foreach(var p in segment.Where(x => obj.Value.Exists(y => y.DistanceTo(x) < gapTolerance)))
                    {
                        double distance = p.DistanceTo(center);
                        if (length > distance)
                        {
                            length = distance;
                            nearest = obj.Key;
                        }
                    }
                }
            }

            return nearest;
        }

        private static bool IsContain(List<Point3d> segment, Point3d point3d)
        {
            for(int i=0;i<segment.Count-1;i+=2)
            {
                if (Math.Abs(segment[i].DistanceTo(segment[i + 1]) - 
                    (segment[i].DistanceTo(point3d) + segment[i + 1].DistanceTo(point3d))) < gapTolerance)
                {
                    return true;
                }
            }
            return false;
        }

        private static void Filter(Point3d center,Transaction tr,Dictionary<Entity,List<Point3d>> objectList,Entity nearestWall,List<Entity> result)
        {
            var objSegment= objectList[nearestWall];
            objectList.Remove(nearestWall);
            result.Add(nearestWall);
            var next = GetNearestIntersect(center, tr, objectList, nearestWall, objSegment);
            if(next !=null)
            {
                FilterNext(center, tr, objectList, next, result);
            }

            next = GetNearestIntersect(center, tr, objectList, nearestWall, objSegment);
            if (next != null)
            {
                FilterNext(center, tr, objectList, next, result);
            }
        }

        private static void FilterNext(Point3d center, Transaction tr, Dictionary<Entity, List<Point3d>> objectList, Entity nearestWall, List<Entity> result)
        {
            var objSegment = objectList[nearestWall];
            objectList.Remove(nearestWall);
            result.Add(nearestWall);
            var next = GetNearestIntersect(center, tr, objectList, nearestWall, objSegment);
            if (next != null)
            {
                FilterNext(center, tr, objectList, next, result);
            }
        }

        private static bool IsConnected(List<Point3d> segment1, List<Point3d> segment2)
        {
            if (segment1.Exists(x => segment2.Exists(y => y.DistanceTo(x) < gapTolerance)))
            {
                return true;
            }
            return false;
        }

        private static void ChangeColor(Transaction tr, ObjectId entId)
        {
            Entity ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
           

            if (ent is BlockReference)
            {
                BlockReference blcRef = ent as BlockReference;
                
                var btr = tr.GetObject(blcRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId _id in btr)
                {
                    Entity _ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
                    _ent.ColorIndex = indexColor;
                    _ent.LineWeight = LineWeight.LineWeight050;
                }
            }

            ent.ColorIndex = indexColor;
            ent.LineWeight = LineWeight.LineWeight050;
        }

        private static Entity GetIntersectObj(Transaction tr,List<ObjectId> ids,Line line,ref double length)
        {
            var dir = line.EndPoint - line.StartPoint;

            Point3dCollection pointIntersect = new Point3dCollection();          
            Entity entityNearest = null;
           

            for (int i=0;i<ids.Count;i++)
            {
                var entity = tr.GetObject(ids[i], OpenMode.ForRead) as Entity;
#pragma warning disable CS0618 // Type or member is obsolete
                line.IntersectWith(entity, Intersect.ExtendThis, pointIntersect, 0, 0);
#pragma warning restore CS061
                if(pointIntersect.Count >0)
                {
                    for(int j=0;j<pointIntersect.Count;j++)
                    {
                        if (dir.DotProduct(pointIntersect[j] - line.StartPoint) < 0)
                        {
                            continue;
                        }
                        var dis = pointIntersect[j].DistanceTo(line.StartPoint);
                        if (length > dis)
                        {
                            length = dis;
                            entityNearest = entity;
                        }
                    }
                    
                    pointIntersect.Clear();
                }
              
            }
            
            return entityNearest;
        }

        //Nhan's code
        #region Nhan
        
        public static List<DoorInfo> GetAllDoorEntitiesFromDoorLayer(string layerName)
        {
            var doors = new List<DoorInfo>();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (var trans = doc.TransactionManager.StartTransaction())
            {
                foreach (var id in GetObjectIdsOnLayer(layerName))
                {
                    var en = trans.GetObject(id, OpenMode.ForRead);
                    if (en == null)
                        continue;

                    var doorBlock = en as BlockReference;
                    if (doorBlock == null)
                        continue;

                    var uniqueId = MathUtils.GetRandomID();
                    var door = new DoorInfo()
                    {
                        ID = uniqueId,
                        Name = doorBlock.Name // $"Door {uniqueId}"
                    };

                    //var blockItems = new DBObjectCollection();
                    //doorBlock.Explode(blockItems);
                    //foreach (var item in blockItems)
                    //{
                    //    if(item is Arc)
                    //    {
                    //        door.Curves.Add(item as Curve);
                    //    }
                    //    else if(item is Polyline)
                    //    {
                    //        var lines = GetLines(item as Polyline);
                    //        foreach (var line in lines)
                    //        {
                    //            door.Curves.Add(line);
                    //        }
                    //    }
                    //    else if(item is Line)
                    //    {
                    //        door.Curves.Add(item as Line);
                    //    }
                    //}
                    
                    doors.Add(door);
                }
            }

            return doors;
        }
        public static void CreateDoorLine(Point3D pos, BoundarySegment seg, Transaction trans)
        {
            var v1 = (seg.StartPoint - pos);
            v1.Normalize();

            var v2 = (seg.EndPoint - pos);
            v2.Normalize();

            var p1 = pos + v1.MultiplyScalar(20);
            var p2 = pos + v2.MultiplyScalar(20);

            Document doc = Application.DocumentManager.MdiActiveDocument;

            Database db = doc.Database;
            BlockTable acBlkTbl = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

            // Open the Block table record Model space for write
            BlockTableRecord acBlkTblRec = trans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;


            var line = new Line(new Point3d(p1.x, p1.y, p1.z), new Point3d(p2.x, p2.y, p2.z));
            line.ColorIndex = 1;
            line.LineWeight = LineWeight.LineWeight050;
            acBlkTblRec.AppendEntity(line);
            trans.AddNewlyCreatedDBObject(line, true);
            trans.Commit();
        }
        public static void DrawLine()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (var trans = doc.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;


                var line = new Line(new Point3d(0, 0, 0), new Point3d(1000, 0, 0));
                line.ColorIndex = 2;
                line.LineWeight = LineWeight.LineWeight050;

                btr.AppendEntity(line);
                //trans.AddNewlyCreatedDBObject(line, true);
                //trans.Commit();
            }
        }
        public static bool IsBlockOnSegment(BoundarySegment segment, BlockReference block, out Point3D point)
        {
            var blockItems = new DBObjectCollection();
            block.Explode(blockItems);

            var valid = false;

            var listPointOnBoundary = new List<Point3D>();
            foreach (var blockItem in blockItems)
            {
                if (!(blockItem is Curve))
                    continue;

                var line = blockItem as Curve;

                var points = new List<Point3d>() { line.StartPoint, line.EndPoint };
                foreach (var _point in points)
                {
                    var dis = segment.Segment.GetClosestPointTo(_point).Point.DistanceTo(_point);
                    if (dis < 0.1)
                    {
                        listPointOnBoundary.Add(new Point3D(_point.X, _point.Y, _point.Z));
                    }
                }

                //var lineStartPoint = new Point3D(line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z);
                //var lineEndPoint = new Point3D(line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z);

                var disToStartPoint = segment.Segment.GetClosestPointTo(line.StartPoint).Point.DistanceTo(line.StartPoint);
                var disToEndPoint = segment.Segment.GetClosestPointTo(line.EndPoint).Point.DistanceTo(line.EndPoint);
                //if (disToStartPoint < 0.1)
                //{
                //    listPointOnBoundary.Add(lineStartPoint);
                //}
                //if (disToEndPoint < 0.1)
                //{
                //    listPointOnBoundary.Add(lineEndPoint);
                //}
                if (disToStartPoint < 0.1 && disToEndPoint < 0.1)
                {

                    //var blockBoundingBox = MathUtils.GetBlockBoundingBox(block);
                    //var center = MathUtils.GetCenter(lineStartPoint, lineEndPoint);

                    //var segmentStartPoint = new Point3D(segment.Segment.StartPoint.X, segment.Segment.StartPoint.Y, segment.Segment.StartPoint.Z);
                    //var segmentEndPoint = new Point3D(segment.Segment.EndPoint.X, segment.Segment.EndPoint.Y, segment.Segment.EndPoint.Z);

                    //point = center.ProjectOnLine(segmentStartPoint, segmentEndPoint);
                    //point = center;
                    //return true;

                    valid = true;
                }
            }

            if (!valid)
            {
                point = null;
                return false;
            }

            if (listPointOnBoundary.Count < 2)
            {
                point = null;
                return false;
            }

            var minPoint = new Point3D();
            var maxPoint = new Point3D();

            var minDis = double.MaxValue;
            var maxDis = 0.0;

            foreach (var _point in listPointOnBoundary)
            {
                var dis = segment.StartPoint.distance(_point);
                if (dis < minDis)
                {
                    minDis = dis;
                    minPoint = _point;
                }
                if (dis > maxDis)
                {
                    maxDis = dis;
                    maxPoint = _point;
                }
            }


            point = MathUtils.GetCenter(minPoint, maxPoint);
            return true;
        }
        public static List<Line> GetLines(Polyline popyline)
        {
            var lines = new List<Line>();
            for (int i = 0; i < popyline.NumberOfVertices; i++)
            {
                var line = popyline.GetLineSegmentAt(i);
                lines.Add(new Line(line.StartPoint, line.EndPoint));
            }
            return lines;
        }


        public static Dictionary<long, Point3dCollection> CachingPoints = new Dictionary<long, Point3dCollection>();

        public static void GetPoints(Entity entity,ref Point3dCollection collection,bool isEntityOwner=true)
        {
            if (entity == null)
                return;

           
            if(isEntityOwner)
            {
              
                if (CachingPoints.ContainsKey(entity.Handle.Value))
                {
                    collection = CachingPoints[entity.Handle.Value];
                    return ;
                }

                if (!IsHasArc(entity))
                {
                    CachingPoints[entity.Handle.Value] = null;
                    return;
                }
            }
            if(entity is Arc)
            {
                Arc arc = entity as Arc;
                collection.Add(arc.StartPoint);
                collection.Add(arc.EndPoint);
            }
            else if(entity is Polyline)
            {
                Polyline line = entity as Polyline;
          
                for(int i=0;i<line.NumberOfVertices;i++)
                {
                    collection.Add(line.GetPoint3dAt(i));
                }
            }
            else if(entity is Curve)
            {
                ((Curve)entity).GetStretchPoints(collection);
            }
            else if(entity is BlockReference)
            {
                BlockReference block = entity as BlockReference;
                DBObjectCollection dbCollection = new DBObjectCollection();

                block.Explode(dbCollection);
                foreach(Entity child in dbCollection)
                {
                    GetPoints(entity,ref collection,false);
                }
            }

            if (isEntityOwner)
            {
                CachingPoints[entity.Handle.Value] = collection;
            }
        }

        public static void GetPointLines(Entity entity, ref Point3dCollection collection, bool isEntityOwner = true)
        {
            if (entity == null)
                return;


            if (isEntityOwner)
            {

                if (CachingPoints.ContainsKey(entity.Handle.Value))
                {
                    collection = CachingPoints[entity.Handle.Value];
                    return;
                }
            }
            if (entity is Arc)
            {
                Arc arc = entity as Arc;
                collection.Add(arc.StartPoint);
                collection.Add(arc.EndPoint);
            }
            else if (entity is Polyline)
            {
                Polyline line = entity as Polyline;

                for (int i = 0; i < line.NumberOfVertices; i++)
                {
                    collection.Add(line.GetPoint3dAt(i));
                }
            }
            else if (entity is Curve)
            {
                ((Curve)entity).GetStretchPoints(collection);
            }
            else if (entity is BlockReference)
            {
                BlockReference block = entity as BlockReference;
                DBObjectCollection dbCollection = new DBObjectCollection();

                block.Explode(dbCollection);
                foreach (Entity child in dbCollection)
                {
                    GetPoints(entity, ref collection, false);
                }
            }

            if (isEntityOwner)
            {
                CachingPoints[entity.Handle.Value] = collection;
            }
        }

        private static bool IsHasArc(Entity entity)
        {
            try
            {
                if (entity is Arc)
                    return true;
                DBObjectCollection collection = new DBObjectCollection();
                entity.Explode(collection);
                bool hasArc = false;
                for(int i=0;i<collection.Count;i++)
                {
                    if(collection[i] is Arc)
                    {
                        hasArc = true;
                        break;
                    }
                }

                collection.Dispose();
                return hasArc;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        internal static void ChangeColor(ObjectId id, int colorIdx)
        {
            try
            {
                Document doc = ActiveDoc;
                Editor ed = doc.Editor;
                using (doc.LockDocument())
                {
                    using (var trans = doc.Database.TransactionManager.StartTransaction())
                    {
                        var entityWriteable = trans.GetObject(id, OpenMode.ForWrite) as Entity;
                        if (entityWriteable != null)
                            entityWriteable.ColorIndex = colorIdx;

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IndoorGML", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        #endregion
    }
}