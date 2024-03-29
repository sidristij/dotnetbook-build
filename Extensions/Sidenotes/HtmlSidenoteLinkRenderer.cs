﻿using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace BookBuilder.Extensions.Sidenotes
{
    public class HtmlSidenoteLinkRenderer : HtmlObjectRenderer<SidenoteLink>
    {
        public HtmlSidenoteLinkRenderer()
        {
            BackLinkString = "&#8617;";
            FootnoteLinkClass = "footnote-ref";
            FootnoteBackLinkClass = "footnote-back-ref";
        }
        public string BackLinkString { get; set; }

        public string FootnoteLinkClass { get; set; }

        public string FootnoteBackLinkClass { get; set; }

        protected override void Write(HtmlRenderer renderer, SidenoteLink link)
        {
            var order = link.Sidenote.Label.Substring(1).Trim();
            renderer.Write(link.IsBackLink
                ? $"<a href=\"#fnref:{link.Index}\" class=\"{FootnoteBackLinkClass}\">{BackLinkString}</a>"
                : $"<a id=\"fnref:{link.Index}\" href=\"#fn:{order}\" class=\"{FootnoteLinkClass}\"><sup>{order}</sup></a>");
        }
    }
}