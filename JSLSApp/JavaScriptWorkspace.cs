using JSLSApp.LspTypes;

namespace JSLSApp;

public class JavaScriptWorkspace
{
    /// <summary>
    /// CAREFUL: EITHER THIS OR _workspaceFolders
    /// </summary>
    private readonly string? _rootAbsolutePath;
    /// <summary>
    /// CAREFUL: EITHER THIS OR _rootAbsolutePath
    /// </summary>
    private readonly List<WorkspaceFolder>? _workspaceFolders;

    public List<string> SourceFileAbsolutePathList { get; } = new();
    public Dictionary<string, JavaScriptDocument> OpenedSourceFileAbsolutePathToInMemoryContentMap { get; set; } = new();

    public JavaScriptWorkspace(string rootAbsolutePath)
    {
        _rootAbsolutePath = rootAbsolutePath;
        Recursive_FileDiscovery(_rootAbsolutePath);
    }

    public JavaScriptWorkspace(List<WorkspaceFolder>? workspaceFolders)
    {
        _workspaceFolders = workspaceFolders;
        foreach (var workspaceFolder in _workspaceFolders)
        {
            Recursive_FileDiscovery(workspaceFolder.uri);
        }
    }

    public void DidOpenTextDocumentNotification(string myPath, string sourceFileAbsolutePath, string text)
    {
        File.AppendAllText(myPath, $"\n====DidOpenTextDocumentNotification(string sourceFileAbsolutePath)====\n");
        OpenedSourceFileAbsolutePathToInMemoryContentMap.Add(sourceFileAbsolutePath, new JavaScriptDocument(text.ToCharArray()));
    }
    
    public void DidCloseTextDocumentNotification(string myPath, string sourceFileAbsolutePath)
    {
        File.AppendAllText(myPath, $"\n====DidCloseTextDocumentNotification(string sourceFileAbsolutePath)____====\n");
        var wasRemoved = OpenedSourceFileAbsolutePathToInMemoryContentMap.Remove(sourceFileAbsolutePath);
        File.AppendAllText(myPath, $"\n====DidCloseTextDocumentNotification(string sourceFileAbsolutePath)_{wasRemoved}====\n");
    }

    public void Recursive_FileDiscovery(string targetDir)
    {
        foreach (var childFile in Directory.EnumerateFiles(targetDir))
        {
            if (Path.GetExtension(childFile) == ".js" || Path.GetExtension(childFile) == ".cjs")
            {
                SourceFileAbsolutePathList.Add(childFile);
            }
        }

        foreach (var childDir in Directory.EnumerateDirectories(targetDir))
        {
            if (Path.GetFileName(childDir) == "node_modules")
            {
                //
            }
            else if (Path.GetFileName(childDir) == ".git")
            {
                //
            }
            else if (Path.GetFileName(childDir) == ".vscode")
            {
                //
            }
            else if (Path.GetFileName(childDir) == "out")
            {
                //
            }
            else if (Path.GetFileName(childDir) == "bin")
            {
                //
            }
            else if (Path.GetFileName(childDir) == "obj")
            {
                //
            }
            else
            {
                Recursive_FileDiscovery(childDir);
            }
        }
    }
}
