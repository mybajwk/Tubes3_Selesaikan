using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;

namespace Selesaikan.Utils
{
    public class Utils
    {

        // Fungsi ConvertToGrayScale untuk mengubah gambar bitmap original menjadi bitmap grayscale
        static Bitmap ConvertToGrayscale(Bitmap original)
        {
            Bitmap grayscale = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    Color originalColor = original.GetPixel(i, j);
                    int grayScale =
                        (int)((originalColor.R * 0.3) + (originalColor.G * 0.59) + (originalColor.B * 0.11));
                    Color grayColor = Color.FromArgb(grayScale, grayScale, grayScale);
                    grayscale.SetPixel(i, j, grayColor);
                }
            }

            return grayscale;
        }

        // Fungsi ConvertToBinary untuk mengubah gambar bitmap grayscale menjadi gambar bitmap binary
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
                    binary.SetPixel(i, j, binaryColor);
                }
            }

            return binary;
        }

        // Fungsi BitmapToASCII untuk mengubah gambar bitmap menjadi string ASCII
        static string BitmapToASCII(Bitmap Binary, int row, int col)
        {
            int widthBox = col * 8;
            int heightBox = row;
            int totalPixel = widthBox * heightBox;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Binary.Height; i += heightBox)
            {
                if (i + heightBox < Binary.Height)
                {
                    break;
                }

                for (int j = 0; j < Binary.Width; j += widthBox)
                {
                    if (j + widthBox < Binary.Width)
                    {
                        break;
                    }

                    StringBuilder buf = new StringBuilder();
                    for (int k = i; k < i + heightBox; k++)
                    {
                        for (int l = j; l < j + widthBox; l++)
                        {
                            Color color = Binary.GetPixel(k, l);
                            int binaryValue = color.R == 0 ? 0 : 1;
                            buf.Append(binaryValue);
                        }
                    }

                    for (int k = 0; k < totalPixel; k += 8)
                    {
                        string byteString = buf.ToString().Substring(k, 8);
                        sb.Append(Convert.ToChar(Convert.ToInt32(byteString, 2)));
                    }
                }
            }

            return sb.ToString();
        }

        // Fungsi GetBinaryString untuk mengubah gambar bitmap binary menjadi string binary
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

        // Fungsi BinaryStringToASCII untuk mengubah string binary menjadi string ASCII
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

        public static string[] GetChoosenBlockBinaryString(string binaryString, int block, int move)
        {
            if (binaryString.Length < block)
                return new string[0]; // Return an empty
            List<string> allSubstrings = new List<string>();

            for (int i = 0; i <= binaryString.Length - block; i += move)
            {
                allSubstrings.Add(binaryString.Substring(i, block));
            }

            List<string> sortedSubstrings = allSubstrings.OrderByDescending(s => CountTransitions(s)).ToList();


            return sortedSubstrings.Take(5).ToArray();
        }

        private static int CountTransitions(string binary)
        {
            int transitionCount = 0;
            for (int i = 0; i < binary.Length - 1; i++)
            {
                if ((binary[i] == '0' && binary[i + 1] == '1') || (binary[i] == '1' && binary[i + 1] == '0'))
                {
                    transitionCount++;
                }
            }

            return transitionCount;
        }

        public static List<string> MatchTexts(string inputText, string[] testTexts)
        {

            List<string> results = new List<string>();
            Regex regex = BuildAlayRegex(inputText);
            foreach (string test in testTexts)
            {
                if (regex.IsMatch(test))
                {
                    results.Add($"Match found: '{test}'");
                }
                else
                {
                    results.Add($"No match: '{test}'");
                }
            }

            return results;
        }


        private static Regex BuildAlayRegex(string inputText)
        {
            Dictionary<char, string> substitutions = new Dictionary<char, string>
            {
                { 'a', "[a4@]?" },
                { 'b', "[b8]" },
                { 'c', "[c<({]" },
                { 'd', "[d)]" },
                { 'e', "[e3]?" },
                { 'f', "[f#]" },
                { 'g', "[g9]" },
                { 'h', "[h#]" },
                { 'i', "[i1!|l]?" },
                { 'j', "[j]" },
                { 'k', "[k]" },
                { 'l', "[l1!|]" },
                { 'm', "[m]" },
                { 'n', "[n]" },
                { 'o', "[o0]?" },
                { 'p', "[p]" },
                { 'q', "[q]" },
                { 'r', "[r]" },
                { 's', "[s5$]" },
                { 't', "[t7+]" },
                { 'u', "[u]?" },
                { 'v', "[v]" },
                { 'w', "[w]" },
                { 'x', "[x%]" },
                { 'y', "[y]" },
                { 'z', "[z2]" },
            };

            string pattern = ".*?";
            foreach (char ch in inputText.ToLower())
            {
                if (substitutions.ContainsKey(ch))
                {
                    pattern += substitutions[ch];
                }
                else
                {
                    pattern += Regex.Escape(ch.ToString()) + "?";
                }

                pattern += ".*?";
            }

            return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

    }
}