namespace Lochs.AutoGenerated.Ast
{
	public interface IExpressionVisitor<T> 
	{
		T VisitTernary(Ternary ternary); 
		T VisitAssign(Assign assign); 
		T VisitBinary(Binary binary); 
		T VisitGrouping(Grouping grouping); 
		T VisitLiteral(Literal literal); 
		T VisitUnary(Unary unary); 
		T VisitVariable(Variable variable); 
 
	}

	public abstract class Expr 
	{ 
		public abstract T Accept<T>(IExpressionVisitor<T> visitor); 
	}


	public class Ternary : Expr
	{
		public Expr Condition { get; set; }
		public Expr ResultIfTrue { get; set; }
		public Expr ResultIfFalse { get; set; }
		public Ternary
		(
			Expr @condition, Expr @resultIfTrue, Expr @resultIfFalse
		) 
		{
			this.Condition = @condition;
			this.ResultIfTrue = @resultIfTrue;
			this.ResultIfFalse = @resultIfFalse;
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor) 
		{
			return visitor.VisitTernary(this); 
		}
	}
        

	public class Assign : Expr
	{
		public Token Name { get; set; }
		public Expr Value { get; set; }
		public Assign
		(
			Token @name, Expr @value
		) 
		{
			this.Name = @name;
			this.Value = @value;
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor) 
		{
			return visitor.VisitAssign(this); 
		}
	}
        

	public class Binary : Expr
	{
		public Expr Left { get; set; }
		public Token Operator { get; set; }
		public Expr Right { get; set; }
		public Binary
		(
			Expr @left, Token @operator, Expr @right
		) 
		{
			this.Left = @left;
			this.Operator = @operator;
			this.Right = @right;
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor) 
		{
			return visitor.VisitBinary(this); 
		}
	}
        

	public class Grouping : Expr
	{
		public Expr Expression { get; set; }
		public Grouping
		(
			Expr @expression
		) 
		{
			this.Expression = @expression;
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor) 
		{
			return visitor.VisitGrouping(this); 
		}
	}
        

	public class Literal : Expr
	{
		public object Value { get; set; }
		public Literal
		(
			object @value
		) 
		{
			this.Value = @value;
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor) 
		{
			return visitor.VisitLiteral(this); 
		}
	}
        

	public class Unary : Expr
	{
		public Token Operator { get; set; }
		public Expr Right { get; set; }
		public Unary
		(
			Token @operator, Expr @right
		) 
		{
			this.Operator = @operator;
			this.Right = @right;
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor) 
		{
			return visitor.VisitUnary(this); 
		}
	}
        

	public class Variable : Expr
	{
		public Token Name { get; set; }
		public Variable
		(
			Token @name
		) 
		{
			this.Name = @name;
		}

		public override T Accept<T>(IExpressionVisitor<T> visitor) 
		{
			return visitor.VisitVariable(this); 
		}
	}
        
	public abstract class Stmt
	{ 
		public abstract void Accept(IStatementVisitor visitor); 
	}


	public interface IStatementVisitor
	{
		void VisitStatementExpression(StatementExpression statementexpression); 
		void VisitStatementPrint(StatementPrint statementprint); 
		void VisitVar(Var var); 
 
	}




	public class StatementExpression : Stmt
	{
		public Expr Expression { get; set; }
		public StatementExpression
		(
			Expr @expression
		) 
		{
			this.Expression = @expression;
		}

		public override void Accept(IStatementVisitor visitor) 
		{
			visitor.VisitStatementExpression(this); 
		}
	}
        

	public class StatementPrint : Stmt
	{
		public Expr Expression { get; set; }
		public StatementPrint
		(
			Expr @expression
		) 
		{
			this.Expression = @expression;
		}

		public override void Accept(IStatementVisitor visitor) 
		{
			visitor.VisitStatementPrint(this); 
		}
	}
        

	public class Var : Stmt
	{
		public Token Name { get; set; }
		public Expr Initializer { get; set; }
		public Var
		(
			Token @name, Expr @initializer
		) 
		{
			this.Name = @name;
			this.Initializer = @initializer;
		}

		public override void Accept(IStatementVisitor visitor) 
		{
			visitor.VisitVar(this); 
		}
	}
        
}


