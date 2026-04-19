/*
 Do not forget to re-publish when applicable
 */

using JSLSApp;
using System.Text;
using System.Text.Json;

var stdoutChunkObjects = new List<StdoutChunkObject>();
var stdoutChunkFirstEntryMetadataSubstringIndexStart = 0;
var stdoutChunkFirstEntryMetadataContentLengthNumber = 0;

JavaScriptWorkspace? _javaScriptWorkspace = null;

// "random note": when lexing, it is 100% better to lex the members than lex the locals; in terms of syntax highlighting, because punctuation is the same color as member identifiers.

string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

if (homePath != "C:\\Users\\hunte")
{
    Console.WriteLine(@"if (homePath != ""C:\\Users\\hunte"")");
    return;
}

//using StreamWriter writer = new StreamWriter("C:\\Users\\hunte\\Repos\\file.txt");
var myPath = "C:\\Users\\hunte\\Repos\\file.txt";
File.WriteAllText(myPath, Environment.ProcessId.ToString() + '\n');

using StreamReader reader = new StreamReader(Console.OpenStandardInput());

while (true) // I'm getting the warning: "Do not use 'reader.EndOfStream' in an async method"
{
    //string? line = reader.ReadLine();
    /*int length = reader.ReadBlock(buffer); // hmm
    if (length > 0)
    {
        var str = new string(buffer, 0, length); // TODO: Don't do this work with the buffer directly.
        File.AppendAllText(myPath, str);
        File.AppendAllText(myPath, "\n====\n");
        MAIN_decodeMessage(str);
        // Process line
    }*/

    /*var text = await reader.ReadToEndAsync(); // hmm
    if (text is not null)
    {
        File.AppendAllText(myPath, text);
        File.AppendAllText(myPath, "\n====\n");
        MAIN_decodeMessage(text);
        // Process line
    }*/

    var text = reader.ReadLine(); // hmm
    File.AppendAllText(myPath, $"\n====reader.ReadLine()====\n");
    if (text is not null)
    {
        File.AppendAllText(myPath, text);
        File.AppendAllText(myPath, "\n====\n");
        MAIN_decodeMessage(text);
        // Process line
    }
}

/*
32060
Content-Length: 163
====

====
{"method":"initialize","id":0,"params":{"processId":13544,"clientInfo":{"name":"TextEditor123","version":"0.0.1"},"rootUri":"C:\\Users\\hunte\\Repos\\JavaScript"}}
====

*/

