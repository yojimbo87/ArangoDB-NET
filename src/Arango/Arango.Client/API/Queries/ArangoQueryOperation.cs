using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary> 
    /// Expose AQL querying functionality.
    /// </summary>
    public class ArangoQueryOperation
    {
        private const int _spaceCount = 4;
        private int _batchSize = 0;
        private CursorOperation _cursorOperation;
        private Dictionary<string, object> _bindVars = new Dictionary<string, object>();

        internal List<Etom> ExpressionTree = new List<Etom>();

        public ArangoQueryOperation()
        {
        }

        internal ArangoQueryOperation(CursorOperation cursorOperation)
        {
            _cursorOperation = cursorOperation;
        }

        #region Query settings
        
        /// <summary> 
        /// Appends AQL query.
        /// </summary>
        /// <param name="queryString">AQL query string to be appended.</param>
        public ArangoQueryOperation Aql(string queryStirng)
        {
            var etom = new Etom(AQL.String);
            etom.AddValues(queryStirng);

            ExpressionTree.Add(etom);

            return this;
        }
        
        /// <summary> 
        /// Specifies maximum number of result documents to be transferred from the server to the client in one roundtrip.
        /// </summary>
        /// <param name="aql">Size of the batch being transferred in one roundtrip.</param>
        public ArangoQueryOperation BatchSize(int batchSize)
        {
            _batchSize = batchSize;
            
            return this;
        }
        
        /// <summary> 
        /// Specifies bind parameter and it's value.
        /// </summary>
        /// <param name="key">Key of the bind parameter. '@' sign will be added automatically.</param>
        /// <param name="value">Value of the bind parameter.</param>
        public ArangoQueryOperation AddParameter(string key, object value)
        {
            _bindVars.Add(key, value);
            
            return this;
        }

        #endregion

        #region FILTER

        public ArangoQueryOperation FILTER(string attribute)
        {
            var etom = new Etom(AQL.FILTER);
            etom.AddValues(attribute);

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region FOR

        public ArangoQueryOperation FOR(string variableName)
        {
            var etom = new Etom(AQL.FOR);
            etom.AddValues(variableName);

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region IN

        public ArangoQueryOperation IN(Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var etom = new Etom(AQL.IN);
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoQueryOperation IN(string expression, Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var etom = new Etom(AQL.IN);
            etom.AddValues(expression);
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region LET

        public ArangoQueryOperation LET(string variableName)
        {
            var etom = new Etom(AQL.LET);
            etom.AddValues(variableName);

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoQueryOperation LET(string variableName, Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var etom = new Etom(AQL.LET);
            etom.AddValues(variableName);
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region RETURN

        public ArangoQueryOperation RETURN(string variableName)
        {
            var etom = new Etom(AQL.RETURN);
            etom.AddValues(variableName);

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        public ArangoQueryOperation Value(object value)
        {
            var etom = new Etom(AQL.Value);
            etom.AddValues(value);

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoQueryOperation Variable(string name)
        {
            var etom = new Etom(AQL.Variable);
            etom.AddValues(name);

            ExpressionTree.Add(etom);

            return this;
        }

        #region ToList
        
        /// <summary> 
        /// Executes AQL query and returns list of documents.
        /// </summary>
        /// <param name="count">Variable where will be stored total number of result documents available after execution.</param>
        public List<Document> ToList(out int count)
        {
            var items = _cursorOperation.Post(ToString(), true, out count, _batchSize, _bindVars);
            
            return items.Cast<Document>().ToList();
        }
        
        /// <summary> 
        /// Executes AQL query and returns list of documents.
        /// </summary>
        public List<Document> ToList()
        {
            var count = 0;
            var items = _cursorOperation.Post(ToString(), false, out count, _batchSize, _bindVars);
            
            return items.Cast<Document>().ToList();
        }
        
        /// <summary> 
        /// Executes AQL query and returns list of objects.
        /// </summary>
        /// <param name="count">Variable where will be stored total number of result objects available after execution.</param>
        public List<T> ToList<T>(out int count) where T: class, new()
        {
            var type = typeof(T);
            var items = _cursorOperation.Post(ToString(), true, out count, _batchSize, _bindVars);
            var genericCollection = new List<T>();

            if (type.IsPrimitive ||
                (type == typeof(string)) ||
                (type == typeof(DateTime)) ||
                (type == typeof(decimal)))
            {
                foreach (object item in items)
                {
                    genericCollection.Add((T)Convert.ChangeType(item, type));
                }
            }
            else if (type == typeof(Document))
            {
                genericCollection = items.Cast<T>().ToList();
            }
            else
            {
                foreach (object item in items)
                {
                    var document = (Document)item;
                    var genericObject = Activator.CreateInstance(type);
                    
                    document.ToObject(genericObject);
                    document.MapAttributesTo(genericObject);
                    
                    genericCollection.Add((T)genericObject);
                }
            }
            
            return genericCollection;
        }
        
        /// <summary> 
        /// Executes AQL query and returns list of objects.
        /// </summary>
        public List<T> ToList<T>()
        {
            var type = typeof(T);
            var count = 0;
            var items = _cursorOperation.Post(ToString(), false, out count, _batchSize, _bindVars);
            var genericCollection = new List<T>();
            
            if (type.IsPrimitive ||
                (type == typeof(string)) ||
                (type == typeof(DateTime)) ||
                (type == typeof(decimal)))
            {
                foreach (object item in items)
                {
                    genericCollection.Add((T)Convert.ChangeType(item, type));
                }
            }
            else if (type == typeof(Document))
            {
                genericCollection = items.Cast<T>().ToList();
            }
            else
            {
                foreach (object item in items)
                {
                    var document = (Document)item;
                    var genericObject = Activator.CreateInstance(type);
                    
                    document.ToObject(genericObject);
                    document.MapAttributesTo(genericObject);
                    
                    genericCollection.Add((T)genericObject);
                }
            }
            
            return genericCollection;
        }
        
        #endregion
        
        #region ToObject
        
        /// <summary> 
        /// Executes AQL query and returns first document available in the result list.
        /// </summary>
        public Document ToObject()
        {
            return ToList().FirstOrDefault();
        }
        
        /// <summary> 
        /// Executes AQL query and returns first object available in the result list.
        /// </summary>
        public T ToObject<T>()
        {
            return ToList<T>().FirstOrDefault();
        }
        
        #endregion
        
        #region ToString
        
        /// <summary> 
        /// Returns AQL query string.
        /// </summary>
        public override string ToString()
        {
            return ToString(ExpressionTree, 0);
        }

        private string ToString(List<Etom> expressionTree, int spaceCount)
        {
            var expression = new StringBuilder();

            foreach (Etom etom in expressionTree)
            {
                switch (etom.Type)
                {
                    case AQL.FILTER:
                        if (spaceCount != 0)
                        {
                            expression.Append("\n" + Tabulate(spaceCount));
                        }

                        expression.Append(AQL.FILTER + Stringify(etom.Values));
                        break;
                    case AQL.FOR:
                        if (spaceCount != 0)
                        {
                            expression.Append("\n" + Tabulate(spaceCount));
                        }

                        expression.Append(AQL.FOR + Stringify(etom.Values));
                        break;
                    case AQL.IN:
                        expression.Append(" " + AQL.IN + Stringify(etom.Values));

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                        }
                        break;
                    case AQL.LET:
                        if (spaceCount != 0)
                        {
                            expression.Append("\n" + Tabulate(spaceCount));
                        }

                        expression.Append(AQL.LET + Stringify(etom.Values) + " =");

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(" (");
                            expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                            expression.Append("\n" + Tabulate(spaceCount) + ")");
                        }
                        break;
                    case AQL.RETURN:
                        if (spaceCount != 0)
                        {
                            expression.Append("\n" + Tabulate(spaceCount));
                        }

                        expression.Append(AQL.RETURN + Stringify(etom.Values));
                        break;
                    case AQL.String:
                        if (spaceCount != 0)
                        {
                            expression.Append(" ");
                        }

                        expression.Append(etom.Values);
                        break;
                    case AQL.Value:
                        expression.Append(Stringify(etom.Values));
                        break;
                    case AQL.Variable:
                        expression.Append(" " + etom.Values.First());
                        break;
                    default:
                        break;
                }
            }

            return expression.ToString();
        }
        
        private string ToString(object value)
        {
            if (value is string)
            {
                return "'" + value + "'";
            }
            else
            {
                return value.ToString();
            }
        }
        
        #endregion

        private string Tabulate(int count)
        {
            var spaces = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                spaces.Append(" ");
            }

            return spaces.ToString();
        }

        private string Stringify(List<object> values)
        {
            var expression = new StringBuilder();

            foreach(object value in values)
            {
                expression.Append(Join(ToString(value)));
            }

            return expression.ToString();
        }

        private string Join(params string[] parts)
        {
            return " " + string.Join(" ", parts);
        }
    }
}
