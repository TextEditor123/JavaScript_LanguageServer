namespace JSLSApp.LspTypes;

class DidOpenTextDocumentParams
{
    /**
	 * The document that was opened.
	 */
    public TextDocumentItem? textDocument { get; set; }
}
