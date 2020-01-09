using Markdig;

namespace BookBuilder.Pipeline.Common
{
    internal abstract class BeforeParsingProcessingBase : ProcessingItemBase
    {
        protected MarkdownPipelineBuilder PipelineBuilder { get; }
        
        protected BeforeParsingProcessingBase(ProjectProcessing processing, MarkdownPipelineBuilder pipelineBuilder) : base(processing)
        {
            PipelineBuilder = pipelineBuilder;
        }
    }
}