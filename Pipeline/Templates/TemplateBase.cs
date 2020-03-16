using System;
using System.IO;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal abstract class TemplateBase : ITemplate
    {
        protected readonly ProcessingOptions ProcessingOptions;

        protected TemplateBase(Context context)
        {
            ProcessingOptions = context.Get<ProcessingOptions>();
        }

        public abstract string Apply(string incoming);

        protected bool TryFindArea(string incoming, string start, string end, out string contents, out (int start, int length) region)
        {
            var startIndex = incoming.IndexOf(start, StringComparison.Ordinal);
            if (startIndex >= 0)
            {
                var endIndex = incoming.IndexOf(end, startIndex, StringComparison.Ordinal);
                if (endIndex >= 0)
                {
                    region = (startIndex, endIndex + end.Length - startIndex);
                    contents = incoming.Substring(startIndex + start.Length, endIndex - startIndex - start.Length);
                    return true;
                }
            }
            region = default;
            contents = null;
            return false;
        }
    }
}