namespace BookBuilder.Extensions.Hyphen
{

    public static class CharEx
    {
        public static bool IsRussian(this char ch) => 
            (ch >= 'а' && ch <= 'я') || (ch >= 'А' && ch <= 'Я');
        public static bool IsVowel(this char ch) => 
            ch == 'а' || ch == 'о' || ch == 'у' || ch == 'э' || 
            ch == 'ю' || ch == 'я' || ch == 'е' || ch == 'ё' || 
            ch == 'и' || ch == 'ы';
        public static bool IsConsonant(this char ch) => 
            !IsVowel(ch) && !IsNoSound(ch);
        public static bool IsNoSound(this char ch) => 
            ch == 'ь' || ch == 'ъ';
    }
}