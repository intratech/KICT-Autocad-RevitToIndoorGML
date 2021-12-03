using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Utils
{
    public static class ViewUtils
    {
        public static Dictionary<Level, View3D> CreateSectionBoxForEachFloor(Document doc)
        {
            var levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().OrderBy(l => l.Elevation).ToList();
            ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
                                             OfClass(typeof(ViewFamilyType)).
                                             Cast<ViewFamilyType>()
                                             where v.ViewFamily == ViewFamily.ThreeDimensional
                                             select v).First();

            var box = CalculateModelBoundingBox(doc);
            var lsViewOfFloor = new Dictionary<Level, View3D>();
            var floorCount = 0;

            using (Transaction t = new Transaction(doc, "Create view"))
            {
                foreach (var level in levels)
                {
                    double zOffset = 0;
                    if (floorCount + 1 < levels.Count)
                        zOffset = levels[floorCount + 1].Elevation;
                    else
                        zOffset = level.Elevation + 10;

                    var levelBoxMin = new XYZ(box.Min.X, box.Min.Y, level.Elevation);
                    var levelBoxMax = new XYZ(box.Max.X, box.Max.Y, zOffset);
                    var levelBox = new BoundingBoxXYZ { Min = levelBoxMin, Max = levelBoxMax };

                    t.Start();

                    View3D view = View3D.CreateIsometric(doc, viewFamilyType.Id);
                    view.Name = $"SectionBox floor {floorCount}";
                    t.SetName($"Create view {view.Name}");
                    view.SetSectionBox(levelBox);

                    t.Commit();

                    lsViewOfFloor.Add(level, view);
                    floorCount++;
                }

                return lsViewOfFloor;
            }
        }
        public static BoundingBoxXYZ CalculateModelBoundingBox(Document pDoc)
        {
            var ft = new FilteredElementCollector(pDoc)
                .WhereElementIsNotElementType().WhereElementIsViewIndependent().ToElements();
            //var rs = ft.Where(e => e.LevelId.IntegerValue == pLv.LevelId.IntegerValue );

            var box = new BoundingBoxXYZ();
            foreach (var e in ft)
            {
                var eBox = e.get_BoundingBox(null);
                if (eBox != null)
                {
                    box.Min = new XYZ(Math.Min(box.Min.X, eBox.Min.X),
                                                Math.Min(box.Min.Y, eBox.Min.Y),
                                                Math.Min(box.Min.Z, eBox.Min.Z));
                    box.Max = new XYZ(Math.Max(box.Max.X, eBox.Max.X),
                                                    Math.Max(box.Max.Y, eBox.Max.Y),
                                                    Math.Max(box.Max.Z, eBox.Max.Z));
                }
            }
            return box;
        }
        public static View3D GetView3D(Document pDoc)
        {
            return new FilteredElementCollector(pDoc)
                    .OfCategory(BuiltInCategory.OST_Views)
                    .OfClass(typeof(View3D)).Cast<View3D>()
                    .Where(v => v.Name == "{3D}")
                    .FirstOrDefault();
        }
        public static List<Phase> GetPhases(Document pDoc)
        {
            return new FilteredElementCollector(pDoc)
                    .OfCategory(BuiltInCategory.OST_Phases)
                    .OfClass(typeof(Phase)).Cast<Phase>().ToList();
        }
    }
}
