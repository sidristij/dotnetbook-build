namespace BookBuilder.Extensions.Hyphen
{
    public static class CharEx
    {
        public static bool IsRussian(this char ch) => 
            (ch >= 'а' && ch <= 'я') || (ch >= 'А' && ch <= 'Я');
        
        public static bool IsEnglish(this char ch) => 
            (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');

        public static bool IsSupportedLanguage(this char ch) =>
            IsRussian(ch) || IsEnglish(ch);

        public static bool IsLanguageEqual(this char ch1, char ch2) =>
            IsRussian(ch1) == IsRussian(ch2) && 
            IsEnglish(ch1) == IsEnglish(ch2);
        
        public static bool IsVowel(this char ch) => 
            // Russian
            ch == 'а' || ch == 'о' || ch == 'у' || ch == 'э' || 
            ch == 'ю' || ch == 'я' || ch == 'е' || ch == 'ё' || 
            ch == 'и' || ch == 'ы' ||
            
            // English
            ch == 'a' || ch == 'e' || ch == 'i' || ch == 'o' || ch == 'u' ;
        public static bool IsConsonant(this char ch) => 
            !IsVowel(ch) && !IsNoSound(ch);
        public static bool IsNoSound(this char ch) => 
            ch == 'ь' || ch == 'ъ' || ch == 'h';
    }
}