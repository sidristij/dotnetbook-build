using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;
using Markdig;
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
            if (markdownObject is HeadingBlock header)
            {
                if (entry.Depth > header.Level - 1)
                {
                    var parent = entry;
                    entry = new DocumentStructureEntry(FileDescription, document, header, header.Level, entry);
                    parent.AddChild(entry);
                } else if (entry.Depth <= header.Level)
                {
                    var cur = entry;
                    while (cur.Depth > header.Level) cur = cur.Parent;
                    if (cur.Depth >= header.Level) cur = cur.Parent;
                    entry = new DocumentStructureEntry(FileDescription, document, header, header.Level, cur);
                    cur.AddChild(entry);
                } 
            }

            foreach (var descendant in markdownObject.Descendants())
            {
                VisitMarkdownObject(document, descendant, ref entry);
            }
        }
    }
}