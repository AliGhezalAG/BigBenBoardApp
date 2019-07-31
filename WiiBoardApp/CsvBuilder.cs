using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RestWCFServiceLibrary.WiiMote
{
    public class CsvBuilder
    {
        public static IEnumerable<string> ToCsv<T>(IEnumerable<T> objectlist, string separator)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();

            yield return String.Join(separator, GetHeaders(fields).Concat(GetHeaders(properties)).ToArray());

            foreach (var o in objectlist)
            {
                yield return string.Join(separator, GetValues(fields, o).Concat(GetValues(properties, o)).Select(v => v.Replace(',', '.')).ToArray());
            }
        }

        private static IEnumerable<string> GetHeaders(FieldInfo[] fields)
        {
            return fields.Select(f => f.Name);
        }

        private static IEnumerable<string> GetHeaders(PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<DisplayNameAttribute>();
                if (null == attribute)
                {
                    yield return property.Name;
                }
                else
                {
                    yield return attribute.DisplayName;
                }
            }
        }

        private static IEnumerable<string> GetValues<T>(FieldInfo[] fields, T o)
        {
            foreach (var field in fields)
            {
                var value = field.GetValue(o);
                if (null == value)
                {
                    yield return "";
                }
                else
                {
                    yield return value.ToString();
                }
            }
        }

        private static IEnumerable<string> GetValues<T>(PropertyInfo[] properties, T o)
        {
            foreach (var p in properties)
            {
                var value = p.GetValue(o, null);
                if (null == value)
                {
                    yield return "";
                }
                else
                {
                    yield return value.ToString();
                }

            }
        }
    }
}
