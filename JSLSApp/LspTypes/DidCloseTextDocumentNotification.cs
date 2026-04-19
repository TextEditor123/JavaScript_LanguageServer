namespace JSLSApp.LspTypes;

public class DidCloseTextDocumentNotification
{
    public string? method { get; set; }
    public DidCloseTextDocumentParams? @params { get; set; }

}
