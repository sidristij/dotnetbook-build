using System.Linq;
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
            var hasMedia = sidenotes.OfType<Sidenote>().Any(note => note.Label == ">m");
            var sideClass = hasMedia ? "side-media-block" : "side-regular-block";
            renderer.WriteLine($"<div class=\"{GroupClass} {sideClass}\">");
            for (int i = 0; i < sidenotes.Count; i++)
            {
                var footnote = (Sidenote)sidenotes[i];
                renderer.WriteChildren(footnote);
            }

            renderer.WriteLine("</div>");
        }
    }
}