using System;
using System.Text;

// ReSharper disable StringLiteralTypo

namespace BookBuilder.Extensions.Hyphen
{
    public ref struct HyphensChecker
    {
        private ReadOnlySpan<char> _src;
        private bool _solved;
        private int _hyphenPosition;
        
        public static string SplitWithHyphens(ReadOnlySpan<char> str)
        {
            var word = str;
            Span<int> positions = stackalloc int[str.Length/2];
            var lastPosition = 0;
            
            do
            {
                var hyphensChecker = HyphensChecker.From(word)
                    .Check("вест-ный")
                    .Check("крос-сплат")
                    .Check("след-стви")
                	.Check("рам-мно")
                    .Check("арен-до")
                    .Check("биб-ли")
                    .Check("о-те")
                    .Check("объ-ем")
                    .Check("обя-за")
                    .Check("тель-но")
                    .Check("рост-но")
                    .Check("вос-тре")
                    .Check("про-из")
                    .Check("чест-во")
                    .Check("клас-со")
                    .Check("при-клю")
                    .Check("пир-клю")
                    .Check("вспом-ни")
                    .Check("вклю-ча")
                    .Check("ко-прои")
                    .Check("мож-но")
                    .Check("взя-ла")
                    .Check("по-ться")
                    .Check("по-тся")
                    .Check("разъ-езд")
                    .Check("ап-ри")
                    .Check("ас-тро")
                    .Check("ки-но")
                    .Check("нош-па")
                    .Check("па-ет")
                    .Check("т-во")
                    ;
			
                if(hyphensChecker._solved)
                {
                    word = word.Slice(hyphensChecker._hyphenPosition);
                    var prev = lastPosition == 0 ? 0 : positions[lastPosition-1];
                    positions[lastPosition++] = prev + hyphensChecker._hyphenPosition;
                } else {
                    word = word.Slice(1);
                }
            } while (word.Length > 3);
	
            var builder = new StringBuilder();

            if (positions[0] < 2) { positions = positions.Slice(1); lastPosition--; }
	
            var lastIndex = 0;
            for (var i = 0; i < lastPosition; i++)
            {
                var charsToWrite = positions[i] - lastIndex;
                builder.Append(str.Slice(lastIndex, charsToWrite));
                builder.Append('-');
                lastIndex += charsToWrite;
            }
	
            builder.Append(str.Slice(lastIndex, str.Length - lastIndex));
            return builder.ToString();
        }

        private static HyphensChecker From(ReadOnlySpan<char> span)
        {
            return new HyphensChecker { _src = span };
        }

        private HyphensChecker Check(string tmpl)
        {
            if (_solved) return this;
            ReadOnlySpan<char> tmplChars = tmpl;
            Span<char> template = stackalloc char[tmplChars.Length-1];  // minus hyphen
            _hyphenPosition=-1;
            for (int i = 0, j = 0; i < tmplChars.Length; i++)
            {
                if (tmplChars[i] == '-')
                {
                    _hyphenPosition = i;
                }
                else
                {
                    template[j] = tmplChars[i];
                    j++;
                }
            }

            for (int i = 0; i < template.Length; i++)
            {
                if (i == _src.Length) return this;
			
                if ((template[i].IsVowel() != _src[i].IsVowel()) ||
                    (template[i].IsNoSound() != _src[i].IsNoSound()) ||
                    (template[i].IsConsonant() != _src[i].IsConsonant()))
                    return this;
            }

            _solved = _hyphenPosition > 0;
		
            return this;
        }
    }
}