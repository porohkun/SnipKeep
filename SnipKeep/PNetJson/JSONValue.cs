using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PNetJson
{
    public class JSONValue:IEnumerable<JSONValue>, IEquatable<JSONValue>
    {
        public JsonPos KeyPos { get; internal set; }
        public JsonPos ValPos { get; internal set; }

        public JSONValueType Type { get; private set; }
        public string Str { get; set; }
        public double Number { get; set; }
        public JSONObject Obj { get; set; }
        public JSONArray Array { get; set; }
        public bool Boolean { get; set; }
        public JSONValue Refer { get; set; }
        public JSONValue Parent { get; private set; }
        public JSONValue Root { get; private set; }

        #region constructors

        public JSONValue()
        {
            Type = JSONValueType.Null;
        }

        public JSONValue(JSONValueType type)
        {
            Type = type;
            switch (type)
            {
                case JSONValueType.String:
                    Str = "";
                    break;
                case JSONValueType.Number:
                    Number = 0;
                    break;
                case JSONValueType.Boolean:
                    Boolean = false;
                    break;
                case JSONValueType.Object:
                    Obj = new JSONObject();
                    break;
                case JSONValueType.Array:
                    Array = new JSONArray();
                    break;
            }
        }

        public JSONValue(string str)
        {
            if (str == null)
                Type = JSONValueType.Null;
            else
            {
                Type = JSONValueType.String;
                Str = str;
            }
        }

        public JSONValue(double number)
        {
            if (double.IsNaN(number) || double.IsInfinity(number))
                Type = JSONValueType.Null;
            else
            {
                Type = JSONValueType.Number;
                Number = number;
            }
        }

        public JSONValue(JSONObject obj)
        {
            if (obj == null)
            {
                Type = JSONValueType.Null;
            }
            else
            {
                Type = JSONValueType.Object;
                Obj = obj;
            }
        }

        public JSONValue(JSONArray array)
        {
            if (array == null)
                Type = JSONValueType.Null;
            else
            {
                Type = JSONValueType.Array;
                Array = array;
            }
        }

        public JSONValue(bool boolean)
        {
            Type = JSONValueType.Boolean;
            Boolean = boolean;
        }

        #endregion

        #region implisit to JSONValue

        public static implicit operator JSONValue(string str)
        {
            return new JSONValue(str);
        }
        
        public static implicit operator JSONValue(double number)
        {
            return new JSONValue(number);
        }

        public static implicit operator JSONValue(JSONObject obj)
        {
            return new JSONValue(obj);
        }

        public static implicit operator JSONValue(JSONArray array)
        {
            return new JSONValue(array);
        }

        public static implicit operator JSONValue(bool boolean)
        {
            return new JSONValue(boolean);
        }

        #endregion

        #region implisit from JSONValue

        private static InvalidCastException GetImplisitException(JSONValueType tryType, JSONValueType realType)
        {
            return new InvalidCastException(String.Concat("Wrong type '", tryType.ToString(), "'. Real type is '", realType.ToString(), "'."));
        }

        public static implicit operator double(JSONValue value)
        {
            if (value.Type != JSONValueType.Number)
                throw GetImplisitException(JSONValueType.Number, value.Type);
            return value.Number;
        }

        public static implicit operator float(JSONValue value)
        {
            if (value.Type != JSONValueType.Number)
                throw GetImplisitException(JSONValueType.Number, value.Type);
            return (float)value.Number;
        }

        public static implicit operator decimal(JSONValue value)
        {
            if (value.Type != JSONValueType.Number)
                throw GetImplisitException(JSONValueType.Number, value.Type);
            return (decimal)value.Number;
        }

        public static implicit operator int(JSONValue value)
        {
            if (value.Type != JSONValueType.Number)
                throw GetImplisitException(JSONValueType.Number, value.Type);
            return (int)value.Number;
        }

        public static implicit operator long(JSONValue value)
        {
            if (value.Type != JSONValueType.Number)
                throw GetImplisitException(JSONValueType.Number, value.Type);
            return (long)value.Number;
        }

        public static implicit operator string(JSONValue value)
        {
            if (value.Type != JSONValueType.String)
                throw GetImplisitException(JSONValueType.String, value.Type);
            return value.Str;
        }

        public static implicit operator JSONObject(JSONValue value)
        {
            if (value.Type != JSONValueType.Object)
                throw GetImplisitException(JSONValueType.Object, value.Type);
            return value.Obj;
        }

        public static implicit operator JSONArray(JSONValue value)
        {
            if (value.Type != JSONValueType.Array)
                throw GetImplisitException(JSONValueType.Array, value.Type);
            return value.Array;
        }

        public static implicit operator bool(JSONValue value)
        {
            if (value.Type != JSONValueType.Boolean)
                throw GetImplisitException(JSONValueType.Boolean, value.Type);
            return value.Boolean;
        }

        #endregion

        #region value getting

        public JSONValue this[int i]
        {
            get
            {
                if (Type != JSONValueType.Array)
                    throw GetImplisitException(JSONValueType.Array, Type);
                return Array[i];
            }
        }

        public JSONValue this[string name]
        {
            get
            {
                if (Type != JSONValueType.Object)
                    throw GetImplisitException(JSONValueType.Object, Type);
                return Obj[name];
            }
        }

        public JSONValue GetReal()
        {
            if (Type == JSONValueType.Refer)
                return Refer;
            else
                return this;
        }

        public JSONValue GetValueByPath(string path)
        {
            return GetValueByPath(path.Split('/'));
        }

        public JSONValue GetValueByPath(string[] path)
        {
            if (path.Length == 0)
                return GetReal();
            if (Type == JSONValueType.Object)
            {
                string child = path[0];
                string[] newpath = new string[path.Length - 1];
                for (int i=0;i<newpath.Length;i++)
                    newpath[i] = path[i + 1];
                if (child == "#")
                {
                    return Root.GetValueByPath(newpath);
                }
                if (Obj.ContainsKey(child))
                {
                    return Obj[child].GetValueByPath(newpath);
                }
            }
            
            return null;
        }

        #endregion

        #region parsing

        public static JSONValue Parse(string jsonString)
        {
            int p = 0;
            return Parse(jsonString, ref p);
        }

        public static JSONValue Parse(string jsonString, ref int position)
        {
            if (string.IsNullOrEmpty(jsonString))
                return new JSONValue(JSONValueType.Null);
            
            JSONValue val = new JSONValue(JSONValueType.Null);
            
            int startPos = position;
            char c;
            if (position < jsonString.Length)
            {
                c = jsonString[position];

                if (c == '"')
                {
                    val = new JSONValue(Parser.ParseString(jsonString, ref position));
                }
                else if (char.IsDigit(c) || c == '-')
                {
                    val = new JSONValue(Parser.ParseNumber(jsonString, ref position));
                }
                else
                    switch (c)
                    {

                        case '{':
                            val = new JSONValue(JSONObject.Parse(jsonString, ref position));
                            break;

                        case '[':
                            val = new JSONValue(JSONArray.Parse(jsonString, ref position));
                            break;

                        case 'f':
                        case 't':
                            val = new JSONValue(Parser.ParseBoolean(jsonString, ref position));
                            break;

                        case 'n':
                            if (Parser.ParseNull(jsonString, ref position))
                                val = new JSONValue(JSONValueType.Null);
                            break;

                        //default:
                        //    return Fail("beginning of value", startPosition);
                    }
            }
            else
                val = new JSONValue(JSONValueType.Null);
            position++;
            val.KeyPos = new JsonPos(startPos, startPos);
            val.ValPos = new JsonPos(startPos, position);
            
            Parser.SkipWhitespace(jsonString, ref position);

            val.UpdateParent(val);

            return val;
        }

        internal void UpdateParent(JSONValue parent)
        {
            if (parent == this)
            {
                Root = parent;
            }
            else
            {
                Parent = parent;
                Root = parent.Root;
            }
            switch (Type)
            {
                case JSONValueType.Object:
                    foreach (KeyValuePair<string, JSONValue> pair in Obj)
                    {
                        pair.Value.UpdateParent(this);
                    }
                    break;
                case JSONValueType.Array:
                    foreach (JSONValue value in Array)
                    {
                        value.UpdateParent(this);
                    }
                    break;
            }
        }

        internal void UpdateRefs()
        {
            switch (Type)
            {
                case JSONValueType.Object:
                    {
                        Dictionary<string,JSONValue> refs = new Dictionary<string,JSONValue>();
                        foreach (KeyValuePair<string, JSONValue> pair in Obj)
                        {
                            if (pair.Value.Type == JSONValueType.Object && pair.Value.Obj.ContainsKey("$ref"))
                            {
                                pair.Value.Type = JSONValueType.Refer;
                                pair.Value.Refer = Root.GetValueByPath(pair.Value.Obj["$ref"].Str);
                            }
                            else
                                pair.Value.UpdateRefs();
                        }
                        break;
                    }
                case JSONValueType.Array:
                    {
                        foreach (JSONValue value in Array)
                            if (value.Type == JSONValueType.Object && value.Obj.ContainsKey("$ref"))
                            {
                                value.Type = JSONValueType.Refer;
                                value.Refer = Root.GetValueByPath(value.Obj["$ref"].Str);
                            }
                            else
                                value.UpdateRefs();
                        break;
                    }
            }
        }

        #endregion

        #region save/load
#if UNITY_EDITOR || !(UNITY_STANDALONE || UNITY_WII || UNITY_IOS || UNITY_ANDROID || UNITY_PS3 || UNITY_PS4 || UNITY_SAMSUNGTV || UNITY_XBOX360 || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS || UNITY_WP_8 || UNITY_WP_8_1 || UNITY_WSA || UNITY_WEBGL)

        public static JSONValue Load(string path)
        {
            return Load(path, Encoding.UTF8);
        }

        public static JSONValue Load(string path, Encoding encoding)
        {
            if (System.IO.File.Exists(path))
            {
                JSONValue result = JSONValue.Parse(System.IO.File.ReadAllText(path, encoding));
                result.UpdateRefs();
                return result;
            }
            return new JSONValue(JSONValueType.Null);
        }

        public void Save(string path, bool formatted = true, bool unix = true)
        {
            Save(path, Encoding.UTF8, formatted, unix);
        }

        public void Save(string path, Encoding encoding, bool formatted = true, bool unix = true)
        {
            Save(path, this.ToString(formatted), encoding, unix);
        }

        public static void Save(string path, string value, bool unix = true)
        {
            Save(path, value, Encoding.UTF8, unix);
        }

        public static void Save(string path, string value, Encoding encoding, bool unix = true)
        {
            string text = value.Replace('\r', '\n');
            while (text.Contains("\n\n"))
                text = text.Replace("\n\n", "\n");
            System.IO.File.WriteAllText(path, text, encoding);
        }
        
#endif
        #endregion

        public override string ToString()
        {
            return ToString(0, true);
        }

        public string ToString(bool formatted)
        {
            return ToString(0, formatted);
        }

        public string ToString(int tabs, bool formatted)
        {
            switch (Type)
            {
                case JSONValueType.Object:
                    return Obj.ToString(formatted ? tabs : -1, formatted);

                case JSONValueType.Array:
                    return Array.ToString(formatted ? tabs : -1, formatted);

                case JSONValueType.Boolean:
                    return Boolean ? "true" : "false";

                case JSONValueType.Number:
                    return Number.ToString(System.Globalization.CultureInfo.InvariantCulture);

                case JSONValueType.String:
                    return "\"" + Str.Replace("\r", @"\r").Replace("\n", @"\n").Replace("\"", @"\""") + "\"";

                case JSONValueType.Refer:
                    return "\"" + Str + "\"";

                case JSONValueType.Null:
                    return "null";
            }
            return "null";
        }

        public JSONValue Clone()
        {
            JSONValue result = new JSONValue(Type);
            switch (Type)
            {
                case JSONValueType.String:
                    result.Str = Str;
                    break;

                case JSONValueType.Boolean:
                    result.Boolean = Boolean;
                    break;

                case JSONValueType.Number:
                    result.Number = Number;
                    break;

                case JSONValueType.Object:
                    result.Obj = Obj.Clone();
                    break;

                case JSONValueType.Array:
                    result.Array = Array.Clone();
                    break;
            }
            return result;
        }
        
        public bool Equals(JSONValue other)
        {
            if (Type != other.Type) return false;
            switch (Type)
            {
                case JSONValueType.Array: return Array.Equals(other.Array);
                case JSONValueType.Boolean: return Boolean.Equals(other.Boolean);
                case JSONValueType.Null: return false;
                case JSONValueType.Number: return Number.Equals(other.Number);
                case JSONValueType.Object: return Obj.Equals(other.Obj);
                case JSONValueType.Refer: return Refer == other.Refer;
                case JSONValueType.String: return Str.Equals(other.Str);
            }
            return false;
        }

        public IEnumerator<JSONValue> GetEnumerator()
        {
            return ((IEnumerable<JSONValue>)Array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<JSONValue>)Array).GetEnumerator();
        }
    }
}
