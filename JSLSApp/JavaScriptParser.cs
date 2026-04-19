namespace JSLSApp;

public class JavaScriptParser
{
    public JavaScriptCompilationUnit Parse(JavaScriptDocument doc)
    {
        doc.HasBeenParsedAtLeastOnce = true;

        var functionDefinitionStartPositionList = new List<int>();

        var pos = 0;

        while (pos < doc.Chars.Length)
        {
            // TODO: This isn't optimal (and is incorrect given the lack of contextual information) but I want to get a "proof of concept"...
            // ...by getting a list of all the functions and then goto definition-ing one or something like this.
            if (doc.Chars[pos] == 'f' && doc.Chars.Length - pos >= 8) /* 8 is length of "function" keyword */
            {
                if (doc.Chars[pos + 1] == 'u' &&
                    doc.Chars[pos + 2] == 'n' &&
                    doc.Chars[pos + 3] == 'c' &&
                    doc.Chars[pos + 4] == 't' &&
                    doc.Chars[pos + 5] == 'i' &&
                    doc.Chars[pos + 6] == 'o' &&
                    doc.Chars[pos + 7] == 'n')
                {
                    functionDefinitionStartPositionList.Add(pos);
                }
            }

            pos++;
        }

        return new JavaScriptCompilationUnit(functionDefinitionStartPositionList);
    }
}
