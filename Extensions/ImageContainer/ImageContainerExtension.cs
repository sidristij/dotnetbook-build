using System.Linq;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace BookBuilder.Extensions.ImageContainer
{
    public class ImageContainerExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            
            pipeline.DocumentProcessed -= PipelineOnDocumentProcessed;
            pipeline.DocumentProcessed += PipelineOnDocumentProcessed;
        }

        private void PipelineOnDocumentProcessed(MarkdownDocument document)
        {
            var paragraphs = document.Descendants<ParagraphBlock>();
            foreach (var paragraph in paragraphs.Where(p => p.Inline.FirstChild == p.Inline.LastChild && p.Inline.FirstChild is LinkInline))
            {
                var inline = (LinkInline)paragraph.Inline.FirstChild;
                if (inline.IsImage)
                {
                    paragraph.WrapWith(document, "image-container");
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            ;
        }
    }
}