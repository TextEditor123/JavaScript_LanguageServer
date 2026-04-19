namespace JSLSApp;

public class JavaScriptWorkspace
{
    private readonly string _rootAbsolutePath;

    public List<string> SourceFileAbsolutePathList { get; } = new();
    public Dictionary<string, char[]> OpenedSourceFileAbsolutePathToInMemoryContentMap { get; set; }

    // In World of Warcraft the demon hunters say: "I've sacrificed everything, what have you given?"

    public JavaScriptWorkspace(string rootAbsolutePath)
    {
        _rootAbsolutePath = rootAbsolutePath;


        Recursive_FileDiscovery(rootAbsolutePath);
        //for ()
        //{
        //
        //}
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
