using System;
using System.IO;
using System.Linq;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace BookBuilder
{
    internal class MarkdownLocalLinksPatchingExtension : IMarkdownExtension
    {
        private readonly ProcessingOptions _options;

        public MarkdownLocalLinksPatchingExtension(ProcessingOptions options)
        {
            _options = options;
        }
        
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
                if (inlineRenderer != null)
                {
                    inlineRenderer.TryWriters.Remove(TryLinkInlineRenderer);
                    inlineRenderer.TryWriters.Insert(0, TryLinkInlineRenderer);
                }
            }
        }

        private bool TryLinkInlineRenderer(HtmlRenderer renderer, LinkInline linkInline)
        {
            if (linkInline.IsImage || linkInline.Url == null)
                return false;
            
            if (linkInline.Url != null &&
                (linkInline.Url.StartsWith("/") || linkInline.Url.StartsWith("./") || linkInline.Url.StartsWith("../")) &&
                linkInline.Url.Contains(".md"))
            {
                var newLink = linkInline.Url.Replace(".md", _options.TargetExt);
                linkInline.Url = newLink;
            }

            return false;
        }
    }
}