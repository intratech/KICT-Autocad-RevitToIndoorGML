using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Config
{
    public class UnitSpaceFunctionExcel
    {
        public List<UnitSpaceClass> UnitSpaceClasses = new List<UnitSpaceClass>();
        public List<FunctionCode> FunctionCodes = new List<FunctionCode>();

        public void ReadFile()
        {
            var file = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Config\\unitSpaceClass and Function Code.xlsx");
            ReadFile(file);
        }
        public void ReadFile(string excelFile)
        {
            if (File.Exists(excelFile))
            {
                string file = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), DateTime.Now.Ticks + Path.GetExtension(excelFile));

                File.Copy(excelFile, file);

                try
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        IWorkbook workbook = null;
                        if (excelFile.IndexOf(".xlsx") > 0)
                            workbook = new XSSFWorkbook(fs);
                        else if (excelFile.IndexOf(".xls") > 0)
                            workbook = new HSSFWorkbook(fs);
                        if (workbook == null)
                            return;

                        ISheet sheet = workbook.GetSheetAt(0);
                        if (sheet != null)
                        {
                            LoadUnitSpace(sheet);
                        }

                        sheet = workbook.GetSheetAt(1);
                        if (sheet != null)
                        {
                            LoadFunctionCode(sheet);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    File.Delete(file);
                }
            }
        }

        private void LoadFunctionCode(ISheet sheet)
        {
            int rowCount = sheet.LastRowNum; // This may not be valid row count.
                                             // If first row is table head, i starts from 1
            for (int i = 2; i <= rowCount; i++)
            {
                IRow curRow = sheet.GetRow(i);
                // Works for consecutive data. Use continue otherwise 
                if (curRow == null)
                {
                    continue;
                }
                FunctionCode functionCode = new FunctionCode()
                {
                    Name = curRow.GetCell(1)?.ToString(),
                    DomainCode = curRow.GetCell(2)?.ToString(),
                    Description = curRow.GetCell(3)?.ToString(),
                    //RevitRoomLabel = curRow.GetCell(4)?.ToString()
                };

                FunctionCodes.Add(functionCode);
            }
        }

        private void LoadUnitSpace(ISheet sheet)
        {
            int rowCount = sheet.LastRowNum; // This may not be valid row count.
                                             // If first row is table head, i starts from 1
            for (int i = 2; i <= rowCount; i++)
            {
                IRow curRow = sheet.GetRow(i);
                // Works for consecutive data. Use continue otherwise 
                if (curRow == null)
                {
                    continue;
                }
                UnitSpaceClass unitSpace = new UnitSpaceClass()
                {
                    Name = curRow.GetCell(1)?.ToString(),
                    DomainCode = curRow.GetCell(2)?.ToString(),
                    Description = curRow.GetCell(3)?.ToString(),
                    //RevitRoomLabel = curRow.GetCell(4)?.ToString()
                };

                UnitSpaceClasses.Add(unitSpace);
            }
        }

        //ISheet sheet = hssfwb.GetSheet("Arkusz1");
        //for (int row = 0; row <= sheet.LastRowNum; row++)
        //{
        //    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
        //    {
        //        MessageBox.Show(string.Format("Row {0} = {1}", row, sheet.GetRow(row).GetCell(0).StringCellValue));
        //    }
        //}
    }


    public class UnitSpaceClass
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public string DomainCode { get; internal set; }
    }

    public class FunctionCode
    {
        public string Name { get; internal set; }
        public string DomainCode { get; internal set; }
        public string Description { get; internal set; }
        public string RevitRoomLabel { get; internal set; }
    }
}
