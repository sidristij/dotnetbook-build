using System;
using System.IO;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal class ImportFileTemplate : TemplateBase
    {
        public ImportFileTemplate(Context context) : base(context)
        {
        }
        
        /// <summary>
        /// Used to do whole file body import.
        /// </summary>
        public override string Apply(string incoming)
        {
            if (TryFindArea(incoming, "<!--import:", "-->", out var fileRelativePath, out var region))
            {
                var filePath = GetPath(fileRelativePath);
                try
                {
                    var contents = File.ReadAllText(filePath);
                    var replace = incoming.Replace(incoming.Substring(region.start, region.length), contents);
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