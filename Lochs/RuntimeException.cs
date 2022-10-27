namespace Lochs
{
    public class RuntimeException : Exception
    {
        public Token Token { get; }

        public RuntimeException(Token token, string message)
            : base(message)
        {
            Token = token;
        }
    }
}
