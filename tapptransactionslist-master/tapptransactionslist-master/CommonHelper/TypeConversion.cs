//$Author: Anilkumar.ko $
//$Date: 9-01-20 10:20 $
//$Header: /Mobileapps/Operator/Davinci/TS/TAppTransactionsList/TAppTransactionsList/CommonHelper/TypeConversion.cs 3     9-01-20 10:20 Anilkumar.ko $
//$Modtime: 8-01-20 12:47 $
//$Revision: 3 $
//Copyright @2012 IMImobile Pvt. Ltd.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace CommonHelper
{
    public static class TypeConverstion
    {
        public static String ToStr(this object val)
        {
            return (val == null || val == DBNull.Value) ? String.Empty : val.ToString();
        }

        public static int ToInt(this object val)
        {
            int res = 0;
            Int32.TryParse(val.ToStr(), out res);
            return res;
        }

        public static int? ToNullInt(this object val)
        {
            int res = 0;
            if (!Int32.TryParse(val.ToStr(), out res))
                return null;
            return res;
        }

        public static DateTime ToDate(this object val)
        {
            return Convert.ToDateTime(val);
        }

        public static DateTime? IsDate(this object val)
        {
            DateTime resdate;
            if (!DateTime.TryParse(val.ToStr(), out resdate))
                return null;
            return resdate;
        }

        public static Double ToDouble(this object val)
        {
            Double resval = 0;
            Double.TryParse(val.ToStr(), out resval);
            return resval;
        }

        public static Decimal ToDecimal(this object val)
        {
            Decimal resval = 0;
            Decimal.TryParse(val.ToStr(), out resval);
            return resval;
        }

        public static Boolean ToBool(this object val)
        {
            bool resval;
            Boolean.TryParse(val.ToStr(), out resval);
            return resval;
        }

        public static T JsonDeserialize<T>(this String s)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Deserialize<T>(s);
        }

        public static String JsonSerialize(this object obj)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Serialize(obj);
        }

        public static String GetAttrValue(this XmlNode xNode, String _AttrName)
        {
            return xNode.Attributes[_AttrName] == null ? "" : xNode.Attributes[_AttrName].Value.Trim();
        }

        public static void AddKeyValue(this Dictionary<String, String> attr, String key, String value)
        {
            attr.AddKeyValue(key, value, false);
        }

        public static void AddKeyValue(this Dictionary<String, String> attr, String key, String value, bool allowEmptyValues)
        {
            if (attr == null || key.Trim().Equals(String.Empty)
                || (allowEmptyValues == false && value.Trim().Equals(String.Empty))) return;
            if (!attr.ContainsKey(key)) attr.Add(key, value);
        }

        public static void RemoveKey(this Dictionary<String, String> attr, String key)
        {
            if (key.Trim().Equals(String.Empty)) return;
            if (attr.ContainsKey(key)) attr.Remove(key);
        }

        public static String GetValue(this Dictionary<String, String> attr, String key)
        {
            if (key.Trim().Equals(String.Empty) || attr.ContainsKey(key) == false) return String.Empty;
            return attr[key].ToString().Trim();
        }

        public static String GetAttrValue(XDocument xDoc, String RootNode, String strAttName)
        {
            if (xDoc.Element(RootNode) == null) return String.Empty;
            if (xDoc.Element(RootNode).Attribute(strAttName) == null) return String.Empty;
            return xDoc.Element(RootNode).Attribute(strAttName).Value;
        }

        public static String GetAttrValue(XElement xElement, String strAttName)
        {
            if (xElement == null) return String.Empty;
            if (xElement.Attribute(strAttName) == null) return String.Empty;
            return xElement.Attribute(strAttName).Value;
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}
