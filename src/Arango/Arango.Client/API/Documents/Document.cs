using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arango.Client
{
    public class Document : Dictionary<string, object>
    {
        public Document() {}
        
        public Document(string json) 
        {
            foreach(KeyValuePair<string, object> field in Deserialize(json))
            {
                this.Add(field.Key, field.Value);
            }
        }
        
        #region Field operations
        
        public T GetField<T>(string fieldPath)
        {
            Type type = typeof(T);
            T value;

            if (type.IsPrimitive || type.IsArray || (type.Name == "String"))
            {
                value = default(T);
            }
            else
            {
                value = (T)Activator.CreateInstance(type);
            }

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Document embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        // if value is collection type, get element type and enumerate over its elements
                        if (value is IList)
                        {
                            Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                            IEnumerator enumerator = ((IEnumerable)embeddedDocument[field]).GetEnumerator();

                            while (enumerator.MoveNext())
                            {
                                // if current element is Document type which is dictionary<string, object>
                                // map its dictionary data to element instance
                                if (enumerator.Current is Document)
                                {
                                    var instance = Activator.CreateInstance(elementType);
                                    ((Document)enumerator.Current).Map(ref instance);

                                    ((IList)value).Add(instance);
                                }
                                else
                                {
                                    ((IList)value).Add(Convert.ChangeType(enumerator.Current, elementType));
                                }
                            }
                        }
                        else if (type.Name == "HashSet`1")
                        {
                            Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                            IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();

                            var addMethod = type.GetMethod("Add");

                            while (enumerator.MoveNext())
                            {
                                // if current element is Document type which is Dictionary<string, object>
                                // map its dictionary data to element instance
                                if (enumerator.Current is Document)
                                {
                                    var instance = Activator.CreateInstance(elementType);
                                    ((Document)enumerator.Current).Map(ref instance);

                                    addMethod.Invoke(value, new object[] { instance });
                                }
                                else
                                {
                                    addMethod.Invoke(value, new object[] { enumerator.Current });
                                }
                            }
                        }
                        else if (type.IsEnum)
                        {
                            value = (T)Enum.ToObject(type, embeddedDocument[field]);
                        }
                        else
                        {
                            if (embeddedDocument[field] is T) 
                            {
                                value = (T)embeddedDocument[field];
                            } 
                            else
                            {
                                value = (T)Convert.ChangeType(embeddedDocument[field], typeof(T));
                            }
                        }
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (Document)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    // if value is list or set type, get element type and enumerate over its elements
                    if (value is IList)
                    {
                        Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                        IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            // if current element is Document type which is Dictionary<string, object>
                            // map its dictionary data to element instance
                            if (enumerator.Current is Document)
                            {
                                var instance = Activator.CreateInstance(elementType);
                                ((Document)enumerator.Current).Map(ref instance);

                                ((IList)value).Add(instance);
                            }
                            else
                            {
                                ((IList)value).Add(Convert.ChangeType(enumerator.Current, elementType));
                            }
                        }
                    }
                    else if (type.Name == "HashSet`1")
                    {
                        Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                        IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();

                        var addMethod = type.GetMethod("Add");

                        while (enumerator.MoveNext())
                        {
                            // if current element is Document type which is Dictionary<string, object>
                            // map its dictionary data to element instance
                            if (enumerator.Current is Document)
                            {
                                var instance = Activator.CreateInstance(elementType);
                                ((Document)enumerator.Current).Map(ref instance);

                                addMethod.Invoke(value, new object[] { instance });
                            }
                            else
                            {
                                addMethod.Invoke(value, new object[] { enumerator.Current });
                            }
                        }
                    }
                    else if (type.IsEnum)
                    {
                        value = (T)Enum.ToObject(type, this[fieldPath]);
                    }
                    else
                    {
                        if (this[fieldPath] is T) 
                        {
                            value = (T)this[fieldPath];
                        } 
                        else
                        {
                            value = (T)Convert.ChangeType(this[fieldPath], typeof(T));
                        }
                    }
                }
            }

            return value;
        }

        public Document SetField<T>(string fieldPath, T value)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Document embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(field))
                        {
                            embeddedDocument[field] = value;
                        }
                        else
                        {
                            embeddedDocument.Add(field, value);
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
                        Document tempDocument = new Document();
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
                    this[fieldPath] = value;
                }
                else
                {
                    this.Add(fieldPath, value);
                }
            }

            return this;
        }
        
        public Document RemoveField(string fieldPath)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Document embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(field))
                        {
                            embeddedDocument.Remove(field);
                        }
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (Document)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    this.Remove(fieldPath);
                }
            }
            
            return this;
        }

        public bool HasField(string fieldPath)
        {
            bool contains = false;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Document embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = embeddedDocument.ContainsKey(field);
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (Document)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                contains = this.ContainsKey(fieldPath);
            }

            return contains;
        }
        
        #endregion
        
        // maps ArangoDocument fields to specified object
        private void Map(ref object obj)
        {
            if (obj is Dictionary<string, object>)
            {
                obj = this;
            }
            else
            {
                Type objType = obj.GetType();

                foreach (KeyValuePair<string, object> item in this)
                {
                    PropertyInfo property = objType.GetProperty(item.Key);

                    if (property != null)
                    {
                        property.SetValue(obj, item.Value, null);
                    }
                }
            }
        }
        
        public Document Except(params string[] fields)
        {
            Document document = new Document();
            
            foreach (KeyValuePair<string, object> field in this)
            {
                if (!fields.Contains(field.Key))
                {
                    document.Add(field.Key, field.Value);
                }
            }
            
            return document;
        }
        
        // evaluate equality of document fields and values
        #region Compare
        
        public bool Compare(Document document)
        {
            return CompareDocuments(document, this);
        }
        
        private bool CompareDocuments(Document document1, Document document2)
        {
            var areEqual = false;
            
            foreach (KeyValuePair<string, object> field in document1)
            {
                if (document2.HasField(field.Key))
                {
                    var obj = document2.GetField<object>(field.Key);
                    
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
                        break;
                    }
                }
            }
            
            return areEqual;
        }
        
        private bool CompareCollections(IList collection1, IList collection2)
        {
            var areEqual = false;

            for (var i = 0; i < collection1.Count; i++)
            {
                var item = collection1[i];
                
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
                    break;
                }
            }
            
            return areEqual;
        }
        
        private bool CompareValues(object value1, object value2)
        {
            return value1.Equals(value2);
        }
        
        #endregion
        
        #region Serialization

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        
        #endregion
        
        #region Deserialization

        public static Document Deserialize(string json)
        {
            Document document = new Document();
            Dictionary<string, JToken> fields = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);

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
            Document embedded = new Document();

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
            List<object> array = new List<object>();
            
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
        
        #region Convert to generic object
        
        public T To<T>() where T : class, new()
        {
            T genericObject = new T();

            genericObject = (T)ToObject<T>(genericObject, this);

            return genericObject;
        }
        
        private T ToObject<T>(T genericObject, Document document) where T : class, new()
        {
            var genericObjectType = genericObject.GetType();

            if (genericObjectType.Name.Equals("ArangoDocument") ||
                genericObjectType.Name.Equals("ArangoEdge"))
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
                    
                    if (document.HasField(propertyName))
                    {
                        fieldValue = document.GetField<object>(propertyName);
                        
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
                    else if (propertyInfo.PropertyType.IsClass &&
                        (propertyInfo.PropertyType.Name != "String"))
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
        
        #region Convert from generic object
        
        public void From<T>(T genericObject)
        {
            foreach (KeyValuePair<string, object> field in FromObject(genericObject))
            {
                this.Add(field.Key, field.Value);
            }
        }
        
        private Document FromObject<T>(T genericObject)
        {
            var document = new Document();
            var genericObjectType = genericObject.GetType();
            
            foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
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
                
                var propertyValue = propertyInfo.GetValue(genericObject);
                
                if (propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType)
                {
                    document.SetField(
                        propertyName, 
                        ConvertFromCollection((IList)propertyValue, propertyInfo.PropertyType)
                    );
                }
                // property is class except the string type since string values are parsed differently
                else if (propertyInfo.PropertyType.IsClass &&
                    (propertyInfo.PropertyType.Name != "String"))
                {
                    document.SetField(
                        propertyName, 
                        ConvertFromObject(propertyValue, propertyInfo.PropertyType)
                    );
                }
                // property is basic type
                else
                {
                    document.SetField(propertyName, propertyValue);
                }
            }
            
            return document;
        }
        
        private List<object> ConvertFromCollection(IList collection, Type collectionType)
        {
            var convertedCollection = new List<object>();
            
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
                        convertedCollection.Add(collection[i]);
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
                            convertedCollection.Add(collection[i]);
                        }
                        // generic collection consists of generic type which should be parsed
                        else
                        {
                            // create instance object based on first element of generic collection
                            var instance = Activator.CreateInstance(collectionType.GetGenericArguments().First(), null);
                            
                            if (elementType == typeof(Document))
                            {
                                convertedCollection.Add((Document)collection[i]);
                            }
                            else
                            {
                                convertedCollection.Add(FromObject(collection[i]));
                            }
                        }
                    }
                    else
                    {
                        var obj = Activator.CreateInstance(elementType, collection[i]);

                        convertedCollection.Add(obj);
                    }
                }
            }
            
            return convertedCollection;
        }
        
        private Document ConvertFromObject(object obj, Type objectType)
        {
            if (obj == null)
            {
                return null;
            }
            if (objectType == typeof(Document))
            {
                return (Document)obj;
            }
            else
            {
                return FromObject(obj);
            }
        }
        
        #endregion
    
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
                        propertyInfo.SetValue(genericObject, GetField<string>("_id"), null);
                    }
                    
                    if (arangoProperty.Key)
                    {
                        propertyInfo.SetValue(genericObject, GetField<string>("_key"), null);
                    }
                    
                    if (arangoProperty.Revision)
                    {
                        propertyInfo.SetValue(genericObject, GetField<string>("_rev"), null);
                    }
                    
                    if (arangoProperty.From)
                    {
                        propertyInfo.SetValue(genericObject, GetField<string>("_from"), null);
                    }
                    
                    if (arangoProperty.To)
                    {
                        propertyInfo.SetValue(genericObject, GetField<string>("_to"), null);
                    }
                }
            }
        }
    }
}
