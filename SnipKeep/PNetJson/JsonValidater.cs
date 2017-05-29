using System.Collections.Generic;

namespace PNetJson
{
    public static class JsonValidater
    {

        public static List<JsonValidateError> Validate(JSONValue value, JSONValue scheme)
        {
            List<JsonValidateError> errors = new List<JsonValidateError>();
            if (scheme.Type == JSONValueType.Null)
            {
                //errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType, "scheme must be not null"));
                return errors;
            }
            if (value.Type == JSONValueType.Refer && value.Obj["$ref"].Str == "#")
                return errors;
            if (scheme.GetReal().Obj.ContainsKey("type"))
                switch (scheme.GetReal().Obj["type"].Str)
                {
                    case "object":
                        errors.AddRange(ValidateObject(value.GetReal(), scheme.GetReal()));
                        break;
                    case "array":
                        errors.AddRange(ValidateArray(value.GetReal(), scheme.GetReal()));
                        break;
                    case "string":
                        errors.AddRange(ValidateString(value.GetReal(), scheme.GetReal()));
                        break;
                    case "integer":
                        errors.AddRange(ValidateInteger(value.GetReal(), scheme.GetReal()));
                        break;
                    case "number":
                        errors.AddRange(ValidateNumber(value.GetReal(), scheme.GetReal()));
                        break;
                    case "boolean":
                        errors.AddRange(ValidateBoolean(value.GetReal(), scheme.GetReal()));
                        break;
                    case "null":
                        errors.AddRange(ValidateNull(value.GetReal(), scheme.GetReal()));
                        break;
                    default:
                        errors.Add(new JsonValidateError(value.GetReal(), scheme, JVErrorType.InvalidType,
                            "Ошибка схемы. Неправильный тип значения: \"" + scheme.GetReal().Obj["type"].Str + "\"."));
                        break;
                }
            else
            {
                if (scheme.GetReal().Obj.ContainsKey("anyOf"))
                {
                    foreach (JSONValue aval in scheme.GetReal().Obj["anyOf"].Array)
                    {
                        if (Validate(value, aval.GetReal()).Count == 0)
                            return errors;
                    }
                    errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidValue,
                        "Структура должна подходить под одну из представленных схем"));
                }
            }

