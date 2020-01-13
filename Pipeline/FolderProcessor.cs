using System;
using System.IO;
using System.Threading.Tasks;
using BookBuilder.Pipeline.Common;
using BookBuilder.Pipeline.Common.Structure;

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

        protected FolderDescription ParentFolder => Context.Get<FolderDescription>();

        public FolderProcessor(Context context) : base(context)
        {
        }

        public override ProcessingStage MyStage  => ProcessingStage.FoldersProcessing;

        public override async Task DoWorkAsync()
        {
            var subfolders = Directory.GetDirectories(Opts.SourcePath);
            foreach (var subfolder in subfolders)
            {
                var context = Context.CreateCopy()
                    .With(Opts.Combine(subfolder))
                    .With(new FolderDescription(ParentFolder?.Root, ParentFolder, subfolder));
                
                ProjectProcessing.TryAddTask(
                    new FolderProcessor(context));
            }

            foreach (var filePath in Directory.GetFiles(Opts.SourcePath))
            {
                var context = Context.CreateCopy()
                    .With(new FileDescription(ParentFolder?.Root, ParentFolder, Path.GetFileName(filePath)));
                
                if (Path.GetExtension(filePath.AsSpan()).Equals(mdExt.AsSpan(), StringComparison.Ordinal))
                {
                    context.With(Opts.Combine(filePath, true));
                    ProjectProcessing.TryAddTask(new MarkdownFileProcessor(context));
                }
                else
                {
                    context.With(Opts.Combine(filePath, false));
                    ProjectProcessing.TryAddTask(new ResourceFileProcessor(context));
                }
            }
        }
    }
}