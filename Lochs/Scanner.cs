namespace Lochs;

public class Scanner
{
    private readonly string _source;
    private readonly IErrorReporter _errorReporter;
    private readonly List<Token> _tokens;
    private int _start = 0;
    private int _current = 0;
    private int _line = 0;

    public Scanner(string source, IErrorReporter errorReporter)
    {
        _source = source;
        _errorReporter = errorReporter;
        _tokens = new List<Token>();
    }

    public IEnumerable<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            // This is at the beginning of the next lexeme
            _start = _current;

            ScanToken();
        }
        
        _tokens.Add(new Token(TokenType.Eof, string.Empty, null, _line));

        // Why? 
        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(': 
                AddToken(TokenType.LeftParen);
                break;
            case ')': 
                AddToken(TokenType.RightParen);
                break;
            case '{': 
                AddToken(TokenType.LeftBrace);
                break;
            case '}': 
                AddToken(TokenType.RightBrace);
                break;
            case ',': 
                AddToken(TokenType.Comma);
                break;
            case '.': 
                AddToken(TokenType.Dot);
                break;
            case '-': 
                AddToken(TokenType.Minus);
                break;
            case '+': 
                AddToken(TokenType.Plus);
                break;
            case ';': 
                AddToken(TokenType.Semicolon);
                break;
            case '*': 
                AddToken(TokenType.Star);
                break;

            default:
                _errorReporter.Error(_line, "Unexpected token");
                break; 
        }
    }

    private bool IsAtEnd()
        => _current >= _source.Length;

    private char Advance()
        => _source[_current++];

    private void AddToken(TokenType tokenType)
        => AddToken(tokenType, null!);

    private void AddToken(TokenType tokenType, object literal)
    {
        var text = _source.Substring(_start, _current);
        var token = new Token(tokenType, text, literal, _line);
        _tokens.Add(token);
    }
}