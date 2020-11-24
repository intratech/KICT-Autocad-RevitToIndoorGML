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
            if(Boundary.Points.Count >=2)
            {
                var center = (Boundary.Points[0]+ Boundary.Points[1])/ 2;
                return new Pos(center.X * Scale, center.Y * Scale, center.Z + Elevation);              
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
    }
}
