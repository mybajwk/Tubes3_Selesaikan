using System;

public class LongestCommonSubsequence {
    public static int Calculate(string s1, string s2) {
        int len1 = s1.Length;
        int len2 = s2.Length;
        int[,] dp = new int[len1 + 1, len2 + 1];

        for (int i = 0; i <= len1; i++) {
            for (int j = 0; j <= len2; j++) {
                if (i == 0 || j == 0) {
                    dp[i, j] = 0;
                } else if (s1[i - 1] == s2[j - 1]) {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                } else {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        return dp[len1, len2];
    }

    public static void Main() {
        string str1 = "ABCBDAB";
        string str2 = "BDCABA";
        Console.WriteLine("LCS Length: " + Calculate(str1, str2));
    }
}
