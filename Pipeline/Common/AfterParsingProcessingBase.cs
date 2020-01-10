using Markdig.Syntax;

namespace BookBuilder.Pipeline.Common
{
    internal abstract class AfterParsingProcessingBase : ProcessingItemBase
    {
        protected MarkdownDocument Document { get; }

        protected AfterParsingProcessingBase(Context baseContext) : base(baseContext)
        {
            Document = Context.Get<MarkdownDocument>();
        }

        public override ProcessingStage MyStage => ProcessingStage.AfterParsing;
    }
}