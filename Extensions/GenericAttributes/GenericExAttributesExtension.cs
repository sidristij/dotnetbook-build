using Markdig;
using Markdig.Renderers;

namespace BookBuilder.GenericAttributes.Extensions
{
    public class GenericExAttributesExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<GenericAttributesExtendedParser>())
            {
                pipeline.InlineParsers.Insert(0, new GenericAttributesExtendedParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }
    }
}