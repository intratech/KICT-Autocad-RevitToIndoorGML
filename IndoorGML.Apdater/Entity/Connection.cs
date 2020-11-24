using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Adapter.Entity
{
    public class Connection
    {
        public int From { get; set; }
        public int To { get; set; }

        public ConnectionType ConnectionType = ConnectionType.RoomToDoor;
    }

    public enum ConnectionType
    {
        RoomToDoor,
        RoomToRoom
    }
}
