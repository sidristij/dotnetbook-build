using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace BookBuilder.Extensions.ParagraphNumbers
{
    public class ParagraphNumbersRenderer : HtmlObjectRenderer<ParagraphNumbersBlock>
    {
        protected override void Write(HtmlRenderer renderer, ParagraphNumbersBlock obj)
        {
            renderer.EnsureLine();
            renderer.WriteLine("<div class=\"paragraph-container\">");
            renderer.WriteLine($"<div class=\"paragraph-left-side\">{obj.ParagraphIndex:00}</div>");
            renderer.WriteChildren(obj);
            renderer.WriteLine($"<div class=\"paragraph-right-side\">{obj.ParagraphIndex:00}</div>");
            renderer.WriteLine("</div>");
        }
    }
}