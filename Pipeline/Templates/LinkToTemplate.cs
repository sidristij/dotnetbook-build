using System;
using System.IO;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal class LinkToTemplate : TemplateBase
    {
        public LinkToTemplate(Context context) : base(context)
        {
        }
        
        /// <summary>
        /// Used to do whole file body import.
        /// </summary>
        public override string Apply(string incoming)
        {
            if (TryFindArea(incoming, "<!--link-root:", "-->", out var fileRelativePath, out var region))
            {
                // fileRelativePath is relative to project target root
                var rootRelative = MakeRelativePath(ProcessingOptions.TargetPath, ProcessingOptions.TargetRootPath);
                var filePath = Path.Combine(rootRelative, fileRelativePath);
                try
                {
                    var replace = incoming.Replace(incoming.Substring(region.start, region.length), filePath);
                    return replace;
                }
                catch (IOException exception)
                {
                    Console.WriteLine($"Error occured wile trying to read importing file: `{fileRelativePath}`-> `{filePath}`: {exception.Message}");
                    return incoming;
                }
            }

            return incoming;
        }
    }
}