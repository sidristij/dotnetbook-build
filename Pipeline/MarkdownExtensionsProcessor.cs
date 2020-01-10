using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;
using Markdig;
using Markdig.Renderers;

namespace BookBuilder.Pipeline
{
    internal class MarkdownExtensionsProcessorBase : BeforeParsingProcessingBase
    {
        public MarkdownExtensionsProcessorBase(Context context) : base(context)
        {
        }

        public override async Task DoWorkAsync()
        {
      //      PipelineBuilder.Extensions.AddIfNotAlready(new MarkdownCommonExtension());
        }

        private class MarkdownCommonExtension : IMarkdownExtension
        {
            private readonly IMarkdownExtension _originalExtension;

            public MarkdownCommonExtension(IMarkdownExtension originalExtension)
            {
                _originalExtension = originalExtension;
            }
            
            public void Setup(MarkdownPipelineBuilder pipeline)
            {
                _originalExtension.Setup(pipeline);
            }

            public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
            {
                _originalExtension.Setup(pipeline, renderer);
            }
        }
    }
}