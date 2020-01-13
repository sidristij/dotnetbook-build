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

        public DocumentStructureEntry(FileDescription source, MarkdownDocument document, HeadingBlock block, DocumentStructureEntry parent = null)
        {
            Source = source;
            Document = document;
            Block = block;
            Parent = parent;
        }

        public DocumentStructureEntry WithTitle(string title)
        {
            this.title = title;
            return this;
        }

        public FileDescription Source { get; }

        public ReadOnlySpan<char> Title => Block?.ToPositionText() ?? title;

        public MarkdownDocument Document { get; }

        public HeadingBlock Block { get; }

        public DocumentStructureEntry Parent { get; }

        public IEnumerable<DocumentStructureEntry> SubEntries => _subentries;
    }
}