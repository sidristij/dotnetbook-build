using System.IO;
using System.Text;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;

namespace BookBuilder.Pipeline
{
    internal class ApplyTemplatesProcessor : ProcessingItemBase
    {
        private MarkdownDocument Document => Context.Get<MarkdownDocument>();

        private MarkdownPipeline Pipeline => Context.Get<MarkdownPipeline>();

        private string DocumentBody => Context.Get<DocumentHolder>().DocumentBody;

        private string Template => Context.Get<TemplateStorage>().Template;

        private ProcessingOptions ProcessingOptions => Context.Get<ProcessingOptions>();
        public override ProcessingStage MyStage => ProcessingStage.Parsing;

        public override bool ShouldWorkInExclusiveMode => true;

        public ApplyTemplatesProcessor(Context context) : base(context)
        {
        }

        public override async Task DoWorkAsync()
        {
            var newDocument = Template?.Replace("<!--BODY-->", DocumentBody) ?? DocumentBody;            

            await using var writer = new StreamWriter(ProcessingOptions.TargetPath, false, Encoding.UTF8);
            writer.Write(newDocument);
        }
    }
}