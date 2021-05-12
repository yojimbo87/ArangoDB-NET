using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arango.Client.ExternalLibraries.dictator
{
    public static class Dictator
    {
        /// <summary>
        /// Contains global settings which affects various operations.
        /// </summary>
        public static DictatorSettings Settings { get; private set; }
        
        /// <summary>
        /// Creates new empty document.
        /// </summary>
        public static Dictionary<string, object> New()
        {
            return new Dictionary<string, object>();
        }
        
        /// <summary>
        /// Creates new schema validator.
        /// </summary>
        public static Schema Schema
        {
            get 
            {
                return new Schema();
            }
        }
        
        static Dictator()
        {
            Settings = new DictatorSettings();
        }
        
        #region Conversion
        
        /// <summary>
        /// Converts specified dictionary list into collection of strongly typed objects.
        /// </summary>
        public static List<T> ToList<T>(List<Dictionary<string, object>> documents)
        {
            var list = new List<T>();
            
            foreach (var document in documents)
            {
                list.Add(document.ToObject<T>());
            }
            
            return list;
        }
        
        /// <summary>
        /// Converts specified object into document.
        /// </summary>
        public static Dictionary<string, object> ToDocument(object obj)
        {
            var inputObjectType = obj.GetType();
            var document = new Dictionary<string, object>();
            
            if (obj is Dictionary<string, object>)
            {
                document = (obj as Dictionary<string, object>).Clone();
            }
            else
            {
                string fieldName;
                object propertyValue;
                IEnumerable<Attribute> customAttributes;
                bool skipField;

                foreach (var propertyInfo in inputObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    fieldName = propertyInfo.Name;
                    propertyValue = propertyInfo.GetValue(obj);
                    customAttributes = propertyInfo.GetCustomAttributes();
                    skipField = false;

                    foreach (var attribute in customAttributes)
                    {
                        // skip property if it should be ingored
                        if (attribute is IgnoreField)
                        {
                            skipField = true;
                        }
                        // skip property if it should ingore null value
                        else if ((attribute is IgnoreNullValue) && (propertyValue == null))
                        {
                            skipField = true;
                        }
                        // set field name as property alias if present
                        else if (attribute is AliasField)
                        {
                            var aliasFieldAttribute = (AliasField)propertyInfo.GetCustomAttribute(typeof(AliasField));

                            fieldName = aliasFieldAttribute.Alias;
                        }
                    }

                    if (skipField)
                    {
                        continue;
                    }

                    if (propertyValue == null)
                    {
                        document.Object(fieldName, null);
                    }
                    else if (propertyValue is IDictionary)
                    {
                        document.Object(fieldName, propertyValue);
                    }
                    // property is array or collection
                    else if ((propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType) && (propertyValue is IList))
                    {
                        document.List(fieldName, ToList(propertyValue));
                    }
                    // property is class except the string type since string values are parsed differently
                    else if (propertyInfo.PropertyType.IsClass && (propertyInfo.PropertyType.Name != "String"))
                    {
                        document.Object(fieldName, ToDocument(propertyValue));
                    }
                    // property is basic type
                    else
                    {
                        document.Object(fieldName, propertyValue);
                    }
                }
            }
                
            return document;
        }
        
        /// <summary>
        /// Converts specified object list into collection of documents.
        /// </summary>
        public static List<Dictionary<string, object>> ToDocuments<T>(List<T> objects)
        {
            var documents = new List<Dictionary<string, object>>();
            
            foreach (var item in objects)
            {
                documents.Add(ToDocument(item));
            }
            
            return documents;
        }
        
        static List<object> ToList(object inputCollection)
        {
            var collectionType = inputCollection.GetType();
            var outputCollection = new List<object>();
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
                        outputCollection.Add(collection[i]);
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
                            outputCollection.Add(collection[i]);
                        }
                        // generic collection consists of generic type which should be parsed
                        else
                        {
                            // create instance object based on first element of generic collection
                            var instance = Activator.CreateInstance(collectionType.GetGenericArguments().First(), null);
                            
                            outputCollection.Add(ToDocument(collection[i]));
                        }
                    }
                    else
                    {
                        var obj = Activator.CreateInstance(elementType, collection[i]);

                        outputCollection.Add(obj);
                    }
                }
            }
            
            return outputCollection;
        }
        
        #endregion
    }
}

