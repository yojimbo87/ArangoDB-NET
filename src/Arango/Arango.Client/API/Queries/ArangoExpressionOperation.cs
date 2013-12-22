using System;
using System.Collections.Generic;
using System.Text;
using Arango.Client.Protocol;

namespace Arango.Client
{
    // TODO: docs
    public class ArangoExpressionOperation
    {
        private CursorOperation _cursorOperation;
        private const int _spaceCount = 2;
        private List<Etom> ExpressionTree { get; set; }

        internal ArangoExpressionOperation(CursorOperation cursorOperation)
        {
            _cursorOperation = cursorOperation;
            ExpressionTree = new List<Etom>();
        }

        public ArangoExpressionOperation()
        {
            ExpressionTree = new List<Etom>();
        }

        #region FILTER

        public ArangoExpressionOperation FILTER(string attribute)
        {
            var etom = new Etom(AQL.FILTER);
            etom.AddValues(attribute);

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region FOR

        public ArangoExpressionOperation FOR(string variableName)
        {
            var etom = new Etom(AQL.FOR);
            etom.AddValues(variableName);

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region IN

        public ArangoExpressionOperation IN(string expression, Func<ArangoExpressionOperation, ArangoExpressionOperation> context)
        {
            var etom = new Etom(AQL.IN);
            etom.AddValues(expression);
            etom.Children = context(new ArangoExpressionOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region LET

        public ArangoExpressionOperation LET(string variableName)
        {
            var etom = new Etom(AQL.LET);
            etom.AddValues(variableName);

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoExpressionOperation LET(string variableName, Func<ArangoExpressionOperation, ArangoExpressionOperation> context)
        {
            var etom = new Etom(AQL.LET);
            etom.AddValues(variableName);
            etom.Children = context(new ArangoExpressionOperation()).ExpressionTree;

            ExpressionTree.Add(etom);

            return this;
        }

        public ArangoExpressionOperation LET(string variableName, object value)
        {
            var etom = new Etom(AQL.LET);
            etom.AddValues(variableName, value);

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

        #region RETURN

        public ArangoExpressionOperation RETURN(string variableName)
        {
            var etom = new Etom(AQL.RETURN);
            etom.AddValues(variableName);

            ExpressionTree.Add(etom);

            return this;
        }

        #endregion

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
                        expression.Append("\n" + Tabulate(spaceCount) + AQL.FILTER + Stringify(etom.Values));
                        break;
                    case AQL.FOR:
                        expression.Append("\n" + Tabulate(spaceCount) + AQL.FOR + Stringify(etom.Values));
                        break;
                    case AQL.IN:
                        expression.Append(" " + AQL.IN + Stringify(etom.Values));

                        if (etom.Children.Count > 0)
                        {
                            expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                        }
                        break;
                    case AQL.LET:
                        expression.Append("\n" + Tabulate(spaceCount) + AQL.LET + Stringify(etom.Values));

                        if (etom.Children.Count > 0)
                        {
                            expression.Append("(");
                            expression.Append(ToString(etom.Children, spaceCount + _spaceCount));
                            expression.Append("\n" + Tabulate(spaceCount) + ")");
                        }
                        break;
                    case AQL.RETURN:
                        expression.Append("\n" + Tabulate(spaceCount) + AQL.RETURN + Stringify(etom.Values));
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
            var expression = "";

            foreach(object value in values)
            {
                expression += Join(ToString(value));
            }

            return expression;
        }

        private string Join(params string[] parts)
        {
            return " " + string.Join(" ", parts);
        }
    }
}

