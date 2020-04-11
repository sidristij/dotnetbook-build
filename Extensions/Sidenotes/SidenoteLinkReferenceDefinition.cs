using Markdig.Syntax;

namespace BookBuilder.Extensions.Footnotes
{
    public class SidenoteLinkReferenceDefinition : LinkReferenceDefinition
    {
        /// <summary>
        /// Gets or sets the footnote related to this link reference definition.
        /// </summary>
        public Sidenote Sidenote { get; set; }
    }
}