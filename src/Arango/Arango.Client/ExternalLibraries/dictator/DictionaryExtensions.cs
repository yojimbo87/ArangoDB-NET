using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Arango.Client
{
    public static class DictionaryExtensions
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
        /// Includes fields from specified document into current dictionary. Existing fields will be overwritten.
        /// </summary>
        public static void Merge(this Dictionary<string, object> dictionary, Dictionary<string, object> document)
        {
            foreach (var field in document)
            {
                dictionary[field.Key] = field.Value;
            }
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
        
        #endregion
    }
}
