namespace Lochs;

public class Token
{
    private readonly TokenType _tokenType;
    private readonly string _lexeme;
    private readonly object _literal;
    private readonly int _line;

    public Token(TokenType tokenType, string lexeme, object literal, int line)
    {
        _tokenType = tokenType;
        _lexeme = lexeme;
        _literal = literal;
        _line = line;
    }

    public override string ToString()
        => $"{_tokenType} {_lexeme} {_literal}";
}