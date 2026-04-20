using JSLSApp.LspTypes;

namespace JSLSApp;

public struct SyntaxToken(SyntaxKind syntaxKind, Position position, int length)
{
    public SyntaxKind SyntaxKind { get; set; } = syntaxKind;
    public Position Position { get; set; } = position;
    public int Length { get; set; } = length;
}
