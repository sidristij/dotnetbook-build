using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;

namespace BookBuilder.Extensions.Hyphen
{
    public class LiteralInlineWithHyphensRenderer : HtmlObjectRenderer<LiteralInline>
    {        
        protected override void Write(HtmlRenderer renderer, LiteralInline inline)
        {
            renderer.WriteWithHyphens(inline);
        }
    }
}