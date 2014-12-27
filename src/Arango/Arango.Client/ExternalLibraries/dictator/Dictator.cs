using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arango.Client
{
    public static class Dictator
    {
        public static DictatorSettings Settings { get; private set; }
        
        public static Dictionary<string, object> New()
        {
            return new Dictionary<string, object>();
        }
        
        static Dictator()
        {
            Settings = new DictatorSettings();
        }
        
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
                foreach (var propertyInfo in inputObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var propertyValue = propertyInfo.GetValue(obj);
                    
                    if (propertyValue == null)
                    {
                        document.Object(propertyInfo.Name, null);
                    }
                    else if (propertyValue is IDictionary)
                    {
                        document.Object(propertyInfo.Name, propertyValue);
                    }
                    // property is array or collection
                    else if ((propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType) && (propertyValue is IList))
                    {
                        document.List(propertyInfo.Name, ToList(propertyValue));
                    }
                    // property is class except the string type since string values are parsed differently
                    else if (propertyInfo.PropertyType.IsClass && (propertyInfo.PropertyType.Name != "String"))
                    {
                        document.Object(propertyInfo.Name, ToDocument(propertyValue));
                    }
                    // property is basic type
                    else
                    {
                        document.Object(propertyInfo.Name, propertyValue);
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
    }
}

