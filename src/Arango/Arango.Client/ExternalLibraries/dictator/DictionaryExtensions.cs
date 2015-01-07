using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Arango.Client
{
    public static partial class DictionaryExtensions
    {
        #region Field getters
        
        /// <summary>
        /// Retrieves bool type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static bool Bool(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return Convert.ToBoolean(GetFieldValue(dictionary, fieldPath));
        }
        /// <summary>
        /// Retrieves byte type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static byte Byte(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return Convert.ToByte(GetFieldValue(dictionary, fieldPath));
        }
        /// <summary>
        /// Retrieves short type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static short Short(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return Convert.ToInt16(GetFieldValue(dictionary, fieldPath));
        }
        /// <summary>
        /// Retrieves int type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static int Int(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return Convert.ToInt32(GetFieldValue(dictionary, fieldPath));
        }
        /// <summary>
        /// Retrieves long type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static long Long(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return Convert.ToInt64(GetFieldValue(dictionary, fieldPath));
        }
        /// <summary>
        /// Retrieves float type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static float Float(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return Convert.ToSingle(GetFieldValue(dictionary, fieldPath));
        }
        /// <summary>
        /// Retrieves double type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static double Double(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return Convert.ToDouble(GetFieldValue(dictionary, fieldPath));
        }
        /// <summary>
        /// Retrieves decimal type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static decimal Decimal(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return Convert.ToDecimal(GetFieldValue(dictionary, fieldPath));
        }
        /// <summary>
        /// Retrieves DateTime type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        public static DateTime DateTime(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            DateTime dateTime;
            
            if (fieldValue is DateTime)
            {
                dateTime = (DateTime)fieldValue;
            }
            else if (fieldValue is string)
            {
                dateTime = System.DateTime.Parse((string)fieldValue, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal);
            }
            else if (fieldValue is long)
            {
                dateTime = Dictator.Settings.UnixEpoch.AddSeconds((long)fieldValue);
            }
            else
            {
                dateTime = Convert.ToDateTime(GetFieldValue(dictionary, fieldPath));
            }
            
            return dateTime;
        }
        /// <summary>
        /// Retrieves string type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static string String(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return Convert.ToString(GetFieldValue(dictionary, fieldPath));
        }
        /// <summary>
        /// Retrieves object type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static object Object(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return GetFieldValue(dictionary, fieldPath);
        }
        /// <summary>
        /// Retrieves generic type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static T Object<T>(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return (T)GetFieldValue(dictionary, fieldPath);
        }
        /// <summary>
        /// Retrieves Dictionary&lt;string, object&gt; type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        /// <exception cref="InvalidFieldTypeException">Field value is not Dictionary&lt;string, object&gt; type.</exception>
        public static Dictionary<string, object> Document(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            
            if (!(fieldValue is Dictionary<string, object>))
            {
                throw new InvalidFieldTypeException(string.Format("Field path '{0}' value does not contain Dictionary<string, object> type.", fieldPath));
            }
            
            return (Dictionary<string, object>)fieldValue;
        }
        /// <summary>
        /// Retrieves enum type value from specified field path. Value is converted to enum if it is byte, sbyte, short, ushort, int, uint, long, ulong or string type. 
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        /// <exception cref="InvalidFieldTypeException">Field value can not be converted to enum type.</exception>
        public static T Enum<T>(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var type = typeof(T);
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            T fieldEnum;
            
            if (fieldValue is Enum)
            {
                fieldEnum = (T)fieldValue;
            }
            else if ((fieldValue is byte) || (fieldValue is sbyte) ||
                    (fieldValue is short) || (fieldValue is ushort) ||
                    (fieldValue is int) || (fieldValue is uint) ||
                    (fieldValue is long) || (fieldValue is ulong))
            {
                fieldEnum = (T)System.Enum.ToObject(type, fieldValue);
            }
            else if (fieldValue is string)
            {
                fieldEnum = (T)System.Enum.Parse(type, (string)fieldValue, true);
            }
            else
            {
                throw new InvalidFieldTypeException(string.Format("Field path '{0}' value does not contain Enum type.", fieldPath));
            }
            
            return fieldEnum;
        }
        /// <summary>
        /// Retrieves generic List type from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        /// <exception cref="InvalidFieldTypeException">Field value is not List&lt;T&gt; type.</exception>
        public static List<T> List<T>(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            
            if (!(fieldValue.GetType().IsGenericType && (fieldValue is IEnumerable)))
            {
                throw new InvalidFieldTypeException(string.Format("Field path '{0}' value does not contain list type.", fieldPath));
            }
            
            return ((IEnumerable)fieldValue).Cast<T>().ToList();
        }
        /// <summary>
        /// Retrieves number of list items from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        /// <exception cref="InvalidFieldTypeException">Field value is not List type.</exception>
        public static int ListSize(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            
            if (!(fieldValue.GetType().IsGenericType && (fieldValue is IList)))
            {
                throw new InvalidFieldTypeException(string.Format("Field path '{0}' value does not contain list type.", fieldPath));
            }
            
            return ((IList)fieldValue).Count;
        }
        
        #endregion
        
        #region Field setters
        
        /// <summary>
        /// Stores bool type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Bool(this Dictionary<string, object> dictionary, string fieldPath, bool fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores byte type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Byte(this Dictionary<string, object> dictionary, string fieldPath, byte fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores short type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Short(this Dictionary<string, object> dictionary, string fieldPath, short fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores int type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Int(this Dictionary<string, object> dictionary, string fieldPath, int fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores long type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Long(this Dictionary<string, object> dictionary, string fieldPath, long fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores float type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Float(this Dictionary<string, object> dictionary, string fieldPath, float fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores double type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Double(this Dictionary<string, object> dictionary, string fieldPath, double fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores decimal type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Decimal(this Dictionary<string, object> dictionary, string fieldPath, decimal fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }

        #region DateTime
        
        /// <summary>
        /// Stores DateTime type to specified field path with value in format specified in global settings DateTimeFormat.
        /// </summary>
        public static Dictionary<string, object> DateTime(this Dictionary<string, object> dictionary, string fieldPath, DateTime fieldValue)
        {
            return DateTime(dictionary, fieldPath, fieldValue, Dictator.Settings.DateTimeFormat);
        }
        /// <summary>
        /// Stores DateTime type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> DateTime(this Dictionary<string, object> dictionary, string fieldPath, DateTime fieldValue, string dateTimeStringFormat)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue.ToUniversalTime().ToString(dateTimeStringFormat, DateTimeFormatInfo.InvariantInfo));
            
            return dictionary;
        }
        /// <summary>
        /// Stores DateTime type to specified field path with value specified by DateTimeFormat.
        /// </summary>
        public static Dictionary<string, object> DateTime(this Dictionary<string, object> dictionary, string fieldPath, DateTime fieldValue, DateTimeFormat dateTimeFormat)
        {
            switch (dateTimeFormat)
            {
                case DateTimeFormat.String:
                    SetFieldValue(dictionary, fieldPath, fieldValue.ToUniversalTime().ToString(Dictator.Settings.DateTimeStringFormat, DateTimeFormatInfo.InvariantInfo));
                    break;
                case DateTimeFormat.UnixTimeStamp:
                    TimeSpan span = (fieldValue.ToUniversalTime() - Dictator.Settings.UnixEpoch);
                    SetFieldValue(dictionary, fieldPath, (long)span.TotalSeconds);
                    break;
                default:
                    SetFieldValue(dictionary, fieldPath, fieldValue);
                    break;
            }
            
            return dictionary;
        }
        
        #endregion
        
        /// <summary>
        /// Stores string type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> String(this Dictionary<string, object> dictionary, string fieldPath, string fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores object type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Object(this Dictionary<string, object> dictionary, string fieldPath, object fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores generic type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Object<T>(this Dictionary<string, object> dictionary, string fieldPath, T fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores Dictionary&lt;string, object&gt; type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Document(this Dictionary<string, object> dictionary, string fieldPath, Dictionary<string, object> fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        /// <summary>
        /// Stores enum type to specified field path with value in format specified in global settings EnumFormat.
        /// </summary>
        public static Dictionary<string, object> Enum<T>(this Dictionary<string, object> dictionary, string fieldPath, T fieldValue)
        {
            return Enum(dictionary, fieldPath, fieldValue, Dictator.Settings.EnumFormat);
        }
        /// <summary>
        /// Stores enum type to specified field path with value specified by EnumFormat.
        /// </summary>
        public static Dictionary<string, object> Enum<T>(this Dictionary<string, object> dictionary, string fieldPath, T fieldValue, EnumFormat enumFormat)
        {
            switch (enumFormat)
            {
                case EnumFormat.Integer:
                    SetFieldValue(dictionary, fieldPath, (int)Convert.ChangeType(fieldValue, Type.GetTypeCode(fieldValue.GetType())));
                    break;
                case EnumFormat.String:
                    SetFieldValue(dictionary, fieldPath, fieldValue.ToString());
                    break;
                default:
                    SetFieldValue(dictionary, fieldPath, fieldValue);
                    break;
            }
            
            return dictionary;
        }
        /// <summary>
        /// Stores generic List type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> List<T>(this Dictionary<string, object> dictionary, string fieldPath, List<T> fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
        
        #endregion
        
        #region Field checkers
        
        /// <summary>
        /// Checks if specified field is present within given path.
        /// </summary>
        public static bool Has(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);

                isValid = true;
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has null value.
        /// </summary>
        public static bool IsNull(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue == null)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has not null value.
        /// </summary>
        public static bool IsNotNull(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue != null)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has bool type value.
        /// </summary>
        public static bool IsBool(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is bool)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has byte type value.
        /// </summary>
        public static bool IsByte(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is byte)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has short type value.
        /// </summary>
        public static bool IsShort(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is short)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has int type value.
        /// </summary>
        public static bool IsInt(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is int)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has long type value.
        /// </summary>
        public static bool IsLong(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is long)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has float type value.
        /// </summary>
        public static bool IsFloat(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is float)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has double type value.
        /// </summary>
        public static bool IsDouble(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is double)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has decimal type value.
        /// </summary>
        public static bool IsDecimal(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is decimal)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has DateTime type value.
        /// </summary>
        public static bool IsDateTime(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return IsDateTime(dictionary, fieldPath, Dictator.Settings.DateTimeFormat);
        }
        /// <summary>
        /// Checks if specified field has value which can be converted to DateTime object in given format.
        /// </summary>
        public static bool IsDateTime(this Dictionary<string, object> dictionary, string fieldPath, DateTimeFormat dateTimeFormat)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                switch (dateTimeFormat)
                {
                    case DateTimeFormat.String:
                        if (fieldValue is string)
                        {
                            var dateTime = System.DateTime.Parse((string)fieldValue, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal);
                            
                            isValid = true;
                        }
                        break;
                    case DateTimeFormat.UnixTimeStamp:
                        if (fieldValue is long)
                        {
                            var dateTime = Dictator.Settings.UnixEpoch.AddSeconds((long)fieldValue);
                            
                            isValid = true;
                        }
                        break;
                    default:
                        if (fieldValue is DateTime)
                        {
                            isValid = true;
                        }
                        break;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has string type value.
        /// </summary>
        public static bool IsString(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is string)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has object type value.
        /// </summary>
        public static bool IsObject(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue != null)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has Dictionary&lt;string, object&gt; type value.
        /// </summary>
        public static bool IsDocument(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is Dictionary<string, object>)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has enum type value.
        /// </summary>
        public static bool IsEnum(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
            
                if (fieldValue.GetType().IsEnum)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has given enum type value. Apart from native enum object also int and string values 
        /// can be converted to given enum type. 
        /// </summary>
        public static bool IsEnum<T>(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                var type = typeof(T);
                T fieldEnum;
            
                if (fieldValue is Enum)
                {
                    fieldEnum = (T)fieldValue;
                    isValid = true;
                }
                else if (fieldValue is int)
                {
                    fieldEnum = (T)System.Enum.ToObject(type, (int)fieldValue);
                    isValid = true;
                }
                else if (fieldValue is string)
                {
                    fieldEnum = (T)System.Enum.Parse(type, (string)fieldValue, true);
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has List type value.
        /// </summary>
        public static bool IsList(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
            
                if (fieldValue.GetType().IsGenericType && (fieldValue is IEnumerable))
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field has given type value.
        /// </summary>
        public static bool IsType<T>(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return IsType(dictionary, fieldPath, typeof(T));
        }
        /// <summary>
        /// Checks if specified field has given type value.
        /// </summary>
        public static bool IsType(this Dictionary<string, object> dictionary, string fieldPath, Type type)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue != null)
                {
                    var fieldType = fieldValue.GetType();
                    
                    if (fieldType == type)
                    {
                        isValid = true;
                    }
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        /// <summary>
        /// Checks if specified field equals to given value. Object's Equals method is used for value comparison.
        /// </summary>
        public static bool IsEqual(this Dictionary<string, object> dictionary, string fieldPath, object compareValue)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue != null)
                {
                    if (fieldValue.Equals(compareValue))
                    {
                        isValid = true;
                    }
                }
                else if (fieldValue == null && compareValue == null)
                {
                    isValid = true;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            
            return isValid;
        }
        
        #endregion

        /// <summary>
        /// Creates a deep clone of current dictionary.
        /// </summary>
        public static Dictionary<string, object> Clone(this Dictionary<string, object> dictionary)
        {
            var clone = new Dictionary<string, object>();
            
            foreach (var field in dictionary)
            {
                var fieldType = field.Value.GetType();
                
                if (fieldType.IsValueType || fieldType.IsEnum || fieldType.Equals(typeof(System.String)))
                {
                    clone.Add(field.Key, field.Value);
                }
                else if (fieldType.Equals(typeof(Dictionary<string, object>)))
                {
                    clone.Add(field.Key, Clone((Dictionary<string, object>)field.Value));
                }
                else
                {
                    clone.Add(field.Key, Activator.CreateInstance(fieldType, new object[] { field.Value }));
                }
            }
            
            return clone;
        }
        /// <summary>
        /// Creates a deep clone of current dictionary without specified fields. Nonexisting fields are ingored.
        /// </summary>
        public static Dictionary<string, object> CloneExcept(this Dictionary<string, object> dictionary, params string[] fieldPaths)
        {
            var clone = Clone(dictionary);
            
            foreach (var fieldPath in fieldPaths)
            {
                clone.Drop(fieldPath);
            }
            
            return clone;
        }
        /// <summary>
        /// Creates a deep clone of current dictionary including only specified fields. Nonexisting fields are ingored.
        /// </summary>
        public static Dictionary<string, object> CloneOnly(this Dictionary<string, object> dictionary, params string[] fieldPaths)
        {
            var clone = new Dictionary<string, object>();
            
            foreach (var fieldPath in fieldPaths)
            {
                try
                {
                    var fieldValue = GetFieldValue(dictionary, fieldPath);
                    var fieldType = fieldValue.GetType();
                    
                    if (fieldType.IsValueType || fieldType.IsEnum || fieldType.Equals(typeof(System.String)))
                    {
                        clone.Object(fieldPath, fieldValue);
                    }
                    else if (fieldType.Equals(typeof(Dictionary<string, object>)))
                    {
                        clone.Object(fieldPath, Clone((Dictionary<string, object>)fieldValue));
                    }
                    else
                    {
                        clone.Object(fieldPath, Activator.CreateInstance(fieldType, new object[] { fieldValue }));
                    }
                }
                catch (Exception)
                {
                }
            }
            
            return clone;
        }
        /// <summary>
        /// Drops specified fields. Nonexisting fields are ignored.
        /// </summary>
        public static Dictionary<string, object> Drop(this Dictionary<string, object> dictionary, params string[] fieldPaths)
        {
            foreach (var fieldPath in fieldPaths)
            {
                var fieldNames = new [] { fieldPath };
                var parentDictionary = dictionary;                
                
                // split field path to separate field name elements if necessary
                if (fieldPath.Contains("."))
                {
                    fieldNames = fieldPath.Split('.');
                }
                
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    var fieldName = fieldNames[i];
    
                    // field is not present in dictionary - next field path iteration
                    if (!parentDictionary.ContainsKey(fieldName))
                    {
                        break;
                    }
    
                    // current field name is final - drop field
                    if (i == (fieldNames.Length - 1))
                    {                       
                        parentDictionary.Remove(fieldName);
                        
                        break;
                    }
                    
                    // descendant field is dictionary - set is as current parent dictionary
                    if (parentDictionary[fieldName] is Dictionary<string, object>)
                    {
                        parentDictionary = (Dictionary<string, object>)parentDictionary[fieldName];
                    }
                    // can not continue with processing - next field path iteration
                    else
                    {
                        break;
                    }
                } 
            }
            
            return dictionary;
        }
        /// <summary>
        /// Merges fields from specified document into current dictionary. Field merge behavior depends on settings.
        /// </summary>
        public static Dictionary<string, object> Merge(this Dictionary<string, object> dictionary, Dictionary<string, object> document)
        {
            dictionary.Merge(document, Dictator.Settings.MergeBehavior);
            
            return dictionary;
        }
        /// <summary>
        /// Merges fields from specified document into current dictionary with given field merge behavior.
        /// </summary>
        public static Dictionary<string, object> Merge(this Dictionary<string, object> dictionary, Dictionary<string, object> document, MergeBehavior mergeBehavior)
        {
            foreach (var field in document)
            {
                switch (mergeBehavior)
                {
                    case MergeBehavior.KeepFields:
                        if (!dictionary.ContainsKey(field.Key))
                        {
                            dictionary.Add(field.Key, field.Value);
                        }
                        break;
                    case MergeBehavior.OverwriteFields:
                    default:
                        dictionary[field.Key] = field.Value;
                        break;
                }
            }
            
            return dictionary;
        }
        /// <summary>
        /// Converts current dictionary into strongly typed object.
        /// </summary>
        public static T ToObject<T>(this Dictionary<string, object> dictionary)
        {
            return (T)ConvertToObject(dictionary, typeof(T));
        }
        
        #region Private methods
        
        /// <summary>
        /// Retrieves value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        static object GetFieldValue(Dictionary<string, object> dictionary, string fieldPath)
        {
            object fieldValue = null;
            var fieldNames = new [] { fieldPath };
            var parentDictionary = dictionary;                
            
            // split field path to separate field name elements if necessary
            if (fieldPath.Contains("."))
            {
                fieldNames = fieldPath.Split('.');
            }
            
            for (int i = 0; i < fieldNames.Length; i++)
            {
                var fieldName = fieldNames[i];

                // throw exception if the field is not present in dictionary
                if (!parentDictionary.ContainsKey(fieldName))
                {
                    throw new NonExistingFieldException(string.Format("Field path '{0}' does not contain field '{1}'.", fieldPath, fieldName));
                }

                // current field name is final - retrieve field value and break loop
                if (i == (fieldNames.Length - 1))
                {        			
                    fieldValue = parentDictionary[fieldName];
                    
                    break;
                }
                
                // descendant field is dictionary - set is as current parent dictionary
                if (parentDictionary[fieldName] is Dictionary<string, object>)
                {
                    parentDictionary = (Dictionary<string, object>)parentDictionary[fieldName];
                }
                // can not continue with processing - throw exception
                else
                {
                    throw new InvalidFieldException(string.Format("Field path '{0}' contains field '{1}' which is not dictionary.", fieldPath, fieldName));
                }
            }
            
            return fieldValue;
        }
        /// <summary>
        /// Stores given value to specified field path.
        /// </summary>
        static void SetFieldValue(Dictionary<string, object> dictionary, string fieldPath, object fieldValue)
        {
            var fieldNames = new [] { fieldPath };
            var parentDictionary = dictionary;
            
            // split field path to separate field name elements if necessary
            if (fieldPath.Contains("."))
            {
                fieldNames = fieldPath.Split('.');
            }
            
            for (int i = 0; i < fieldNames.Length; i++)
            {
                var fieldName = fieldNames[i];
                
                // current field name is final - set field value and break loop
                if (i == (fieldNames.Length - 1))
                {
                    parentDictionary[fieldName] = fieldValue;
                    
                    break;
                }
                
                // descendant field is dictionary - set is as current parent dictionary
                if (parentDictionary.ContainsKey(fieldName) && (parentDictionary[fieldName] is Dictionary<string, object>))
                {
                    parentDictionary = (Dictionary<string, object>)parentDictionary[fieldName];
                }
                // descendant field does not exist or isn't dictioanry - field needs to be set as dictionary
                else
                {
                    var newDictionary = Dictator.New();
                    parentDictionary[fieldName] = newDictionary;
                    parentDictionary = newDictionary;
                }
            }
        }
        /// <summary>
        /// Converts specified dictionary into strongly typed object.
        /// </summary>
        static object ConvertToObject(Dictionary<string, object> dictionary, Type objectType)
        {
            var stronglyTypedObject = Activator.CreateInstance(objectType);
            
            if (objectType == typeof(Dictionary<string, object>))
            {
                foreach (var item in dictionary)
                {
                    (stronglyTypedObject as Dictionary<string, object>).Object(item.Key, item.Value);
                }
            }
            else
            {
                foreach (var propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    // skip property if it should be ignored
                    if (propertyInfo.IsDefined(typeof(IgnoreField)))
                    {
                        continue;
                    }
                    
                    var fieldName = propertyInfo.Name;
                    object fieldValue = null;
                    Type fieldType = null;
                    
                    // set field name to property alias if present
                    if (propertyInfo.IsDefined(typeof(AliasField)))
                    {
                        var aliasFieldAttribute = (AliasField)propertyInfo.GetCustomAttribute(typeof(AliasField));
                        
                        fieldName = aliasFieldAttribute.Alias;
                    }
                    
                    if (dictionary.Has(fieldName))
                    {
                        fieldValue = GetFieldValue(dictionary, fieldName);
                        
                        if (fieldValue != null)
                        {
                            fieldType = fieldValue.GetType();
                        }
                        else
                        {
                            // skip property if it should ingore null value
                            if (propertyInfo.IsDefined(typeof(IgnoreNullValue)))
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                    
                    if (propertyInfo.PropertyType == typeof(Dictionary<string, object>))
                    {
                        propertyInfo.SetValue(stronglyTypedObject, ConvertToObject((Dictionary<string, object>)fieldValue, propertyInfo.PropertyType), null);
                    }
                    // property is a collection
                    else if ((propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType) && (fieldValue is IList))
                    {
                        var instance = Activator.CreateInstance(propertyInfo.PropertyType);
                            
                        propertyInfo.SetValue(
                            stronglyTypedObject,
                            ConvertToCollection(instance, (IList)fieldValue, propertyInfo.PropertyType),
                            null
                        );
                    }
                    // property is class except the string type since string values are parsed differently
                    else if (propertyInfo.PropertyType.IsClass && (propertyInfo.PropertyType.Name != "String"))
                    {
                        if (fieldType == typeof(Dictionary<string, object>))
                        {
                            var instance = Activator.CreateInstance(propertyInfo.PropertyType);
                            
                            propertyInfo.SetValue(stronglyTypedObject, ConvertToObject((Dictionary<string, object>)fieldValue, propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(stronglyTypedObject, fieldValue, null);
                        }
                    }
                    // property is basic type
                    else
                    {
                        if ((fieldValue == null) || (propertyInfo.PropertyType == fieldType))
                        {
                            propertyInfo.SetValue(stronglyTypedObject, fieldValue, null);
                        } 
                        else
                        {
                            propertyInfo.SetValue(stronglyTypedObject, Convert.ChangeType(fieldValue, propertyInfo.PropertyType), null);
                        }
                    }
                }
            }
            
            return stronglyTypedObject;
        }
        /// <summary>
        /// Converts specified object into collection of items of specified type.
        /// </summary>
        static object ConvertToCollection(object collectionObject, IList collection, Type collectionType)
        {
            if (collection == null)
            {
                return null;
            }
            
            if (collection.Count > 0)
            {
                // create instance of property type
                var collectionInstance = Activator.CreateInstance(collectionType, collection.Count);

                for (int i = 0; i < collection.Count; i++)
                {
                    var elementType = collection[i].GetType();
                    
                    // collection is simple array
                    if (collectionType.IsArray)
                    {
                        ((IList)collectionObject).Add(collection[i]);
                    }
                    // collection is generic
                    else if (collectionType.IsGenericType && (collection is IEnumerable))
                    {
                        // generic collection consists of basic types
                        if (elementType.IsPrimitive ||
                            (elementType == typeof(string)) ||
                            (elementType == typeof(DateTime)) ||
                            (elementType == typeof(decimal)))
                        {
                            ((IList)collectionObject).Add(collection[i]);
                        }
                        // generic collection consists of generic type which should be parsed
                        else
                        {
                            // create instance object based on first element of generic collection
                            var instance = Activator.CreateInstance(collectionType.GetGenericArguments().First(), null);
                            var instanceType = instance.GetType();
                            
                            if (elementType == typeof(Dictionary<string, object>))
                            {
                                ((IList)collectionObject).Add(ConvertToObject((Dictionary<string, object>)collection[i], instanceType));
                            }
                            else
                            {
                                if (elementType == instanceType)
                                {
                                    ((IList)collectionObject).Add(collection[i]);
                                } 
                                else
                                {
                                    ((IList)collectionObject).Add(Convert.ChangeType(collection[i], collectionType));
                                }
                            }
                        }
                    }
                    else
                    {
                        var obj = Activator.CreateInstance(elementType, collection[i]);

                        ((IList)collectionObject).Add(obj);
                    }
                }
            }
            
            return collectionObject;
        }
        
        #endregion
    }
}
