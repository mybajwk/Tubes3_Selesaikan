using System;

public class KMPMatcher {
    public static int KMPSearch(string text, string pattern) {
        int n = text.Length;
        int m = pattern.Length;
        int[] lps = new int[m];
        int j = 0; // Length of previous longest prefix suffix

        // Preprocess the pattern to calculate lps array
        ComputeLPSArray(pattern, m, lps);

        int i = 0; // Index for text
        while (i < n) {
            if (pattern[j] == text[i]) {
                j++;
                i++;
            }
            if (j == m) {
                Console.WriteLine("Found pattern at index " + (i - j));
                j = lps[j - 1];
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

    private static void ComputeLPSArray(string pattern, int m, int[] lps) {
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

    // public static void Main() {
    //     string text = "ABABDABACDABABCABAB";
    //     string pattern = "ABABCABAB";
    //     KMPSearch(text, pattern);
    // }
}
