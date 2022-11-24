﻿using System.Text;
using Lochs.AutoGenerated.Ast;

namespace Lochs
{
    public class PrettyPrinter : IAstPrinter, IExpressionVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitTernary(Ternary ternary)
            => Parenthesize($"{ternary.Condition.Accept(this)} ?", ternary.ResultIfTrue, new Literal(":"), ternary.ResultIfFalse);

        public string VisitBinary(Binary binary)
            => Parenthesize(binary.Operator.Lexeme, binary.Left, binary.Right);

        public string VisitGrouping(Grouping grouping)
            => Parenthesize("group", grouping.Expression);

        public string VisitLiteral(Literal literal)
            => literal.Value?.ToString() ?? "nil";

        public string VisitUnary(Unary unary)
            => Parenthesize(unary.Operator.Lexeme, unary.Right);

        private string Parenthesize(string name, params Expr[] expressions)
        {
            var sb = new StringBuilder();

            sb.Append("(");
            sb.Append(name);

            foreach (var expression in expressions)
            {
                sb.Append(" ");
                sb.Append(expression.Accept(this));
            }

            sb.Append(")");
            return sb.ToString();
        }
    }
}
