using System;
using Markdig;

namespace BookBuilder.Pipeline.Common
{
    internal abstract class BeforeParsingProcessingBase : ProcessingItemBase
    {
        protected MarkdownPipelineBuilder PipelineBuilder => Context.Get<MarkdownPipelineBuilder>();
        
        public override ProcessingStage MyStage => ProcessingStage.BeforeParsing;
        
        protected BeforeParsingProcessingBase(Context context) : base(context)
        {
        }
    }
}