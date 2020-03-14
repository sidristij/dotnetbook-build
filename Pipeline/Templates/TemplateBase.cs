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

        protected string GetPath(string relative)
        {
            return Path.Combine(
                relative.StartsWith('.') 
                    ? ProcessingOptions.TargetPath 
                    : Environment.CurrentDirectory, 
                relative);
        }

        protected static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath))   throw new ArgumentNullException(nameof(toPath));

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath;
        }
    }
}