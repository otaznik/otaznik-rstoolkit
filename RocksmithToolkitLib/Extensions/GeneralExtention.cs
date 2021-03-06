﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace RocksmithToolkitLib.Extensions
{
    public static class GeneralExtensions
    {
        public static bool Contains(this String obj, char[] chars)
        {
            return (obj.IndexOfAny(chars) >= 0);
        }

        public static string GetDescription(this object value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string[] SelectLines(this string[] content, string value) {
            return (from j in content
                    where j.Contains(value)
                    select j).ToArray<string>();
        }

        public static int ToInt32(this string value)
        {
            int v;
            if (int.TryParse(value, out v) == false)
                return -1;
            return v;
        }
    }
}
