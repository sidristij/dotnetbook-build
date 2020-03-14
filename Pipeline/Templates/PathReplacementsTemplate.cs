using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal class PathReplacementsTemplate : TemplateBase
    {
        private Dictionary<string, string> resourcesPathes = new Dictionary<string, string>
        {
            {"{RES_PATH}", @"./res"}
        };
        
        public PathReplacementsTemplate(Context context) : base(context)
        {
            foreach (var (template, target) in resourcesPathes.ToList())
            {
                resourcesPathes[template] = Path.Combine(
                    MakeRelativePath(_processingOptions.TargetPath, _processingOptions.TargetRootPath),
                    target);
            }
        }

        public override string Apply(string incoming)
        {
            foreach (var (template, target) in resourcesPathes)
            {
                incoming = incoming.Replace(template, target);
            }

            return incoming;
        }
    }
}