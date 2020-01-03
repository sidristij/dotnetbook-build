using Markdig;

namespace BookBuilder
{
    public static class MarkdownPipelineBuilderEx
    {
        public static MarkdownPipelineBuilder UsePodcastPlacementExtension(this MarkdownPipelineBuilder builder)
        {
            var extension = new PipelineBuilderExtension();
            extension.Setup(builder);
            builder.Extensions.AddIfNotAlready(extension);
            return builder;
        }
    }
}