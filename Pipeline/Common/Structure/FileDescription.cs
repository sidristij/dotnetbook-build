using System;
using System.Diagnostics;
using System.IO;

namespace BookBuilder.Pipeline.Common.Structure
{
    [DebuggerDisplay("Name: ({Name}), Extension: ({Extension}), File")]
    internal class FileDescription : IFileEntry
    {
        private readonly string path;

        public FileDescription(FolderDescription root, FolderDescription parent, string path)
        {
            Root = root;
            Parent = parent;
            this.path = path;
        }

        public bool IsFolder => false;

        public FolderDescription Root { get; }

        public FolderDescription Parent { get; }

        public ReadOnlySpan<char> Name => Path.GetFileName(path);

        public ReadOnlySpan<char> Extension => Path.GetExtension(Name);
    }
}