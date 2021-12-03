using Autodesk.Revit.DB;
using IndoorGML.Exporter.Revit.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Utils
{
    public class ViewScheduleUtils
    {
        private RoomScheduleBindingList LoadRoomSchedule(Document doc)
        {
            var viewSchedule = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>().
                Where(v => v.Definition != null && v.Definition.CategoryId != null && v.Definition.CategoryId.IntegerValue == (int)BuiltInCategory.OST_Rooms).FirstOrDefault();

            if (viewSchedule == null)
                return null;

            var table = viewSchedule.GetTableData();
            var bodySection = table.GetSectionData(SectionType.Body);
            var rows = new RoomScheduleBindingList();

            var levelColIndex = -1;
            var nameColIndex = -1;
            var numberColIndex = -1;

            for (int r = 0; r < bodySection.NumberOfRows; r++)
            {
                var row = new RoomScheduleItem();
                if (r == 0)
                {
                    for (int c = 0; c < bodySection.NumberOfColumns; c++)
                    {

                        if (r == 0)
                        {
                            if (bodySection.GetCellText(r, c) == "Level")
                                levelColIndex = c;
                            if (bodySection.GetCellText(r, c) == "Name")
                                nameColIndex = c;
                            if (bodySection.GetCellText(r, c) == "Number")
                                numberColIndex = c;
                        }
                    }
                }
                else
                {
                    if (levelColIndex != -1)
                        row.Level = viewSchedule.GetCellText(SectionType.Body, r, levelColIndex);
                    if (nameColIndex != -1)
                        row.Name = viewSchedule.GetCellText(SectionType.Body, r, nameColIndex);
                    if (numberColIndex != -1)
                    {
                        row.Number = int.Parse(viewSchedule.GetCellText(SectionType.Body, r, numberColIndex));
                        //var _number = -1;
                        //if (int.TryParse(viewSchedule.GetCellText(SectionType.Body, r, numberColIndex), out _number))
                        //{
                        //    row.Number = _number;
                        //}
                    }


                    if (row.Level == "Not Placed")
                    {
                        row.HighLight = true;
                    }

                    if (!string.IsNullOrEmpty(row.Level) || !string.IsNullOrEmpty(row.Name))
                    {
                        rows.Add(row);
                    }
                }
            }

            return rows;
        }
        private List<DoorScheduleItem> LoadDoorSchedule(Document doc)
        {
            var viewSchedule = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>().
                Where(v => v.Definition != null && v.Definition.CategoryId != null && v.Definition.CategoryId.IntegerValue == (int)BuiltInCategory.OST_Doors).FirstOrDefault();

            if (viewSchedule == null)
                return null;

            var table = viewSchedule.GetTableData();
            var bodySection = table.GetSectionData(SectionType.Body);
            var rows = new List<DoorScheduleItem>();

            var levelColIndex = -1;
            var markColIndex = -1;
            var typeColIndex = -1;
            var fromRoomColIndex = -1;
            var toRoomColIndex = -1;


            for (int r = 0; r < bodySection.NumberOfRows; r++)
            {
                var row = new DoorScheduleItem();
                if (r == 0)
                {
                    for (int c = 0; c < bodySection.NumberOfColumns; c++)
                    {
                        if (bodySection.GetCellText(r, c) == "Level")
                            levelColIndex = c;
                        if (bodySection.GetCellText(r, c) == "Mark")
                            markColIndex = c;
                        if (bodySection.GetCellText(r, c) == "Type")
                            typeColIndex = c;
                        if (bodySection.GetCellText(r, c) == "From Room: Name")
                            fromRoomColIndex = c;
                        if (bodySection.GetCellText(r, c) == "To Room: Name")
                            toRoomColIndex = c;
                    }
                }
                else
                {
                    if (levelColIndex != -1)
                        row.Level = viewSchedule.GetCellText(SectionType.Body, r, levelColIndex);
                    if (markColIndex != -1)
                    {
                        row.Mark = int.Parse(viewSchedule.GetCellText(SectionType.Body, r, markColIndex));
                        //var _mark = -1;
                        //if (int.TryParse(viewSchedule.GetCellText(SectionType.Body, r, markColIndex), out _mark))
                        //{
                        //    row.Mark = _mark;
                        //}
                    }

                    if (typeColIndex != -1)
                        row.Type = viewSchedule.GetCellText(SectionType.Body, r, typeColIndex);
                    if (fromRoomColIndex != -1)
                        row.FromRoom = viewSchedule.GetCellText(SectionType.Body, r, fromRoomColIndex);
                    if (toRoomColIndex != -1)
                        row.ToRoom = viewSchedule.GetCellText(SectionType.Body, r, toRoomColIndex);

                    if (string.IsNullOrEmpty(row.FromRoom) && string.IsNullOrEmpty(row.ToRoom))
                    {
                        row.HighLight = true;
                    }

                    if (!string.IsNullOrEmpty(row.Level)
                        || row.Mark != null
                        || !string.IsNullOrEmpty(row.Type)
                        || !string.IsNullOrEmpty(row.FromRoom)
                        || !string.IsNullOrEmpty(row.ToRoom))
                    {
                        rows.Add(row);
                    }
                }
            }

            return rows;
        }
    }
}
