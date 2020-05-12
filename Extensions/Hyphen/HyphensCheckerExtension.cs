using System;
using Markdig;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

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
                htmlRenderer.ObjectRenderers.TryRemove<ParagraphRenderer>();
                htmlRenderer.ObjectRenderers.AddIfNotAlready<ParagraphWithHyphensRenderer>();
            }
        }
    }

    public class ParagraphWithHyphensRenderer : HtmlObjectRenderer<ParagraphBlock>
    {
        protected override void Write(HtmlRenderer renderer, ParagraphBlock obj)
        {
            renderer.EnsureLine();
            var leafBlock = (LeafBlock) obj;
            if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));
            var inline = leafBlock.Inline;
            if (inline != null)
            {
                while (inline != null)
                {
                    Write(renderer, inline);
                    inline = inline.NextSibling as ContainerInline;
                }
            }
        }

        private void Write(HtmlRenderer renderer, ContainerInline inline)
        {
            var leaf = inline.FirstChild;
            renderer.Write("<p ");
            renderer.Write(inline.GetAttributes());
            renderer.WriteLine(">");

            while (leaf != null)
            {
                if (leaf is LiteralInline inlineText)
                {
                    var index = 0;
                    var content = inlineText.Content;
                    var text = content.Text.AsSpan(content.Start, content.End - content.Start + 1);
                    
                    while (index < text.Length)
                    {
                        while (index < text.Length)
                        {
                            if (text[index].IsSupportedLanguage())
                            {
                                var start = index;
                                
                                while (index < text.Length && text[index].IsLanguageEqual(text[start]))
                                    index++;

                                var word = text.Slice(start, index - start);

                                if (word.Length > 3)
                                {
                                    var spliced = HyphensChecker.SplitWithHyphens(word);
                                    if (spliced == word)
                                        renderer.Write(spliced.ToString());
                                    else
                                        renderer.Write(spliced.ToString().Replace("-", "&shy;"));
                                    
                                }
                                else
                                {
                                    var slice = new StringSlice(content.Text, content.Start + start,
                                        content.Start + index - 1);
                                    
                                    renderer.Write(slice);
                                }

                                continue;
                            }
                            
                            renderer.Write(text[index]);
                            index++;
                        }
                    }
                }
                else
                {
                    renderer.Write(leaf);
                }
                leaf = leaf.NextSibling;
            }
            renderer.WriteLine("</p>");
        }
    }
}