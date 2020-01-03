using Markdig;

namespace BookBuilder
{
    internal static class MarkdownPipelineBuilderEx
    {
        public static MarkdownPipelineBuilder UseMarkdownLocalLinksPatchingExtension(this MarkdownPipelineBuilder builder, ProcessingOptions opts)
        {
            var ex = new MarkdownLocalLinksPatchingExtension(opts);
            builder.Extensions.AddIfNotAlready(ex);
            return builder;
        }
    }
}