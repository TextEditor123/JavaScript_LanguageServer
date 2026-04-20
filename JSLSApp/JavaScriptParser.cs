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

    public JavaScriptCompilationUnit Parse()
    {
        /*doc.HasBeenParsedAtLeastOnce = true;
        var functionDefinitionStartPositionList = new List<Position>();

        while (true)
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
            }
        }

        exitOuterWhileLoop:*/
        return new JavaScriptCompilationUnit(_functionDefinitionStartPositionList);
    }

    /*public SyntaxToken Lex()
    {
        while (_pos < _doc.Chars.Length)
        {
            switch (_doc.Chars[_pos])
            {
                case '\r':
                    _indexChar = 0;
                    _indexLine++;
                    if (_doc.Chars.Length - _pos >= 2)
                    {
                        if (_doc.Chars[_pos + 1] == '\n')
                            _pos++;
                    }
                    break;
                case '\n':
                    _indexChar = 0;
                    _indexLine++;
                    break;
                case 'f':
                    // TODO: This isn't optimal (and is incorrect given the lack of contextual information) but I want to get a "proof of concept"...
                    // ...by getting a list of all the functions and then goto definition-ing one or something like this.
                    if (_doc.Chars.Length - _pos >= 8) /* 8 is length of "function" keyword *//*
                    {
                        if (_doc.Chars[_pos + 1] == 'u' &&
                            _doc.Chars[_pos + 2] == 'n' &&
                            _doc.Chars[_pos + 3] == 'c' &&
                            _doc.Chars[_pos + 4] == 't' &&
                            _doc.Chars[_pos + 5] == 'i' &&
                            _doc.Chars[_pos + 6] == 'o' &&
                            _doc.Chars[_pos + 7] == 'n')
                        {
                            _functionDefinitionStartPositionList.Add(new Position { line = _indexLine, character = _indexChar });
                        }
                    }
                    goto default;
                default:
                    _indexChar++;
                    break;
            }

            _pos++;
        }
    }*/
}
