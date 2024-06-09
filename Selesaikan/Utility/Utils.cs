namespace Selesaikan.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Selesaikan.Models;
using Selesaikan.Algorithm;
using System.Windows;

public class Utils
    {
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            if (bitmapImage.StreamSource == null || !bitmapImage.StreamSource.CanRead)
            {
                throw new ArgumentException("StreamSource is either null or cannot be read");
            }

            // Copy the stream to keep the original stream intact
            MemoryStream outStream = new MemoryStream();
            bitmapImage.StreamSource.Seek(0, SeekOrigin.Begin); // Ensure we start from the beginning
            bitmapImage.StreamSource.CopyTo(outStream);
            outStream.Seek(0, SeekOrigin.Begin); // Reset the position to the beginning

            // Create bitmap from copied stream
            return new Bitmap(outStream);
        }
        
        // Fungsi ConvertToGrayScale untuk mengubah gambar bitmap original menjadi bitmap grayscale
        public static Bitmap ConvertToGrayscale(Bitmap original)
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
        public static Bitmap ConvertToBinary(Bitmap grayscale)
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

        // Fungsi GetBinaryString untuk mengubah gambar bitmap binary menjadi string binary
        public static string GetBinaryString(Bitmap binary)
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
        public static string BinaryStringToASCII(string binaryString)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < binaryString.Length-8; i += 8)
            {
                
                string byteString = binaryString.Substring(i, 8);
                sb.Append(Convert.ToChar(Convert.ToInt32(byteString, 2)));
            }

            return sb.ToString();
        }

        public static string preproccToBinary(BitmapImage image){
            Bitmap tempEntryBitmap = ResizeImage(Utils.BitmapImage2Bitmap(image),96,103);
            Bitmap imageSidikJariGrayscale = Utils.ConvertToGrayscale(tempEntryBitmap);
            Bitmap imageSidikJariBinary = Utils.ConvertToBinary(imageSidikJariGrayscale);
            string imageSidikJariBinaryString = Utils.GetBinaryString(imageSidikJariBinary);
            return imageSidikJariBinaryString;
        }
        public static string PreproccToASCII(BitmapImage image){
            
            string imageSidikJariAscii = BinaryStringToASCII(preproccToBinary(image));
            return imageSidikJariAscii;
        }

        public static Bitmap ResizeImage(Bitmap originalBitmap, int newWidth, int newHeight)
        {
            // Check if the original image already has the desired dimensions
            if (originalBitmap.Width == newWidth && originalBitmap.Height == newHeight)
            {
                // If the dimensions are already the same, return a copy of the original image
                return new Bitmap(originalBitmap);
            }

            // Create a new bitmap with the desired dimensions
            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);
            
            // Create a graphics object for the new bitmap
            using (Graphics graphics = Graphics.FromImage(resizedBitmap))
            {
                // Set the interpolation mode to high quality
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // Draw the original image onto the new bitmap with the desired dimensions
                graphics.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);
            }

            // Return the resized bitmap
            return resizedBitmap;
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

            return sortedSubstrings.Take(2).ToArray();
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

        public static string MatchTexts(string inputText, List<string> testTexts)
        {

            Regex regex = BuildAlayRegex(inputText);
            foreach (string test in testTexts)
            {
                if (regex.IsMatch(test))
                {
                    return test;
                }
            }

            return "";
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
