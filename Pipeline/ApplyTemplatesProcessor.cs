using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;
using BookBuilder.Pipeline.Templates;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;

namespace BookBuilder.Pipeline
{
    internal class ApplyTemplatesProcessor : ProcessingItemBase
    {
        private string DocumentBody => Context.Get<DocumentHolder>().DocumentBody;

        private string Template => Context.Get<TemplateStorage>().Template;

        public override ProcessingStage MyStage => ProcessingStage.Parsing;

        private ProcessingOptions ProcessingOptions => Context.Get<ProcessingOptions>();

        public ApplyTemplatesProcessor(Context context) : base(context)
        {
        }

        public override async Task DoWorkAsync()
        {
            var document = Template ?? DocumentBody;
            var changed = false;
            var templatesProcessor = new AggregateTemplateProcessor(Context);
        
            do
            {
                var oldDocument = document;
                document = templatesProcessor.Apply(oldDocument);
                changed = oldDocument != document;
            } while (!changed);

            await using var writer = new StreamWriter(ProcessingOptions.TargetPath, false, Encoding.UTF8);
            writer.Write(document);
        }
    }
}