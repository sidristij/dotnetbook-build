using Markdig.Syntax;

namespace BookBuilder.Pipeline.Common
{
    internal abstract class AfterParsingProcessingBase : ProcessingItemBase
    {
        protected MarkdownDocument Document { get; }

        protected AfterParsingProcessingBase(ProjectProcessing processing, MarkdownDocument document) 
            : base(processing)
        {
            Document = document;
        }

        public override ProcessingStage MyStage => ProcessingStage.AfterParsing;
    }
}