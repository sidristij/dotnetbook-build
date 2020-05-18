using System.Threading.Tasks;
using BookBuilder.Extensions;
using BookBuilder.Pipeline.Common;
using Markdig;
using Markdig.Extensions.SmartyPants;
using Markdig.SyntaxHighlighting;

namespace BookBuilder.Pipeline
{
    internal class MarkdownExtensionsProcessor : ProcessingItemBase
    {
        private ProcessingOptions ProcessingOptions => Context.Get<ProcessingOptions>();

        public MarkdownExtensionsProcessor(Context context) : base(context)
        {
        }

        public override ProcessingStage MyStage => ProcessingStage.MdPipelineBuilder;

        public override bool ShouldWorkInExclusiveMode => true;

        public override async Task DoWorkAsync()
        {
            var smartyOptions = new SmartyPantOptions();
            smartyOptions.Mapping[SmartyPantType.Dash2] = " &mdash; ";
            smartyOptions.Mapping[SmartyPantType.LeftDoubleQuote] = "&laquo;";
            smartyOptions.Mapping[SmartyPantType.RightDoubleQuote] = "&raquo;";

            var pipelineBuilder = new MarkdownPipelineBuilder();
            var newContext =
                Context.CreateCopy()
                    .With(pipelineBuilder)
                    .With(pipelineBuilder
                        .UseAdvancedExtensions()
                        .UseExtendedGenericAttributes()
                        .UseSmartyPants(smartyOptions)
                        .UseSyntaxHighlighting()
                        .UseSidenotes()
                        .UseSplitWithHyphens()
                        .UseParagraphsNumbering()
                        .UseImageContainers()
                        .UseMarkdownLocalLinksPatchingExtension(ProcessingOptions)
                        .UsePodcastFrameSupport(new PodcastSupportOptions{Width = "400px", Height = "102px", Class = "music-player"})
                        .Build());

            ProjectProcessing.TryAddTask(new MarkdownParsingProcessor(newContext));
        }
    }
}