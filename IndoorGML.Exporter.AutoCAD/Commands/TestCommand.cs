//using Autodesk.AutoCAD.ApplicationServices;
//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.EditorInput;
//using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.Runtime;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//[assembly: CommandClass(typeof(CityGML.Exporter.AutoCAD.Command.TestCommand))]
//namespace CityGML.Exporter.AutoCAD.Command
//{
//    public class TestCommand
//    {
//        [CommandMethod("PrintLayers")]
//        public void PrintLayers()
//        {
//            Document doc = Application.DocumentManager.MdiActiveDocument;
//            Database db = doc.Database;
//            if (doc == null)
//                return;

//            var layerTable = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
//            foreach (ObjectId acObjId in layerTable)
//            {
//                LayerTableRecord acLyrTblRec;
//                acLyrTblRec = acObjId.GetObject(OpenMode.ForRead) as LayerTableRecord;


//                doc.Editor.WriteMessage("\n" + acLyrTblRec.Name);
//            }
//        }

//        [CommandMethod("EntitiesOnLayer")]
//        public void GetentitiesOnLayer()
//        {
//            Document doc = Application.DocumentManager.MdiActiveDocument;
//            if (doc == null)
//                return;

//            Database db = doc.Database;
//            var doorLayerName = "E-F-DOOR";
//            //var ppr = doc.Editor.GetString("\nEnter layer name: ");
//            //if (ppr.Status != PromptStatus.OK)
//            //{
//            //    return;
//            //}

//            //doc.Editor.GetEntity("Selecte entity");

//            //var entities = new List<DBObject>();
//            using (DocumentLock docLock = doc.LockDocument())
//            using (Transaction tr = doc.TransactionManager.StartTransaction())
//            {
//                var objIds = API.GetObjectIdsOnLayer(doorLayerName);
                

//                foreach (var item in objIds)
//                {
//                    var en = tr.GetObject(item, OpenMode.ForRead);
//                    //entities.Add(en);
//                    if (en is BlockReference)
//                    {
//                        var _item = en as BlockReference;
//                        DBObjectCollection childEntities = new DBObjectCollection();
//                        _item.Explode(childEntities);
//                    }
//                }
//            }

            

//            //foreach (var item in entities)
//            //{
                
//            //}

//            return;
            
//        }
//        [CommandMethod("RoomPick2")]
//        public void RoomPick()
//        {
//            Document doc = Application.DocumentManager.MdiActiveDocument;

//            DBObjectCollection roomBoundary;
//            API.RoomPicker(out roomBoundary);
//            if (roomBoundary == null)
//            {
//                doc.Editor.WriteMessage("Boundary not found");
//                return;
//            }

//            Polyline polyline = null;

//            foreach (var item in roomBoundary)
//            {
//                polyline = item as Polyline;
//                break;
//            }

//            if(polyline == null)
//            {
//                doc.Editor.WriteMessage("Boundary is not a polyline");
//                return;
//            }

//            var selectDoorResult = doc.Editor.GetEntity("Select door: ");

//            using (var trans = doc.TransactionManager.StartTransaction())
//            {
//                foreach (var obj in API.GetObjectIdsOnLayer("E-F-DOOR"))
//                {
//                    var doorBlock = trans.GetObject(obj, OpenMode.ForRead) as BlockReference;


//                    if(doorBlock == null)
//                    {
//                        continue;
//                    }

//                    var doorBlockChildren = new DBObjectCollection();
//                    doorBlock.Explode(doorBlockChildren);
//                    foreach (var item in doorBlockChildren)
//                    {
//                        if (!(item is Line))
//                            continue;

//                        var line = item as Line;
//                        var disToStartPoint = polyline.GetClosestPointTo(line.StartPoint, true).DistanceTo(line.StartPoint);
//                        var disToEndPoint = polyline.GetClosestPointTo(line.EndPoint, true).DistanceTo(line.EndPoint);
//                        if (disToStartPoint < 0.1 && disToEndPoint < 0.1)
//                        {
//                            doc.Editor.WriteMessage("Line is on boundary");
//                        }
//                    }
//                }                
//            }

//        }
//        [CommandMethod("GetLineType")]
//        public void GetLineType()
//        {
//            var doc = Application.DocumentManager.MdiActiveDocument;
//            using (var trans = doc.TransactionManager.StartTransaction())
//            {
//                var lineTypeTb = trans.GetObject(doc.Database.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

//                foreach (var id in lineTypeTb)
//                {
//                    var lineTypeTbRec = trans.GetObject(id, OpenMode.ForRead) as LinetypeTableRecord;
//                    doc.Editor.WriteMessage(lineTypeTbRec.Name + "\n");
//                }
//            }
//        }
//    }
//}
