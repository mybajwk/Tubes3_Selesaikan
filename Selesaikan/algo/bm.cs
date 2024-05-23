public class BoyerMooreMatcher {
    public static int Search(string text, string pat) {
        int n = text.Length;
        int m = pat.Length;
        int skip;
        var right = new Dictionary<char, int>();

        // Build the skip table
        for (int i = 0; i < m; i++) {
            right[pat[i]] = i;
        }

        for (int i = 0; i <= n - m; i += skip) {
            skip = 0;
            for (int j = m - 1; j >= 0; j--) {
                if (pat[j] != text[i + j]) {
                    int val = -1;
                    if(right.ContainsKey(text[i+j])){
                        val = right[text[i+j]];
                    }
                    skip = Math.Max(1, j - val);
                    break;
                }
            }
            if (skip == 0) {
                Console.WriteLine("Pattern found at position " + i);
                return i; // Found
            }
        }

        return -1; // Not found
    }

    // public static void Main() {
    //     // Load an image
    //     string imagePath = "tes.bmp";

    //     Bitmap original = new Bitmap(imagePath);
        
    //     // Convert to grayscale
    //     Bitmap grayscale = ConvertToGrayscale(original);
        
    //     // Convert grayscale to binary
    //     Bitmap binary = ConvertToBinary(grayscale);

    //     // Convert binary string to ASCII
    //     string asciiString = BitmapToASCII(binary,6,5);
    //     Console.WriteLine(asciiString);


    // }
}
