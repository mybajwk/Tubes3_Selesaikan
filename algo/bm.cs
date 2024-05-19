using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

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
    static Bitmap ConvertToGrayscale(Bitmap original)
    {
        Bitmap grayscale = new Bitmap(original.Width, original.Height);

        for (int i = 0; i < original.Width; i++)
        {
            for (int j = 0; j < original.Height; j++)
            {
                Color originalColor = original.GetPixel(i, j);
                int grayScale = (int)((originalColor.R * 0.3) + (originalColor.G * 0.59) + (originalColor.B * 0.11));
                Color grayColor = Color.FromArgb(grayScale, grayScale, grayScale);
                grayscale.SetPixel(i, j, grayColor);
            }
        }

        return grayscale;
    }

    static Bitmap ConvertToBinary(Bitmap grayscale)
    {
        Bitmap binary = new Bitmap(grayscale.Width, grayscale.Height);

        for (int i = 0; i < grayscale.Width; i++)
        {
            for (int j = 0; j < grayscale.Height; j++)
            {
                Color color = grayscale.GetPixel(i, j);
                int value = color.R < 128 ? 0 : 255;
                Color binaryColor = Color.FromArgb(value, value, value);
                int binaryValue = color.R == 0 ? 0 : 1;
                binary.SetPixel(i, j, binaryValue);
            }
        }

        return binary;
    }

    static string BitmapToASCII(Bitmap Binary,int row,int col){
        int widthBox = col*8;
        int heightBox = row;
        int totalPixel = widthBox*heightBox;
        StringBuilder sb = new StringBuilder();

        for(int i=0;i<Binary.Height;i+=heightBox){
            if(Binary.Height-i<rowBox){
                break;
            }
            for(int j=0;j<Binary.Width;j+=widthBox){
                if(Binary.Width-j<heightBox){
                    break;
                }
                stringBuilder buf = new stringBuilder();
                for(int k=i;k<i+heightBox;k++){
                    for(int l=j;l<j+widthBox;l++){
                        buf.Append(Binary.GetPixel(k,l));
                    }
                }
                for(int k=0;k<totalPixel;k+=8){
                    string byteString = buf.Substring(k,8);
                    sb.Append(ConvertToBinary.ToChar(Convert.ToInt32(byteString,2)));
                }
            }
        }
        return sb.ToString();
    }

    static string GetBinaryString(Bitmap binary)
    {
        StringBuilder sb = new StringBuilder();

        for (int j = 0; j < binary.Height; j++)
        {
            for (int i = 0; i < binary.Width; i++)
            {
                Color color = binary.GetPixel(i, j);
                int binaryValue = color.R == 0 ? 0 : 1;
                sb.Append(binaryValue);
            }
        }

        return sb.ToString();
    }

    static string BinaryStringToASCII(string binaryString)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < binaryString.Length; i += 8)
        {
            string byteString = binaryString.Substring(i, 8);
            sb.Append(Convert.ToChar(Convert.ToInt32(byteString, 2)));
        }

        return sb.ToString();
    } 

    public static void Main() {
        // Load an image
        string imagePath = "tes.bmp";
        string alterPath = "tes_alter.bmp";

        Bitmap original = new Bitmap(imagePath);
        
        // Convert to grayscale
        Bitmap grayscale = ConvertToGrayscale(original);
        
        // Convert grayscale to binary
        Bitmap binary = ConvertToBinary(grayscale);

        // Print binary values (0 and 1) to the console
        string binaryString = GetBinaryString(binary);

        // Convert binary string to ASCII
        string asciiString = BinaryStringToASCII(binaryString);
        Search(asciiString,alterPath);


    }
}
