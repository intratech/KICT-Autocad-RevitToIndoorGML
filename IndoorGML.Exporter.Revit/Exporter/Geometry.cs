using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit
{
    public class GeometryInfo
    {
        public ElementId ElementID;
        public Dictionary<ElementId, MeshTriangleData> GeoData = new Dictionary<ElementId, MeshTriangleData>();

        public bool HasGeo
        {
            get
            {
                if (GeoData != null)
                    return GeoData.Count > 0;
                return false;
            }
        }

        internal void Add(PolymeshTopology node, Transform currentTransform, Material currentMat)
        {
            var id = new ElementId(-1);
            if (currentMat != null)
            {
                id = currentMat.Id;
            }

            if (GeoData.ContainsKey(id))
            {
                var mesh = GeoData[id];
                mesh.AddMesh(node, currentTransform);
            }
            else
            {
                var mesh = new MeshTriangleData();
                mesh.Material = currentMat;
                GeoData[id] = mesh;
                mesh.AddMesh(node, currentTransform);
            }

        }

        internal void Clear()
        {
            if (GeoData != null)
            {
                foreach (var data in GeoData)
                {
                    data.Value.Clear();
                }

                GeoData.Clear();
            }
        }
    }

    public class MeshTriangleData

    {
        public float ScaleUnit = 1 / 0.3048f;
        public List<int> Triangles = new List<int>();
        public List<float> Vertexs = new List<float>();
        public float[] Box = new float[] { int.MaxValue, int.MaxValue, int.MaxValue, int.MinValue, int.MinValue, int.MinValue };
        public Material Material { get; internal set; }

        internal void AddMesh(PolymeshTopology node, Transform currentTransform)
        {
            int currentPos = Vertexs.Count / 3;
            var faces = node.GetFacets();
            foreach (var face in faces)
            {
                Triangles.Add(face.V1 + currentPos);
                Triangles.Add(face.V2 + currentPos);
                Triangles.Add(face.V3 + currentPos);
            }

            var points = node.GetPoints();
            foreach (XYZ xyz in points)
            {
                var p = currentTransform.OfPoint(xyz);
                UnitUtils.ConvertFromInternalUnits(p.X, DisplayUnitType.DUT_METERS);
                float x = (float)UnitUtils.ConvertFromInternalUnits(p.X, DisplayUnitType.DUT_METERS);
                float y = (float)UnitUtils.ConvertFromInternalUnits(p.Y, DisplayUnitType.DUT_METERS);
                float z = (float)UnitUtils.ConvertFromInternalUnits(p.Z, DisplayUnitType.DUT_METERS);
                Box[0] = Math.Min(x, Box[0]);
                Box[1] = Math.Min(y, Box[1]);
                Box[2] = Math.Min(z, Box[2]);

                Box[3] = Math.Max(x, Box[3]);
                Box[4] = Math.Max(y, Box[4]);
                Box[5] = Math.Max(z, Box[5]);
                Vertexs.Add(x);
                Vertexs.Add(y);
                Vertexs.Add(z);

            }



        }

        internal void Clear()
        {
            if (Triangles != null)
            {
                Triangles.Clear();
                Triangles = null;
            }

            if (Vertexs != null)
            {
                Vertexs.Clear();
                Vertexs = null;
            }
        }
    }
}