            return errors;
        }

        public static List<JsonValidateError> ValidateObject(JSONValue value, JSONValue scheme)
        {
            List<JsonValidateError> errors = new List<JsonValidateError>();

            if (value==null || value.Type != JSONValueType.Object)
            {
                errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                    "Тип значения должен быть \"object\"."));
            }
            if (errors.Count > 0) return errors;

            if (scheme.Obj.ContainsKey("required"))
                foreach (JSONValue req in scheme.Obj["required"].GetReal().Array)
                {
                    JSONValue val = req.GetReal();
                    if (!value.Obj.ContainsKey(val.Str))
                    {
                        errors.Add(new JsonValidateError(value, scheme, JVErrorType.PropertyMissing,
                            "Поле \"" + val.Str + "\" отсутствует."));
                    }
                }

            bool additional = scheme.Obj.ContainsKey("additionalProperties") &&
                ((scheme.Obj["additionalProperties"].GetReal().Type == JSONValueType.Boolean && scheme.Obj["additionalProperties"].GetReal().Boolean)
                || (scheme.Obj["additionalProperties"].GetReal().Type != JSONValueType.Boolean));
            JSONValue additionalProperties = scheme.Obj.ContainsKey("additionalProperties") ? scheme.Obj["additionalProperties"].GetReal() : null;

            foreach (KeyValuePair<string, JSONValue> prop in value.Obj)
            {
                if (scheme.Obj.ContainsKey("properties"))
                    if (scheme.Obj["properties"].GetReal().Obj.ContainsKey(prop.Key))
                    {
                        errors.AddRange(Validate(prop.Value, scheme.Obj["properties"].GetReal().Obj[prop.Key].GetReal()));
                    }
                    else
                    {
                        if (!additional)
                        {
                            errors.Add(new JsonValidateError(prop.Value, scheme, JVErrorType.PropertyMissing,
                                "Поле \"" + prop.Key + "\" отсутствует в схеме."));
                        }
                    }
                if (additional)
                {
                    if (additionalProperties.Type == JSONValueType.Object)
                    {
                        errors.AddRange(Validate(prop.Value, additionalProperties));
                    }
                }
            }

            return errors;
        }

        public static List<JsonValidateError> ValidateArray(JSONValue value, JSONValue scheme)
        {
            List<JsonValidateError> errors = new List<JsonValidateError>();

            if (value.Type != JSONValueType.Array)
            {
                errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                    "Тип значения должен быть \"array\"."));
            }
            if (errors.Count > 0) return errors;

            if (scheme.Obj.ContainsKey("items"))
                foreach (JSONValue item in value.Array)
                {
                    errors.AddRange(Validate(item, scheme.Obj["items"].GetReal()));
                }

            return errors;
        }

        public static List<JsonValidateError> ValidateString(JSONValue value, JSONValue scheme)
        {
            List<JsonValidateError> errors = new List<JsonValidateError>();

            if (value.Type != JSONValueType.String)
            {
                errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                    "Тип значения должен быть \"string\"."));
            }
            if (errors.Count > 0) return errors;

            if (scheme.Obj.ContainsKey("enum"))
            {
                bool correct = false;
                for (int i = 0; i < scheme.Obj["enum"].GetReal().Array.Length; i++)
                {
                    if (value.Str == scheme.Obj["enum"].GetReal().Array[i].Str)
                    {
                        correct = true;
                        i = scheme.Obj["enum"].GetReal().Array.Length;
                    }
                }
                if (!correct)
                    errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                        "Значение должно быть из списка: " + scheme.Obj["enum"].GetReal().Array.ToString() + "."));
            }

            if (scheme.Obj.ContainsKey("minLength"))
            {
                int minLength = (int)scheme.Obj["minLength"].GetReal().Number;
                if (value.Str.Length < minLength)
                    errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                        "Длина строки должна быть больше " + minLength + "."));
            }

            if (scheme.Obj.ContainsKey("maxLength"))
            {
                int maxLength = (int)scheme.Obj["maxLength"].GetReal().Number;
                if (value.Str.Length > maxLength)
                    errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                        "Длина строки должна быть больше " + maxLength + "."));
            }

            return errors;
        }

        public static List<JsonValidateError> ValidateInteger(JSONValue value, JSONValue scheme)
        {
            List<JsonValidateError> errors = new List<JsonValidateError>();

            if (value.Type != JSONValueType.Number)
            {
                errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                    "Тип значения должен быть \"integer\"."));
            }
            if (errors.Count > 0) return errors;

            errors.AddRange(ValidateNumber(value, scheme));

            if (value.Number%1!=0)
            {
                errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                    "Значение должно быть целым числом."));
            }

            return errors;
        }

        public static List<JsonValidateError> ValidateNumber(JSONValue value, JSONValue scheme)
        {
            List<JsonValidateError> errors = new List<JsonValidateError>();

            if (value.Type != JSONValueType.Number)
            {
                errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                    "Тип значения должен быть \"number\"."));
            }
            if (errors.Count > 0) return errors;
            
            if (scheme.Obj.ContainsKey("minimum"))
            {
                double min= scheme.Obj["minimum"].GetReal().Number;
                if (scheme.Obj.ContainsKey("exclusiveMinimum")&&scheme.Obj["exclusiveMinimum"].GetReal().Boolean)
                {
                    if (value.Number <= min)
                        errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                            "Значение должно быть больше " + min + "."));
                }
                else
                {
                    if (value.Number < min)
                        errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                            "Значение должно быть больше или равно " + min + "."));
                }
            }

            if (scheme.Obj.ContainsKey("maximum"))
            {
                double max = scheme.Obj["maximum"].GetReal().Number;
                if (scheme.Obj.ContainsKey("exclusiveMaximum") && scheme.Obj["exclusiveMaximum"].GetReal().Boolean)
                {
                    if (value.Number >= max)
                        errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                            "Значение должно быть меньше " + max + "."));
                }
                else
                {
                    if (value.Number > max)
                        errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                            "Значение должно быть меньше или равно " + max + "."));
                }
            }

            return errors;
        }

        public static List<JsonValidateError> ValidateBoolean(JSONValue value, JSONValue scheme)
        {
            List<JsonValidateError> errors = new List<JsonValidateError>();

            if (value.Type != JSONValueType.Boolean)
            {
                errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                    "Тип значения должен быть \"boolean\"."));
            }
            if (errors.Count > 0) return errors;

            return errors;
        }

        public static List<JsonValidateError> ValidateNull(JSONValue value, JSONValue scheme)
        {
            List<JsonValidateError> errors = new List<JsonValidateError>();

            if (value.Type != JSONValueType.Null)
            {
                errors.Add(new JsonValidateError(value, scheme, JVErrorType.InvalidType,
                    "Тип значения должен быть \"null\"."));
            }
            if (errors.Count > 0) return errors;

            return errors;
        }

    }
}
