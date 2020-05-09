using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;
using Markdig.Renderers.Html;

namespace BookBuilder.Pipeline.Templates
{
    internal class DocumentStructureTemplate : TemplateBase
    {
        private DocumentStructureEntry structure;
        
        public DocumentStructureTemplate(Context context) : base(context)
        {
            structure = context.Get<DocumentStructureEntry>();
        }

        public override string Apply(string incoming)
        {
            if (TryFindArea(incoming, "<!--structure:", "-->", out var startingLevel, out var region))
            {
                var sb = new StringBuilder();
                foreach (var subEntry in structure.SubEntries.SelectMany(x => x.SubEntries).Reverse())
                {
                    sb.AppendLine($"<li><a href=\"#{subEntry.Block.GetAttributes().Id}\">");
                    sb.Append(subEntry.Title);
                    sb.AppendLine("</a></li>");
                }

                var replace = incoming.Replace(incoming.Substring(region.start, region.length), sb.ToString());
                return replace;
            }

            return incoming;
        }

        private IEnumerable<DocumentStructureEntry> GetHeadingsList(DocumentStructureEntry root, int level)
        {
            foreach (var entry in root.SubEntries)
            {
                if (entry.Depth == level)
                {
                    yield return entry;
                } else if (entry.Depth < level)
                {
                    
                }
            }
        }
    }
}