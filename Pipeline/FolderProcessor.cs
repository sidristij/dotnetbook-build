using System;
using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;

namespace BookBuilder.Pipeline
{
    internal class FolderProcessor : IProcessingItem
    {
        private static string mdExt = ".md";
        private static string targetExt = ".html";
        protected ProjectProcessing ProjectProcessing { get; }
        protected ProcessingOptions Opts { get; }

        public FolderProcessor(ProjectProcessing projectProcessing, ProcessingOptions opts)
        {
            ProjectProcessing = projectProcessing;
            Opts = opts;
        }

        public ProcessingStage MyStage => ProcessingStage.FoldersProcessing;
       
        public async Task DoWorkAsync()
        {
            var subfolders = Directory.GetDirectories(Opts.SourcePath);
            foreach (var subfolder in subfolders)
            {
                var po = Opts.Combine(subfolder);
                ProjectProcessing.TryAddTask(new FolderProcessor(ProjectProcessing, po));
            }

            foreach (var filePath in Directory.GetFiles(Opts.SourcePath))
            {
                if (Path.GetExtension(filePath.AsSpan()).Equals(mdExt.AsSpan(), StringComparison.Ordinal))
                {
                    ProjectProcessing.TryAddTask(new MarkdownFileProcessor(ProjectProcessing, Opts.Combine(filePath, true)));
                }
                else
                {
                    ProjectProcessing.TryAddTask(new ResourceFileProcessor(ProjectProcessing, Opts.Combine(filePath, false)));
                }
            }
        }
    }
}