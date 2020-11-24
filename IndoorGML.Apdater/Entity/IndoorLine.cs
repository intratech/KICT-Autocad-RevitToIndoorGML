using System.Numerics;

namespace IndoorGML.Adapter
{
    public class IndoorLine
    {
        public IndoorLine(Vector3 start, Vector3 end)
        {
            StartPos = start;
            EndPos = end;
        }

        public Vector3 StartPos { get; internal set; }
        public Vector3 EndPos { get; internal set; }
    }
}