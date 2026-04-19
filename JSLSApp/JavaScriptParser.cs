using JSLSApp.LspTypes;

namespace JSLSApp;

public class JavaScriptParser
{
    public JavaScriptCompilationUnit Parse(JavaScriptDocument doc)
    {
        doc.HasBeenParsedAtLeastOnce = true;

        var functionDefinitionStartPositionList = new List<Position>();

        var pos = 0;
        var indexLine = 0;
        var indexChar = 0;

        while (pos < doc.Chars.Length)
        {
            switch (doc.Chars[pos])
            {
                case '\r':
                    indexChar = 0;
                    indexLine++;
                    if (doc.Chars.Length - pos >= 2)
                    {
                        if (doc.Chars[pos + 1] == '\n')
                            pos++;
                    }
                    break;
                case '\n':
                    indexChar = 0;
                    indexLine++;
                    break;
                case 'f':
                    // TODO: This isn't optimal (and is incorrect given the lack of contextual information) but I want to get a "proof of concept"...
                    // ...by getting a list of all the functions and then goto definition-ing one or something like this.
                    if (doc.Chars.Length - pos >= 8) /* 8 is length of "function" keyword */
                    {
                        if (doc.Chars[pos + 1] == 'u' &&
                            doc.Chars[pos + 2] == 'n' &&
                            doc.Chars[pos + 3] == 'c' &&
                            doc.Chars[pos + 4] == 't' &&
                            doc.Chars[pos + 5] == 'i' &&
                            doc.Chars[pos + 6] == 'o' &&
                            doc.Chars[pos + 7] == 'n')
                        {
                            functionDefinitionStartPositionList.Add(new Position { line = indexLine, character = indexChar });
                        }
                    }
                    goto default;
                default:
                    indexChar++;
                    break;
            }

            pos++;
        }

        return new JavaScriptCompilationUnit(functionDefinitionStartPositionList);
    }
}
