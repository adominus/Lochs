﻿using Lochs.AutoGenerated.Ast;

namespace Lochs
{
    internal class Parser
    {
        private readonly IList<Token> _tokens;
        private readonly IErrorReporter _errorReporter;

        private int _current = 0;

        public Parser(
            IList<Token> tokens,
            IErrorReporter errorReporter)
        {
            _tokens = tokens;
            _errorReporter = errorReporter;
        }

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private Stmt Declaration()
        {
            try
            {
                if (ConsumeIfMatch(TokenType.Var))
                {
                    return VarDeclaration();
                }

                return Statement();
            }
            catch (ParserException _)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt Statement()
        {
            if (ConsumeIfMatch(TokenType.Print))
            {
                return PrintStatement();
            }

            return ExpressionStatement();
        }

        private Stmt PrintStatement()
        {
            var val = Expression();

            Consume(TokenType.Semicolon, "Expect ';' after value. ");

            return new StatementPrint(val);
        }

        private Stmt ExpressionStatement()
        {
            var val = Expression();

            Consume(TokenType.Semicolon, "Expect ';' after expression. ");

            return new StatementExpression(val);
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected token name");

            Expr initializer = null; 
            if (ConsumeIfMatch(TokenType.Equal))
            {
                initializer = Expression();
            }

            Consume(TokenType.Semicolon, "Expected semicolon after variable declaration");
            return new Var(name, initializer);  
        }

        private Expr Expression()
            => Ternary();

        private Expr Ternary()
        {
            var expr = Equality();

            // a ? b : ( c ? d : e ) 
            while (ConsumeIfMatch(TokenType.QuestionMark))
            {
                var resultIfTrue = Expression();

                Consume(TokenType.Colon, "Expected ':' after ternary");

                var resultIfFalse = Expression();

                expr = new Ternary(expr, resultIfTrue, resultIfFalse);
            }

            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();

            while (ConsumeIfMatch(TokenType.EqualEqual, TokenType.BangEqual))
            {
                var @operator = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            var expr = Term();

            while (ConsumeIfMatch(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                var @operator = Previous();
                Expr right = Term();
                expr = new Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Term()
        {
            var expr = Factor();

            while (ConsumeIfMatch(TokenType.Minus, TokenType.Plus))
            {
                var @operator = Previous();
                Expr right = Factor();
                expr = new Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            var expr = Unary();

            while (ConsumeIfMatch(TokenType.Star, TokenType.Slash))
            {
                var @operator = Previous();
                Expr right = Unary();
                expr = new Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (ConsumeIfMatch(TokenType.Bang, TokenType.Minus))
            {
                var @operator = Previous();
                Expr right = Unary();
                return new Unary(@operator, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (ConsumeIfMatch(TokenType.True))
                return new Literal(true);
            if (ConsumeIfMatch(TokenType.False))
                return new Literal(false);
            if (ConsumeIfMatch(TokenType.Nil))
                return new Literal(null);

            if (ConsumeIfMatch(TokenType.String, TokenType.Number))
                return new Literal(Previous().Literal);

            if (ConsumeIfMatch(TokenType.LeftParen))
            {
                var expr = Expression();
                Consume(TokenType.RightParen, "Expected ')' after expression");

                return new Grouping(expr);
            }

            if (ConsumeIfMatch(TokenType.Identifier))
                return new Variable(Previous());

            throw Error(Peek(), "Expect expression. ");
        }

        private Token Consume(TokenType tokenType, string errorIfDoesNotMatch)
        {
            if (Check(tokenType))
            {
                return Advance();
            }

            throw Error(Peek(), errorIfDoesNotMatch);
        }

        private ParserException Error(Token token, string message)
        {
            _errorReporter.Error(token, message);

            return new ParserException(message);
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().TokenType == TokenType.Semicolon)
                {
                    return;
                }

                switch (Peek().TokenType)
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.For:
                    case TokenType.Var:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        return;

                }

                Advance();
            }
        }

        private Token Previous()
            => _tokens[_current - 1];

        private Token Peek()
            => _tokens[_current];

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                _current++;
            }

            // TODO: Check - odd behavior if it is at end
            // Returns the last token before EOF 
            return Previous();
        }

        private bool IsAtEnd()
            => Peek().TokenType == TokenType.Eof;

        private bool Check(TokenType tokenType)
            => Peek().TokenType == tokenType;

        private bool ConsumeIfMatch(params TokenType[] tokenTypes)
        {
            if (tokenTypes.Any(Check))
            {
                Advance();
                return true;
            }

            return false;
        }
    }
}
