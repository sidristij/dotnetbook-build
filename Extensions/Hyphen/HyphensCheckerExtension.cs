using System;
using System.Text;
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
            Span<int> posBuffer = stackalloc int[20];
            

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
                            // check UpperCase
                            if (text[index].IsAlphaUpper())
                            {
                                var lastIndex = index + 1;
                                var upperCount = 0;
                                var lowerCount = 0;
                                for (; lastIndex < text.Length && text[lastIndex].IsAlpha(); lastIndex++)
                                {
                                    if (!text[lastIndex].IsAlphaUpper())
                                    {
                                        lowerCount++;
                                        continue;
                                    }
                                    
                                    // -1: prev symbol, not capital
                                    if (lowerCount > 0)
                                    {
                                        posBuffer[upperCount] = lastIndex - index - 1;
                                        upperCount++;
                                    }
                                    lowerCount = 0;
                                }

                                if (upperCount > 0)
                                {
                                    var word = text.Slice(index, lastIndex - index);
                                    var positions = posBuffer.Slice(0, upperCount);
                                    if (word.Length > 2)
                                    {
                                        var sb = new StringBuilder(word.Length + upperCount);
                                        var lastPos = 0;
                                        for (int pos = 0; pos < positions.Length; pos++)
                                        {
                                            sb.Append(word.Slice(lastPos, positions[pos] - lastPos + 1));
                                            if (pos != posBuffer.Length - 1)
                                                sb.Append("&shy;");
                                            lastPos = positions[pos] + 1;
                                        }

                                        sb.Append(word.Slice(lastPos));
                                        renderer.Write(sb.ToString());
                                        index = index + word.Length;
                                        continue;
                                    }
                                }
                            }
                            
                            // Check language-specific hyphens
                            if (text[index].IsSupportedLanguage())
                            {
                                var start = index;
                                
                                while (index < text.Length && text[index].IsLanguageEqual(text[start]))
                                    index++;

                                var word = text.Slice(start, index - start);

                                if (word.Length > 3)
                                {
                                    var spliced = HyphensChecker.SplitWithHyphens(word);
                                    renderer.Write(spliced == word
                                        ? spliced.ToString()
                                        : spliced.ToString().Replace("-", "&shy;"));
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