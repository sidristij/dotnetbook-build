using Markdig.Parsers;
using Markdig.Syntax;

namespace BookBuilder.Extensions.Footnotes
{
    public class SidenoteGroup : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SidenoteGroup"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public SidenoteGroup(BlockParser parser) : base(parser)
        {
        }

        public Block ContainerBlock { get; set; }

        internal int CurrentOrder { get; set; }
    }
}