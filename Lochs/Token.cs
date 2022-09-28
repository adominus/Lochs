namespace Lochs;

public class Token
{
    private readonly TokenType _tokenType;
    private readonly string _lexeme;
    private readonly object _literal;
    private readonly int _line;

    public Token(TokenType tokenType, string lexeme, object? literal, int line)
    {
        _tokenType = tokenType;
        _lexeme = lexeme;
        _literal = literal;
        _line = line;
    }

    public string Lexeme => _lexeme;

    public TokenType TokenType => _tokenType;

    public object Literal => _literal;

    public int Line => _line;

    public override string ToString()
        => $"{TokenType} {_lexeme} {Literal}";
}