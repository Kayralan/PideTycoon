using UnityEngine;
using System.Collections.Generic;

public static class NumberFormatter
{
    // Standart sonekler
    private static readonly string[] standardSuffixes = new string[] 
    { 
        "", "K", "M", "B", "T" 
    };

    // Alfabetik sonekler (aa, ab, ac...) üretmek için harfler
    private static readonly string[] alphabet = new string[]
    {
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", 
        "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
    };

    public static string FormatNumber(float number)
    {
        // 1. Eğer sayı 1000'den küçükse direkt yazdır (Örn: 950)
        if (number < 1000)
            return number.ToString("N0");

        // 2. Bilimsel gösterim mantığı ile üssü bul (Logaritma 1000 tabanında)
        // 1.000.000 için exponent = 2 olur (1000^2)
        int exponent = (int)(Mathf.Log10(number) / 3);

        // 3. Bölünecek değeri hesapla
        // Eğer sayı 1.500.000 ise ve exponent 2 (M) ise, 
        // 1.000.000'a (1000^2) böleriz -> Sonuç: 1.5
        float shortNumber = number / Mathf.Pow(1000, exponent);

        // 4. Soneki Bul
        string suffix = GetSuffix(exponent);

        // 5. Sonucu birleştir (Örn: "1.50" + "M")
        return shortNumber.ToString("N2") + suffix;
    }

    private static string GetSuffix(int exponent)
    {
        // Eğer standart listenin içindeyse (K, M, B, T) oradan döndür
        if (exponent < standardSuffixes.Length)
        {
            return standardSuffixes[exponent];
        }

        // Eğer T'yi geçtiysek (exponent 5 ve üzeriyse) "aa, ab, ac" mantığına geç
        // Standart dışındaki indexi bul (T 4. index, yani 5. index "aa" olacak)
        int alphabetIndex = exponent - standardSuffixes.Length;

        // aa, ab, ac... mantığını matematiksel olarak üret
        // Bu formül sonsuza kadar aa, ab... az, ba, bb... diye gider.
        
        int firstCharIndex = alphabetIndex / 26; // İlk harf (a...)
        int secondCharIndex = alphabetIndex % 26; // İkinci harf (...a, ...b)

        // Eğer harfler biterse (zz'den sonra) başa sarmaması için basit koruma:
        if (firstCharIndex >= alphabet.Length) return "???"; 

        return alphabet[firstCharIndex] + alphabet[secondCharIndex];
    }
}