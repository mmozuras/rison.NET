namespace Rison
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    public class RisonEncoder : IRisonEncoder
    {
        private StringBuilder output;

        public string Encode<T>(T obj)
        {
            output = new StringBuilder();

            WriteValue(obj);

            return output.ToString();
        }

        private void WriteValue(object obj)
        {
            if (obj == null)
            {
                WriteNull();
            }
            else if (obj is char)
            {
                WriteString(((char) obj).ToString());
            }
            else if (obj is string)
            {
                WriteString((string) obj);
            }
            else if (obj is bool)
            {
                WriteBool((bool) obj);
            }
            else if (obj.IsNumber())
            {
                WriteNumber(obj);
            }
            else if (obj is DateTime)
            {
                WriteDateTime((DateTime) obj);
            }
            else if (obj is Guid || obj is Enum)
            {
                WriteString(obj.ToString());
            }
            else if (obj is IDictionary)
            {
                WriteDictionary((IDictionary) obj);
            }
            else if (obj is IEnumerable)
            {
                WriteEnumerable((IEnumerable) obj);
            }
            else
            {
                WriteObject(obj);
            }
        }

        private void WriteNull()
        {
            output.Append("!n");
        }

        private void WriteBool(bool obj)
        {
            output.Append(obj ? "!t" : "!f");
        }

        private void WriteNumber(object obj)
        {
            var numberAsString = ((IConvertible) obj).ToString(NumberFormatInfo.InvariantInfo);
            output.Append(numberAsString.ToLower().Replace("+", ""));
        }

        private void WriteDateTime(DateTime dateTime)
        {
            var s = dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
            WriteString(s);
        }

        private void WritePair(string name, object value)
        {
            output.Append(name);
            output.Append(':');
            WriteValue(value);
        }

        private void WriteString(string s)
        {
            output.Append('\"');
            output.Append(s);
            output.Append('\"');
        }

        private void WriteEnumerable(IEnumerable enumerable)
        {
            output.Append("!(");

            var pendingSeperator = false;

            foreach (var obj in enumerable)
            {
                if (pendingSeperator) output.Append(',');

                WriteValue(obj);

                pendingSeperator = true;
            }
            output.Append(')');
        }

        private void WriteDictionary(IDictionary dictionary)
        {
            output.Append("!(");

            var pendingSeparator = false;

            foreach (DictionaryEntry entry in dictionary)
            {
                if (pendingSeparator) output.Append(',');
                output.Append('(');
                WritePair("k", entry.Key);
                output.Append(",");
                WritePair("v", entry.Value);
                output.Append(')');

                pendingSeparator = true;
            }
            output.Append(')');
        }

        private void WriteObject(object obj)
        {
            output.Append('(');

            var pendingSeperator = false;

            var getters = GetGetters(obj.GetType());

            foreach (var getter in getters)
            {
                if (pendingSeperator) output.Append(',');
                var o = getter.Value(obj);
                WritePair(getter.Key, o);

                pendingSeperator = true;
            }
            output.Append(')');
        }

        private static IEnumerable<KeyValuePair<string, Func<object, object>>> GetGetters(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var getters = (from property in properties
                           where property.CanWrite
                           let getMethod = CreateGetMethod(property)
                           where getMethod != null
                           select new KeyValuePair<string, Func<object, object>>(property.Name, getMethod)).ToList();

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            getters.AddRange(from field in fields
                             let getField = CreateGetField(type, field)
                             where getField != null
                             select new KeyValuePair<string, Func<object, object>>(field.Name, getField));

            return getters;
        }

        private static Func<object, object> CreateGetMethod(PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
            {
                return null;
            }

            var arguments = new Type[1];
            arguments[0] = typeof(object);

            var getter = new DynamicMethod("_", typeof(object), arguments);
            var il = getter.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
            il.EmitCall(OpCodes.Callvirt, getMethod, null);

            if (!propertyInfo.PropertyType.IsClass)
            {
                il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (Func<object, object>)getter.CreateDelegate(typeof(Func<object, object>));
        }

        private static Func<object, object> CreateGetField(Type type, FieldInfo fieldInfo)
        {
            var dynamicGet = new DynamicMethod("_", typeof(object), new[] { typeof(object) }, type, true);
            var il = dynamicGet.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldInfo);
            if (fieldInfo.FieldType.IsValueType)
            {
                il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)dynamicGet.CreateDelegate(typeof(Func<object, object>));
        }
    }
}