/**
 * @param {string} json 
 * @returns {object | null}
 * 
 * // TODO: you probably can reinvoke this method if you have extra unread content beyond the length needed to read the message
 * //     TODO: You could incrementally approach an optimized and correct answer by having this re-invocation for the time being just be a substring of the remaining text.
 * // TODO: Preferably neither of these would allocate a "substring" But they both will for the time being because I'm using JSON.parse and at the moment I know not of any other way than providing this a string.
*/
object? MAIN_decodeMessage(string json)
{
    File.AppendAllText(myPath, $"\n====MAIN_decodeMessage====\n");
    try
    {
        // I've seen both the header and content in a single 'MAIN_decodeMessage' while debugging.
        // But just the same I've seen only the header in a 'MAIN_decodeMessage' with a separate invocation for the content.
        //
        // So the seemingly non-deterministic nature of this is something to note.
        //
        // In both scenarios the total content "seemed" equivalent at a glance but I didn't do thorough checking

        //var json = jsonBytes.ToString(); // TODO: Don't toString() this, work with the bytes directly until the end (does JSON.parse take bytes as input? If so never have to do a toString()?).

        if (stdoutChunkObjects.Count == 0)
        {
            // Parse Content-Length
            var indexOfContentLengthToken = json.IndexOf("Content-Length: ");
            if (indexOfContentLengthToken == -1) return null;
            var substringIndexStart = indexOfContentLengthToken + 16; /* 16 === 'Content-Length: '.length */
            var substringIndexEnd = substringIndexStart;
            for (; substringIndexEnd < json.Length; substringIndexEnd++)
            {
                switch (json[substringIndexEnd])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                    default:
                        goto afterOuterForLoop;
                }
            }
            afterOuterForLoop:
            if (substringIndexEnd == substringIndexStart) return null;
            var contentLengthString = json.Substring(substringIndexStart, substringIndexEnd - substringIndexStart);
            if (!int.TryParse(contentLengthString, out var contentLengthNumber))
            {
                File.AppendAllText(myPath, $"\n====if (!int.TryParse(contentLengthString, out var contentLengthNumber))====\n");
                return null;
            }

            File.AppendAllText(myPath, $"\n====contentLengthNumber:{contentLengthNumber}====\n");

            // Parse Content
            var indexOfSearchTerm = json.IndexOf("\r\n\r\n");
            File.AppendAllText(myPath, $"\n====indexOfSearchTerm:{indexOfSearchTerm}====\n");
            if (indexOfSearchTerm == -1) {
                // TODO: This is a little scuffed because readline is losing the line endings that delimiter header from content...
                // ...
                File.AppendAllText(myPath, $"\n====indexOfSearchTerm == -1-delaying====\n");
                // ... continue delaying
                stdoutChunkObjects.Add(new StdoutChunkObject(json));
                stdoutChunkFirstEntryMetadataSubstringIndexStart = json.Length;
                stdoutChunkFirstEntryMetadataContentLengthNumber = contentLengthNumber;
                return null;
            }
            substringIndexStart = indexOfSearchTerm + 4; /* 4 === "\r\n\r\n".length */

            // Payload
            if (substringIndexStart + contentLengthNumber <= json.Length)
            {
                // ... read
                var content = json.Substring(substringIndexStart, (substringIndexStart + contentLengthNumber) - substringIndexStart);
                File.AppendAllText(myPath, $"\n====single-event-content:{content}====\n");
                return DeserializeContent(content);
            }
            else
            {
                File.AppendAllText(myPath, $"\n====single-event-continue-delaying====\n");
                // ... continue delaying
                stdoutChunkObjects.Add(new StdoutChunkObject(json));

                stdoutChunkFirstEntryMetadataSubstringIndexStart = substringIndexStart;
                stdoutChunkFirstEntryMetadataContentLengthNumber = contentLengthNumber;
                return null;
            }
        }
        else
        {
            // Parse Content
            // 0th
            var sumUnreadStdout = stdoutChunkObjects[0].BytesDecoded.Length - stdoutChunkFirstEntryMetadataSubstringIndexStart; // initialize to the remaining length that was in the first message of the batch

            // >first && <last
            for (var i = 1; i < stdoutChunkObjects.Count; i++)
            { // TODO: You could determine the necessary length of the NEXT chunk that will cause the necessary length requirement to be met then avoid an 'n complexity' and just have 'constant'.
              // TODO: Further commenting about determining the necessary length of the NEXT chunk, that is what the original 'if' block is doing on the first message. Perhaps these two conditional branches are equivalent when following a "necessary length" implementation.
                sumUnreadStdout += stdoutChunkObjects[i].BytesDecoded.Length;
            }

            // current
            sumUnreadStdout += json.Length;

            // Payload
            if (stdoutChunkFirstEntryMetadataContentLengthNumber <= sumUnreadStdout)
            {
                // ... read
                var builder = new StringBuilder();

                // 0th
                var lenZeroth = stdoutChunkObjects[0].BytesDecoded.Length - stdoutChunkFirstEntryMetadataSubstringIndexStart;
                if (lenZeroth != 0)
                {
                    var zerothSubstring = stdoutChunkObjects[0].BytesDecoded.Substring(stdoutChunkFirstEntryMetadataSubstringIndexStart, stdoutChunkObjects[0].BytesDecoded.Length);
                    builder.Append(zerothSubstring); // initialize to the remaining length that was in the first message of the batch
                }

                // >first && <last
                for (var i = 1; i < stdoutChunkObjects.Count; i++)
                { // TODO: You could determine the necessary length of the NEXT chunk that will cause the necessary length requirement to be met then avoid an 'n complexity' and just have 'constant'.
                  // TODO: Further commenting about determining the necessary length of the NEXT chunk, that is what the original 'if' block is doing on the first message. Perhaps these two conditional branches are equivalent when following a "necessary length" implementation.
                    builder.Append(stdoutChunkObjects[i].BytesDecoded);
                }

                // current
                builder.Append(json);

                var joinedJson = builder.ToString();

                stdoutChunkObjects.Clear(); // TODO: clear the array entries to permit garbage collection (since stdoutChunkObjects is always in the app's scope any entries would as well never be collected)

                string content;

                if (joinedJson.Length == stdoutChunkFirstEntryMetadataContentLengthNumber)
                {
                    content = joinedJson;
                }
                else
                {
                    content = joinedJson.Substring(0, stdoutChunkFirstEntryMetadataContentLengthNumber - 0);
                    // I can't decide on what to put here, at the end of the day just make sure this case has something instrusive so its incompleteness isn't swept under the rug
                    // maybe I should throw an error I can't describe how "confused" I am at the moment I am just pushing to make progress with every last bit of energy I have
                    // and all the anxiety and decisions i.e.: you get a message box idk
                    throw new NotImplementedException();
                }

                File.AppendAllText(myPath, $"\n====multi-event-content:{content}====\n");
                return DeserializeContent(content);

            }
            else
            {
                File.AppendAllText(myPath, $"\n====multi-event-continue-delaying====\n");
                // ... continue delaying
                stdoutChunkObjects.Add(new StdoutChunkObject(json));
                return null;
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return null;
    }
}

string MAIN_encodeMessageObject(object messageObject)
{
    var content = JsonSerializer.Serialize(messageObject);
    var spacing = "\r\n\r\n";
    return $"Content-Length: {content.Length}{spacing}{content}";
}

object? DeserializeContent(string content)
{
    File.AppendAllText(myPath, $"\n====DeserializeContent====\n");
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var request = JsonSerializer.Deserialize<Message>(content, options);
    if (request is null)
    {
        File.AppendAllText(myPath, $"\n====request is null====\n");
    }
    if (!string.IsNullOrWhiteSpace(request?.Method))
    {
        File.AppendAllText(myPath, $"\n====request?.Method:{request?.Method}====\n");
    }
    else
    {
        File.AppendAllText(myPath, $"\n====ELSE====\n");
    }
    switch (request?.Method)
    {
        case "initialize":
            var initializeRequest = JsonSerializer.Deserialize<InitializeRequest>(content);
            File.AppendAllText(myPath, $"\n====Id:{initializeRequest?.Id ?? -123}====\n");
            if (!string.IsNullOrWhiteSpace(initializeRequest?.@params?.rootUri))
            {
                File.AppendAllText(myPath, $"\n====initializeRequest?.Params?.RootUri:{initializeRequest?.@params?.rootUri}====\n");
                _javaScriptWorkspace = new JavaScriptWorkspace(initializeRequest?.@params?.rootUri);
                File.AppendAllText(myPath, $"\n====_javaScriptWorkspace.SourceFileList.Count:{_javaScriptWorkspace.SourceFileAbsolutePathList.Count}====\n");
                // ====_javaScriptWorkspace.SourceFileList.Count:12====
            }
            else
            {
                File.AppendAllText(myPath, $"\n====initializeRequest?.Params?.RootUri:null====\n");
            }
            var initializeResponse = new InitializeResponse(new InitializeResponseResult());
            Console.Out.WriteLine(MAIN_encodeMessageObject(initializeResponse));
            return initializeRequest;
        case "textDocument/didOpen":
            var didOpenTextDocumentNotification = JsonSerializer.Deserialize<DidOpenTextDocumentNotification>(content);
            var p = didOpenTextDocumentNotification?.@params is null ? "null" : "nn";
            File.AppendAllText(myPath, $"\n====dotdn...params:{p}====\n");
            var td = didOpenTextDocumentNotification?.@params?.textDocument is null ? "null" : "nn";
            File.AppendAllText(myPath, $"\n====dotdn...textDocument:{td}====\n");
            File.AppendAllText(myPath, $"\n====dotdn...uri:{didOpenTextDocumentNotification?.@params?.textDocument?.uri ?? "null"}====\n");
            if (didOpenTextDocumentNotification?.@params?.textDocument?.uri is not null)
            {
                if (_javaScriptWorkspace is null)
                {
                    File.AppendAllText(myPath, $"\n====_javaScriptWorkspace is null====\n");
                }
                else
                {
                    File.AppendAllText(myPath, $"\n====_javaScriptWorkspace is NOT null====\n");
                    _javaScriptWorkspace.DidOpenTextDocumentNotification(
                        myPath,
                        didOpenTextDocumentNotification?.@params?.textDocument?.uri,
                        didOpenTextDocumentNotification?.@params?.textDocument?.text);
                }
            }
            return didOpenTextDocumentNotification;
        case "textDocument/didClose":
            var didCloseTextDocumentNotification = JsonSerializer.Deserialize<DidCloseTextDocumentNotification>(content);
            File.AppendAllText(myPath, $"\n____====DidCloseTextDocumentNotification {didCloseTextDocumentNotification?.@params?.textDocument?.uri ?? "null"} ====\n");
            if (_javaScriptWorkspace is null)
            {
                File.AppendAllText(myPath, $"\n====_javaScriptWorkspace is null====\n");
            }
            else
            {
                File.AppendAllText(myPath, $"\n____====_javaScriptWorkspace is NOT null====\n");
                _javaScriptWorkspace.DidCloseTextDocumentNotification(myPath, didCloseTextDocumentNotification?.@params?.textDocument?.uri);
            }
            return request;
        case "textDocument/documentSymbol":
            File.AppendAllText(myPath, $"\n====RECEIVED DOCUMENT SYMBOL====\n");
            return request;
        default:
            return request;
    }
}

class StdoutChunkObject
{
    public StdoutChunkObject(string bytesDecoded)
    {
        BytesDecoded = bytesDecoded;
    }

