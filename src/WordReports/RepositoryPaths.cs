using System;
using System.IO;

namespace WordReports
{
    /// <summary>
    /// Resolves the repository folders no matter where the binary runs from: the build output,
    /// a published folder or a container. The marker is the <c>templates</c> folder.
    /// </summary>
    public static class RepositoryPaths
    {
        public static string Root { get; } = FindRoot();

        public static string Templates => Path.Combine(Root, "templates");

        public static string Data => Path.Combine(Root, "data");

        public static string Output { get; } = EnsureOutput();

        public static string Template(string fileName) => Path.Combine(Templates, fileName);

        public static string DataFile(string fileName) => Path.Combine(Data, fileName);

        public static string OutputFile(string fileName) => Path.Combine(Output, fileName);

        private static string FindRoot()
        {
            // The binary may sit in the build output, in a published folder or in a container,
            // so look above it first and above the working directory second.
            foreach (string start in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
            {
                string? found = SearchUpwards(start);
                if (found != null)
                {
                    return found;
                }
            }

            throw new DirectoryNotFoundException(
                "Could not find the repository root: no 'templates' folder above " +
                AppContext.BaseDirectory + " or " + Directory.GetCurrentDirectory());
        }

        private static string? SearchUpwards(string start)
        {
            string current = Path.GetFullPath(start);

            while (!string.IsNullOrEmpty(current))
            {
                if (Directory.Exists(Path.Combine(current, "templates")))
                {
                    return current;
                }

                string? parent = Path.GetDirectoryName(current);
                if (parent == null || parent == current)
                {
                    return null;
                }

                current = parent;
            }

            return null;
        }

        private static string EnsureOutput()
        {
            string path = Path.Combine(Root, "output");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
