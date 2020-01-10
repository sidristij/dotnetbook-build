using System.Threading.Tasks;
using BookBuilder.Extensions;
using BookBuilder.Pipeline.Common;
using Markdig;
using Markdig.SyntaxHighlighting;

namespace BookBuilder.Pipeline
{
    internal class MarkdownExtensionsProcessor : ProcessingItemBase
    {
        private MarkdownPipelineBuilder PipelineBuilder => Context.Get<MarkdownPipelineBuilder>();
        private ProcessingOptions ProcessingOptions => Context.Get<ProcessingOptions>();
        
        public MarkdownExtensionsProcessor(Context context) : base(context)
        {
        }

        public override ProcessingStage MyStage => ProcessingStage.MdPipelineBuilder;
        
        public override bool ShouldWorkInExclusiveMode => true;
        
        public override async Task DoWorkAsync()
        {
            var pipelineBuilder = new MarkdownPipelineBuilder();
            var newContext = 
                Context.CreateCopy()
                    .With(pipelineBuilder)
                    .With(pipelineBuilder.UseAdvancedExtensions()
                        .UseSyntaxHighlighting()
                        .UseMarkdownLocalLinksPatchingExtension(ProcessingOptions)
                        .UsePodcastFrameSupport(new PodcastSupportOptions{Width = "400px", Height = "102px"})
                        .Build());

            ProjectProcessing.TryAddTask(new MarkdownParsingProcessor(newContext));
        }
    }
}