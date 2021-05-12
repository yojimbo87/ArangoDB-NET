using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Arango.Client.ExternalLibraries.dictator
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
        /// Retrieves Guid type value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        /// /// <exception cref="InvalidFieldTypeException">Field value can not be converted to Guid type.</exception>
        public static Guid Guid(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            
            if (!(fieldValue is Guid))
            {
                throw new InvalidFieldTypeException(string.Format("Field path '{0}' value does not contain Guid type.", fieldPath));
            }
            
            return (Guid)fieldValue;
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
            var type = typeof(T);
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            
            if (!(fieldValue.GetType().IsGenericType && (fieldValue is IEnumerable)))
            {
                throw new InvalidFieldTypeException(string.Format("Field path '{0}' value does not contain list type.", fieldPath));
            }
            
            var collection = (IList)fieldValue;
            var collectionElementType = collection.GetType().GetGenericArguments()[0];
            
            if (collectionElementType == type)
            {
                return collection.Cast<T>().ToList();
            }
            // when retrieved type is different from stored one its items needs to be converted to desired type
            else
            {
                var returnCollection = new List<T>();
                
                for (int i = 0; i < collection.Count; i++)
                {
                    returnCollection.Add((T)Convert.ChangeType(collection[i], type));
                }
                
                return returnCollection;
            }
        }
        /// <summary>
        /// Retrieves generic array type from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        /// <exception cref="InvalidFieldTypeException">Field value is not T[] type.</exception>
        public static T[] Array<T>(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var type = typeof(T);
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            
            if (!(fieldValue is IEnumerable))
            {
                throw new InvalidFieldTypeException(string.Format("Field path '{0}' value does not contain array type.", fieldPath));
            }
            
            var collection = (IList)fieldValue;
            var collectionElementType = collection.GetType().GetElementType();
            
            if (collectionElementType == type)
            {
                return collection.Cast<T>().ToArray();
            }
            // when retrieved type is different from stored one its items needs to be converted to desired type
            else
            {
                var returnCollection = Activator.CreateInstance(typeof(T[]), collection.Count);
                
                for (int i = 0; i < collection.Count; i++)
                {
                    ((T[])returnCollection)[i] = (T)Convert.ChangeType(collection[i], type);
                }
                
                return (T[])returnCollection;
            }
        }
        /// <summary>
        /// Retrieves number of items contained in specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        /// <exception cref="InvalidFieldTypeException">Field value does not contain type which can retrieve items count.</exception>
        public static int Size(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            
            if (!(fieldValue is IList))
            {
                throw new InvalidFieldTypeException(string.Format("Field path '{0}' value does not contain type which can retrieve items count.", fieldPath));
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
        /// Stores Guid type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Guid(this Dictionary<string, object> dictionary, string fieldPath, Guid fieldValue)
        {
            SetFieldValue(dictionary, fieldPath, fieldValue);
            
            return dictionary;
        }
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
        /// <summary>
        /// Stores generic array type value to specified field path.
        /// </summary>
        public static Dictionary<string, object> Array<T>(this Dictionary<string, object> dictionary, string fieldPath, T[] fieldValue)
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
            return HasField(dictionary, fieldPath); 
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
        /// Checks if specified field has Guid type value.
        /// </summary>
        public static bool IsGuid(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue is Guid)
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
        /// Checks if specified field has array type value.
        /// </summary>
        public static bool IsArray(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
            
                if (fieldValue.GetType().IsArray)
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
        /// Checks if specified field has given type value.
        /// </summary>
        public static bool IsType<T>(this Dictionary<string, object> dictionary, string fieldPath)
        {
            return IsType(dictionary, fieldPath, typeof(T));
        }
        /// <summary>
        /// Checks if specified field has integer value. Applicable integer types are byte, sbyte, short, ushort, int, uint, long and ulong.
        /// </summary>
        public static bool IsInteger(this Dictionary<string, object> dictionary, string fieldPath)
        {
            var isValid = false;
            
            try
            {
                var fieldValue = GetFieldValue(dictionary, fieldPath);
                
                if (fieldValue != null)
                {
                    if ((fieldValue is byte) ||
                        (fieldValue is sbyte) ||
                        (fieldValue is short) ||
                        (fieldValue is ushort) ||
                        (fieldValue is int) ||
                        (fieldValue is uint) ||
                        (fieldValue is long) ||
                        (fieldValue is ulong))
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
                
                if ((fieldValue == null) && (compareValue == null))
                {
                    isValid = true;
                }
                else if ((fieldValue != null) && (compareValue != null))
                {
                    if ((fieldValue.GetType() == compareValue.GetType()) && fieldValue.Equals(compareValue))
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
        
        #endregion

        /// <summary>
        /// Iterates over documents from specified field path and performs given action.
        /// </summary>
        public static Dictionary<string, object> Each(this Dictionary<string, object> dictionary, string fieldPath, Action<int, Dictionary<string, object>> action)
        {
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            
            if (fieldValue is IList)
            {
                for (int i = 0; i < ((IList)fieldValue).Count; i++)
                {
                    if (((IList)fieldValue)[i] is Dictionary<string, object>)
                    {
                        action(i, (Dictionary<string, object>)((IList)fieldValue)[i]);
                    }
                } 
            }
            
            return dictionary;
        }
        
        /// <summary>
        /// Iterates over items from specified field path and performs given action.
        /// </summary>
        public static Dictionary<string, object> Each<T>(this Dictionary<string, object> dictionary, string fieldPath, Action<int, T> action)
        {
            var fieldValue = GetFieldValue(dictionary, fieldPath);
            
            if (fieldValue is IList)
            {
                for (int i = 0; i < ((IList)fieldValue).Count; i++)
                {
                    action(i, (T)((IList)fieldValue)[i]);
                } 
            }
            
            return dictionary;
        }
        
        /// <summary>
        /// Creates a deep clone of current dictionary.
        /// </summary>
        public static Dictionary<string, object> Clone(this Dictionary<string, object> dictionary)
        {
            var clone = new Dictionary<string, object>();
            
            foreach (var field in dictionary)
            {
                // null value is a special case so it should be processed first
                if (field.Value == null)
                {
                    clone.Add(field.Key, null);
                    
                    continue;
                }
                
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
                    var arrayContent = "";

                    if (fieldName.Contains("[") && fieldName.Contains("]"))
                    {
                        var firstIndex = fieldName.IndexOf('[');
                        var lastIndex = fieldName.IndexOf(']');
                        
                        arrayContent = fieldName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                        fieldName = fieldName.Substring(0, firstIndex);
                    }
    
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
                    
                    var tempParentObject = GetFieldObject(fieldName, arrayContent, parentDictionary);
                
                    // descendant field is dictionary - set is as current parent dictionary
                    if (tempParentObject is Dictionary<string, object>)
                    {
                        parentDictionary = (Dictionary<string, object>)tempParentObject;
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
        ///  Checks if specified field is present in dictionary.
        /// </summary>
        // TODO: can this be refactored with GetFieldValue method?
        static bool HasField(Dictionary<string, object> dictionary, string fieldPath)
        {
            object fieldValue = null;
            var fieldNames = new[] { fieldPath };
            var parentDictionary = dictionary;

            // split field path to separate field name elements if necessary
            if (fieldPath.Contains("."))
            {
                fieldNames = fieldPath.Split('.');
            }

            for (int i = 0; i < fieldNames.Length; i++)
            {
                var fieldName = fieldNames[i];
                var arrayContent = "";

                if (fieldName.Contains("[") && fieldName.Contains("]"))
                {
                    var firstIndex = fieldName.IndexOf('[');
                    var lastIndex = fieldName.IndexOf(']');

                    arrayContent = fieldName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                    fieldName = fieldName.Substring(0, firstIndex);
                }

                // field is not present in dictionary
                if (!parentDictionary.ContainsKey(fieldName))
                {
                    return false;
                }

                // current field name is final - retrieve field value and break loop
                if (i == (fieldNames.Length - 1))
                {
                    fieldValue = GetFieldObject(fieldName, arrayContent, parentDictionary);

                    break;
                }

                var tempParentObject = GetFieldObject(fieldName, arrayContent, parentDictionary);

                // descendant field is dictionary - set is as current parent dictionary
                if (tempParentObject is Dictionary<string, object>)
                {
                    parentDictionary = (Dictionary<string, object>)tempParentObject;
                }
                // can not continue with processing - field not present
                else
                {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Retrieves value from specified field path.
        /// </summary>
        /// <exception cref="NonExistingFieldException">Field does not exist in specified path.</exception>
        /// <exception cref="InvalidFieldException">Field path contains field which is not traversable.</exception>
        public static object GetFieldValue(Dictionary<string, object> dictionary, string fieldPath)
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
                var arrayContent = "";

                if (fieldName.Contains("[") && fieldName.Contains("]"))
                {
                    var firstIndex = fieldName.IndexOf('[');
                    var lastIndex = fieldName.IndexOf(']');
                    
                    arrayContent = fieldName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                    fieldName = fieldName.Substring(0, firstIndex);
                }
                
                // throw exception if the field is not present in dictionary
                if (!parentDictionary.ContainsKey(fieldName))
                {
                    throw new NonExistingFieldException(string.Format("Field path '{0}' does not contain field '{1}'.", fieldPath, fieldName));
                }

                // current field name is final - retrieve field value and break loop
                if (i == (fieldNames.Length - 1))
                {
                    fieldValue = GetFieldObject(fieldName, arrayContent, parentDictionary);
                    
                    break;
                }
                
                var tempParentObject = GetFieldObject(fieldName, arrayContent, parentDictionary);
                
                 // descendant field is dictionary - set is as current parent dictionary
                if (tempParentObject is Dictionary<string, object>)
                {
                    parentDictionary = (Dictionary<string, object>)tempParentObject;
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
        /// Retrieves object from specified field depending on array content.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Index in field specified field is out of range.</exception>
        static object GetFieldObject(string fieldName, string arrayContent, Dictionary<string, object> fieldDocument)
        {
            if (!string.IsNullOrEmpty(arrayContent))
            {
                int indexNumber;
                
                if (int.TryParse(arrayContent, out indexNumber))
                {
                    var collection = ((IList)fieldDocument[fieldName]);
                    
                    if ((indexNumber >= 0) && ((indexNumber + 1) <= collection.Count))
                    {
                        return collection[indexNumber];
                    }
                    
                    throw new IndexOutOfRangeException("Index in field '" + fieldName + "' is out of range.");
                }
            }
            
            return fieldDocument[fieldName];
        }
        /// <summary>
        /// Stores given value to specified field path.
        /// </summary>
        public static void SetFieldValue(Dictionary<string, object> dictionary, string fieldPath, object fieldValue)
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
                var arrayContent = "";

                if (fieldName.Contains("[") && fieldName.Contains("]"))
                {
                    var firstIndex = fieldName.IndexOf('[');
                    var lastIndex = fieldName.IndexOf(']');
                    
                    arrayContent = fieldName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                    fieldName = fieldName.Substring(0, firstIndex);
                }
                
                // current field name is final - set field value and break loop
                if (i == (fieldNames.Length - 1))
                {
                    SetFieldObject(fieldName, arrayContent, parentDictionary, fieldValue);
                    
                    break;
                }
                
                if (parentDictionary.ContainsKey(fieldName))
                {
                    var tempParentObject = GetFieldObject(fieldName, arrayContent, parentDictionary);
                    
                    // descendant field is not dictionary - dictionary field needs to be created
                    if (!(tempParentObject is Dictionary<string, object>))
                    {
                        var newDictionary = new Dictionary<string, object>();
                        parentDictionary[fieldName] = newDictionary;
                        parentDictionary = newDictionary;
                    }
                    else
                    {
                        parentDictionary = (Dictionary<string, object>)tempParentObject;
                    }
                }
                // descendant field does not exist - dictionary field needs to be created
                else
                {
                    var newDictionary = new Dictionary<string, object>();
                    parentDictionary[fieldName] = newDictionary;
                    parentDictionary = newDictionary;
                }
            }
        }
        /// <summary>
        /// Stores given value to specified field depending on array content.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Index in field specified field is out of range.</exception>
        static void SetFieldObject(string fieldName, string arrayContent, Dictionary<string, object> fieldDocument, object fieldValue)
        {
            if (!string.IsNullOrEmpty(arrayContent))
            {
                var collection = ((IList)fieldDocument[fieldName]);
                
                // append value to collection
                if (arrayContent == "*")
                {
                    collection.Add(fieldValue);
                }
                // insert value at specified index
                else
                {
                    int indexNumber;
                    
                    if (int.TryParse(arrayContent, out indexNumber))
                    {
                        if ((indexNumber >= 0) && ((indexNumber + 1) <= collection.Count))
                        {
                            collection[indexNumber] = fieldValue;
                        }
                        else
                        {
                            throw new IndexOutOfRangeException("Index in field '" + fieldName + "' is out of range.");
                        }
                    }
                }
            }
            // no array content
            else
            {
                fieldDocument[fieldName] = fieldValue;
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
                        propertyInfo.SetValue(
                            stronglyTypedObject,
                            ConvertToCollection((IList)fieldValue, propertyInfo.PropertyType),
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
                    // property is Enum type
                    else if (propertyInfo.PropertyType.IsEnum)
                    {
                        // field value in document is stored as enum object
                        if (fieldValue is Enum)
                        {
                            propertyInfo.SetValue(stronglyTypedObject, fieldValue, null);
                        }
                        // field value in document is stored as integer
                        else if ((fieldValue is byte) || (fieldValue is sbyte) ||
                                (fieldValue is short) || (fieldValue is ushort) ||
                                (fieldValue is int) || (fieldValue is uint) ||
                                (fieldValue is long) || (fieldValue is ulong))
                        {
                            propertyInfo.SetValue(stronglyTypedObject, System.Enum.ToObject(propertyInfo.PropertyType, fieldValue), null);
                        }
                        // field value in document is stored as string
                        else if (fieldValue is string)
                        {
                            propertyInfo.SetValue(stronglyTypedObject, System.Enum.Parse(propertyInfo.PropertyType, (string)fieldValue, true), null);
                        }
                    }
                    // property is Guid type
                    else if (propertyInfo.PropertyType == typeof(Guid))
                    {
                        if (fieldValue is string)
                        {
                            propertyInfo.SetValue(stronglyTypedObject, new Guid((string)fieldValue), null);
                        }
                        else if (fieldValue is Guid)
                        {
                            propertyInfo.SetValue(stronglyTypedObject, fieldValue, null);
                        }
                    }
                    // property is nullable type
                    else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];

                        if ((fieldValue == null) || (propertyType == fieldType))
                        {
                            propertyInfo.SetValue(stronglyTypedObject, fieldValue, null);
                        }
                        else
                        {
                            propertyInfo.SetValue(stronglyTypedObject, Convert.ChangeType(fieldValue, propertyType), null);
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
        static object ConvertToCollection(IList collection, Type collectionType)
        {
            if (collection == null)
            {
                return null;
            }

            // create instance of property type
            var collectionInstance = Activator.CreateInstance(collectionType, collection.Count);
            
            // get type of items within collection
            Type collectionElementType;
            
            if (collectionType.IsArray)
            {
                collectionElementType = collectionType.GetElementType();
            }
            else
            {
                collectionElementType = collectionType.GetGenericArguments()[0];
            }
            
            if (collection.Count > 0)
            {
                // TODO: move type inference and processing out of the loop for performance reasons
                
                for (int i = 0; i < collection.Count; i++)
                {
                    var elementType = collection[i].GetType();
                    
                    // collection is simple array
                    if (collectionType.IsArray)
                    {
                        if (collectionElementType == elementType)
                        {
                            ((IList)collectionInstance)[i] = collection[i];
                        }
                        else
                        {
                            ((IList)collectionInstance)[i] = Convert.ChangeType(collection[i], collectionElementType);
                        }
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
                            if (collectionElementType == elementType)
                            {
                                ((IList)collectionInstance).Add(collection[i]);
                            }
                            else
                            {
                                ((IList)collectionInstance).Add(Convert.ChangeType(collection[i], collectionElementType));
                            }
                        }
                        // generic collection consists of generic type which should be parsed
                        else
                        {
                            if ((collectionType.GetGenericTypeDefinition() == typeof(SortedList<,>)) &&
                                (elementType == typeof(Dictionary<string, object>)))
                            {
                                var element = (Dictionary<string, object>)collection[i];
                                var addMethod = collectionType.GetMethod("Add");
                                
                                addMethod.Invoke(
                                    collectionInstance, 
                                    new [] { 
                                        Convert.ChangeType(element["k"], collectionType.GetGenericArguments()[0]),
                                        Convert.ChangeType(element["v"], collectionType.GetGenericArguments()[1])
                                    }
                                );
                            }
                            else if (elementType == typeof(Dictionary<string, object>))
                            {
                                ((IList)collectionInstance).Add(ConvertToObject((Dictionary<string, object>)collection[i], collectionType.GetGenericArguments()[0]));
                            }
                            else
                            {
                                if (elementType == collectionType.GetGenericArguments()[0])
                                {
                                    ((IList)collectionInstance).Add(collection[i]);
                                } 
                                else
                                {
                                    ((IList)collectionInstance).Add(Convert.ChangeType(collection[i], collectionType));
                                }
                            }
                        }
                    }
                    else
                    {
                        var obj = Activator.CreateInstance(elementType, collection[i]);

                        ((IList)collectionInstance).Add(obj);
                    }
                }
            }
            
            return collectionInstance;
        }
        
        #endregion
    }
}
