using System;
using System.Collections.Generic;

namespace Arango.Client
{
    // contains ArangoDB specific extension methods, Dictator implementation code can be found in external libraries
    public static partial class DictionaryExtensions
    {
        #region _id
        
        /// <summary>
        /// Checks if '_id' field is present within document.
        /// </summary>
        public static bool HasID(this Dictionary<string, object> dictionary)
        {
            return Has(dictionary, "_id");
        }
        
        /// <summary>
        /// Retrieves value of '_id' field from document. If the field is missing null value is returned.
        /// </summary>
        public static string ID(this Dictionary<string, object> dictionary)
        {
            string id;
            
            try
            {
                id = String(dictionary, "_id");
            }
            catch (Exception)
            {
                id = null;
            }
            
            return id;
        }
        
        /// <summary>
        /// Stores _id field value.
        /// </summary>
        public static Dictionary<string, object> ID(this Dictionary<string, object> dictionary, string id)
        {
            SetFieldValue(dictionary, "_id", id);
            
            return dictionary;
        }
        
        #endregion
        
        #region _key
        
        /// <summary>
        /// Checks if '_key' field is present within document.
        /// </summary>
        public static bool HasKey(this Dictionary<string, object> dictionary)
        {
            return Has(dictionary, "_key");
        }
        
        /// <summary>
        /// Retrieves value of '_key' field from document. If the field is missing null value is returned.
        /// </summary>
        public static string Key(this Dictionary<string, object> dictionary)
        {
            string key;
            
            try
            {
                key = String(dictionary, "_key");
            }
            catch (Exception)
            {
                key = null;
            }
            
            return key;
        }
        
        /// <summary>
        /// Stores _key field value.
        /// </summary>
        public static Dictionary<string, object> Key(this Dictionary<string, object> dictionary, string key)
        {
            SetFieldValue(dictionary, "_key", key);
            
            return dictionary;
        }
        
        #endregion
        
        #region _rev
        
        /// <summary>
        /// Checks if '_rev' field is present within document.
        /// </summary>
        public static bool HasRev(this Dictionary<string, object> dictionary)
        {
            return Has(dictionary, "_rev");
        }
        
        /// <summary>
        /// Retrieves value of '_rev' field from document. If the field is missing null value is returned.
        /// </summary>
        public static string Rev(this Dictionary<string, object> dictionary)
        {
            string rev;
            
            try
            {
                rev = String(dictionary, "_rev");
            }
            catch (Exception)
            {
                rev = null;
            }
            
            return rev;
        }
        
        /// <summary>
        /// Stores _rev field value.
        /// </summary>
        public static Dictionary<string, object> Rev(this Dictionary<string, object> dictionary, string rev)
        {
            SetFieldValue(dictionary, "_rev", rev);
            
            return dictionary;
        }
        
        #endregion
    }
}
