using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Markdig.Syntax;

namespace BookBuilder.Pipeline.Common.Structure
{
    internal class DocumentStructureEntry
    {
        private ConcurrentBag<DocumentStructureEntry> _subentries;
        private string title;
        private int depth;

        public DocumentStructureEntry(FileDescription source, MarkdownDocument document, HeadingBlock block, int depth = 0, DocumentStructureEntry parent = null)
        {
            Source = source;
            Document = document;
            Block = block;
            this.depth = depth;
            Parent = parent;
            _subentries = new ConcurrentBag<DocumentStructureEntry>();
        }

        public void AddChild(DocumentStructureEntry entry)
        {
            _subentries.Add(entry);
        }

        public DocumentStructureEntry WithTitle(string title)
        {
            this.title = title;
            return this;
        }

        public DocumentStructureEntry WithDepth(int depth)
        {
            this.depth = depth;
            return this;
        }

        public FileDescription Source { get; }

        public int Depth => depth;

        public ReadOnlySpan<char> Title => Block?.ToPositionText() ?? title;

        public MarkdownDocument Document { get; }

        public HeadingBlock Block { get; }

        public DocumentStructureEntry Parent { get; }

        public IEnumerable<DocumentStructureEntry> SubEntries => _subentries;
    }
}