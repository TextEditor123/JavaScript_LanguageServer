namespace JSLSApp;

public struct SyntaxToken(SyntaxKind syntaxKind)
{
    public SyntaxKind SyntaxKind { get; set; } = syntaxKind;
}
