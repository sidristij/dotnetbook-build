using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookBuilder.Extensions;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace BookBuilder.Pipeline
{
    internal class MarkdownParsingProcessor : ProcessingItemBase
    {
        private MarkdownPipeline Pipeline => Context.Get<MarkdownPipeline>();
        
        private ProcessingOptions ProcessingOptions => Context.Get<ProcessingOptions>();
        
        private FileDescription FileDescription => Context.Get<FileDescription>();

        public override ProcessingStage MyStage => ProcessingStage.Parsing;
        
        public override bool ShouldWorkInExclusiveMode => true;

        public MarkdownParsingProcessor(Context context) : base(context)
        {
        }

        private List<(int maxLength, int size, int Skip)> TableSizes = new List<(int, int, int)>
        {
            (0,        6,  3),
            (256,      8,  2),
            (1024,     10, 1),
            (1024+512, 12, 0)
        };
        
        public override async Task DoWorkAsync()
        {
            var document = MarkdownParser.Parse(File.ReadAllText(ProcessingOptions.SourcePath), Pipeline);
            var documentStructure = new DocumentStructureEntry(FileDescription, document, null);
            var ctx = Context.CreateCopy()
                .With(document)
                .With(documentStructure);

            VisitDocument(document, documentStructure);
            
            ProjectProcessing.TryAddTask(new MarkdownDocumentProcessor(ctx));
        }

        private void VisitDocument(MarkdownDocument document, DocumentStructureEntry entry)
        {
            foreach (var markdownObject in document.Descendants())
            {
                VisitMarkdownObject(document, markdownObject, ref entry);    
            }
        }

        private void VisitMarkdownObject(MarkdownDocument document, MarkdownObject markdownObject, ref DocumentStructureEntry entry)
        {
            switch (markdownObject)
            {
                case HeadingBlock header when entry.Depth > header.Level - 1:
                {
                    var parent = entry;
                    entry = new DocumentStructureEntry(FileDescription, document, header, header.Level, entry);
                    parent.AddChild(entry);
                    break;
                }
                case HeadingBlock header when entry.Depth <= header.Level:
                {
                    var cur = entry;
                    while (cur.Depth > header.Level) cur = cur.Parent;
                    if (cur.Depth >= header.Level) cur = cur.Parent;
                    entry = new DocumentStructureEntry(FileDescription, document, header, header.Level, cur);
                    cur.AddChild(entry);
                    break;
                }
                case Table table:
                    var rows = table.Descendants().OfType<TableRow>()
                        .Select((row, rowIndex) =>
                        (
                            row.Descendants()
                                .OfType<TableCell>()
                                .Select((cell, colIndex) => (cell.GetTextLength(), colIndex))
                                .ToList(),
                            rowIndex
                        )).ToList();
                    
                    var map = new Dictionary<int, int>(rows.First().Item1.Select(col => new KeyValuePair<int, int>(col.colIndex, col.Item1)));
                    foreach (var (cols, rowIndex) in rows.Skip(1))
                    {
                        for (var i = 0; i < cols.Count; i++)
                        {
                            if (map[i] < cols[i].Item1)
                                map[i] = cols[i].Item1;
                        }
                    }

                    var totalLength = map.Sum(x => x.Value);
                    var tableProps = TableSizes.Last(x => x.maxLength < totalLength);
                    table.SetAttributes(new HtmlAttributes {Classes = new List<string>{"col-" + tableProps.size, "offset-" + tableProps.Skip}});
                    break;
            }

            foreach (var descendant in markdownObject.Descendants())
            {
                VisitMarkdownObject(document, descendant, ref entry);
            }
        }
    }
}