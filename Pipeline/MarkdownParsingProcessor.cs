using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookBuilder.Extensions;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Parsers;
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

            VisitDocument(document, documentStructure); // depth = 0
            
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
            if (markdownObject is HeadingBlock header)
            {
                // Console.WriteLine("{0}: # {1} {2}", document.GetHashCode(), blk.Level, blk.GetTitle().ToString());

                // Если найден более глубокий, что есть у нас, добавить к себе 
                if (header.Level > entry.Depth)
                {
                    var parent = entry;
                    entry = new DocumentStructureEntry(FileDescription, document, header, header.Level, entry);
                    parent.AddChild(entry);
                }
                else if (header.Level <= entry.Depth)
                {
                    var cur = entry;
                    while (cur.Depth >= header.Level) cur = cur.Parent;
                    entry = new DocumentStructureEntry(FileDescription, document, header, header.Level, cur);
                    cur.AddChild(entry);
                }
            }
            
            if (markdownObject is Table table)
            {
                var rows = table
                    .Descendants().OfType<TableRow>()
                    .Select((row, rowIndex) =>
                    (
                        row.Descendants()
                            .OfType<TableCell>()
                            .Select((cell, colIndex) => (cellLength: cell.CalculateTextLength(), colIndex))
                            .ToList(),
                        rowIndex
                    )).ToList();
                
                var tableParameters = GetTableParameters(table, rows);
                var width = tableParameters.GetLayoutInfo();

                table
                    .WrapWith(document, "table-width-" + width, "table-cols-" + table.ColumnDefinitions.Count)
                    .WrapWith(document,"table-container");
            }
        }

        private static TableParameters GetTableParameters(Table table,
            IReadOnlyCollection<(List<(int cellLength, int colIndex)> columns, int rowIndex)> rows)
        {
            var map = rows.First()
                .columns.ToDictionary(o => o.colIndex, o => o.cellLength);

            var widths = rows.Skip(1)
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
                    })
                .ToList()
                .OrderBy(x => x.Key)
                .Select(x => x.Value)
                .ToList();
            
            return new TableParameters(table, widths);
        }
    }

    internal class TableParameters
    {
        private readonly Table _table;
        private readonly List<int> _cellsInfo;
        private readonly List<int> _columnRatios;

        private List<(int MaxLength, int Size, int Columns)> _tableSizes = new List<(int, int, int)>
        {
            // symbols, width, max columns count
            (0,         8,     3),
            (12,        6,     4),
            (128,       8,     8),
            (256,       10,    10),
            (384,       12,    100)
        };

        public TableParameters(Table table, List<int> cellsInfo)
        {
            _table = table;
            _cellsInfo = cellsInfo;
            TotalLength = _cellsInfo.Sum();
            // var minValue = _cellsInfo.Max(x => x.Value, )
            // _columnRatios = 
        }

        public int ColumnsCount => _table.ColumnDefinitions.Count;

        public int TotalLength { get; }

        public int GetColumnMaxLength(int columnIndex) => _cellsInfo[columnIndex];

        public int GetLayoutInfo()
        {
            var allowed = _tableSizes
                .Select((val, index) =>
                    (
                        index, 
                        byLen: val.MaxLength < TotalLength, 
                        byCol: val.Columns >= ColumnsCount
                    )
                )
                .ToList();
            
            var allowedByLength = 
                allowed.Last(x => x.byLen).index;
            
            var allowedByColsCount = 
                Enumerable.Range(allowedByLength, _tableSizes.Count - allowedByLength)
                .First(x => allowed[x].byCol);

            var (_, size, _) = _tableSizes[allowed[allowedByColsCount].index];
            return size;
        }
    }
}