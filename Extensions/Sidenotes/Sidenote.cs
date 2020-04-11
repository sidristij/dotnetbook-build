using System.Collections.Generic;
using Markdig.Parsers;
using Markdig.Syntax;

namespace BookBuilder.Extensions.Footnotes
{
    public class Sidenote : ContainerBlock
    {
        public Sidenote(BlockParser parser) : base(parser)
        {
            Links = new List<SidenoteLink>();
            Order = -1;
        }

        /// <summary>
        /// Gets or sets the label used by this footnote.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the order of this footnote (determined by the order of the <see cref="SidenoteLink"/> in the document)
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets the links referencing this footnote.
        /// </summary>
        public List<SidenoteLink> Links { get; private set; }

        /// <summary>
        /// The label span
        /// </summary>
        public SourceSpan LabelSpan;

        internal bool IsLastLineEmpty { get; set; }
    }
}