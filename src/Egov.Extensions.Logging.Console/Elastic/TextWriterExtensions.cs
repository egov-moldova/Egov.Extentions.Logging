using System.Collections;
using System.Globalization;
using System.Text.Encodings.Web;

namespace Egov.Extensions.Logging.Elastic;

internal static class TextWriterExtensions
{
    private static readonly JavaScriptEncoder Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    public static void WriteFirstJsonPropertyUnescapedName(this TextWriter writer, string name, string value)
    {
        if (string.IsNullOrEmpty(value)) return;

        writer.Write('"');
        writer.Write(name);
        writer.Write("\":\"");
        Encoder.Encode(writer, value);
        writer.Write('"');
    }

    public static void WriteJsonPropertyUnescapedName(this TextWriter writer, string name, string? value)
    {
        if (string.IsNullOrEmpty(value)) return;

        writer.WriteJsonPropertyNameUnescaped(name);
        writer.Write('"');
        Encoder.Encode(writer, value);
        writer.Write('"');
    }

    public static void WriteJsonPropertyUnescapedName(this TextWriter writer, string name, int value)
    {
        writer.WriteJsonPropertyNameUnescaped(name);
        writer.Write(value);
    }

    public static void WriteJsonProperty(this TextWriter writer, string name, object? value)
    {
        switch (value)
        {
            case null:
                break;
            case string stringValue:
                writer.WriteJsonPropertyName(name);
                writer.Write('"');
                Encoder.Encode(writer, stringValue);
                writer.Write('"');
                break;
            case IEnumerable<KeyValuePair<string, object>> pairValues:
                writer.WriteJsonProperty(name, pairValues);
                break;
            case IList listValue:
                writer.WriteJsonProperty(name, listValue);
                break;
            case IEnumerable enumerableValue:
                writer.WriteJsonProperty(name, enumerableValue);
                break;
            default:
                writer.WriteJsonPropertyName(name);
                writer.WriteJsonPropertyValue(value);
                break;
        }
    }

    private static void WriteJsonProperty(this TextWriter writer, string name, IEnumerable<KeyValuePair<string, object>> pairs)
    {
        var hasValues = false;
        foreach (var pair in pairs)
        {
            // ReSharper disable twice ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (pair.Key == null || pair.Value == null) continue;

            if (hasValues)
            {
                writer.WriteJsonProperty(pair.Key, pair.Value);
            }
            else
            {
                writer.WriteJsonPropertyName(name);
                writer.Write('{');
                writer.WriteFirstJsonPropertyName(pair.Key);
                writer.WriteJsonPropertyValue(pair.Value);
                hasValues = true;
            }
        }
        if (hasValues) writer.Write('}');
    }

    public static void WriteJsonProperty(this TextWriter writer, string name, IList? values)
    {
        if (values == null || values.Count == 0) return;

        if (values.Count == 1)
        {
            writer.WriteJsonProperty(name, values[0]);
            return;
        }

        var hasValues = false;
        foreach (var value in values)
        {
            if (value == null) continue;
            if (hasValues)
            {
                writer.Write(',');
            }
            else
            {
                writer.WriteJsonPropertyName(name);
                writer.Write('[');
                hasValues = true;
            }

            writer.WriteJsonPropertyValue(value);
        }
        if (hasValues) writer.Write(']');
    }

    private static void WriteJsonProperty(this TextWriter writer, string name, IEnumerable values)
    {
        object? firstValue = null;
        object? secondValue = null;

        foreach (var value in values)
        {
            if (value == null) continue;
            if (firstValue == null)
            {
                firstValue = value;
                continue;
            }
            if (secondValue == null)
            {
                secondValue = value;
                writer.WriteJsonPropertyName(name);
                writer.Write('[');
                writer.WriteJsonPropertyValue(firstValue);
            }

            writer.Write(',');
            writer.WriteJsonPropertyValue(value);
        }

        if (secondValue != null)
        {
            writer.Write(']');
        }
        else if (firstValue != null)
        {
            writer.WriteJsonProperty(name, firstValue);
        }
    }

    private static void WriteFirstJsonPropertyName(this TextWriter writer, string name)
    {
        writer.Write('"');
        Encoder.Encode(writer, name);
        writer.Write("\":");
    }

    private static void WriteJsonPropertyName(this TextWriter writer, string name)
    {
        writer.Write(",\"");
        Encoder.Encode(writer, name);
        writer.Write("\":");
    }

    private static void WriteJsonPropertyNameUnescaped(this TextWriter writer, string name)
    {
        writer.Write(",\"");
        writer.Write(name);
        writer.Write("\":");
    }

    private static void WriteJsonPropertyValue(this TextWriter writer, object value)
    {
        switch (value)
        {
            case bool boolValue:
                writer.Write(boolValue);
                break;
            case byte byteValue:
                writer.Write(byteValue);
                break;
            case sbyte sbyteValue:
                writer.Write(sbyteValue);
                break;
            case char charValue:
                writer.Write(charValue);
                break;
            case decimal decimalValue:
                writer.Write(decimalValue);
                break;
            case double doubleValue:
                writer.Write(doubleValue);
                break;
            case float floatValue:
                writer.Write(floatValue);
                break;
            case int intValue:
                writer.Write(intValue);
                break;
            case uint uintValue:
                writer.Write(uintValue);
                break;
            case long longValue:
                writer.Write(longValue);
                break;
            case ulong ulongValue:
                writer.Write(ulongValue);
                break;
            case short shortValue:
                writer.Write(shortValue);
                break;
            case ushort ushortValue:
                writer.Write(ushortValue);
                break;
            default:
                writer.Write('"');
                Encoder.Encode(writer, Convert.ToString(value, CultureInfo.InvariantCulture)!);
                writer.Write('"');
                break;
        }
    }
}