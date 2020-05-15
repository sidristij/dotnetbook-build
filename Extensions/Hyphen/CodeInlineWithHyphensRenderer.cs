using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;

namespace BookBuilder.Extensions.Hyphen
{
    public class CodeInlineWithHyphensRenderer : HtmlObjectRenderer<CodeInline>
    {        
        protected override void Write(HtmlRenderer renderer, CodeInline inline)
        {
            renderer.Write("<code").WriteAttributes(inline).Write(">");
            renderer.WriteWithHyphens(inline);
            renderer.Write("</code>");
        }
    }
}