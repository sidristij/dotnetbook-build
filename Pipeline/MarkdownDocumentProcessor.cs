using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace BookBuilder.Pipeline
{
    internal class MarkdownDocumentProcessor : ProcessingItemBase
    {
        private MarkdownDocument Document => Context.Get<MarkdownDocument>();

        private MarkdownPipeline Pipeline => Context.Get<MarkdownPipeline>();

        public override ProcessingStage MyStage => ProcessingStage.Parsing;

        public override bool ShouldWorkInExclusiveMode => true;

        public MarkdownDocumentProcessor(Context context) : base(context)
        {
        }

        public override async Task DoWorkAsync()
        {
            await using var writer = new StringWriter();

            var renderer = new HtmlRenderer(writer);
            Pipeline.Setup(renderer);
            renderer.Writer = writer;
            renderer.Render(Document);

            await renderer.Writer.FlushAsync();

            var result = writer.ToString();

            var ctx = Context.CreateCopy().With(new DocumentHolder{ DocumentBody = result });
            
            ProjectProcessing.TryAddTask(new ApplyTemplatesProcessor(ctx));
        }
    }
}