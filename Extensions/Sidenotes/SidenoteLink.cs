using Markdig.Syntax.Inlines;

namespace BookBuilder.Extensions.Sidenotes
{
    public class SidenoteLink : Inline
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is back link (from a footnote to the link)
        /// </summary>
        public bool IsBackLink { get; set; }

        /// <summary>
        /// Gets or sets the global index number of this link.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the footnote this link refers to.
        /// </summary>
        public Sidenote Sidenote { get; set; }
    }
}