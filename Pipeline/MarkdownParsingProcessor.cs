using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;
using Markdig;

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
            var document = Markdown.Parse(File.ReadAllText(ProcessingOptions.SourcePath), Pipeline);
            var ctx = Context.CreateCopy().With(document);

            ProjectProcessing.TryAddTask(new MarkdownDocumentProcessor(ctx));
        }
    }
}