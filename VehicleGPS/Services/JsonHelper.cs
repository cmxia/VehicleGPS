using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

//change or add by shiwei 2013/12/27 19:15
namespace VehicleGPS.Services
{
    public static class JsonHelper
    {
        /// <summary> 
        /// List转成json 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="jsonName"></param> 
        /// <param name="list"></param> 
        /// <returns></returns> 
        public static string ListToJson<T>(IList<T> list, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
                jsonName = list[0].GetType().Name;
            Json.Append("{\"" + jsonName + "\":[");
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    T obj = Activator.CreateInstance<T>();
                    PropertyInfo[] pi = obj.GetType().GetProperties();
                    Json.Append("{");
                    for (int j = 0; j < pi.Length; j++)
                    {
                        Type type = pi[j].GetValue(list[i], null).GetType();
                        Json.Append("\"" + pi[j].Name.ToString() + "\":" + StringFormat(pi[j].GetValue(list[i], null).ToString(), type));
                        if (j < pi.Length - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < list.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
        /// <summary> 
        /// List转成json 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="list"></param> 
        /// <returns></returns> 
        public static string ListToJson<T>(IList<T> list)
        {
            object obj = list[0];
            return ListToJson<T>(list, obj.GetType().Name);
        }
        /// <summary> 
        /// 对象转换为Json字符串 
        /// </summary> 
        /// <param name="jsonObject">对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(object jsonObject)
        {
            string jsonString = "{";
            PropertyInfo[] propertyInfo = jsonObject.GetType().GetProperties();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                object objectValue = propertyInfo[i].GetGetMethod().Invoke(jsonObject, null);
                string value = string.Empty;
                if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                {
                    value = "'" + objectValue.ToString() + "'";
                }
                else if (objectValue is string)
                {
                    value = "'" + ToJson(objectValue.ToString()) + "'";
                }
                else if (objectValue is IEnumerable)
                {
                    value = ToJson((IEnumerable)objectValue);
                }
                else
                {
                    value = ToJson(objectValue.ToString());
                }
                jsonString += "\"" + ToJson(propertyInfo[i].Name) + "\":" + value + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "}";
        }
        /// <summary> 
        /// 对象集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString += ToJson(item) + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "]";
        }
        /// <summary> 
        /// 普通集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToArrayString(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString = ToJson(item.ToString()) + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "]";
        }
        /// <summary> 
        /// Datatable转换为Json 
        /// </summary> 
        /// <param name="table">Datatable对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();

            if (dt.Rows.Count == 0)
            {
                jsonString.Append("[{}]");
                return jsonString.ToString();
            }

            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }
        /// <summary> 
        /// DataTable转成Json 
        /// </summary> 
        /// <param name="jsonName"></param> 
        /// <param name="dt"></param> 
        /// <returns></returns> 
        public static string ToJson(DataTable dt, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
                jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + StringFormat(dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
        /// <summary> 
        /// DataReader转换为Json 
        /// </summary> 
        /// <param name="dataReader">DataReader对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DbDataReader dataReader)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            while (dataReader.Read())
            {
                jsonString.Append("{");
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    Type type = dataReader.GetFieldType(i);
                    string strKey = dataReader.GetName(i);
                    string strValue = dataReader[i].ToString();
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (i < dataReader.FieldCount - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            dataReader.Close();
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }
        /// <summary> 
        /// DataSet转换为Json 
        /// </summary> 
        /// <param name="dataSet">DataSet对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                jsonString += "\"" + table.TableName + "\":" + ToJson(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }
        /// <summary> 
        /// 过滤特殊字符 
        /// </summary> 
        /// <param name="s"></param> 
        /// <returns></returns> 
        private static string String2Json(String s)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];

                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }
        /// <summary> 
        /// 格式化字符型、日期型、布尔型 
        /// </summary> 
        /// <param name="str"></param> 
        /// <param name="type"></param> 
        /// <returns></returns> 
        private static string StringFormat(string str, Type type)
        {
            if (str.Length > 0)
            {
                if (type == typeof(string))
                {
                    str = String2Json(str);
                    str = "\"" + str + "\"";
                }
                else if (type == typeof(DateTime))
                {
                    str = "\"" + Convert.ToDateTime(str).ToShortDateString() + "\"";
                }
                else if (type == typeof(bool))
                {
                    str = str.ToLower();
                }
                else if (type == typeof(int))
                {
                    str = str.ToString();
                }
                else
                {
                    str = str.ToString();
                }

            }
            if (str.Length == 0)
                str = "\"\"";
            //str = "null";

            return str;
        }

        #region json 转换为datatable 测试通过
        /// <summary>
        /// json 转换为datatable
        /// </summary>
        /// <param name="strJson">待转换的json字符串</param>
        /// <returns>datatable</returns>
        public static DataTable JsonToDataTable(string strJson)
        {
            //取出表名   
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名   
            if (strJson != "error")
            {
                strJson = strJson.Substring(strJson.IndexOf("[") + 1);
                strJson = strJson.Substring(0, strJson.IndexOf("]"));
            }
     

            //获取数据   
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split(',');

                //创建表   
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split(':');
                        string header = strCell[0];
                        if (header.ToString().Contains("\""))
                        {
                            header = header.Replace("\"", "");
                        }
                        dc.ColumnName = header;
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }

                //增加内容   
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    string s = strRows[r];
                    int index = s.IndexOf(":");
                    int end = s.Length;
                    s = s.Substring(index + 1, end - index-1);
                    s = s.Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                    dr[r] = s;
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }

