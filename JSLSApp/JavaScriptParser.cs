using JSLSApp.LspTypes;

namespace JSLSApp;

/// <summary>
/// Current intention is 1 parser instance per document.
/// This is possibly GC heavy design.
/// But I'm thinking I'll only have an instance for an "open" file.
/// And that the parser would understand the edits made to the document to re-parse the document quickly.
/// And if needed, a CompilationUnit would be the representation of the document semantically,
/// and this CompilationUnit would exist independent of a file being "open".
/// </summary>
public class JavaScriptParser
{
    private JavaScriptDocument _doc;
    private int _pos = 0;
    private int _indexLine = 0;
    private int _indexChar = 0;
    private bool IsEof => _pos >= _doc.Chars.Length;
    private List<Position> _functionDefinitionStartPositionList = new List<Position>();

    public JavaScriptParser(JavaScriptDocument doc)
    {
        _doc = doc;
    }

    /// <summary>
    /// TODO: Look up the documented ways to parse something.
    /// i.e.: I think I recall "recursive descent parsing" and such.
    /// what are these things... for now this is gonna be quickly written and only accurate to the degree that I need it to be given the circumstances.
    /// </summary>
    public JavaScriptCompilationUnit Parse()
    {
        _doc.HasBeenParsedAtLeastOnce = true;

        while (_pos < _doc.Chars.Length)
        {
            var token = Lex();
            switch (token.SyntaxKind)
            {
                case SyntaxKind.EndOfFileToken:
                    goto exitOuterWhileLoop;
                case SyntaxKind.FunctionKeywordToken:
                    break;
                case SyntaxKind.IdentifierToken:
                    break;
                case SyntaxKind.WhitespaceToken:
                    break;
            }
        }

        exitOuterWhileLoop:
        return new JavaScriptCompilationUnit(_functionDefinitionStartPositionList);
    }

    public SyntaxToken Lex()
    {
        while (_pos < _doc.Chars.Length)
        {
            switch (_doc.Chars[_pos])
            {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '_':
                    return Lex_IdentifierOrKeyword();
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return Lex_Number();
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    return Lex_Whitespace();
                //default:
                //    return CharacterKind.Punctuation;
            }

            _pos++;
        }

        return new SyntaxToken(SyntaxKind.EndOfFileToken, new Position(_indexLine, _indexChar), 0);
    }

    /// <summary>
    /// TODO: Usage of reserved words with '@' prefix
    /// </summary>
    public SyntaxToken Lex_IdentifierOrKeyword()
    {
        var charIntSum = (int)_doc.Chars[_pos];
        var startPosition = new Position(_indexLine, _indexChar);
        var length = 1;
        _pos++;

        while (_pos < _doc.Chars.Length)
        {
            if (char.IsLetterOrDigit(_doc.Chars[_pos]))
            {
                length++;
                charIntSum += _doc.Chars[_pos];
            }
            else
            {
                if (_doc.Chars[_pos] == '_')
                {
                    length++;
                    charIntSum += _doc.Chars[_pos];
                }
                else
                {
                    break;
                }
            }

            _pos++;
        }

        if (charIntSum == 870)
        {
            if (length == 8 &&
                _doc.Chars[_pos - 8] == 102 /* 'f' */ &&
                _doc.Chars[_pos - 7] == 117 /* 'u' */ &&
                _doc.Chars[_pos - 6] == 110 /* 'n' */ &&
                _doc.Chars[_pos - 5] == 99  /* 'c' */ &&
                _doc.Chars[_pos - 4] == 116 /* 't' */ &&
                _doc.Chars[_pos - 3] == 105 /* 'i' */ &&
                _doc.Chars[_pos - 2] == 111 /* 'o' */ &&
                _doc.Chars[_pos - 1] == 110 /* 'n' */)
            {
                _functionDefinitionStartPositionList.Add(startPosition);
            }
        }

        return new SyntaxToken(SyntaxKind.IdentifierToken, startPosition, length);
    }

    /// <summary>
    /// TODO: alternative syntaxes for typing numbers; supports '123' and '123.456'
    /// </summary>
    public SyntaxToken Lex_Number()
    {
        var startPosition = new Position(_indexLine, _indexChar);
        var length = 1;
        _pos++;

        while (_pos < _doc.Chars.Length)
        {
            if (char.IsDigit(_doc.Chars[_pos]))
            {
                length++;
            }
            else
            {
                if (_doc.Chars[_pos] == '.')
                {
                    length++;
                }
                else
                {
                    break;
                }
            }

            _pos++;
        }

        return new SyntaxToken(SyntaxKind.NumberToken, startPosition, length);
    }
    
    public SyntaxToken Lex_Whitespace()
    {
        var startPosition = new Position(_indexLine, _indexChar);
        var length = 1;
        _pos++;

        while (_pos < _doc.Chars.Length)
        {
            if (char.IsWhiteSpace(_doc.Chars[_pos]))
            {
                length++;
            }
            else
            {
                break;
            }

            _pos++;
        }

        return new SyntaxToken(SyntaxKind.NumberToken, startPosition, length);
    }
}
