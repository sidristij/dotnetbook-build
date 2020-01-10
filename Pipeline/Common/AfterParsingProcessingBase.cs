using Markdig.Syntax;

namespace BookBuilder.Pipeline.Common
{
    internal abstract class AfterParsingProcessingBase : ProcessingItemBase
    {
        protected MarkdownDocument Document => Context.Get<MarkdownDocument>();

        public override ProcessingStage MyStage => ProcessingStage.AfterParsing;
        
        protected AfterParsingProcessingBase(Context baseContext) : base(baseContext)
        {
        }
    }
}