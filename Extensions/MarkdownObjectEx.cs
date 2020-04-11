using System;
using System.Collections.Generic;
using System.Linq;
using Markdig.Extensions.CustomContainers;
using Markdig.Extensions.Tables;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace BookBuilder.Extensions
{
    public static class MarkdownObjectEx
    {
        public static IEnumerable<(TableCell Cell, int Length)> Traverse(this IEnumerable<TableCell> root) =>
            root.Select(cell => (cell, cell.CalculateTextLength()));

        public static int CalculateTextLength(this TableCell root)
        {
            var stack = new Stack<MarkdownObject>(root);
            int length = 0;
            while(stack.Count > 0)
            {
                var current = stack.Pop();

                switch (current)
                {
                    case LinkInline inline:
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

        public static ContainerBlock WrapWith<T>(this T obj, MarkdownDocument document, params string[] classes) where T : Block
        {
            return obj.WrapWith(document, new HtmlAttributes{Classes = classes.ToList()});
        }
        public static ContainerBlock WrapWith<T>(this T obj, MarkdownDocument document, HtmlAttributes attributes) where T : Block
        {
            var parent = obj.Parent ?? document;
            if (parent == null)
            {
                throw new ArgumentException("parent should have ContainerBlock type");
            }

            for (var i = 0; i < parent.Count; i++)
            {
                if (parent[i] == obj)
                {
                    var wrapper = new CustomContainer(new CustomContainerParser());
                    parent.RemoveAt(i);
                    parent.Insert(i, wrapper);
                    wrapper.SetAttributes(attributes);
                    wrapper.Insert(0, obj);
                    return wrapper;
                }
            }
            throw new ArgumentException("Cannot find block in parent");
        }
    }
}