    public string BytesDecoded { get; }
}

/*
{
    "method": "initialize",
    "id": 0,
    "params": {
        "processId": 31892,
        "clientInfo": {
            "name": "TextEditor123",
            "version": "0.0.1"
        },
        "rootUri": "C:\\Users\\hunte\\Repos\\JavaScript"
    }
}
 */

/// <summary>
/// TODO: Replicate the typescipt interfaces that the protocol provides.
/// </summary>
class Message
{
    public string? Method { get; set; }
}

/// <summary>
/// TODO: Replicate the typescipt interfaces that the protocol provides.
/// </summary>
class InitializeRequest
{
    public string? Method { get; set; }
    public int Id { get; set; }
    public InitializeRequestParams? @params { get; set; }
}

/// <summary>
/// TODO: Replicate the typescipt interfaces that the protocol provides.
/// </summary>
class InitializeRequestParams
{
    public int ProcessId { get; set; }
    public InitializeRequestParams_clientInfo? ClientInfo { get; set; }
    public string? rootUri { get; set; }
}

/// <summary>
/// TODO: Replicate the typescipt interfaces that the protocol provides.
/// </summary>
class InitializeRequestParams_clientInfo
{
    public string? Name { get; set; }
    public string? Version { get; set; }
}

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

/// <summary>
/// TODO: Replicate the typescipt interfaces that the protocol provides.
/// </summary>
class InitializeResponseResult
{
    public int capabilities { get; set; } = 1;
}

class DidOpenTextDocumentNotification
{
    public string? method { get; set; }
    public DidOpenTextDocumentParams? @params { get; set; }

}

class DidOpenTextDocumentParams
{
    /**
	 * The document that was opened.
	 */
    public TextDocumentItem? textDocument { get; set; }
}

class DidCloseTextDocumentNotification
{
    public string? method { get; set; }
    public DidCloseTextDocumentParams? @params { get; set; }

}

class DidCloseTextDocumentParams
{
    /**
	 * The document that was closed.
	 */
    public TextDocumentIdentifier textDocument { get; set; }
}

class TextDocumentIdentifier
{
    /**
	 * The text document's URI.
	 */
    public string uri { get; set; }
}

class TextDocumentItem
{
    /**
	 * The text document's URI.
	 */
    public string? uri { get; set; }

