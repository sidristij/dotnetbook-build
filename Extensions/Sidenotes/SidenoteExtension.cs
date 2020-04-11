using Markdig;
using Markdig.Renderers;

namespace BookBuilder.Extensions.Footnotes
{
    public class SidenoteExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<SidenoteParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new SidenoteParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlSidenoteGroupRenderer());
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlSidenoteLinkRenderer());
            }
        }
    }
}