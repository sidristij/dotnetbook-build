using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace BookBuilder.Extensions.Hyphen
{
    public class HyphensCheckerExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ObjectRenderers.TryRemove<LiteralInlineRenderer>();
                htmlRenderer.ObjectRenderers.TryRemove<CodeInlineRenderer>();
                htmlRenderer.ObjectRenderers.AddIfNotAlready<LiteralInlineWithHyphensRenderer>();
                htmlRenderer.ObjectRenderers.AddIfNotAlready<CodeInlineWithHyphensRenderer>();
            }
        }
    }
}