    /**
	 * The text document's language identifier.
	 */
    public string? languageId { get; set; }

    /**
	 * The version number of this document (it will increase after each
	 * change, including undo/redo).
	 */
    public int version { get; set; }

    /**
	 * The content of the opened text document.
	 */
    public string? text { get; set; }
}

class DocumentSymbol
{
    /**
	 * The name of this symbol. Will be displayed in the user interface and
	 * therefore must not be an empty string or a string only consisting of
	 * white spaces.
	 */
    public string name { get; set; }

    /**
	 * More detail for this symbol, e.g the signature of a function.
	 */
    public string detail { get; set; }

    /**
	 * The kind of this symbol.
	 */
    public SymbolKind kind { get; set; }

    /**
	 * Tags for this document symbol.
	 *
	 * @since 3.16.0
	 */
    public SymbolTag[]? tags { get; set; }

    /**
	 * Indicates if this symbol is deprecated.
	 *
	 * @deprecated Use tags instead
	 */
    public bool deprecated { get; set; }

    /**
	 * The range enclosing this symbol not including leading/trailing whitespace
	 * but everything else like comments. This information is typically used to
	 * determine if the clients cursor is inside the symbol to reveal it  in the
	 * UI.
	 */
    public Range range { get; set; }

    /**
	 * The range that should be selected and revealed when this symbol is being
	 * picked, e.g. the name of a function. Must be contained by the `range`.
	 */
    public Range selectionRange { get; set; }

