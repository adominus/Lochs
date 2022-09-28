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

    public IList<Token> ScanTokens()
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
            case '!':
                AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                }
                else if (Match('*'))
                {
                    AddBlockComment();
                }
                else
                {
                    AddToken(TokenType.Slash);
                }

                break;

            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace
                break;

            case '\n':
                _line++;
                break;

            case '"':
                AddStringLiteral();
                break;

            default:
                if (IsDigit(c))
                {
                    AddNumber();
                }
                else if (IsAlphaOrUnderscore(c))
                {
                    AddIdentifier();
                    // LEFT OFF 4.7 Reserved Words and Identifiers 
                }
                else
                {
                    _errorReporter.Error(_line, "Unexpected token");
                }

                break;
        }
    }

    // Called identifier() in the book 
    private void AddIdentifier()
    {
        while (IsAlphaNumericOrUnderscore(Peek()))
            Advance();

        var identifier = _source.Substring(_start, _current - _start);
        var tokenType = TokenType.Identifier;
        if (!ReservedKeywords.TryGetValue(identifier, out tokenType))
        {
            tokenType = TokenType.Identifier;
        }
        
        AddToken(tokenType);
    }

    // Called number() in the book 
    private void AddNumber()
    {
        while (IsDigit(Peek()))
            Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek()))
                Advance();
        }

        var number = double.Parse(_source.Substring(_start, _current - _start));
        AddToken(TokenType.Number, number);
    }

    // Called string() in the book 
    private void AddStringLiteral()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
                _line++;

            Advance();
        }

        if (IsAtEnd())
        {
            _errorReporter.Error(_line, "Unterminated string");
            return;
        }

        // Closing " 
        Advance();

        var literal = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.String, literal);
    }

    private void AddBlockComment()
    {
        while (Peek() != '*' && 
               PeekNext() != '/' && 
               !IsAtEnd())
        {
            if (Peek() == '\n')
                _line++;

            Advance();
        }

        if (IsAtEnd())
        {
            _errorReporter.Error(_line, "Unterminated block comment");
            return; 
        }

        // Closing *
        Advance();

        if (IsAtEnd())
        {
            _errorReporter.Error(_line, "Unterminated block comment");
            return; 
        }
        
        // Closing /
        Advance();
        
        // Not recording it as it's a comment 
    }

    private bool Match(char c)
    {
        if (IsAtEnd())
            return false;

        if (_source[_current] != c)
            return false;

        _current++;
        return true;
    }

    private char Peek()
    {
        if (IsAtEnd())
            return '\0';

        return _source[_current];
    }

    private char PeekNext()
    {
        if (_current + 1 >= _source.Length)
            return '\0';

        return _source[_current + 1];
    }

    private bool IsAlphaOrUnderscore(char c)
        => (c >= 'a' && c <= 'z') ||
           (c >= 'A' && c <= 'Z') ||
           c == '_';

    private bool IsDigit(char c)
        => c >= '0' && c <= '9';

    private bool IsAlphaNumericOrUnderscore(char c)
        => IsAlphaOrUnderscore(c) || IsDigit(c);

    private bool IsAtEnd()
        => _current >= _source.Length;

    private char Advance()
        => _source[_current++];

    private void AddToken(TokenType tokenType)
        => AddToken(tokenType, null!);

    private void AddToken(TokenType tokenType, object literal)
    {
        var text = _source.Substring(_start, _current - _start);
        var token = new Token(tokenType, text, literal, _line);
        _tokens.Add(token);
    }

    private static IDictionary<string, TokenType> ReservedKeywords = new Dictionary<string, TokenType>()
    {
        {"and", TokenType.And},
        {"class", TokenType.Class},
        {"else", TokenType.Else},
        {"false", TokenType.False},
        {"fun", TokenType.Fun},
        {"for", TokenType.For},
        {"if", TokenType.If},
        {"nil", TokenType.Nil},
        {"or", TokenType.Or},
        {"print", TokenType.Print},
        {"return", TokenType.Return},
        {"super", TokenType.Super},
        {"this", TokenType.This},
        {"true", TokenType.True},
        {"var", TokenType.Var},
        {"while", TokenType.While},
    };
}