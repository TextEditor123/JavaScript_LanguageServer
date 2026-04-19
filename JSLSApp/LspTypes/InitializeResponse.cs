namespace JSLSApp.LspTypes;

/// <summary>
/// TODO: Replicate the typescipt interfaces that the protocol provides.
/// </summary>
class InitializeResponse
{
    public InitializeResponse(InitializeResponseResult result)
    {
        this.result = result;
    }

    public InitializeResponseResult result { get; set; }
}
