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
        
        internal ArangoQueryOperation(List<Etom> expressionTree)
        {
        	ExpressionTree.AddRange(expressionTree);
        }

        internal ArangoQueryOperation(CursorOperation cursorOperation)
        {
            _cursorOperation = cursorOperation;
        }
        
        public ArangoQueryOperation()
        {
        }

        #region Query settings
        
        /// <summary> 
        /// Appends AQL query.
        /// </summary>
        /// <param name="queryString">AQL query string to be appended.</param>
        public ArangoQueryOperation Aql(string queryStirng)
        {
        	var etom = new Etom();
        	etom.Type = AQL.String;
            etom.Value = queryStirng;

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

        /*
         *  standard high level operations
         */

        public ArangoQueryOperation FOR(string variableName)
        {
            var etom = new Etom();
            etom.Type = AQL.FOR;
            etom.Value = variableName;

            return AddEtom(etom);
        }

        #region IN

        public ArangoQueryOperation IN(string name, ArangoQueryOperation aql)
        {
            var etom = new Etom();
            etom.Type = AQL.IN;
            etom.Value = name;
            etom.Children = aql.ExpressionTree;

            return AddEtom(etom);
        }

        public ArangoQueryOperation IN(List<object> list, ArangoQueryOperation aql)
        {
            var etom = new Etom();
            etom.Type = AQL.IN;

            var expression = new StringBuilder("[");

            for (int i = 0; i < list.Count; i++)
            {
                expression.Append(ToString(list[i]));

                if (i < (list.Count - 1))
                {
                    expression.Append(", ");
                }
            }

            expression.Append("]");

            etom.Value = expression.ToString();
            etom.Children = aql.ExpressionTree;

            return AddEtom(etom);
        }

        #endregion

        #region LET

        public ArangoQueryOperation LET(string variableName)
        {
            var etom = new Etom();
            etom.Type = AQL.LET;
            etom.Value = variableName;

            return AddEtom(etom);
        }

        #endregion

        #region RETURN

        public ArangoQueryOperation RETURN 
        { 
            get
            {
                var etom = new Etom();
                etom.Type = AQL.RETURN;

                return AddEtom(etom);
            } 
        }

        #endregion

        /*
         *  standard functions
         */

        public ArangoQueryOperation CONCAT(params ArangoQueryOperation[] values)
        {
            var etom = new Etom();
            etom.Type = AQL.CONCAT;
            etom.ChildrenList = new List<List<Etom>>();

            for (int i = 0; i < values.Length; i++)
            {
                etom.ChildrenList.Add(values[i].ExpressionTree);
            }

            return AddEtom(etom);
        }

        #region DOCUMENT

        public ArangoQueryOperation DOCUMENT(List<string> documentIds)
        {
            var etom = new Etom();
            etom.Type = AQL.DOCUMENT;

            // if parameter consists of more than one value it should be enclosed in square brackets
            if (documentIds.Count > 1)
            {
                var expression = new StringBuilder("[");

                for (int i = 0; i < documentIds.Count; i++)
                {
                    expression.Append(ToString(documentIds[i]));

                    if (i < (documentIds.Count - 1))
                    {
                        expression.Append(", ");
                    }
                }

                expression.Append("]");

                etom.Value = expression.ToString();
            }
            else if (documentIds.Count == 1)
            {
                // if documentId is valid document ID/handle enclose it with single quotes
                // otherwise it's most probably variable which shouldn't be enclosed
                if (Document.IsId(documentIds.First()))
                {
                    etom.Value = ToString(documentIds.First());
                }
                else
                {
                    etom.Value = documentIds.First();
                }
            }

            return AddEtom(etom);
        }

        public ArangoQueryOperation DOCUMENT(params string[] documentIds)
        {
            return DOCUMENT(documentIds.ToList());
        }

        #endregion

        #region EDGES

        public ArangoQueryOperation EDGES(string collection, string vertexId, ArangoEdgeDirection edgeDirection, ArangoQueryOperation aql)
        {
            var etom = new Etom();
            etom.Type = AQL.EDGES;

            var expression = new StringBuilder(collection + ", ");

            // if vertex is valid document ID/handle enclose it with single quotes
            // otherwise it's most probably variable which shouldn't be enclosed
            if (Document.IsId(vertexId))
            {
                expression.Append("'" + vertexId + "'");
            }
            else
            {
                expression.Append(vertexId);
            }

            expression.Append(", ");

            switch (edgeDirection)
            {
                case ArangoEdgeDirection.In:
                    expression.Append("inbound");
                    break;
                case ArangoEdgeDirection.Out:
                    expression.Append("outbound");
                    break;
                case ArangoEdgeDirection.Any:
                    expression.Append("any");
                    break;
                default:
                    break;
            }

            etom.Value = expression;
            etom.Children = aql.ExpressionTree;

            return AddEtom(etom);
        }

        #endregion

        public ArangoQueryOperation FIRST(ArangoQueryOperation aql)
        {
            var etom = new Etom();
            etom.Type = AQL.FIRST;

            etom.Children = aql.ExpressionTree;

            return AddEtom(etom);
        }
        
        /*
         *  internal operations
         */

        public ArangoQueryOperation Field(string name)
        {
            var etom = new Etom();
            etom.Type = AQL.Field;
            etom.Value = name;

            return AddEtom(etom);
        }

        #region List

        public ArangoQueryOperation List(List<object> values)
        {
            var etom = new Etom();
            etom.Type = AQL.List;

            var expression = new StringBuilder();

            for (int i = 0; i < values.Count; i++)
            {
                expression.Append(ToString(values[i]));

                if (i < (values.Count - 1))
                {
                    expression.Append(", ");
                }
            }

            etom.Value = expression.ToString();

            return AddEtom(etom);
        }

        public ArangoQueryOperation List(params object[] values)
        {
            return List(values.ToList());
        }

        public ArangoQueryOperation List(ArangoQueryOperation aql)
        {
            var etom = new Etom();
            etom.Type = AQL.ListExpression;
            etom.Children = aql.ExpressionTree;

            return AddEtom(etom);
        }

        #endregion

        public ArangoQueryOperation Object(ArangoQueryOperation aql)
        {
            var etom = new Etom();
            etom.Type = AQL.Object;
            etom.Children = aql.ExpressionTree;

            return AddEtom(etom);
        }
        
        public ArangoQueryOperation Value(object value)
        {
            var etom = new Etom();
            etom.Type = AQL.Value;
            etom.Value = value;

            return AddEtom(etom);
        }
        
        public ArangoQueryOperation Variable(string name)
        {
            var etom = new Etom();
            etom.Type = AQL.Variable;
            etom.Value = name;

            return AddEtom(etom);
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
        public string ToString(bool prettyPrint = true)
        {
            var expression = ToString(ExpressionTree, 0, prettyPrint);

            return expression.Trim();
        }

        private string ToString(List<Etom> expressionTree, int spaceLevel, bool prettyPrint)
        {
            var expression = new StringBuilder();
            
            for (int i = 0; i < expressionTree.Count; i++)
            {
                var etom = expressionTree[i];

	            switch (etom.Type)
	            {
	                // standard high level operations
	                case AQL.FOR:
                        if (prettyPrint)
                        {
                            expression.Append("\n" + Tabulate(spaceLevel * _spaceCount));
                        }
                        else
                        {
                            expression.Append(" ");
                        }

	                	expression.Append(AQL.FOR + " " + etom.Value + " ");
	                    break;
                    case AQL.IN:
                        expression.Append(AQL.IN + " " + etom.Value);

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceLevel + 1, prettyPrint));
                        }
                        break;
	                case AQL.LET:
                        if (prettyPrint)
                        {
                            expression.Append("\n" + Tabulate(spaceLevel * _spaceCount));
                        }
                        else
                        {
                            expression.Append(" ");
                        }

	                    expression.Append(AQL.LET + " " + etom.Value + " = ");
	                    break;
                    case AQL.RETURN:
                        if (prettyPrint)
                        {
                            expression.Append("\n" + Tabulate(spaceLevel * _spaceCount));
                        }
                        else
                        {
                            expression.Append(" ");
                        }

                        expression.Append(AQL.RETURN + " ");
                        break;
                    // standard functions
                    case AQL.CONCAT:
                        expression.Append(AQL.CONCAT + "(");

                        for (int j = 0; j < etom.ChildrenList.Count; j++)
                        {
                            expression.Append(ToString(etom.ChildrenList[j], 0, prettyPrint));

                            if (j < (etom.ChildrenList.Count - 1))
                            {
                                expression.Append(", ");
                            }
                        }

                        expression.Append(")");
                        break;
                    case AQL.DOCUMENT:
                        expression.Append(AQL.DOCUMENT + "(" + etom.Value + ")");
                        break;
                    case AQL.EDGES:
                        expression.Append(AQL.EDGES + "(" + etom.Value + ")");

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceLevel + 1, prettyPrint));
                        }
                        break;
                    case AQL.FIRST:
                        expression.Append(AQL.FIRST + "(");

                        if ((etom.Children.Count == 1) && (etom.Children.First().Children.Count == 0))
                        {
                            expression.Append(ToString(etom.Children, 0, prettyPrint) + ")");
                        }
                        else
                        {
                            expression.Append(ToString(etom.Children, spaceLevel, prettyPrint) + ")");
                        }
                        break;
	                // internal operations
                    case AQL.Field:
                        if (i != 0)
                        {
                            expression.Append(",");
                        }

                        if (prettyPrint)
                        {
                            expression.Append("\n" + Tabulate(spaceLevel * _spaceCount));
                        }
                        else
                        {
                            expression.Append(" ");
                        }

                        expression.Append("'" + etom.Value + "': ");
                        break;
                    case AQL.List:
                        expression.Append("[" + etom.Value + "]");
                        break;
                    case AQL.ListExpression:
                        expression.Append("(");

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceLevel + 1, prettyPrint));
                        }

                        if (prettyPrint)
                        {
                            expression.Append("\n" + Tabulate(spaceLevel * _spaceCount));
                        }
                        else
                        {
                            expression.Append(" ");
                        }

                        expression.Append(")");
                        break;
                    case AQL.Object:
                        expression.Append("{");

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceLevel + 1, prettyPrint));
                        }

                        if (prettyPrint)
                        {
                            expression.Append("\n" + Tabulate(spaceLevel * _spaceCount));
                        }
                        else
                        {
                            expression.Append(" ");
                        }

                        expression.Append("}");
                        break;
	                case AQL.Value:
	                    expression.Append(ToString(etom.Value));
	                    break;
	                case AQL.Variable:
	                    expression.Append(etom.Value);
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

        private ArangoQueryOperation AddEtom(Etom etom)
        {
            ExpressionTree.Add(etom);

            var aqo = new ArangoQueryOperation(ExpressionTree);

            ExpressionTree.Clear();

            return aqo;
        }

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

        public ArangoQueryOperation Aql(Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            ExpressionTree.AddRange(context(new ArangoQueryOperation()).ExpressionTree);

            return this;
        }
    }
}
