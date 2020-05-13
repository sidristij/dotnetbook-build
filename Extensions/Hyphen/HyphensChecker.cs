using System;
using System.Text;

// ReSharper disable StringLiteralTypo

namespace BookBuilder.Extensions.Hyphen
{
    public ref struct HyphensChecker
    {
        private static string[] russianTemplates = {
            "вест-ный", "крос-сплат", "след-стви", "рам-мно", "арен-до", 
            "биб-ли",   "о-те",       "объ-ем",    "обя-за",  "тель-но", 
            "рост-но",  "вос-тре",    "про-из",    "чест-во", "клас-со", 
            "при-клю",  "пир-клю",    "вспом-ни",  "вклю-ча", "ко-прои", 
            "мож-но",   "взя-ла",     "по-ться",   "по-тся",  "разъ-езд", 
            "ап-ри",    "ас-тро",     "ки-но",     "нош-па",  "па-ет", 
            "т-во"
        };
        
        public static ReadOnlySpan<char> SplitWithHyphens(ReadOnlySpan<char> str)
        {
            var word = str;
            Span<int> positions = stackalloc int[str.Length >> 1];
            var positionsCount = 0;

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
            } while (word.Length > 3);

            if (positionsCount == 0) 
                return str;

            var trailLength = word[0].IsRussian() ? 2 : 3;
            var builder = new StringBuilder();
            if (positions[0] <= trailLength)
            {
                positions = positions.Slice(1);
                positionsCount--;
            }

            if (positionsCount == 0) 
                return str;

            if ((str.Length - positions[positionsCount - 1]) <= trailLength)
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
                builder.Append('-');
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
            foreach (var tmpl in russianTemplates)
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