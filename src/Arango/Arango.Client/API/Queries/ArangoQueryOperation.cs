﻿using System;
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

        /*
         *  standard high level operations
         */

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

        #region LET

        public ArangoQueryOperation LET(string variableName)
        {
            var etom = new Etom(AQL.LET);
            etom.AddValues(variableName);

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region RETURN

        public ArangoQueryOperation RETURN 
        { 
            get
            {
                var etom = new Etom(AQL.RETURN);

                ExpressionTree.Add(etom);

                return this;
            } 
        }

        #endregion

        /*
         *  standard AQL functions
         */

        #region DOCUMENT

        public ArangoQueryOperation DOCUMENT(List<string> documentIds)
        {
            var etom = new Etom(AQL.DOCUMENT);

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

                etom.AddValues(expression.ToString());
            }
            else if (documentIds.Count == 1)
            {
                // if documentId is valid document ID/handle enclose it with single quotes
                // otherwise it's most probably variable which shouldn't be enclosed
                if (Document.IsId(documentIds.First()))
                {
                    etom.AddValues(ToString(documentIds.First()));
                }
                else
                {
                    etom.AddValues(documentIds.First());
                }
            }

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoQueryOperation DOCUMENT(params string[] documentIds)
        {
            return DOCUMENT(documentIds.ToList());
        }

        #endregion

        #region EDGES

        public ArangoQueryOperation EDGES(string collection, string vertexId, ArangoEdgeDirection edgeDirection, Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var expression = collection + ", ";

            // if vertex is valid document ID/handle enclose it with single quotes
            // otherwise it's most probably variable which shouldn't be enclosed
            if (Document.IsId(vertexId))
            {
                expression += "'" + vertexId + "'";
            }
            else
            {
                expression += vertexId;
            }

            expression += ", ";

            switch (edgeDirection)
            {
                case ArangoEdgeDirection.In:
                    expression += "inbound";
                    break;
                case ArangoEdgeDirection.Out:
                    expression += "outbound";
                    break;
                case ArangoEdgeDirection.Any:
                    expression += "any";
                    break;
                default:
                    break;
            }

            var etom = new Etom(AQL.EDGES);
            etom.AddValues(expression);
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region FIRST

        public ArangoQueryOperation FIRST(Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var etom = new Etom(AQL.FIRST);
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        /*
         *  internal functions
         */

        public ArangoQueryOperation Collection(string name, Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var etom = new Etom(AQL.Collection);
            etom.AddValues(name);
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoQueryOperation Field(string name)
        {
            var etom = new Etom(AQL.Field);
            etom.AddValues(name);

            ExpressionTree.Add(etom);

            return this;
        }

        #region List

        public ArangoQueryOperation List(List<object> values)
        {
            var etom = new Etom(AQL.List);

            var expression = new StringBuilder();

            for (int i = 0; i < values.Count; i++)
            {
                expression.Append(ToString(values[i]));

                if (i < (values.Count() - 1))
                {
                    expression.Append(", ");
                }
            }

            etom.AddValues(expression.ToString());

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoQueryOperation List(params object[] values)
        {
            return List(values.ToList());
        }

        public ArangoQueryOperation List(List<object> values, Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var etom = new Etom(AQL.List);

            var expression = new StringBuilder();

            for (int i = 0; i < values.Count; i++)
            {
                expression.Append(ToString(values[i]));

                if (i < (values.Count() - 1))
                {
                    expression.Append(", ");
                }
            }

            etom.AddValues(expression.ToString());
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoQueryOperation List(Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var etom = new Etom(AQL.ListExpression);
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        public ArangoQueryOperation Object(Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var etom = new Etom(AQL.Object);
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoQueryOperation Value(object value)
        {
            var etom = new Etom(AQL.Value);
            etom.AddValues(value);

            ExpressionTree.Add(etom);

            return this;
        }

        #region Variable

        public ArangoQueryOperation Variable(string name)
        {
            var etom = new Etom(AQL.Variable);
            etom.AddValues(name);

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoQueryOperation Variable(string name, Func<ArangoQueryOperation, ArangoQueryOperation> context)
        {
            var etom = new Etom(AQL.Variable);
            etom.AddValues(name);
            etom.Children = context(new ArangoQueryOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

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

        private string ToString(List<Etom> expressionTree, int spaceCount, bool leadingSpace = true)
        {
            var expression = new StringBuilder();

            for (int i = 0; i < expressionTree.Count; i++)
            {
                var etom = expressionTree[i];

                switch (etom.Type)
                {
                    // standard high level operations
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

                        expression.Append(AQL.FOR + " " + etom.Value + " " + AQL.IN);
                        break;
                    case AQL.LET:
                        if (spaceCount != 0)
                        {
                            expression.Append("\n" + Tabulate(spaceCount));
                        }

                        expression.Append(AQL.LET + " " + etom.Value + " =");
                        break;
                    case AQL.RETURN:
                        if (spaceCount != 0)
                        {
                            expression.Append("\n" + Tabulate(spaceCount));
                        }

                        expression.Append(AQL.RETURN);
                        break;
                    // standard AQL functions
                    case AQL.DOCUMENT:
                        expression.Append(" " + AQL.DOCUMENT + "(" + etom.Value + ")");
                        break;
                    case AQL.EDGES:
                        expression.Append(" " + AQL.EDGES + "(" + etom.Value + ")");

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                        }
                        break;
                    case AQL.FIRST:
                        expression.Append(" " + AQL.FIRST + "(");

                        if ((etom.Children.Count == 1) && (etom.Children.First().Children.Count == 0))
                        {
                            expression.Append(ToString(etom.Children, 0, false) + ")");
                        }
                        else
                        {
                            expression.Append(ToString(etom.Children, spaceCount, false) + ")");
                        }
                        break;
                    // internal operations
                    case AQL.Collection:
                        expression.Append(" " + etom.Value);

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                        }
                        break;
                    case AQL.Field:
                        if (i != 0)
                        {
                            expression.Append(",");
                        }

                        expression.Append("\n" + Tabulate(spaceCount) + "'" + etom.Value + "':");
                        break;
                    case AQL.List:
                        if (leadingSpace)
                        {
                            expression.Append(" ");
                        }

                        expression.Append("[" + etom.Value + "]");

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                        }
                        break;
                    case AQL.ListExpression:
                        if (leadingSpace)
                        {
                            expression.Append(" ");
                        }

                        expression.Append("(");
                        expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                        expression.Append("\n" + Tabulate(spaceCount) + ")");
                        break;
                    case AQL.Object:
                        expression.Append(" {");
                        expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                        expression.Append("\n" + Tabulate(spaceCount) + "}");
                        break;
                    case AQL.String:
                        if (leadingSpace)
                        {
                            expression.Append(" ");
                        }

                        expression.Append(etom.Values);
                        break;
                    case AQL.Value:
                        expression.Append(Stringify(etom.Values));
                        break;
                    case AQL.Variable:
                        if (leadingSpace)
                        {
                            expression.Append(" ");
                        }

                        expression.Append(etom.Value);

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                        }
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
