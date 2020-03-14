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
        private const string MdExt = ".md";
        private const string TargetExt = ".html";

        private ProcessingOptions Opts => Context.Get<ProcessingOptions>();

        private FolderDescription ParentFolder => Context.Get<FolderDescription>();

        public FolderProcessor(Context context) : base(context)
        {
        }

        public override ProcessingStage MyStage  => ProcessingStage.FoldersProcessing;

        public override async Task DoWorkAsync()
        {
            var subfolders = Directory.GetDirectories(Opts.SourcePath);
            var parentFolder = ParentFolder;

            foreach (var subfolder in subfolders)
            {
                var folderDesc = new FolderDescription(ParentFolder?.Root, ParentFolder, subfolder);
                parentFolder.AddEntry(folderDesc);

                var context = Context.CreateCopy()
                    .With(Opts.Combine(subfolder))
                    .With(folderDesc);

                ProjectProcessing.TryAddTask(new FolderProcessor(context));
            }

            foreach (var filePath in Directory.GetFiles(Opts.SourcePath))
            {
                var fileDesc = new FileDescription(ParentFolder?.Root, ParentFolder, Path.GetFileName(filePath));
                parentFolder.AddEntry(fileDesc);

                var context = Context.CreateCopy().With(fileDesc);

                if (Path.GetExtension(filePath.AsSpan()).Equals(MdExt.AsSpan(), StringComparison.Ordinal))
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