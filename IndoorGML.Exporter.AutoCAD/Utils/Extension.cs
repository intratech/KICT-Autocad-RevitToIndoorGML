using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Extension
{
    public static DataRow ToDataRow<T>(this T item, DataTable table)
    {
        PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
        DataRow row = table.NewRow();
        foreach (PropertyDescriptor prop in properties)
            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

        table.Rows.Add(row);
        return row;
    }

    public static DataTable ToDataTable<T>(this IList<T> data)
    {
        PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }
        return table;
    }

    public static List<T> ConvertDataTable<T>(this DataTable dt)
    {
        List<T> data = new List<T>();
        foreach (DataRow row in dt.Rows)
        {
            T item = GetItem<T>(row);
            data.Add(item);
        }
        return data;
    }
    public static T GetItem<T>(this DataRow dr)
    {
        Type temp = typeof(T);
        T obj = Activator.CreateInstance<T>();
        var properties = temp.GetProperties();

        foreach (DataColumn column in dr.Table.Columns)
        {
            foreach (var pro in properties)
            {
                try
                {

                    if (pro.Name == column.ColumnName)
                    {
                        var objValue = dr[column.ColumnName];
                        if (DBNull.Value == objValue || objValue == null)
                        {
                            continue;
                        }
                        else
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                    }
                    else
                        continue;
                }
                catch (Exception ex)
                {
                    //LogUtility.Instance.Log(pro.Name + ":" + ex.Message, LogType.Error);
                }
            }
        }
        return obj;
    }
}