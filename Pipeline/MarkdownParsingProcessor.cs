using System;
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
                                .Select((cell, colIndex) => (cellLength: cell.CalculateTextLength(), colIndex))
                                .ToList(),
                            rowIndex
                        )).ToList();
                    
                    var tableParameters = GetTableParameters(rows);
                    var tableProps = tableParameters.GetLayoutInfo();
                    
                    table.SetAttributes(
                        new HtmlAttributes
                        {
                            Classes = new List<string>
                            {
                                "offset-" + tableProps.Offset,
                                "col-" + tableProps.Width, 
                            }
                        });
                    break;
            }

            foreach (var descendant in markdownObject.Descendants())
            {
                VisitMarkdownObject(document, descendant, ref entry);
            }
        }

        private static TableParameters GetTableParameters(
            IReadOnlyCollection<(List<(int cellLength, int colIndex)> columns, int rowIndex)> rows)
        {
            var map = rows.First().columns.ToDictionary(o => o.colIndex, o => o.cellLength);

            rows.Skip(1)
                .Aggregate(
                    map,
                    (agr, nextItem) =>
                    {
                        for (var i = 0; i < agr.Count; i++)
                        {
                            if (agr[i] < nextItem.columns[i].cellLength)
                                agr[i] = nextItem.columns[i].cellLength;
                        }
                        return agr;
                    });
            return new TableParameters(map);
        }
    }

    internal class TableParameters
    {
        private readonly Dictionary<int, int> _cellsInfo;

        private List<(int MaxLength, int Size, int Skip)> _tableSizes = new List<(int, int, int)>
        {
            (0,        6,  3),
            (256,      8,  2),
            (1024,     10, 1),
            (1024+512, 12, 0)
        };

        public TableParameters(Dictionary<int, int> cellsInfo)
        {
            _cellsInfo = cellsInfo;
            TotalLength = _cellsInfo.Sum(x => x.Value);
        }

        public int ColumnsCount => _cellsInfo.Count;

        public int TotalLength { get; }

        public int GetColumnMaxLength(int columnIndex) => _cellsInfo[columnIndex];

        public (int Offset, int Width) GetLayoutInfo()
        {
            var (_, size, skip) = _tableSizes.Last(x => x.MaxLength < TotalLength);
            return (skip, size);
        }
    }
}