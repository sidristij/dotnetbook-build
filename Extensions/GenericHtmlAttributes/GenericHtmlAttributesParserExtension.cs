using Markdig;
using Markdig.Renderers;

namespace BookBuilder.GenericAttributes.Extensions
{
    public class GenericHtmlAttributesParserExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<GenericHtmlAttributesParser>())
            {
                pipeline.InlineParsers.Insert(0, new GenericHtmlAttributesParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }
    }
}