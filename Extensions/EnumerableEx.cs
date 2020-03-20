using System.Collections.Generic;
using System.Linq;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace BookBuilder.Extensions
{
    public static class EnumerableEx
    {
        public static IEnumerable<(TableCell Cell, int Length)> Traverse(this IEnumerable<TableCell> root) =>
            root.Select(cell => (cell, cell.GetTextLength()));

        public static int GetTextLength(this TableCell root)
        {
            var stack = new Stack<MarkdownObject>(root);
            int length = 0;
            while(stack.Count > 0)
            {
                var current = stack.Pop();

                switch (current)
                {
                    case LiteralInline inline:
                        length += inline.Span.Length;
                        break;
                    case ParagraphBlock paragraphBlock:
                        length += paragraphBlock.Span.Length;
                        break;
                }
                
                foreach(var child in current.Descendants())
                    stack.Push(child);
            }
            return length;
        }
    }
}