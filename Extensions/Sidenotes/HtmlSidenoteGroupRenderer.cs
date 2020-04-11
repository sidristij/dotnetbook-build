using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace BookBuilder.Extensions.Footnotes
{
    public class HtmlSidenoteGroupRenderer : HtmlObjectRenderer<SidenoteGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlSidenoteGroupRenderer"/> class.
        /// </summary>
        public HtmlSidenoteGroupRenderer()
        {
            GroupClass = "aside-side-container";
        }

        /// <summary>
        /// Gets or sets the CSS group class used when rendering the &lt;div&gt; of this instance.
        /// </summary>
        public string GroupClass { get; set; }

        protected override void Write(HtmlRenderer renderer, SidenoteGroup sidenotes)
        {
            renderer.EnsureLine();
            renderer.WriteLine($"<div class=\"{GroupClass}\">");
            renderer.WriteLine("</ol>");

            for (int i = 0; i < sidenotes.Count; i++)
            {
                var footnote = (Sidenote)sidenotes[i];
                renderer.WriteLine($"<li id=\"fn:{footnote.Order}\">");
                renderer.WriteChildren(footnote);
                renderer.WriteLine("</li>");
            }

            renderer.WriteLine("</ol>");
            renderer.WriteLine("</div>");
        }
    }
}