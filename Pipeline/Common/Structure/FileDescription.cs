using System;
using System.IO;

namespace BookBuilder.Pipeline.Common.Structure
{
    internal class FileDescription : IFileEntry
    {
        public FileDescription(FolderDescription root, FolderDescription parent, ReadOnlySpan<char> name)
        {
            Root = root;
            Parent = parent;
            Name = name;
        }
        
        public ReadOnlySpan<char> Name { get; }

        public bool IsFolder => false;
        
        public FolderDescription Root { get; }
        
        public FolderDescription Parent { get; }

        public ReadOnlySpan<char> Extension => Path.GetExtension(Name);
    }
}