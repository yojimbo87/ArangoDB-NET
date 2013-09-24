using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arango.Client
{
    public class Document : Dictionary<string, object>
    {
        public static DocumentSettings Settings = new DocumentSettings();
        
        public Document() {  }
        
        public Document(string json)
        {
            foreach(KeyValuePair<string, object> field in DeserializeDocument(json))
            {
                this.Add(field.Key, field.Value);
            }
        }
        
        #region Field getters
        
        public bool Bool(string fieldPath)
        {
            var fieldValue = GetField(fieldPath);
            
            if (fieldValue == null)
            {
                throw new Exception("Value is null.");
            }
            else
            {
                return Convert.ToBoolean(fieldValue);
            }
        }
        
        public byte Byte(string fieldPath)
        {
            var fieldValue = GetField(fieldPath);
            
            if (fieldValue == null)
            {
                throw new Exception("Value is null.");
            }
            else
            {
                return Convert.ToByte(fieldValue);
            }
        }
        
        public short Short(string fieldPath)
        {
            var fieldValue = GetField(fieldPath);
            
            if (fieldValue == null)
            {
                throw new Exception("Value is null.");
            }
            else
            {
                return Convert.ToInt16(fieldValue);
            }
        }
        
        public int Int(string fieldPath)
        {
            var fieldValue = GetField(fieldPath);
            
            if (fieldValue == null)
            {
                throw new Exception("Value is null.");
            }
            else
            {
                return Convert.ToInt32(fieldValue);
            }
        }
        
        public long Long(string fieldPath)
        {
            var fieldValue = GetField(fieldPath);
            
            if (fieldValue == null)
            {
                throw new Exception("Value is null.");
            }
            else
            {
                return Convert.ToInt64(fieldValue);
            }
        }
        
        public float Float(string fieldPath)
        {
            var fieldValue = GetField(fieldPath);
            
            if (fieldValue == null)
            {
                throw new Exception("Value is null.");
            }
            else
            {
                return Convert.ToSingle(fieldValue);
            }
        }
        
        public double Double(string fieldPath)
        {
            var fieldValue = GetField(fieldPath);
            
            if (fieldValue == null)
            {
                throw new Exception("Value is null.");
            }
            else
            {
                return Convert.ToDouble(fieldValue);
            }
        }
        
        public decimal Decimal(string fieldPath)
        {
            var fieldValue = GetField(fieldPath);
            
            if (fieldValue == null)
            {
                throw new Exception("Value is null.");
            }
            else
            {
                return Convert.ToDecimal(fieldValue);
            }
        }
        
        public string String(string fieldPath)
        {
            return (string)GetField(fieldPath);
        }
        
        public DateTime DateTime(string fieldPath)
        {
            var value = GetField(fieldPath);
            
            if (value is string)
            {
                return System.DateTime.Parse((string)value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal);
            }
            else if (value is long)
            {
                return DocumentSettings.UnixEpoch.AddSeconds((long)value);
            }
            else
            {
                return (DateTime)value;
            }
        }
        
        public object Object(string fieldPath)
        {
            return (object)GetField(fieldPath);
        }
        
        public T Object<T>(string fieldPath)
        {
            return (T)GetField(fieldPath);
        }
        
        public Document Docunet(string fieldPath)
        {
            return (Document)GetField(fieldPath);
        }
        
        public T Enum<T>(string fieldPath)
        {
            var type = typeof(T);
            
            return (T)System.Enum.ToObject(type, GetField(fieldPath));
        }
        
        public List<T> List<T>(string fieldPath)
        {
            var collection = new List<T>();
            var type = typeof(T);
            var data = GetField(fieldPath);
            
            if (data is List<T>)
            {
                collection = ((IEnumerable)data).Cast<T>().ToList();
            }
            else
            {
                switch (type.Name)
                {
                    case "Boolean":
                        collection = ((List<object>)data).Select(Convert.ToBoolean).ToList() as List<T>;
                        break;
                    case "Byte":
                        collection = ((List<object>)data).Select(Convert.ToByte).ToList() as List<T>;
                        break;
                    case "Int16":
                        collection = ((List<object>)data).Select(Convert.ToInt16).ToList() as List<T>;
                        break;
                    case "Int32":
                        collection = ((List<object>)data).Select(Convert.ToInt32).ToList() as List<T>;
                        break;
                    case "Int64":
                        collection = ((List<object>)data).Select(Convert.ToInt64).ToList() as List<T>;
                        break;
                    case "Single":
                        collection = ((List<object>)data).Select(Convert.ToSingle).ToList() as List<T>;
                        break;
                    case "Double":
                        collection = ((List<object>)data).Select(Convert.ToDouble).ToList() as List<T>;
                        break;
                    case "Decimal":
                        collection = ((List<object>)data).Select(Convert.ToDecimal).ToList() as List<T>;
                        break;
                    case "DateTime":
                        collection = ((List<object>)data).Select(Convert.ToDateTime).ToList() as List<T>;
                        break;
                    case "String":
                        collection = ((List<object>)data).Select(Convert.ToString).ToList() as List<T>;
                        break;
                    default:
                        collection = ((IEnumerable)data).Cast<T>().ToList();
                        break;
                }
            }
            
            return collection;
        }
        
        private object GetField(string fieldPath)
        {
            var currentField = "";
            var arrayContent = "";
            
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                var iteration = 1;
                var embeddedDocument = this;
                
                foreach (var field in fields)
                {
                    currentField = field;
                    arrayContent = "";
                    
                    if (field.Contains("["))
                    {
                        var firstIndex = field.IndexOf('[');
                        var lastIndex = field.IndexOf(']');
                        
                        arrayContent = field.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                        currentField = field.Substring(0, firstIndex);
                    }
                    
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(currentField))
                        {
                            return GetFieldValue(currentField, arrayContent, embeddedDocument);
                        }
                        
                        break;
                    }

                    if (embeddedDocument.ContainsKey(currentField))
                    {
                        embeddedDocument = (Document)GetFieldValue(currentField, arrayContent, embeddedDocument);
                    }
                    else
                    {
                        // if current field in path isn't present
                        break;
                    }

                    iteration++;
                }
            }
            else
            {
                currentField = fieldPath;
                
                if (fieldPath.Contains("["))
                {
                    var firstIndex = fieldPath.IndexOf('[');
                    var lastIndex = fieldPath.IndexOf(']');
                    
                    arrayContent = fieldPath.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                    currentField = fieldPath.Substring(0, firstIndex);
                }
                
                if (this.ContainsKey(currentField))
                {
                    return GetFieldValue(currentField, arrayContent, this);
                }
            }
            
            return null;
        }
        
        private object GetFieldValue(string fieldName, string arrayContent, Document fieldObject)
        {
            if (arrayContent == "")
            {
                return fieldObject[fieldName];
            }
            else
            {
                return ((IList)fieldObject[fieldName])[int.Parse(arrayContent)];
            }
        }
        
        #endregion
        
        #region Field setters
        
        public Document Bool(string fieldPath, bool value)
        {
            SetField(fieldPath, value);

            return this;
        }
        
        public Document Byte(string fieldPath, byte value)
        {
            SetField(fieldPath, value);

            return this;
        }
        
        public Document Short(string fieldPath, short value)
        {
            SetField(fieldPath, value);

            return this;
        }
        
        public Document Int(string fieldPath, int value)
        {
            SetField(fieldPath, value);

            return this;
        }
        
        public Document Long(string fieldPath, long value)
        {
            SetField(fieldPath, value);

            return this;
        }
        
        public Document Float(string fieldPath, float value)
        {
            SetField(fieldPath, value);

            return this;
        }
        
        public Document Double(string fieldPath, double value)
        {
            SetField(fieldPath, value);

            return this;
        }
        
        public Document Decimal(string fieldPath, decimal value)
        {
            SetField(fieldPath, value);

            return this;
        }
        
        public Document String(string fieldPath, string value)
        {
            SetField(fieldPath, value);

            return this;
        }
        
        public Document DateTime(string fieldPath, DateTime value)
        {
            return DateTime(fieldPath, value, Settings.DateTimeFormat);
        }
        
        public Document DateTime(string fieldPath, DateTime value, DateTimeFormat format)
        {
            switch (format)
            {
                case DateTimeFormat.Iso8601String:
                    SetField(fieldPath, value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", DateTimeFormatInfo.InvariantInfo));
                    break;
                case DateTimeFormat.UnixTimeStamp:
                    TimeSpan span = (value.ToUniversalTime() - DocumentSettings.UnixEpoch);
                    SetField(fieldPath, (long)span.TotalSeconds);
                    break;
                case DateTimeFormat.DateTimeObject:
                default:
                    SetField(fieldPath, value);
                    break;
            }

            return this;
        }
        
        // used for null inputObject
        public Document Object(string fieldPath, object inputObject)
        {
            SetField(fieldPath, inputObject);

            return this;
        }
        
        public Document Object<T>(string fieldPath, T inputObject)
        {
            SetField(fieldPath, inputObject);
            
            return this;
        }
        
        public Document Docunet<T>(string fieldPath, T inputObject)
        {
            if (inputObject is Document)
            {
                SetField(fieldPath, inputObject);
            }
            else
            {
                SetField(fieldPath, ToDocument<T>(inputObject));
            }
            
            return this;
        }
        
        public Document Enum<T>(string fieldPath, T inputObject)
        {
            SetField(fieldPath, inputObject);
            
            return this;
        }
        
        public Document List<T>(string fieldPath, List<T> inputCollection)
        {
            SetField(fieldPath, inputCollection);

            return this;
        }
        
        private void SetField(string fieldPath, object inputObject)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                var iteration = 1;
                var embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(field))
                        {
                            embeddedDocument[field] = inputObject;
                        }
                        else
                        {
                            embeddedDocument.Add(field, inputObject);
                        }
                        
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (Document)embeddedDocument[field];
                    }
                    else
                    {
                        // if document which contains the field doesn't exist create it first
                        var tempDocument = new Document();
                        embeddedDocument.Add(field, tempDocument);
                        embeddedDocument = tempDocument;
                    }

                    iteration++;
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    this[fieldPath] = inputObject;
                }
                else
                {
                    this.Add(fieldPath, inputObject);
                }
            }
        }
        
        #endregion
        
        #region Field checkers
        
        public bool Has(string fieldPath)
        {
            var currentField = "";
            var arrayContent = "";
            
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                var iteration = 1;
                var embeddedDocument = this;
                
                foreach (var field in fields)
                {
                    currentField = field;
                    arrayContent = "";
                    
                    if (field.Contains("["))
                    {
                        var firstIndex = field.IndexOf('[');
                        var lastIndex = field.IndexOf(']');
                        
                        arrayContent = field.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                        currentField = field.Substring(0, firstIndex);
                    }
                    
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(currentField))
                        {
                            // it's array - should check if there is value at specific index
                            if (arrayContent != "")
                            {
                                // passed array index is less than total number of elements in the array
                                if (((IList)embeddedDocument[currentField]).Count > int.Parse(arrayContent))
                                {
                                    return true;
                                }
                            }
                            // it's single value
                            else
                            {
                                return true;
                            }
                        }
                        
                        break;
                    }

                    if (embeddedDocument.ContainsKey(currentField))
                    {
                        embeddedDocument = (Document)GetFieldValue(currentField, arrayContent, embeddedDocument);
                    }
                    else
                    {
                        // if current field in path isn't present
                        break;
                    }

                    iteration++;
                }
            }
            else
            {
                currentField = fieldPath;
                
                if (fieldPath.Contains("["))
                {
                    var firstIndex = fieldPath.IndexOf('[');
                    var lastIndex = fieldPath.IndexOf(']');
                    
                    arrayContent = fieldPath.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                    currentField = fieldPath.Substring(0, firstIndex);
                }
                
                if (this.ContainsKey(currentField))
                {
                    // it's array - should check if there is value at specific index
                    if (arrayContent != "")
                    {
                        // passed array index is less than total number of elements in the array
                        if (((IList)this[currentField]).Count > int.Parse(arrayContent))
                        {
                            return true;
                        }
                    }
                    // it's single value
                    else
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        public bool IsNull(string fieldPath)
        {
            var currentField = "";
            var arrayContent = "";
            
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                var iteration = 1;
                var embeddedDocument = this;
                
                foreach (var field in fields)
                {
                    currentField = field;
                    arrayContent = "";
                    
                    if (field.Contains("["))
                    {
                        var firstIndex = field.IndexOf('[');
                        var lastIndex = field.IndexOf(']');
                        
                        arrayContent = field.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                        currentField = field.Substring(0, firstIndex);
                    }
                    
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(currentField))
                        {
                            // it's array - should check if there is value at specific index
                            if (arrayContent != "")
                            {
                                var collection = (IList)embeddedDocument[currentField];
                                var index = int.Parse(arrayContent);                        
                                // passed array index is less than total number of elements in the array
                                if (collection.Count > index)
                                {
                                    if (collection[index] == null)
                                    {
                                        return true;
                                    }
                                }
                            }
                            // it's single value
                            else
                            {
                                if (embeddedDocument[currentField] == null)
                                {
                                    return true;
                                }
                            }
                        }
                        
                        break;
                    }

                    if (embeddedDocument.ContainsKey(currentField))
                    {
                        embeddedDocument = (Document)GetFieldValue(currentField, arrayContent, embeddedDocument);
                    }
                    else
                    {
                        // if current field in path isn't present
                        break;
                    }

                    iteration++;
                }
            }
            else
            {
                currentField = fieldPath;
                
                if (fieldPath.Contains("["))
                {
                    var firstIndex = fieldPath.IndexOf('[');
                    var lastIndex = fieldPath.IndexOf(']');
                    
                    arrayContent = fieldPath.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                    currentField = fieldPath.Substring(0, firstIndex);
                }
                
                if (this.ContainsKey(currentField))
                {
                    // it's array - should check if there is value at specific index
                    if (arrayContent != "")
                    {
                        var collection = (IList)this[currentField];
                        var index = int.Parse(arrayContent);                        
                        // passed array index is less than total number of elements in the array
                        if (collection.Count > index)
                        {
                            if (collection[index] == null)
                            {
                                return true;
                            }
                        }
                    }
                    // it's single value
                    else
                    {
                        if (this[currentField] == null)
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
        
        public Type Type(string fieldPath)
        {
            var currentField = "";
            var arrayContent = "";
            
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                var iteration = 1;
                var embeddedDocument = this;
                
                foreach (var field in fields)
                {
                    currentField = field;
                    arrayContent = "";
                    
                    if (field.Contains("["))
                    {
                        var firstIndex = field.IndexOf('[');
                        var lastIndex = field.IndexOf(']');
                        
                        arrayContent = field.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                        currentField = field.Substring(0, firstIndex);
                    }
                    
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(currentField))
                        {
                            return embeddedDocument[currentField].GetType();
                        }
                        
                        break;
                    }

                    if (embeddedDocument.ContainsKey(currentField))
                    {
                        embeddedDocument = (Document)GetFieldValue(currentField, arrayContent, embeddedDocument);
                    }
                    else
                    {
                        // if current field in path isn't present
                        break;
                    }

                    iteration++;
                }
            }
            else
            {
                currentField = fieldPath;
                
                if (fieldPath.Contains("["))
                {
                    var firstIndex = fieldPath.IndexOf('[');
                    var lastIndex = fieldPath.IndexOf(']');
                    
                    arrayContent = fieldPath.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                    currentField = fieldPath.Substring(0, firstIndex);
                }
                
                if (this.ContainsKey(currentField))
                {
                    return this[currentField].GetType();
                }
            }
            
            return null;
        }
        
        #endregion
        
        public Document Drop(string fieldPath)
        {
            var currentField = "";
            var arrayContent = "";
            
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                var iteration = 1;
                var embeddedDocument = this;
                
                foreach (var field in fields)
                {
                    currentField = field;
                    arrayContent = "";
                    
                    if (field.Contains("["))
                    {
                        var firstIndex = field.IndexOf('[');
                        var lastIndex = field.IndexOf(']');
                        
                        arrayContent = field.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                        currentField = field.Substring(0, firstIndex);
                    }
                    
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(currentField))
                        {
                            embeddedDocument.Remove(currentField);
                        }
                        
                        break;
                    }

                    if (embeddedDocument.ContainsKey(currentField))
                    {
                        embeddedDocument = (Document)GetFieldValue(currentField, arrayContent, embeddedDocument);
                    }
                    else
                    {
                        // if current field in path isn't present
                        break;
                    }

                    iteration++;
                }
            }
            else
            {
                currentField = fieldPath;
                
                if (fieldPath.Contains("["))
                {
                    var firstIndex = fieldPath.IndexOf('[');
                    var lastIndex = fieldPath.IndexOf(']');
                    
                    arrayContent = fieldPath.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                    currentField = fieldPath.Substring(0, firstIndex);
                }
                
                if (this.ContainsKey(currentField))
                {
                    this.Remove(currentField);
                }
            }
            
            return this;
        }
        
        #region Clone
        
        public Document Clone()
        {
            return Clone(this);
        }
        
        private Document Clone(Document document)
        {
            var clonedDocument = new Document();
            
            foreach (KeyValuePair<string, object> field in document)
            {
                if (field.Value is Document)
                {
                    clonedDocument.Add(field.Key, Clone((Document)field.Value));
                }
                else
                {
                    clonedDocument.Add(field.Key, field.Value);
                }
            }
            
            return clonedDocument;
        }
        
        #endregion
        
        #region Merge
        
        public void Merge(Document document, MergeOptions mergeOptions = MergeOptions.MergeFields)
        {
            var mergedDocument = Merge(this, document, mergeOptions);
            
            this.Clear();
            
            foreach (KeyValuePair<string, object> field in mergedDocument)
            {
                this.Add(field.Key, field.Value);
            }
        }
        
        public static Document Merge(Document document1, Document document2, MergeOptions mergeOptions = MergeOptions.MergeFields)
        {
            // clone first document to prevent its poisoning/injection of fields from second document
            var clonedDocument1 = document1.Clone();
            
            foreach (KeyValuePair<string, object> field in document2)
            {
                if (clonedDocument1.ContainsKey(field.Key))
                {
                    var field1Value = clonedDocument1[field.Key];
                    var field2Value = field.Value;
                    
                    if ((field1Value is Document) && (field2Value is Document))
                    {
                        if (mergeOptions == MergeOptions.MergeFields)
                        {
                            clonedDocument1.Remove(field.Key);
                            clonedDocument1.Add(field.Key, Merge((Document)field1Value, (Document)field2Value, mergeOptions));
                        }
                        else if (mergeOptions == MergeOptions.ReplaceFields)
                        {
                            clonedDocument1.Remove(field.Key);
                            clonedDocument1.Add(field.Key, (Document)field2Value);
                        }
                        else if (mergeOptions == MergeOptions.KeepFields)
                        {
                            // do nothing - keep it as it is
                        }
                    }
                    else if ((field1Value is IList) && (field2Value is IList) && (mergeOptions == MergeOptions.MergeFields))
                    {
                        var collection1 = (IList)field1Value;
                        var collection2 = (IList)field2Value;
                        
                        for (var i = 0; i < collection2.Count; i++)
                        {
                            var item = collection2[i];
                            
                            if (!collection1.Contains(item))
                            {
                                collection1.Add(item);
                            }
                        }
                    }
                    else if (mergeOptions == MergeOptions.ReplaceFields)
                    {
                        clonedDocument1.Remove(field.Key);
                        clonedDocument1.Add(field.Key, field2Value);
                    }
                    else if (mergeOptions == MergeOptions.KeepFields)
                    {
                        // do nothing - keep it as it is
                    }
                }
                else
                {
                    clonedDocument1.Add(field.Key, field.Value);
                }
            }
            
            return clonedDocument1;
        }
        
        #endregion
        
        public void Replace(Document document)
        {
            this.Clear();
            
            foreach(KeyValuePair<string, object> field in document)
            {
                this.Add(field.Key, field.Value);
            }
        }
        
        public Document Except(params string[] fields)
        {
            var document = Clone();
            
            foreach (string field in fields)
            {
                document.Drop(field);
            }
            
            return document;
        }
        
        public Document Only(params string[] fields)
        {
            var document = new Document();
            
            foreach (string field in fields)
            {
                document.SetField(field, GetField(field));
            }
            
            return document;
        }
        
        #region Equals
        
        public bool Equals(Document document)
        {
            return CompareDocuments(document, this);
        }
        
        private bool CompareDocuments(Document document1, Document document2)
        {
            var iterations = 0;
            
            foreach (KeyValuePair<string, object> field in document1)
            {
                if (document2.Has(field.Key))
                {
                    var areEqual = false;
                    var obj = document2.GetField(field.Key);
                    
                    if ((field.Value is Document) && (obj is Document))
                    {
                        areEqual = CompareDocuments((Document)field.Value, (Document)obj);
                    }
                    else if ((field.Value is IList) && (obj is IList))
                    {
                        areEqual = CompareCollections((IList)field.Value, (IList)obj);
                    }
                    else
                    {
                        areEqual = CompareValues(field.Value, obj);
                    }
                    
                    if (!areEqual)
                    {
                        return false;
                    }
                    
                    iterations++;
                }
                else
                {
                    return false;
                }
            }
            
            if (iterations != document2.Count)
            {
                return false;
            }
            
            return true;
        }
        
        private bool CompareCollections(IList collection1, IList collection2)
        {
            if (collection1.Count != collection2.Count)
            {
                return false;
            }

            for (var i = 0; i < collection1.Count; i++)
            {
                var item = collection1[i];
                var areEqual = false;
                
                if ((item is Document) && (collection2[i] is Document))
                {
                    areEqual = CompareDocuments((Document)item, (Document)collection2[i]);
                }
                else
                {
                    areEqual = CompareValues(item, collection2[i]);
                }
                
                if (!areEqual)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private bool CompareValues(object value1, object value2)
        {
            var areEqual = false;
            
            if ((value1 != null) && (value2 != null))
            {
                areEqual = value1.Equals(value2);
            }
            else if ((value1 == null) && (value2 == null))
            {
                areEqual = true;
            }
            
            return areEqual;
        }
        
        #endregion
        
        #region Convert to generic object
        
        /// <summary>
        /// Converts and copies document fields to specified generic object.
        /// </summary>
        public T ToObject<T>() where T : class, new()
        {
            T genericObject = new T();

            genericObject = (T)ToObject<T>(genericObject, this);

            return genericObject;
        }
        
        public void ToObject<T>(T genericObject) where T : class, new()
        {
            genericObject = (T)ToObject<T>(genericObject, this);
        }
        
        private T ToObject<T>(T genericObject, Document document) where T : class, new()
        {
            var genericObjectType = genericObject.GetType();

            if (genericObject is Document)
            {
                // if generic object is arango specific class - use set field to copy data
                foreach (KeyValuePair<string, object> item in document)
                {
                    (genericObject as Document).SetField(item.Key, item.Value);
                }
            }
            else
            {
                foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var propertyName = propertyInfo.Name;
                    var arangoProperty = propertyInfo.GetCustomAttribute<ArangoProperty>();
                    object fieldValue = null;
                    Type fieldType = null;
                    
                    if (arangoProperty != null)
                    {
                        if (!arangoProperty.Deserializable)
                        {
                            continue;
                        }
                        
                        if (!string.IsNullOrEmpty(arangoProperty.Alias))
                        {
                            propertyName = arangoProperty.Alias;
                        }
                    }
                    
                    if (document.Has(propertyName))
                    {
                        fieldValue = document.GetField(propertyName);
                        
                        if (fieldValue != null)
                        {
                            fieldType = fieldValue.GetType();
                        }
                    }
                    else
                    {
                        continue;
                    }
                    
                    // property is a collection
                    if ((propertyInfo.PropertyType.IsArray || 
                         propertyInfo.PropertyType.IsGenericType))
                    {
                        var instance = Activator.CreateInstance(propertyInfo.PropertyType);
                            
                        propertyInfo.SetValue(
                            genericObject,
                            ConvertToCollection(instance, (IList)fieldValue, propertyInfo.PropertyType),
                            null
                        );
                    }
                    // property is class except the string type since string values are parsed differently
                    else if (propertyInfo.PropertyType.IsClass && (propertyInfo.PropertyType.Name != "String"))
                    {
                        // create object instance of embedded class
                        var instance = Activator.CreateInstance(propertyInfo.PropertyType);

                        if (fieldType == typeof(Document))
                        {
                            propertyInfo.SetValue(genericObject, ToObject(instance, (Document)fieldValue), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(genericObject, fieldValue, null);
                        }
                    }
                    // property is basic type
                    else
                    {
                        if ((fieldValue == null) || (propertyInfo.PropertyType == fieldType))
                        {
                            propertyInfo.SetValue(genericObject, fieldValue, null);
                        } 
                        else
                        {
                            propertyInfo.SetValue(genericObject, Convert.ChangeType(fieldValue, propertyInfo.PropertyType), null);
                        }
                    }
                }
            }

            return genericObject;
        }
        
        private object ConvertToCollection(object collectionObject, IList collection, Type collectionType)
        {
            if (collection == null)
            {
                return null;
            }
            
            //List<object> convertedCollection = new List<object>();
            
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
                            
                            if (elementType == typeof(Document))
                            {
                                ((IList)collectionObject).Add(ToObject(instance, (Document)collection[i]));
                            }
                            else
                            {
                                if (elementType == instance.GetType())
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
        
        public static Document ToDocument<T>(T inputObject)
        {
            if (inputObject is Document)
            {
                return inputObject as Document;
            }
            else if (inputObject is Dictionary<string, object>)
            {
                var document = new Document();
                
                foreach (KeyValuePair<string, object> field in inputObject as Dictionary<string, object>)
                {
                    document.Object(field.Key, field.Value);
                }
                
                return document;
            }
            else
            {
                var inputObjectType = inputObject.GetType();
                var document = new Document();
                
                foreach (var propertyInfo in inputObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var propertyName = propertyInfo.Name;
                    var arangoProperty = propertyInfo.GetCustomAttribute<ArangoProperty>();
    
                    if (arangoProperty != null)
                    {
                        // do not convert properties which are not flagged for serialization or
                        // represent arango specific fields
                        if (!arangoProperty.Serializable ||
                            arangoProperty.Identity ||
                            arangoProperty.Key ||
                            arangoProperty.Revision ||
                            arangoProperty.From ||
                            arangoProperty.To)
                        {
                            continue;
                        }
                        
                        if (!string.IsNullOrEmpty(arangoProperty.Alias))
                        {
                            propertyName = arangoProperty.Alias;
                        }
                    }
                    
                    var propertyValue = propertyInfo.GetValue(inputObject);
                    
                    if (propertyValue == null)
                    {
                        document.SetField(propertyName, null);
                    }
                    // property is array or collection
                    else if (propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType)
                    {
                        document.SetField(propertyName, ToList(propertyValue));
                    }
                    // property is class except the string type since string values are parsed differently
                    else if (propertyInfo.PropertyType.IsClass && (propertyInfo.PropertyType.Name != "String"))
                    {
                        document.SetField(propertyName, ToDocument(propertyValue));
                    }
                    // property is basic type
                    else
                    {
                        document.SetField(propertyName, propertyValue);
                    }
                }
                
                return document;
            }
        }
        
        public static List<object> ToList<T>(T inputCollection)
        {
            var collectionType = inputCollection.GetType();
            var documents = new List<object>();
            
            if (collectionType.IsArray || collectionType.IsGenericType)
            {
                var collection = (IList)inputCollection;
                
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
                            documents.Add(collection[i]);
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
                                documents.Add(collection[i]);
                            }
                            // generic collection consists of generic type which should be parsed
                            else
                            {
                                // create instance object based on first element of generic collection
                                var instance = Activator.CreateInstance(collectionType.GetGenericArguments().First(), null);
                                
                                documents.Add(ToDocument(collection[i]));
                            }
                        }
                        else
                        {
                            var obj = Activator.CreateInstance(elementType, collection[i]);
    
                            documents.Add(obj);
                        }
                    }
                }
            }
            
            return documents;
        }
        
        #region Serialization
        
        public string Serialize()
        {
            return Serialize(this);
        }
        
        /// <summary>
        /// Serializes specified object to JSON string.
        /// </summary>
        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        
        #endregion
        
        #region Deserialization

        public void Parse(string json)
        {
            foreach(KeyValuePair<string, object> field in DeserializeDocument(json))
            {
                this.Add(field.Key, field.Value);
            }
        }
        
        public static List<T> DeserializeArray<T>(string json)
        {
            var collection = new List<T>();
            var type = typeof(T);
            var data = DeserializeArray(JArray.Parse(json));
            
            if (data is List<T>)
            {
                collection = ((IEnumerable)data).Cast<T>().ToList();
            }
            else
            {
                switch (type.Name)
                {
                    case "Boolean":
                        collection = data.Select(Convert.ToBoolean).ToList() as List<T>;
                        break;
                    case "Byte":
                        collection = data.Select(Convert.ToByte).ToList() as List<T>;
                        break;
                    case "Int16":
                        collection = data.Select(Convert.ToInt16).ToList() as List<T>;
                        break;
                    case "Int32":
                        collection = data.Select(Convert.ToInt32).ToList() as List<T>;
                        break;
                    case "Int64":
                        collection = data.Select(Convert.ToInt64).ToList() as List<T>;
                        break;
                    case "Single":
                        collection = data.Select(Convert.ToSingle).ToList() as List<T>;
                        break;
                    case "Double":
                        collection = data.Select(Convert.ToDouble).ToList() as List<T>;
                        break;
                    case "Decimal":
                        collection = data.Select(Convert.ToDecimal).ToList() as List<T>;
                        break;
                    case "DateTime":
                        collection = data.Select(Convert.ToDateTime).ToList() as List<T>;
                        break;
                    case "String":
                        collection = data.Select(Convert.ToString).ToList() as List<T>;
                        break;
                    default:
                        collection = ((IEnumerable)data).Cast<T>().ToList();
                        break;
                }
            }
            
            return collection;
        }
        
        /// <summary>
        /// Deserializes specified json string to document object.
        /// </summary>
        public static Document DeserializeDocument(string json)
        {
            var document = new Document();
            var fields = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json, DocumentSettings.SerializerSettings);
            
            foreach (KeyValuePair<string, JToken> field in fields)
            {
                switch (field.Value.Type)
                {
                    case JTokenType.Array:
                        document.Add(field.Key, DeserializeArray((JArray)field.Value));
                        break;
                    case JTokenType.Object:
                        document.Add(field.Key, DeserializeEmbeddedObject((JObject)field.Value));
                        break;
                    default:
                        document.Add(field.Key, DeserializeValue(field.Value));
                        break;
                }
            }
            
            return document;
        }

        private static object DeserializeEmbeddedObject(JObject jObject)
        {
            var embedded = new Document();

            foreach (KeyValuePair<string, JToken> field in jObject)
            {
                switch (field.Value.Type)
                {
                    case JTokenType.Array:
                        embedded.Add(field.Key, DeserializeArray((JArray)field.Value));
                        break;
                    case JTokenType.Object:
                        embedded.Add(field.Key, DeserializeEmbeddedObject((JObject)field.Value));
                        break;
                    default:
                        embedded.Add(field.Key, DeserializeValue(field.Value));
                        break;
                }
            }

            return embedded;
        }

        private static List<object> DeserializeArray(JArray jArray)
        {
            var array = new List<object>();
            
            foreach (JToken item in jArray)
            {
                switch (item.Type)
                {
                    case JTokenType.Array:
                        array.Add(DeserializeArray((JArray)item));
                        break;
                    case JTokenType.Object:
                        array.Add(DeserializeEmbeddedObject((JObject)item));
                        break;
                    default:
                        array.Add(DeserializeValue(item));
                        break;
                }
            }
            
            return array;
        }
        
        private static object DeserializeValue(JToken token)
        {
            return token.ToObject<object>();
        }
        
        #endregion
        
        /// <summary>
        /// Maps ArangoDB document specific attributes (_id, _key, _rev, _from, _to) from document to specified object.
        /// </summary>
        public void MapAttributesTo<T>(T genericObject)
        {
            // get arango specific fields to generic object if it has properties flagged with attributes
            foreach (PropertyInfo propertyInfo in genericObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var arangoProperty = propertyInfo.GetCustomAttribute<ArangoProperty>();
                
                if (arangoProperty != null)
                {
                    if (arangoProperty.Identity)
                    {
                        propertyInfo.SetValue(genericObject, String("_id"), null);
                    }
                    
                    if (arangoProperty.Key)
                    {
                        propertyInfo.SetValue(genericObject, String("_key"), null);
                    }
                    
                    if (arangoProperty.Revision)
                    {
                        propertyInfo.SetValue(genericObject, String("_rev"), null);
                    }
                    
                    if (arangoProperty.From)
                    {
                        propertyInfo.SetValue(genericObject, String("_from"), null);
                    }
                    
                    if (arangoProperty.To)
                    {
                        propertyInfo.SetValue(genericObject, String("_to"), null);
                    }
                }
            }
        }
        
        /// <summary>
        /// Maps ArangoDB document specific attributes (_id, _key, _rev, _from, _to) specified object to document.
        /// </summary>
        public void MapAttributesFrom<T>(T genericObject)
        {
            // get arango specific fields to generic object if it has properties flagged with attributes
            foreach (PropertyInfo propertyInfo in genericObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var arangoProperty = propertyInfo.GetCustomAttribute<ArangoProperty>();
                
                if (arangoProperty != null)
                {
                    if (arangoProperty.Identity)
                    {
                        String("_id", (string)propertyInfo.GetValue(genericObject));
                    }
                    
                    if (arangoProperty.Key)
                    {
                        String("_key", (string)propertyInfo.GetValue(genericObject));
                    }
                    
                    if (arangoProperty.Revision)
                    {
                        String("_rev", (string)propertyInfo.GetValue(genericObject));
                    }
                    
                    if (arangoProperty.From)
                    {
                        String("_from", (string)propertyInfo.GetValue(genericObject));
                    }
                    
                    if (arangoProperty.To)
                    {
                        String("_to", (string)propertyInfo.GetValue(genericObject));
                    }
                }
            }
        }
    }
}
