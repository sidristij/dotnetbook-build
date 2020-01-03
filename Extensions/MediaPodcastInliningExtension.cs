using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;

namespace BookBuilder
{
    public class PipelineBuilderExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.DocumentProcessed -= BuilderOnDocumentProcessed;
            pipeline.DocumentProcessed += BuilderOnDocumentProcessed;
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        private void BuilderOnDocumentProcessed(MarkdownDocument document)
        {
            foreach (var node in document.Descendants())
            {
                switch (node)
                {
                    
                }
            }
        }
    }
}