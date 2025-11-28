using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Data;
using System.Data.Common;

namespace BMS.Core.Data.ParameterParser;

public class ParameterParser : IParameterParser
{
    public IList<DbParameter> ExtractAndReplaceParameters(ref string sqlQuery)
    {
        TSql150Parser parser = new TSql150Parser(true);
        TSqlFragment fragment;
        IList<ParseError> errors;

        using (TextReader reader = new StringReader(sqlQuery))
        {
            fragment = parser.Parse(reader, out errors);
        }

        if (errors != null && errors.Count > 0)
        {
            throw new Exception("SQL parsing errors: " + string.Join(", ", errors));
        }

        ParameterReplacementVisitor visitor = new ParameterReplacementVisitor();
        fragment.Accept(visitor);

        // Use the SQL generator to produce the modified query instead of string.Replace method.
        Sql150ScriptGenerator scriptGenerator = new Sql150ScriptGenerator();

        scriptGenerator.GenerateScript(fragment, out sqlQuery);

        return visitor.DbParameters;
    }

    private class ParameterReplacementVisitor : TSqlFragmentVisitor
    {
        private int _parameterIndex = 0;
        public List<DbParameter> DbParameters { get; private set; } = new List<DbParameter>();

        public override void Visit(BooleanComparisonExpression node)
        {
            base.Visit(node);

            // Replace literals in the right expression with parameters
            if (node.SecondExpression is Literal literal)
            {
                string parameterName = $"@param{_parameterIndex++}";

                DbParameter DbParameter = new SqlParameter(parameterName, GetSqlDbType(literal));
                DbParameter.Value = GetValue(literal);
                DbParameters.Add(DbParameter);

                // Replace the literal with a parameter
                node.SecondExpression = new VariableReference { Name = parameterName };
            }
        }

        public override void Visit(LikePredicate node)
        {
            base.Visit(node);

            // Replace literals in the pattern expression with parameters
            if (node.SecondExpression is Literal literal)
            {
                string parameterName = $"@param{_parameterIndex++}";

                DbParameter DbParameter = new SqlParameter(parameterName, GetSqlDbType(literal));
                DbParameter.Value = GetValue(literal);
                DbParameters.Add(DbParameter);

                // Replace the literal with a parameter
                node.SecondExpression = new VariableReference { Name = parameterName };
            }
        }

        private object GetValue(Literal literal)
        {
            if (literal is IntegerLiteral intLiteral)
            {
                return int.Parse(intLiteral.Value);
            }
            else if (literal is StringLiteral stringLiteral)
            {
                return stringLiteral.Value;
            }
            else if (literal is NumericLiteral numericLiteral)
            {
                return decimal.Parse(numericLiteral.Value);
            }
            else if (literal is MoneyLiteral moneyLiteral)
            {
                return decimal.Parse(moneyLiteral.Value);
            }
            else if (literal is RealLiteral realLiteral)
            {
                return double.Parse(realLiteral.Value);
            }
            else if (literal is NullLiteral)
            {
                return DBNull.Value;
            }
            throw new NotSupportedException("Unsupported literal type.");
        }

        private SqlDbType GetSqlDbType(Literal literal)
        {
            if (literal is IntegerLiteral)
            {
                return SqlDbType.Int;
            }
            else if (literal is StringLiteral)
            {
                return SqlDbType.NVarChar;
            }
            else if (literal is NumericLiteral || literal is MoneyLiteral)
            {
                return SqlDbType.Decimal;
            }
            else if (literal is RealLiteral)
            {
                return SqlDbType.Float;
            }
            else if (literal is NullLiteral)
            {
                return SqlDbType.Variant;
            }
            throw new NotSupportedException("Unsupported literal type.");
        }
    }
}

