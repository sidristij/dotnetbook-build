using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BookBuilder.Pipeline.Common.Structure
{
    [DebuggerDisplay("Name: ({Name}), Folder, Count: ({Count})")]
    internal class FolderDescription : IFileEntry
    {
        private ConcurrentBag<IFileEntry> _entries;
        private string _path;

        public FolderDescription(FolderDescription root, FolderDescription parent, string path)
        {
            Root = root;
            Parent = parent;
            _path = path;
            _entries = new ConcurrentBag<IFileEntry>();
        }

        public ReadOnlySpan<char> Name => Path.GetFileName(_path);

        public bool IsFolder => true;

        private int Count => _entries.Count;

        public FolderDescription Parent { get; }

        public FolderDescription Root { get; }

        public IEnumerable<IFileEntry> SubEntries => _entries;

        public IEnumerable<FolderDescription> SubFolders => _entries.OfType<FolderDescription>();

        public IEnumerable<FileDescription> Files => _entries.OfType<FileDescription>();

        public void AddEntry(IFileEntry entry)
        {
            _entries.Add(entry);
        }
    }
}