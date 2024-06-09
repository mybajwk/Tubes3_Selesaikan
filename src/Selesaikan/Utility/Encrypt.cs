using System;
using System.Text;

public class Blowfish
{
    // P-array nyimpan 18 subkey hasil ekspansi
    private uint[] P;
    // S-boxes nyimpan value sembarang, untuk membuat data non-linear sehingga lebih aman
    private uint[,] S;
    private const int BlockSize = 8;

    public Blowfish(byte[] key)
    {
        SetupKey(key);
    }

    private void SetupKey(byte[] key)
    {
        P = new uint[18];
        S = new uint[4, 256];

        // P-array bakal diinisialisasi dengan nilai i, trus bakal diXOR-in sama key spt di bawah
        // Untuk s-boxes, diinisialisasi nilai random. disini dibuat seperti dibawah.
        for (int i = 0; i < 18; i++) P[i] = (uint)i;
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 256; j++)
                S[i, j] = (uint)(i * 256 + j);

        int keyIndex = 0;
        for (int i = 0; i < 18; i++)
        {
            // Mencari nilai untuk diXORin sama nilai p-array sekarang
            // Penambahan key dilakukan berkelanjutan antara subkey, jadi misal
            // Untuk subkey-0 keyindex berakhir di 2, maka subkey-1 nilai data ny mulai dari indexkey-3, dst
            uint data = 0x00000000;
            for (int j = 0; j < 4; j++)
            {
                data = (data << 8) | key[keyIndex];
                keyIndex++;
                if (keyIndex >= key.Length)
                    keyIndex = 0;
            }
            P[i] ^= data;
        }
    }

    public string Encrypt(string plainText)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        // Tambahin padding kalau belum kelipatan 64 bit, karena blowfish operates pada data 64 bit
        byte[] paddedBytes = ApplyPadding(plainBytes, BlockSize);
        byte[] cipherBytes = ProcessBytes(paddedBytes, true);
        return Convert.ToBase64String(cipherBytes);
    }

    public string Decrypt(string cipherText)
    {
        // Kebalikan dari encrypt, tahap2nya sama cuma kebalikannya
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        byte[] plainBytes = ProcessBytes(cipherBytes, false);

        // Membuang padding tambahan (syarat kelipatan 64 bit di encrypt)
        byte[] unpaddedBytes = RemovePadding(plainBytes, BlockSize);
        return Encoding.UTF8.GetString(unpaddedBytes);
    }

    private byte[] ProcessBytes(byte[] inputBytes, bool encrypt)
    {
        int paddedLength = inputBytes.Length;

        // Nampung hasil dari tiap 64 bit block
        byte[] outputBytes = new byte[paddedLength];

        // variabel temp nyimpan untuk nyimpan tiap 64 bit dari inputBytes
        byte[] block = new byte[BlockSize];
        for (int i = 0; i < paddedLength; i += BlockSize){
            // Ambil 64 bit block yang ke-i
            Array.Copy(inputBytes, i, block, 0, BlockSize);
            if (encrypt){
                EncryptBlock(block);
            }else{
                DecryptBlock(block);
            }

            // Simpan hasil encrypt/decrypt
            Array.Copy(block, 0, outputBytes, i, BlockSize);
        }

        return outputBytes;
    }

    private void EncryptBlock(byte[] block)
    {
        // Pisahin block jadi dua, bagian kiri dan kanan
        // sehingga left dan right masing2 berukuran 4 byte
        uint left = BitConverter.ToUInt32(block, 0);
        uint right = BitConverter.ToUInt32(block, 4);

        // Iterasi untuk key-0 hingga key-16
        for (int i = 0; i < 16; i++)
        {
            // Fungsi encrypt nya adalah left dixor dengan nilai p,
            // right dixor kan sama hasil f(left)
            //left dan right ditukar
            // bagian ini melakukan enkripsi block dengan menambahkan konsep non-linear juga
            left ^= P[i];
            right ^= F(left);

            uint temp = left;
            left = right;
            right = temp;
        }

        // Kemudian left akhir diassign nilai right, dan sebaliknya
        uint finalLeft = right;
        uint finalRight = left;

        // right dixor dengan subkey ke 17, left dixor dengan subkey ke 18
        finalRight ^= P[16];
        finalLeft ^= P[17];

        // Gabungin hasil final left dan right dalam satu array byte berukuran 4+4 = 8 byte
        byte[] encryptedBlock = new byte[8];
        Array.Copy(BitConverter.GetBytes(finalLeft), 0, encryptedBlock, 0, 4);
        Array.Copy(BitConverter.GetBytes(finalRight), 0, encryptedBlock, 4, 4);

        Array.Copy(encryptedBlock, block, 8);
    }

    private void DecryptBlock(byte[] block)
    {
        // Sama seperti EncryptBlock, tetapi bedanya iterasi subkey kebalikan dari encrypt,
        // yakni dari subkey terakhir hingga awal
        uint left = BitConverter.ToUInt32(block, 0);
        uint right = BitConverter.ToUInt32(block, 4);

        for (int i = 17; i > 1; i--)
        {
            left ^= P[i];
            right ^= F(left);

            uint temp = left;
            left = right;
            right = temp;
        }

        uint finalLeft = right;
        uint finalRight = left;

        finalRight ^= P[1];
        finalLeft ^= P[0];

        byte[] decryptedBlock = new byte[8];
        Array.Copy(BitConverter.GetBytes(finalLeft), 0, decryptedBlock, 0, 4);
        Array.Copy(BitConverter.GetBytes(finalRight), 0, decryptedBlock, 4, 4);

        Array.Copy(decryptedBlock, block, 8);
    }

    // fungsi ini membuat data menjadi semakin tidak traceable, dengan menggunakan nilai s-box untuk mengacak2 nilai x
    private uint F(uint x)
    {
        uint h = S[0, (x >> 24) & 0xFF] + S[1, (x >> 16) & 0xFF];
        h ^= S[2, (x >> 8) & 0xFF];
        h += S[3, x & 0xFF];
        return h;
    }

    // Fungsi untuk menambahkan padding agar menjadi kelipatan 64 bit
    private byte[] ApplyPadding(byte[] inputBytes, int blockSize)
    {
        // Mendapatkan jumlah padding yang perlu ditambahkan
        // Jika inputBytes sudah berupa kelipatan 64 bit, nilai padding length menjadi blockSize / 8
        int paddingLength = blockSize - (inputBytes.Length % blockSize);
        byte[] paddedBytes = new byte[inputBytes.Length + paddingLength];
        Array.Copy(inputBytes, paddedBytes, inputBytes.Length);
        // Mengisi nilai byte padding yang ditambahkan dengan jumlah padding yg ditambahkan
        // Hal ini berguna untuk decrypt, dimana padding tambahan ini perlu diremove
        for (int i = inputBytes.Length; i < paddedBytes.Length; i++)
        {
            paddedBytes[i] = (byte)paddingLength;
        }
        return paddedBytes;
    }

    // Fungsi untuk mengurangi padding yang sebelumnya ditambahkan pada operasi encrypt
    private byte[] RemovePadding(byte[] inputBytes, int blockSize)
    {
        // Mendapatkan jumlah padding tambahan dari index paling akhir
        int paddingLength = inputBytes[inputBytes.Length - 1];
        if (paddingLength < 1 || paddingLength > blockSize){
            throw new InvalidOperationException("Invalid padding");
        }
        byte[] unpaddedBytes = new byte[inputBytes.Length - paddingLength];
        Array.Copy(inputBytes, unpaddedBytes, unpaddedBytes.Length);
        return unpaddedBytes;
    }
}

// public class Program
// {
//     public static void Main()
//     {
//         byte[] key = Encoding.UTF8.GetBytes("samplekey");

//         // Initialize Blowfish with the key
//         Blowfish blowfish = new Blowfish(key);

//         // Sample plaintext
//         string plainText = "awdawdawdawd";

//         // Encrypt the plaintext
//         string cipherText = blowfish.Encrypt(plainText);
//         Console.WriteLine("Encrypted: " + cipherText);

//         // Decrypt the ciphertext
//         string decryptedText = blowfish.Decrypt(cipherText);
//         Console.WriteLine("Decrypted: " + decryptedText);

//         // Check if the decrypted text matches the original plaintext
//         if (plainText == decryptedText)
//         {
//             Console.WriteLine("Success: The decrypted text matches the original plaintext.");
//         }
//         else
//         {
//             Console.WriteLine("Failure: The decrypted text does not match the original plaintext.");
//         }
//     }
// }
