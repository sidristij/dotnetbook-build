using System.Linq;
using BookBuilder.Extensions.Footnotes;
using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Syntax;

namespace BookBuilder.Extensions.ParagraphNumbers
{
    public class ParagraphNumbersExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.DocumentProcessed -= PipelineOnDocumentProcessed;
            pipeline.DocumentProcessed += PipelineOnDocumentProcessed;
        }

        private void PipelineOnDocumentProcessed(MarkdownDocument document)
        {
            var paragraphIndex = 1;
            var paragraphs = document
                .Where(node => node is ParagraphBlock || node is SidenoteGroup)
                .ToList();
            
            foreach (var paragraph in paragraphs)
            {
                if (paragraph.Span.Length > 80)
                {
                    var container = new ParagraphNumbersBlock(new CustomContainerParser())
                    {
                        ParagraphIndex = paragraphIndex++
                    };
                    var index = document.IndexOf(paragraph);
                    document.RemoveAt(index);
                    document.Insert(index, container);
                    
                    container.Insert(0, paragraph);
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new ParagraphNumbersRenderer());
            }
        }
    }
}