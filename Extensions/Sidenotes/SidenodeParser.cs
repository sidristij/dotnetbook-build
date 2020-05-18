using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace BookBuilder.Extensions.Sidenotes
{
        /// <summary>
    /// The block parser for a <see cref="Sidenote"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class SidenoteParser : BlockParser
    {
        /// <summary>
        /// The key used to store at the document level the pending <see cref="SidenoteGroup"/>
        /// </summary>
        private static readonly object DocumentKey = typeof(Sidenote);

        public SidenoteParser()
        {
            OpeningCharacters = new [] {'['};
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            return TryOpen(processor, false);
        }

        private BlockState TryOpen(BlockProcessor processor, bool isContinue)
        {
            // We expect footnote to appear only at document level and not indented more than a code indent block
            var currentContainer = processor.GetCurrentContainerOpened();
            if (processor.IsCodeIndent ||
                (!isContinue && currentContainer.GetType() != typeof(MarkdownDocument)) ||
                (isContinue && !(currentContainer is SidenoteGroup)))
            {
                return BlockState.None;
            }

            var saved = processor.Column;
            var start = processor.Start;
            if (!LinkHelper.TryParseLabel(ref processor.Line, false, out var label, out var labelSpan) ||
                !label.StartsWith(">") ||
                processor.CurrentChar != ':')
            {
                processor.GoToColumn(saved);
                return BlockState.None;
            }

            // Advance the column
            var deltaColumn = processor.Start - start;
            processor.Column += deltaColumn;
            processor.NextChar(); // Skip ':'

            var footnote = new Sidenote(this)
            {
                Label = label,
                LabelSpan = labelSpan,
            };

            // Maintain a list of ll footnotes at document level
            var footnotes = processor.Document.GetData(DocumentKey) as Dictionary<Block, SidenoteGroup>;
            if (footnotes == null)
            {
                footnotes = new Dictionary<Block, SidenoteGroup>();
                processor.Document.SetData(DocumentKey, footnotes);
                processor.Document.ProcessInlinesEnd += Document_ProcessInlinesEnd;
            }

            if (!footnotes.TryGetValue(processor.LastBlock, out var group))
            {
                group = new SidenoteGroup(this) {ContainerBlock = processor.LastBlock};
                footnotes[processor.LastBlock] = group;
                processor.Document.Add(group);
            }

            var linkRef = new SidenoteLinkReferenceDefinition
            {
                Sidenote = footnote,
                CreateLinkInline = CreateLinkToFootnote,
                Line = processor.LineIndex,
                Span = new SourceSpan(start, processor.Start - 2), // account for ]:
                LabelSpan = labelSpan,
                Label = label
            };

            // wrap with div + class
            group.Add(footnote);
            processor.NewBlocks.Push(footnote);
            processor.Document.SetLinkReferenceDefinition(footnote.Label, linkRef);
            return BlockState.Continue;
        }

        private void Document_ProcessInlinesEnd(InlineProcessor processor, Inline inline)
        {
            // Unregister
            processor.Document.ProcessInlinesEnd -= Document_ProcessInlinesEnd;

            var notes = (Dictionary<Block, SidenoteGroup>)processor.Document.GetData(DocumentKey);
            processor.Document.RemoveData(DocumentKey);

            foreach (var (paragraph, group) in notes)
            {
                var container = paragraph.WrapWith(processor.Document, "aside-container");
                processor.Document.Remove(group);
                container.Insert(container.Count, group);
            }
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            var footnote = (Sidenote) block;

            if (processor.CurrentBlock == null || processor.CurrentBlock.IsBreakable)
            {
                if (processor.IsBlankLine)
                {
                    footnote.IsLastLineEmpty = true;
                    return BlockState.ContinueDiscard;
                }

                if (processor.Column == 0)
                {
                    if (footnote.IsLastLineEmpty)
                    {
                        // Close the current footnote
                        processor.Close(footnote);

                        // Parse any opening footnote
                        return TryOpen(processor);
                    }

                    // Make sure that consecutive footnotes without a blanklines are parsed correctly
                    if (TryOpen(processor, true) == BlockState.Continue)
                    {
                        processor.Close(footnote);
                        return BlockState.Continue;
                    }
                }
            }
            footnote.IsLastLineEmpty = false;

            if (processor.IsCodeIndent)
            {
                processor.GoToCodeIndent();
            }

            return BlockState.Continue;
        }

        private static Inline CreateLinkToFootnote(InlineProcessor state, LinkReferenceDefinition linkRef, Inline child)
        {
            var footnote = ((SidenoteLinkReferenceDefinition)linkRef).Sidenote;
            var link = new SidenoteLink { Sidenote = footnote };

            footnote.Links.Add(link);

            return link;
        }
    }
}