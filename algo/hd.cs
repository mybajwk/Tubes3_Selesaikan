using System;

public class HammingDistance {
    public static int Calculate(string s1, string s2) {
        if (s1.Length != s2.Length) {
            throw new ArgumentException("Strings must be of equal length");
        }

        int distance = 0;
        for (int i = 0; i < s1.Length; i++) {
            if (s1[i] != s2[i]) {
                distance++;
            }
        }

        return distance;
    }

    // public static void Main() {
    //     string str1 = "karat";
    //     string str2 = "karma";
    //     try {
    //         int distance = Calculate(str1, str2);
    //         Console.WriteLine("Hamming Distance: " + distance);
    //     } catch (ArgumentException e) {
    //         Console.WriteLine(e.Message);
    //     }
    // }
}
