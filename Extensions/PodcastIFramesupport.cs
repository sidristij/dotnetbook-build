using System;
using System.Collections.Generic;
using System.Linq;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace BookBuilder.Extensions
{
    /// <summary>
    /// Осуществляет подстановку плеера подкаста книги
    /// </summary>
    internal class PodcastSupportExtension : IMarkdownExtension
    {
        private readonly PodcastSupportOptions _opts;
        public PodcastSupportExtension(PodcastSupportOptions opts = null)
        {
            _opts = opts ?? new PodcastSupportOptions
            {
                Width = "400px",
                Height = "102px",
                Class = null
            };
        }
        
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
                if (inlineRenderer != null)
                {
                    inlineRenderer.TryWriters.Remove(TryLinkInlineRenderer);
                    inlineRenderer.TryWriters.Add(TryLinkInlineRenderer);
                }
            }
        }

        private bool TryLinkInlineRenderer(HtmlRenderer renderer, LinkInline linkInline)
        {
            if (linkInline.IsImage || linkInline.Url == null)
            {
                return false;
            }

            // Only process absolute Uri
            if (!Uri.TryCreate(linkInline.Url, UriKind.RelativeOrAbsolute, out var uri) || !uri.IsAbsoluteUri)
            {
                return false;
            }

            if (TryRenderIframeFromKnownProviders(uri, renderer, linkInline))
            {
                return true;
            }

            return false;
        }
        
        private class KnownProvider
        {
            public string HostPrefix { get; set; }
            public Func<Uri, string> Delegate { get; set; }
            public bool AllowFullScreen { get; set; } = true; //Should be false for audio embedding
        }

        private static readonly List<KnownProvider> KnownHosts = new List<KnownProvider>()
        {
            new KnownProvider {HostPrefix = "anchor.fm", Delegate = AnchorFm},
        };

        private static string AnchorFm(Uri arg)
        {
            var path = arg.AbsolutePath;
            var episodeId = path.Substring(path.LastIndexOf('/') + 1);
            return $"https://anchor.fm/stanislav-sidristij/embed/episodes/ep-{episodeId}";
        }
        
        private static HtmlAttributes GetHtmlAttributes(LinkInline linkInline)
        {
            var htmlAttributes = new HtmlAttributes();
            var fromAttributes = linkInline.TryGetAttributes();
            if (fromAttributes != null)
            {
                fromAttributes.CopyTo(htmlAttributes, false, false);
            }

            return htmlAttributes;
        }
        private bool TryRenderIframeFromKnownProviders(Uri uri, HtmlRenderer renderer, LinkInline linkInline)
        {
            var foundProvider =
                KnownHosts
                    .Where(pair => uri.Host.StartsWith(pair.HostPrefix, StringComparison.OrdinalIgnoreCase))  // when host is match
                    .Select(provider =>
                        new
                        {
                            provider.AllowFullScreen,
                            Result = provider.Delegate(uri) // try to call delegate to get iframeUrl
                        }
                        )
                    .FirstOrDefault(provider => provider.Result != null);                                   // use first success

            if (foundProvider == null)
            {
                return false;
            }

            var htmlAttributes = GetHtmlAttributes(linkInline);
            renderer.Write("<iframe src=\"");
            renderer.WriteEscapeUrl(foundProvider.Result);
            renderer.Write("\"");

            if(_opts.Width != null)
                htmlAttributes.AddPropertyIfNotExist("width", _opts.Width);

            if (_opts.Height != null)
                htmlAttributes.AddPropertyIfNotExist("height", _opts.Height);

            if (!string.IsNullOrEmpty(_opts.Class))
                htmlAttributes.AddPropertyIfNotExist("class", _opts.Class);

            htmlAttributes.AddPropertyIfNotExist("frameborder", "0");
            renderer.WriteAttributes(htmlAttributes);
            renderer.Write("></iframe>");

            return true;
        }
    }
}