using System;

namespace BookBuilder.Pipeline.Common.Structure
{
    internal interface IFileEntry
    {
        public ReadOnlySpan<char> Name { get; }

        public bool IsFolder { get; }
        
        public FolderDescription Root { get; }
        
        public FolderDescription Parent { get; }
    }
}