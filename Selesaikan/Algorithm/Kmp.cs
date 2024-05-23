namespace Selesaikan.Algorithm;

public class Kmp
{
    public static int KmpSearch(string text, string pattern) {
        int n = text.Length;
        int m = pattern.Length;
        int[] lps = new int[m];
        int j = 0; // Length of previous longest prefix suffix

        // Preprocess the pattern to calculate lps array
        ComputeLpsArray(pattern, m, lps);

        int i = 0; // Index for text
        while (i < n) {
            if (pattern[j] == text[i]) {
                j++;
                i++;
            }
            if (j == m) {
                Console.WriteLine("Found pattern at index " + (i - j));
                // j = lps[j - 1];
                return i;
            }
            else if (i < n && pattern[j] != text[i]) {
                if (j != 0)
                    j = lps[j - 1];
                else
                    i = i + 1;
            }
        }
        return -1;
    }

    private static void ComputeLpsArray(string pattern, int m, int[] lps) {
        int length = 0;
        int i = 1;
        lps[0] = 0; // lps[0] is always 0

        // Loop calculates lps[i] for i=1 to m-1
        while (i < m) {
            if (pattern[i] == pattern[length]) {
                length++;
                lps[i] = length;
                i++;
            } else {
                if (length != 0) {
                    length = lps[length - 1];
                } else {
                    lps[i] = 0;
                    i++;
                }
            }
        }
    }
}