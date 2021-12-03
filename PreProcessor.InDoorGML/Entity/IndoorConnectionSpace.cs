using IndoorGML.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Adapter.Entity
{
    public class IndoorConnectionSpace : IndoorEntity
    {
        public string BoundaryID
        {
            get => $"B{Name}";
        }

        public string DualityToTransation
        {
            get => $"#T{Name}";
        }
        public string TransationId
        {
            get => $"T{Name}";
        }
        public string Usage { get; internal set; }
        public string Function { get; internal set; }

        public CellSpaceBoundaryMember GetFrontDoor()
        {
            CellSpaceBoundaryMember cellBoundary = new CellSpaceBoundaryMember();
            cellBoundary.ConnectionBoundary = new ConnectionBoundary();
            cellBoundary.ConnectionBoundary.Id = BoundaryID;
            cellBoundary.ConnectionBoundary.Name = DisplayName;
            cellBoundary.ConnectionBoundary.Description = DisplayName;
            IndoorLine line = new IndoorLine(Boundary.Points[0], Boundary.Points[1]);
            cellBoundary.ConnectionBoundary.CellSpaceBoundaryGeometry = ConvertBoundaryGeometry(line,
                 cellBoundary.ConnectionBoundary.Id
                 , Elevation, Height);
            cellBoundary.ConnectionBoundary.Duality = new Duality() { Href = DualityToTransation };

            return cellBoundary;
        }

        public override Pos GetCenterPos()
        {
            if(Boundary.Points.Count ==2)
            {
                var center = (Boundary.Points[0]+ Boundary.Points[1])/ 2;              
                return new Pos(center.X * Scale, center.Y * Scale, center.Z + Elevation);              
            }
            else if (Boundary.Points.Count > 2)
            {
                return new Pos(Boundary.Points.Average(x => x.X) * Scale, Boundary.Points.Average(y => y.Y) * Scale, Boundary.Points.Average(z => z.Z) + Elevation);
            }
            return base.GetCenterPos();
        }

        internal CellSpaceBoundaryMember GetBackDoor()
        {
            if (Boundary.Points.Count < 3)
                return null;
            CellSpaceBoundaryMember cellBoundary = new CellSpaceBoundaryMember();
            cellBoundary.ConnectionBoundary = new ConnectionBoundary();
            cellBoundary.ConnectionBoundary.Id = $"{BoundaryID}-REVERSE";
            cellBoundary.ConnectionBoundary.Name = DisplayName;
            cellBoundary.ConnectionBoundary.Description = DisplayName;
            IndoorLine line = new IndoorLine(Boundary.Points[2], Boundary.Points[3]);
            cellBoundary.ConnectionBoundary.CellSpaceBoundaryGeometry = ConvertBoundaryGeometry(line, 
                cellBoundary.ConnectionBoundary.Id, Elevation, Height);
            cellBoundary.ConnectionBoundary.Duality = new Duality() { Href = DualityToTransation };

            return cellBoundary;
        }

        public override IEnumerable<SurfaceMember> ConvertPolylineToSurfaces(InDoorPolyline polyline, float Elevation, float floorHeight)
        {
            int i = 0;
            var newPoints = OrderPosition.SortPoint(polyline.Points.Select(x => new OrderPosition(x, i++)).ToList());
            InDoorPolyline line = new InDoorPolyline() { Points = newPoints.Select(x => x.Pos).ToList() };
            line.Standard();
            return base.ConvertPolylineToSurfaces(line, Elevation, floorHeight);
        }

        internal bool IsSelfOverlap(float tolerance)
        {
            if (Boundary.Points.Count > 3)
            {
                if ((Boundary.GetCenter() - (Boundary.Points[0] + Boundary.Points[1]) / 2).Length() < tolerance)
                    return true;
                return false;
            }
            return true;
        }
    }
}
