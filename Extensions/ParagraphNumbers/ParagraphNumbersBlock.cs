using Markdig.Parsers;
using Markdig.Syntax;

namespace BookBuilder.Extensions.ParagraphNumbers
{
    public class ParagraphNumbersBlock : ContainerBlock
    {
        public ParagraphNumbersBlock(BlockParser parser) : base(parser)
        {
        }
        public int ParagraphIndex { get; set; }
    }
}