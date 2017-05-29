using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNetJson
{
    public struct JOPair
    {
        private string _Key;
        private JSONValue _Value;
        public string Key
        {
            get
            {
                return _Key;
            }
        }
        public JSONValue Value
        {
            get
            {
                return _Value;
            }
        }

        public JOPair(string key, JSONValue value)
        {
            _Key = key;
            _Value = value;
        }

        public static implicit operator KeyValuePair<string, JSONValue>(JOPair pair)
        {
            return new KeyValuePair<string, JSONValue>(pair.Key, pair.Value);
        }

        public static implicit operator JOPair(KeyValuePair<string, JSONValue> pair)
        {
            return new JOPair(pair.Key, pair.Value);
        }

    }


}
