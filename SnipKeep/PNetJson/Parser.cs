using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PNetJson
{
    internal static class Parser
    {
        private static readonly Regex unicodeRegex = new Regex(@"\\u([0-9a-fA-F]{4})");
        private static readonly byte[] unicodeBytes = new byte[2];
        internal static bool CheckObjectStart(string jsonString, int position)
        {
            return CheckChar(jsonString, position, '{');
        }
        internal static bool CheckObjectEnd(string jsonString, int position)
        {
            return CheckChar(jsonString, position, '}');
        }
        internal static bool CheckArrayStart(string jsonString, int position)
        {
            return CheckChar(jsonString, position, '[');
        }
        internal static bool CheckArrayEnd(string jsonString, int position)
        {
            return CheckChar(jsonString, position, ']');
        }
        internal static bool CheckKeyValueSeparator(string jsonString, int position)
        {
            return CheckChar(jsonString, position, ':');
        }
        internal static bool CheckValueSeparator(string jsonString, int position)
        {
            return CheckChar(jsonString, position, ',');
        }
        private static bool CheckChar(string jsonString, int position, char c)
        {
            if (position >= jsonString.Length)
                return Fail(c, position);
            if (jsonString[position] != c)
                return false;
            return true;
        }
        internal static void SkipWhitespace(string str, ref int pos)
        {
            for (; pos < str.Length && char.IsWhiteSpace(str[pos]); ++pos) ;
        }
        internal static string ParseString(string str, ref int startPosition)
        {
            if (startPosition >= str.Length || startPosition + 1 >= str.Length || str[startPosition] != '"')
            {
                Fail('"', startPosition);
                return null;
            }

            var endPosition = str.IndexOf('"', startPosition + 1);
            if (endPosition <= startPosition)
            {
                Fail('"', startPosition + 1);
                return null;
            }

            while (str[endPosition - 1] == '\\')
            {
                endPosition = str.IndexOf('"', endPosition + 1);
                if (endPosition <= startPosition)
                {
                    Fail('"', startPosition + 1);
                    return null;
                }
            }

            var result = string.Empty;

            if (endPosition > startPosition + 1)
            {
                result = str.Substring(startPosition + 1, endPosition - startPosition - 1);
            }

            startPosition = endPosition;

            // Parse Unicode characters that are escaped as \uXXXX
            do
            {
                Match m = unicodeRegex.Match(result);
                if (!m.Success)
                {
                    break;
                }

                string s = m.Groups[1].Captures[0].Value;
                unicodeBytes[1] = byte.Parse(s.Substring(0, 2), NumberStyles.HexNumber);
                unicodeBytes[0] = byte.Parse(s.Substring(2, 2), NumberStyles.HexNumber);
                s = Encoding.Unicode.GetString(unicodeBytes);

                result = result.Replace(m.Value, s);
            } while (true);

            return result.Replace(@"\r", "\r").Replace(@"\n", "\n").Replace(@"\""", "\"");
        }
        internal static double ParseNumber(string str, ref int startPosition)
        {
            if (startPosition >= str.Length || (!char.IsDigit(str[startPosition]) && str[startPosition] != '-'))
            {
                return double.NaN;
            }

            var endPosition = startPosition + 1;

            for (;
                endPosition < str.Length && str[endPosition] != ',' && str[endPosition] != ']' && str[endPosition] != '}';
                ++endPosition) ;

            double result;
            if (
                !double.TryParse(str.Substring(startPosition, endPosition - startPosition), System.Globalization.NumberStyles.Float,
                                 System.Globalization.CultureInfo.InvariantCulture, out result))
            {
                return double.NaN;
            }
            startPosition = endPosition - 1;
            return result;
        }
        internal static bool ParseBoolean(string str, ref int startPosition)
        {
            if (str[startPosition] == 't')
            {
                if (str.Length < startPosition + 4 ||
                    str[startPosition + 1] != 'r' ||
                    str[startPosition + 2] != 'u' ||
                    str[startPosition + 3] != 'e')
                {
                    return Fail("true", startPosition);
                }
                startPosition += 3;
                return true;
            }
            else
            {
                if (str.Length < startPosition + 5 ||
                    str[startPosition + 1] != 'a' ||
                    str[startPosition + 2] != 'l' ||
                    str[startPosition + 3] != 's' ||
                    str[startPosition + 4] != 'e')
                {
                    return Fail("false", startPosition);
                }
                startPosition += 4;
                return false;
            }

        }
        internal static bool ParseNull(string str, ref int startPosition)
        {
            if (str.Length < startPosition + 4 ||
                                str[startPosition + 1] != 'u' ||
                                str[startPosition + 2] != 'l' ||
                                str[startPosition + 3] != 'l')
            {
                return Fail("null", startPosition);
            }
            startPosition += 3;
            return true;
        }
        internal static bool Fail(char expected, int position)
        {
            return Fail(new string(expected, 1), position);
        }
        internal static bool Fail(string expected, int position)
        {
            JSONLogger.Error("Invalid json string, expecting " + expected + " at " + position);
            return false;
        }

    }
}
