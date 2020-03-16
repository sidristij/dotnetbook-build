using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline.Templates
{
    internal class PathReplacementsTemplate : TemplateBase
    {
        private readonly Dictionary<string, string> _resourcesPaths;

        public PathReplacementsTemplate(Context context) : base(context)
        {
            _resourcesPaths = new Dictionary<string, string>
            {
                {"{RES_SOURCE_PATH}", ProcessingOptions.Resources},
                {"{RES_PATH}", @".\res\"},
                {"{SOURCE_PATH}", ProcessingOptions.SourcePath},
                {"{TARGET_PATH}", @".\"},
                {"{SOURCE_ROOT_PATH}", ProcessingOptions.SourceRootPath},
                {"{TARGET_ROOT_PATH}", MakeRelativePath(ProcessingOptions.TargetPath, ProcessingOptions.TargetRootPath)},
                {"{TARGET_ROOT_ABS_PATH}", Path.Combine(Environment.CurrentDirectory, ProcessingOptions.TargetRootPath)},
            };
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