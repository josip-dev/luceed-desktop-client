using System.Reflection;

namespace Tomsoft.DeveloperTask.Data;

public static class Project
{
    public static Assembly Assembly { get; }
    public static AssemblyName AssemblyNameObject { get; }
    public static string? Name { get; }
    public static Version? Version { get; }
    public static string FilePath { get; }
    public static string DirectoryPath { get; }
    public static DirectoryInfo Directory { get; }

    static Project()
    {
        Assembly = Assembly.GetEntryAssembly() ?? throw new ApplicationException("Entry assembly wasn't found, but it's required");
        AssemblyNameObject = Assembly.GetName();
        Name = AssemblyNameObject.Name;
        Version = AssemblyNameObject.Version;
        FilePath = Assembly.Location;
        var directoryPath = Path.GetDirectoryName(FilePath);
        DirectoryPath = string.IsNullOrWhiteSpace(directoryPath)
            ? System.IO.Directory.GetCurrentDirectory()
            : directoryPath;
        Directory = new DirectoryInfo(DirectoryPath);
    }
    
    public static string RelativePath(string path) => Path.Combine(DirectoryPath, path);
}