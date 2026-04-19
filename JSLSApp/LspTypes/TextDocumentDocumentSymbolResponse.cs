namespace JSLSApp.LspTypes;

public class TextDocumentDocumentSymbolResponse
{
    public TextDocumentDocumentSymbolResponse(TextDocumentDocumentSymbolResponseResult result)
    {
        this.result = result;
    }

    public TextDocumentDocumentSymbolResponseResult result { get; set; }
}
