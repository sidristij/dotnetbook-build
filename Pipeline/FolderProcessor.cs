using System;
using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline
{
    /// <summary>
    /// Depending on file type redirects file processing to md parser or to resources
    /// copying task 
    /// </summary>
    internal class FolderProcessor : ProcessingItemBase
    {
        private static string mdExt = ".md";
        private static string targetExt = ".html";

        protected ProcessingOptions Opts => Context.Get<ProcessingOptions>();

        public FolderProcessor(Context context) : base(context)
        {
        }

        public override ProcessingStage MyStage  => ProcessingStage.FoldersProcessing;

        public override async Task DoWorkAsync()
        {
            var subfolders = Directory.GetDirectories(Opts.SourcePath);
            foreach (var subfolder in subfolders)
            {
                ProjectProcessing.TryAddTask(
                    new FolderProcessor(Context.CreateCopy(Opts.Combine(subfolder))));
            }

            foreach (var filePath in Directory.GetFiles(Opts.SourcePath))
            {
                if (Path.GetExtension(filePath.AsSpan()).Equals(mdExt.AsSpan(), StringComparison.Ordinal))
                {
                    ProjectProcessing.TryAddTask(new MarkdownFileProcessor(
                        Context.CreateCopy(Opts.Combine(filePath, true))));
                }
                else
                {
                    ProjectProcessing.TryAddTask(new ResourceFileProcessor(
                        Context.CreateCopy(Opts.Combine(filePath, false))));
                }
            }
        }
    }
}