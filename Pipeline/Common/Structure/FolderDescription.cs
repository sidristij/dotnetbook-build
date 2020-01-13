using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BookBuilder.Pipeline.Common.Structure
{
    internal class FolderDescription : IFileEntry
    {
        private ConcurrentBag<IFileEntry> _entries;
        private Func<ReadOnlySpan<char>> x;
        public FolderDescription(FolderDescription root, FolderDescription parent, ReadOnlySpan<char> name)
        {
            Root = root;
            Parent = parent;
            Name = name;
            _entries = new ConcurrentBag<IFileEntry>();
        }

        private ReadOnlySpan<char> Name { get; }

        public bool IsFolder => true;
        public FolderDescription Parent { get; }
        public FolderDescription Root { get; }

        public IEnumerable<IFileEntry> SubEntries => _entries;

        public IEnumerable<FolderDescription> SubFolders => _entries.OfType<FolderDescription>();

        public IEnumerable<FileDescription> Files => _entries.OfType<FileDescription>();
    }
}