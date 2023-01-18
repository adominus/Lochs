﻿using Lochs.AutoGenerated.Ast;

namespace Lochs
{
    internal class Interpreter : IExpressionVisitor<object>, IStatementVisitor, IInterpreter
    {
        private readonly IErrorReporter _errorReporter;
        private EnvironmentMap _environment = new();

        public Interpreter(IErrorReporter errorReporter)
        {
            _errorReporter = errorReporter;
        }

        public string InterpretAndStringify(Expr expr)
        {
            var result = Evaluate(expr);

            return Stringify(result);
        }

        public void Interpret(IEnumerable<Stmt> statements)
        {
            try
            {
                foreach (var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (RuntimeException ex)
            {
                _errorReporter.Error(ex);
            }
        }

        public object VisitLiteral(Literal literal)
            => literal.Value;

        public object VisitGrouping(Grouping grouping)
            => Evaluate(grouping.Expression);

        public object VisitUnary(Unary unary)
        {
            var rhs = Evaluate(unary.Right);

            switch (unary.Operator.TokenType)
            {
                case TokenType.Minus:
                    CheckNumberOperand(unary.Operator, rhs);
                    return -(double)rhs;

                case TokenType.Bang:
                    return IsTruthy(rhs);
            }

            throw new InvalidOperationException("Unknown unary");
        }

        public object VisitVariable(Variable variable)
        {
            return _environment.Get(variable.Name);
        }

        public object VisitBinary(Binary binary)
        {
            var lhs = Evaluate(binary.Left);
            var rhs = Evaluate(binary.Right);

            switch (binary.Operator.TokenType)
            {
                case TokenType.Greater:
                    CheckNumberOperand(binary.Operator, lhs, rhs);
                    return (double)lhs > (double)rhs;

                case TokenType.GreaterEqual:
                    CheckNumberOperand(binary.Operator, lhs, rhs);
                    return (double)lhs >= (double)rhs;

                case TokenType.Less:
                    CheckNumberOperand(binary.Operator, lhs, rhs);
                    return (double)lhs < (double)rhs;

                case TokenType.LessEqual:
                    CheckNumberOperand(binary.Operator, lhs, rhs);
                    return (double)lhs <= (double)rhs;

                case TokenType.Minus:
                    CheckNumberOperand(binary.Operator, lhs, rhs);
                    return (double)lhs - (double)rhs;

                case TokenType.Slash:
                    CheckNumberOperand(binary.Operator, lhs, rhs);
                    return (double)lhs / (double)rhs;

                case TokenType.Star:
                    CheckNumberOperand(binary.Operator, lhs, rhs);
                    return (double)lhs * (double)rhs;

                case TokenType.Plus:
                    if (lhs is double lDouble && rhs is double rDouble)
                    {
                        return lDouble + rDouble;
                    }
                    else if (lhs is string lString && rhs is string rString)
                    {
                        return lString + rString;
                    }

                    throw new RuntimeException(binary.Operator, "Operands must be 2 numbers or 2 strings. ");

                case TokenType.BangEqual:
                    return !IsEqual(lhs, rhs);

                case TokenType.EqualEqual:
                    return IsEqual(lhs, rhs);
            }

            throw new InvalidOperationException("Unknown binary");
        }

        public object VisitTernary(Ternary ternary)
        {
            var conditionResult = Evaluate(ternary.Condition);

            if (conditionResult is bool result)
            {
                if (result)
                {
                    return Evaluate(ternary.ResultIfTrue);
                }

                return Evaluate(ternary.ResultIfFalse);
            }

            throw new InvalidOperationException("Unknown ternary");
        }

        public void VisitStatementExpression(StatementExpression statementexpression)
        {
            Evaluate(statementexpression.Expression);
        }

        public void VisitStatementPrint(StatementPrint statementprint)
        {
            var result = Evaluate(statementprint.Expression);
            Console.WriteLine(Stringify(result));
        }

        public void VisitStatementBlock(StatementBlock statementblock)
        {
            var environment = new EnvironmentMap(_environment);
            ExecuteBlock(statementblock.Statements, environment);
        }

        public void VisitVar(Var varStatement)
        {
            object value = null;
            if (varStatement.Initializer != null)
            {
                value = Evaluate(varStatement.Initializer);
            }

            _environment.Define(varStatement.Name.Lexeme, value);
        }

        public object VisitAssign(Assign assign)
        {
            var result = Evaluate(assign.Value);

            _environment.Assign(assign.Name, result);

            return result; 
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is bool b)
            {
                return b;
            }

            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            if (a is double doubleA && b is double doubleB)
            {
                return doubleA == doubleB;
            }

            return a == b;
        }

        private void CheckNumberOperand(Token @operator, object operand)
        {
            if (operand is not double)
            {
                throw new RuntimeException(@operator, "Operand must be a number.");
            }
        }

        private void CheckNumberOperand(Token @operator, object leftOperand, object rightOperand)
        {
            if (leftOperand is not double || rightOperand is not double)
            {
                throw new RuntimeException(@operator, "Operands must be a number.");
            }
        }

        private object Evaluate(Expr expr)
            => expr.Accept(this);

        private void Execute(Stmt stmt)
            => stmt.Accept(this);

        private void ExecuteBlock(List<Stmt> statements, EnvironmentMap environment)
        {
            var previousEnvironment = _environment;

            try
            {
                _environment = environment;

                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                _environment = previousEnvironment;
            }
        }

        private static string Stringify(object o)
        {
            if (o == null)
            {
                return "nil";
            }

            if (o is double)
            {
                var s = o.ToString();
                if (s.EndsWith(".0"))
                {
                    return s.Substring(0, s.Length - 2);
                }

                return s;
            }

            return o.ToString();
        }
    }
}
