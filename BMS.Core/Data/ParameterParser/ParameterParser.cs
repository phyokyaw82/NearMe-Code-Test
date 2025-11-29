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

        // Generate modified SQL
        Sql150ScriptGenerator scriptGenerator = new Sql150ScriptGenerator();
        scriptGenerator.GenerateScript(fragment, out sqlQuery);

        return visitor.DbParameters;
    }

    private class ParameterReplacementVisitor : TSqlFragmentVisitor
    {
        private int _parameterIndex = 0;
        public List<DbParameter> DbParameters { get; private set; } = new List<DbParameter>();

        // Handle WHERE clauses
        public override void Visit(BooleanComparisonExpression node)
        {
            base.Visit(node);

            if (node.SecondExpression is Literal literal)
            {
                node.SecondExpression = ReplaceLiteral(literal);
            }
        }

        // Handle LIKE predicates
        public override void Visit(LikePredicate node)
        {
            base.Visit(node);

            if (node.SecondExpression is Literal literal)
            {
                node.SecondExpression = ReplaceLiteral(literal);
            }
        }

        // Handle INSERT statements
        public override void Visit(InsertStatement node)
        {
            base.Visit(node);

            var insertSpec = node.InsertSpecification;
            if (insertSpec?.InsertSource is ValuesInsertSource valuesSource)
            {
                foreach (var row in valuesSource.RowValues)
                {
                    for (int i = 0; i < row.ColumnValues.Count; i++)
                    {
                        if (row.ColumnValues[i] is Literal literal)
                        {
                            row.ColumnValues[i] = ReplaceLiteral(literal);
                        }
                    }
                }
            }
        }

        private VariableReference ReplaceLiteral(Literal literal)
        {
            string parameterName = $"@param{_parameterIndex++}";

            DbParameter dbParameter = new SqlParameter(parameterName, GetSqlDbType(literal))
            {
                Value = GetValue(literal)
            };
            DbParameters.Add(dbParameter);

            return new VariableReference { Name = parameterName };
        }

        private object GetValue(Literal literal)
        {
            if (literal is IntegerLiteral intLiteral)
                return int.Parse(intLiteral.Value);
            if (literal is StringLiteral stringLiteral)
                return stringLiteral.Value;
            if (literal is NumericLiteral numericLiteral)
                return decimal.Parse(numericLiteral.Value);
            if (literal is MoneyLiteral moneyLiteral)
                return decimal.Parse(moneyLiteral.Value);
            if (literal is RealLiteral realLiteral)
                return double.Parse(realLiteral.Value);
            if (literal is NullLiteral)
                return DBNull.Value;

            throw new NotSupportedException("Unsupported literal type.");
        }

        public override void Visit(UpdateStatement node)
        {
            base.Visit(node);

            var updateSpec = node.UpdateSpecification;
            if (updateSpec?.SetClauses != null)
            {
                foreach (var setClause in updateSpec.SetClauses)
                {
                    if (setClause is AssignmentSetClause assignment)
                    {
                        if (assignment.NewValue is Literal literal)
                        {
                            assignment.NewValue = ReplaceLiteral(literal);
                        }
                    }
                }
            }
            // WHERE clause is handled automatically by BooleanComparisonExpression visit
        }

        private SqlDbType GetSqlDbType(Literal literal)
        {
            if (literal is IntegerLiteral)
                return SqlDbType.Int;
            if (literal is StringLiteral)
                return SqlDbType.NVarChar;
            if (literal is NumericLiteral || literal is MoneyLiteral)
                return SqlDbType.Decimal;
            if (literal is RealLiteral)
                return SqlDbType.Float;
            if (literal is NullLiteral)
                return SqlDbType.Variant;

            throw new NotSupportedException("Unsupported literal type.");
        }
    }
}
