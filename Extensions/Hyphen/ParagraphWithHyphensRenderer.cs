using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;

namespace BookBuilder.Extensions.Hyphen
{
    public class LiteralInlineWithHyphensRenderer : HtmlObjectRenderer<LiteralInline>
    {        
        protected override void Write(HtmlRenderer renderer, LiteralInline inline)
        {
            renderer.RenderWithHyphens(inline);
        }
    }
    
    public class CodeInlineWithHyphensRenderer : HtmlObjectRenderer<CodeInline>
    {        
        protected override void Write(HtmlRenderer renderer, CodeInline inline)
        {
            renderer.Write("<code").WriteAttributes(inline).Write(">");
            renderer.RenderWithHyphens(inline);
            renderer.Write("</code>");
        }
    }
}