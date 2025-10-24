using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AvaAIChat.Converters;

/// <summary>
/// 对象相等性转换器
/// </summary>
public class ObjectEqualityConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
            return false;

        return Equals(values[0], values[1]);
    }

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}