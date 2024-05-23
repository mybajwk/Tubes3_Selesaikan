using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

public class Utils {

    // Fungsi ConvertToGrayScale untuk mengubah gambar bitmap original menjadi bitmap grayscale
    static Bitmap ConvertToGrayscale(Bitmap original) {
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

    // Fungsi ConvertToBinary untuk mengubah gambar bitmap grayscale menjadi gambar bitmap binary
    static Bitmap ConvertToBinary(Bitmap grayscale) {
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
    static string BitmapToASCII(Bitmap Binary,int row,int col) {
        int widthBox = col*8;
        int heightBox = row;
        int totalPixel = widthBox*heightBox;
        StringBuilder sb = new StringBuilder();

        for(int i=0;i<Binary.Height;i+=heightBox){
            if(i+heightBox<Binary.Height){
                break;
            }
            for(int j=0;j<Binary.Width;j+=widthBox){
                if(j+widthBox<Binary.Width){
                    break;
                }
                StringBuilder buf = new StringBuilder();
                for(int k=i;k<i+heightBox;k++){
                    for(int l=j;l<j+widthBox;l++){
                        Color color = Binary.GetPixel(k,l);
                        int binaryValue = color.R == 0 ? 0 : 1;
                        buf.Append(binaryValue);
                    }
                }
                for(int k=0;k<totalPixel;k+=8){
                    string byteString = buf.ToString().Substring(k,8);
                    sb.Append(Convert.ToChar(Convert.ToInt32(byteString,2)));
                }
            }
        }
        return sb.ToString();
    }

    // Fungsi GetBinaryString untuk mengubah gambar bitmap binary menjadi string binary
    static string GetBinaryString(Bitmap binary) {
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
    static string BinaryStringToASCII(string binaryString) {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < binaryString.Length; i += 8)
        {
            string byteString = binaryString.Substring(i, 8);
            sb.Append(Convert.ToChar(Convert.ToInt32(byteString, 2)));
        }

        return sb.ToString();
    } 

}