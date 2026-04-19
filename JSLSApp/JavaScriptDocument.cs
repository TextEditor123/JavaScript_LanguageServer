using JSLSApp.LspTypes;

namespace JSLSApp;

public class JavaScriptDocument
{
    public JavaScriptDocument(char[] charArray)
    {
        Chars = charArray;
    }

    public char[] Chars { get; }
    public bool HasBeenParsedAtLeastOnce { get; set; }
    public JavaScriptCompilationUnit CompilationUnit { get; set; } = new JavaScriptCompilationUnit(new List<Position>());
}
