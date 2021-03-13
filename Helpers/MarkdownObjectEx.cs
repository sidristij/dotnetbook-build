using System;
using System.Runtime.CompilerServices;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace BookBuilder.Helpers
{
    public static class MarkdownObjectEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> GetTitle(this HeadingBlock block)
        {
            var content = (block?.Inline?.FirstChild as LiteralInline)?.Content;
            return content != null ? content.Value.AsSpan() : ReadOnlySpan<char>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> AsSpan(this StringSlice slice)
        {
            return slice.Text.AsSpan(slice.Start, slice.End - slice.Start + 1);
        }
    }
}