using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal class PathReplacementsTemplate : TemplateBase
    {
        private readonly Dictionary<string, string> _resourcesPaths = new Dictionary<string, string>
        {
            {"{RES_PATH}", @"./res"}
        };
        
        public PathReplacementsTemplate(Context context) : base(context)
        {
            foreach (var (template, target) in _resourcesPaths.ToList())
            {
                _resourcesPaths[template] = Path.Combine(
                    MakeRelativePath(ProcessingOptions.TargetPath, ProcessingOptions.TargetRootPath),
                    target);
            }
        }

        public override string Apply(string incoming)
        {
            foreach (var (template, target) in _resourcesPaths)
            {
                incoming = incoming.Replace(template, target);
            }

            return incoming;
        }
    }
}