    /**
	 * Children of this symbol, e.g. properties of a class.
	 */
    public DocumentSymbol[] children { get; set; }
}

enum SymbolKind
{
    File = 1,
    Module = 2,
    Namespace = 3,
    Package = 4,
    Class = 5,
    Method = 6,
    Property = 7,
    Field = 8,
    Constructor = 9,
    Enum = 10,
    Interface = 11,
    Function = 12,
    Variable = 13,
    Constant = 14,
    String = 15,
    Number = 16,
    Boolean = 17,
    Array = 18,
    Object = 19,
    Key = 20,
    Null = 21,
    EnumMember = 22,
    Struct = 23,
    Event = 24,
    Operator = 25,
    TypeParameter = 26,
}

/**
 * Symbol tags are extra annotations that tweak the rendering of a symbol.
 *
 * @since 3.16
 */
enum SymbolTag
{
    /**
	 * Render a symbol as obsolete, usually using a strike-out.
	 */
    Deprecated = 1,
}

class Range
{
    /**
	 * The range's start position.
	 */
    public Position start { get; set; }

    /**
	 * The range's end position.
	 */
    public Position end { get; set; }
}

class Position
{
    /**
	 * Line position in a document (zero-based).
	 * 
	 * This is a comment from myself not the docs:
	 *     TODO: consider uint because the docs specifically said 'uinteger'
	 */
    public int line { get; set; }

    /**
	 * Character offset on a line in a document (zero-based). The meaning of this
	 * offset is determined by the negotiated `PositionEncodingKind`.
	 *
	 * If the character value is greater than the line length it defaults back
	 * to the line length.
	 * 
	 * This is a comment from myself not the docs:
	 *     TODO: consider uint because the docs specifically said 'uinteger'
	 */
    public int character { get; set; }
}
