using System;
using System.IO;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal class CloneFromTemplate : TemplateBase
    {
        public CloneFromTemplate(Context context) : base(context)
        {
        }

        /// <summary>
        /// Used to do whole file body import.
        /// </summary>
        public override string Apply(string incoming)
        {
            if (TryFindArea(incoming, "<!--clone-from:", "-->", out var fileRelativePath, out var region))
            {
                var resourcesSourcePath = Path.Combine(ProcessingOptions.Resources, fileRelativePath);
                try
                {
                    var contents = File.ReadAllText(resourcesSourcePath);
                    var replace = incoming.Replace(incoming.Substring(region.start, region.length), contents);
                    return replace;
                }
                catch (IOException exception)
                {
                    Console.WriteLine($"Error occured wile trying to read importing file: `{fileRelativePath}`-> `{fileRelativePath}`: {exception.Message}");
                    return incoming;
                }
            }

            return incoming;
        }
    }
}