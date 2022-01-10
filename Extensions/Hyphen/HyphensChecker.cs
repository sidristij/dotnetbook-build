using System;
using System.Text;
using BookBuilder.Helpers;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;

// ReSharper disable StringLiteralTypo

namespace BookBuilder.Extensions.Hyphen
{
    public static class HyphensChecker
    {
        private static string[] templates = {
            "вест-ный", "крос-сплат", "след-стви", "рам-мно", "арен-до", 
            "биб-ли",   "о-те",       "объ-ем",    "обя-за",  "тель-но", 
            "рост-но",  "вос-тре",    "про-из",    "чест-во", "клас-со", 
            "при-клю",  "пир-клю",    "вспом-ни",  "вклю-ча", "ко-прои", 
            "мож-но",   "взя-ла",     "по-ться",   "по-тся",  "разъ-езд", 
            "ап-ри",    "ас-тро",     "ки-но",     "нош-па",  "па-ет", 
            "т-во"
        };
        
        public static void WriteWithHyphens(this HtmlRenderer renderer, LeafInline inlineText)
        {
            var index = 0;
            ReadOnlySpan<char> text = null;
            
            if(inlineText is LiteralInline literalInline) text = literalInline.Content.AsSpan();
            if(inlineText is CodeInline codeInline) text = codeInline.Content;
            
            if(text == null) return;

            Span<int> posBuffer = stackalloc int[20];
            while (index < text.Length)
            {
                while (index < text.Length)
                {
                    // check UpperCase
                    if (text[index].IsAlphaUpper())
                    {
                        var lastIndex = index + 1;
                        var upperCount = 0;
                        var lowerCount = 0;
                        for ( ; 
                            lastIndex < text.Length && (text[lastIndex].IsAlpha() || text[lastIndex].IsAsciiPunctuation()); 
                            lastIndex++)
                        {
                            var ch = text[lastIndex];
                            if (ch.IsAlpha() && !ch.IsAlphaUpper())
                            {
                                lowerCount++;
                                continue;
                            }

                            // -1: prev symbol, not capital
                            if (lowerCount > 0)
                            {
                                posBuffer[upperCount] = lastIndex - index - 1;
                                upperCount++;
                            }

                            lowerCount = 0;
                        }

                        if (upperCount > 0)
                        {
                            var word = text.Slice(index, lastIndex - index);
                            var positions = posBuffer.Slice(0, upperCount);
                            if (word.Length > 2)
                            {
                                var sb = new StringBuilder(word.Length + upperCount);
                                var lastPos = 0;
                                for (int pos = 0; pos < positions.Length; pos++)
                                {
                                    sb.Append(word.Slice(lastPos, positions[pos] - lastPos + 1));
                                    if (pos != posBuffer.Length - 1)
                                        sb.Append("&shy;");
                                    lastPos = positions[pos] + 1;
                                }

                                sb.Append(word.Slice(lastPos));
                                renderer.Write(sb.ToString());
                                index = index + word.Length;
                                continue;
                            }
                        }
                    }

                    // Check language-specific hyphens
                    else if (text[index].IsSupportedLanguage())
                    {
                        var start = index;

                        while (index < text.Length && text[index].IsLanguageEqual(text[start]))
                            index++;

                        var word = text.Slice(start, index - start);

                        if (word.Length > 3)
                        {
                            var spliced = HyphensChecker.SplitWithHyphens(word);
                            renderer.Write(spliced.ToString());
                        }
                        else
                        {
                            var slice = new StringSlice(new string(text), start, index - 1);
                            renderer.Write(slice);
                        }

                        continue;
                    } else if (text[index] == ' ') // for customizable spaces btw words
                    {
                        renderer.Write("<span class=\"spacer\"> </span>");
                        index++;
                        continue;
                    }

                    renderer.Write(text[index]);
                    index++;
                }
            }
        }

        
        public static ReadOnlySpan<char> SplitWithHyphens(ReadOnlySpan<char> str)
        {
            ReadOnlySpan<char> word = str;//"оптимизаций".AsSpan();
            Span<int> positions = stackalloc int[str.Length];
            var positionsCount = 0;
            var trailLength = str[0].IsRussian() ? 2 : 3;
            
            do
            {
                var hyphensChecker = TryGetRussianHypenPosition(word);
                if(hyphensChecker.HasValue)
                {
                    word = word.Slice((int)hyphensChecker);
                    var prev = positionsCount == 0 ? 0 : positions[positionsCount-1];
                    positions[positionsCount++] = prev + (int)hyphensChecker;
                } else {
                    word = word.Slice(1);
                }
            } while (word.Length > trailLength);

            if (positionsCount == 0) 
                return str;

            var builder = new StringBuilder();
            while (positions.Length > 0 && positions[0] < trailLength)
            {
                positions = positions.Slice(1);
                positionsCount--;
            }

            if (positionsCount <= 0) 
                return str;

            if ((positions.Length - positions[positionsCount - 1]) < trailLength)
            {
                positionsCount--;
                positions = positions.Slice(0, positionsCount);
            }
            
            if (positionsCount == 0) 
                return str;
            
            var lastIndex = 0;
            for (var i = 0; i < positionsCount; i++)
            {
                var charsToWrite = positions[i] - lastIndex;
                builder.Append(str.Slice(lastIndex, charsToWrite));
                builder.Append("&shy;");
                lastIndex += charsToWrite;
            }

            builder.Append(str.Slice(lastIndex, str.Length - lastIndex));
            return builder.ToString();
        }
        
        private static int? TryGetRussianHypenPosition(ReadOnlySpan<char> span)
        {
            // nothing to check
            if (span.Length == 0) return null;

            // check for russian
            foreach (var tmpl in templates)
            {
                if (TryFindHypenTemplate(tmpl, span, out var hyphenPosition))
                {
                    return hyphenPosition;
                }
            }

            return null;
        }

        private static bool TryFindHypenTemplate(ReadOnlySpan<char> tmplChars, ReadOnlySpan<char> src, out int hyphenPosition)
        {
            Span<char> template = stackalloc char[tmplChars.Length - 1]; // minus hyphen

            hyphenPosition = -1;

            for (int i = 0, j = 0; i < tmplChars.Length; i++)
            {
                if (tmplChars[i] == '-')
                {
                    hyphenPosition = i;
                }
                else
                {
                    template[j] = tmplChars[i];
                    j++;
                }
            }

            for (int i = 0; i < template.Length; i++)
            {
                if (i == src.Length)
                {
                    return false;
                }

                if ((template[i].IsVowel() != src[i].IsVowel()) ||
                    (template[i].IsNoSound() != src[i].IsNoSound()) ||
                    (template[i].IsConsonant() != src[i].IsConsonant()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}