            return tb;
        }
        #endregion

        #region DataTable 转换为Json 字符串
        /// <summary>
        /// DataTable 对象 转换为Json 字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        //public static string DtToJson(this DataTable dt)
        //{
        //    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        //    javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
        //    ArrayList arrayList = new ArrayList();
        //    foreach (DataRow dataRow in dt.Rows)
        //    {
        //        Dictionary<string, object> dictionary = new Dictionary<string, object>();  //实例化一个参数集合
        //        foreach (DataColumn dataColumn in dt.Columns)
        //        {
        //            dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToStr());
        //        }
        //        arrayList.Add(dictionary); //ArrayList集合中添加键值
        //    }

        //    return javaScriptSerializer.Serialize(arrayList);  //返回一个json字符串
        //}
        #endregion

        #region Json 字符串 转换为 DataTable数据集合
        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        //public static DataTable ToDataTable(this string json)
        //{
        //    DataTable dataTable = new DataTable();  //实例化
        //    DataTable result;
        //    try
        //    {
        //        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        //        javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
        //        ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
        //        if (arrayList.Count > 0)
        //        {
        //            foreach (Dictionary<string, object> dictionary in arrayList)
        //            {
        //                if (dictionary.Keys.Count<string>() == 0)
        //                {
        //                    result = dataTable;
        //                    return result;
        //                }
        //                if (dataTable.Columns.Count == 0)
        //                {
        //                    foreach (string current in dictionary.Keys)
        //                    {
        //                        dataTable.Columns.Add(current, dictionary[current].GetType());
        //                    }
        //                }
        //                DataRow dataRow = dataTable.NewRow();
        //                foreach (string current in dictionary.Keys)
        //                {
        //                    dataRow[current] = dictionary[current];
        //                }

        //                dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    result = dataTable;
        //    return result;
        //}
        #endregion

        #region 转换为string字符串类型
        /// <summary>
        ///  转换为string字符串类型
        /// </summary>
        /// <param name="s">获取需要转换的值</param>
        /// <param name="format">需要格式化的位数</param>
        /// <returns>返回一个新的字符串</returns>
        public static string ToStr(this object s, string format = "")
        {
            string result = "";
            try
            {
                if (format == "")
                {
                    result = s.ToString();
                }
                else
                {
                    result = string.Format("{0:" + format + "}", s);
                }
            }
            catch
            {
            }
            return result;
        }
        #endregion
    }
}
