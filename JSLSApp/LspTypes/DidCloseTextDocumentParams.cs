namespace JSLSApp.LspTypes;

class DidCloseTextDocumentParams
{
    /**
	 * The document that was closed.
	 */
    public TextDocumentIdentifier textDocument { get; set; }
}
