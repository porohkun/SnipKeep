
namespace PNetJson
{
    public class JsonValidateError
    {
        public JSONValue Value { get; private set; }
        public JSONValue SchemeValue { get; private set; }
        public string Message { get; private set; }
        public JVErrorType ErrorType { get; private set; }
        public JsonValidateError(JSONValue value, JSONValue schemeValue, JVErrorType type, string message)
        {
            Value = value;
            SchemeValue = schemeValue;
            ErrorType = type;
            Message = message;
        }

    }

    public enum JVErrorType
    {
        InvalidType,
        PropertyMissing,
        InvalidValue
    }
}
