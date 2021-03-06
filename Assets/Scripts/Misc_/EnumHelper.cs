﻿using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;

public static class EnumHelper
{
    public static string GetDescription(Enum @enum)
    {
        if (@enum == null)
            return null;

        string description = @enum.ToString();

        try
        {
            FieldInfo fi = @enum.GetType().GetField(@enum.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                description = attributes[0].Description;
        }
        catch
        {
        }

        return description;
    }

    public static IEnumerable<T> GetEnumerable<T>() {
        return (IEnumerable<T>)Enum.GetValues(typeof(T));
    }

}
