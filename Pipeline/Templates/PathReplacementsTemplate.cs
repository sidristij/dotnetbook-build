using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookBuilder.Helpers;
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
                {"{RES_SOURCE_PATH}", ProcessingOptions.ResourcesSource},
                {"{RES_PATH}", Path.Combine(PathHelper.MakeRelativePath(ProcessingOptions.TargetPath, ProcessingOptions.TargetRootPath), @"res")},
                {"{SOURCE_PATH}", ProcessingOptions.SourcePath},
                {"{TARGET_PATH}", @".\"},
                {"{SOURCE_ROOT_PATH}", ProcessingOptions.SourceRootPath},
                {"{TARGET_ROOT_PATH}", PathHelper.MakeRelativePath(ProcessingOptions.TargetPath, ProcessingOptions.TargetRootPath)},
                {"{TARGET_ROOT_ABS_PATH}", Path.Combine(Environment.CurrentDirectory, ProcessingOptions.TargetRootPath)},
            };

            foreach (var (key, path) in _resourcesPaths.ToList())
            {
                _resourcesPaths[key] = PathHelper.ReplaceSlashes(PathHelper.Simplify(path), web: !key.Contains("SOURCE"));
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