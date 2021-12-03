using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD
{
    static class Config
    {
        private static double _scale = 1;//25.4; //1;
        private static double _defaultRoomHeight = 3000;//118; //3000;
        private static double _defaultDoorWidth = 800;//31.5; //800;
        private static double _defaultDoorHeight = 2000;//78.7; //2000;
        private static double _doorIsOnBoundaryToleren = 250;//3.9; //100;
        private static double _defaultGapToleren = 100;//3.9; //100;

        public static double Scale = 1;//25.4; //1;
        public static double DefaultRoomHeight = _defaultRoomHeight;
        public static double DefaultDoorWidth = _defaultDoorWidth;
        public static double DefaultDoorHeight = _defaultDoorHeight;
        public static double DoorIsOnBoundaryToleren = _doorIsOnBoundaryToleren;
        public static double DefaultGapToleren = _defaultGapToleren;
        public static string DefaultDoorLayer = "DOOR";
        public static string CSLayer = "GML-bldg-cs";
        public static string DoorLayer = "GML-bldg-door";
        public static string RoomLayer = "GML-bldg-room";
        public static string GapFillLayer = "GML-bldg-gap";
        public static string PreLayer = "GML-bldg-pre";
        public static string VirtualLayer = "GML-bldg-vd";
        public static string AppName = "CityGML";

        public static void ChangeUnit(Unit selectedUnit)
        {
            double scale = 1.0;

            switch (selectedUnit)
            {
                case Unit.Millimeter:
                    scale = 1;
                    break;
                case Unit.Centimeter:
                    scale *= 10;
                    break;
                case Unit.Decimeters:
                    scale = 100;
                    break;
                case Unit.Meter:
                    scale = 1000;
                    break;
                case Unit.Inch:
                    scale = 25.4;
                    break;

                default:
                    scale = 1;
                    break;
            }

            Scale = _scale / scale;
            DefaultRoomHeight = _defaultRoomHeight / scale;
            DefaultDoorWidth = _defaultDoorWidth / scale;
            DefaultDoorHeight = _defaultDoorHeight / scale;
            DoorIsOnBoundaryToleren = _doorIsOnBoundaryToleren / scale;
            DefaultGapToleren = _defaultGapToleren / scale;            
        }

     

    }
}
