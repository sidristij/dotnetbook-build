using BookBuilder.Extensions.Footnotes;
using BookBuilder.Extensions.ParagraphNumbers;
using BookBuilder.GenericAttributes.Extensions;
using Markdig;

namespace BookBuilder.Extensions
{
    internal static class MarkdownPipelineBuilderEx
    {
        public static MarkdownPipelineBuilder UseMarkdownLocalLinksPatchingExtension(this MarkdownPipelineBuilder builder, ProcessingOptions opts)
        {
            var ex = new MarkdownLocalLinksPatchingExtension(opts);
            builder.Extensions.AddIfNotAlready(ex);
            return builder;
        }

        public static MarkdownPipelineBuilder UsePodcastFrameSupport(this MarkdownPipelineBuilder builder, PodcastSupportOptions opts)
        {
            var ex = new PodcastSupportExtension(opts);
            builder.Extensions.AddIfNotAlready(ex);
            return builder;
        }

        public static MarkdownPipelineBuilder UseSidenotes(this MarkdownPipelineBuilder builder)
        {
            builder.Extensions.AddIfNotAlready<SidenoteExtension>();
            return builder;
        }

        public static MarkdownPipelineBuilder UseExtendedGenericAttributes(this MarkdownPipelineBuilder builder)
        {
            builder.Extensions.AddIfNotAlready<GenericExAttributesExtension>();
            return builder;
        }

        public static MarkdownPipelineBuilder UseParagraphsNumbering(this MarkdownPipelineBuilder builder)
        {
            builder.Extensions.AddIfNotAlready<ParagraphNumbersExtension>();
            return builder;
        }
    }
}