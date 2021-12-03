using CityGML.Core.DTO.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessor.InDoorGML.Models
{
    [Serializable]
    public class JWindowInDoorGML : JWindow
    {
        public JWindowInDoorGML(JWindow p)
        {
            foreach (FieldInfo prop in p.GetType().GetFields())
                GetType().GetField(prop.Name).SetValue(this, prop.GetValue(p));

            foreach (PropertyInfo prop in p.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(p, null), null);
        }

        /// <summary>
        /// Gets all rooms have connectivity with door.
        /// </summary>
        /// <param name="rooms"></param>
        /// <returns></returns>
        public List<JRoomInDoorGML> GetRooms(List<JRoomInDoorGML> rooms)
        {
            return new InDoorUtil().GetRooms(rooms, this);
        }
    }
}
