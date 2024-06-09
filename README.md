# Tubes3_Selesaikan

### Algoritma KMP (Knuth-Morris-Pratt)

Algoritma KMP digunakan untuk mencari suatu pola di dalam teks dengan efisiensi tinggi. KMP menggunakan preproses pola untuk membuat tabel "longest prefix which is also suffix" (LPS) yang membantu dalam mempercepat pencarian dengan menghindari pemeriksaan ulang karakter yang sudah diproses.

### Algoritma BM (Boyer-Moore)

Algoritma Boyer-Moore juga digunakan untuk pencarian pola di dalam teks. Algoritma ini berjalan dari kanan ke kiri dan menggunakan dua heuristik utama yaitu "bad character rule" dan "good suffix rule" untuk menggeser pola sejauh mungkin ketika terjadi ketidakcocokan, sehingga pencarian menjadi lebih cepat pada rata-rata kasus.

### Ekspresi Reguler

Ekspresi reguler adalah rangkaian karakter yang mendefinisikan pola pencarian. Ekspresi reguler digunakan untuk mencocokkan, mencari, dan memanipulasi teks berdasarkan pola yang didefinisikan. Dalam proyek ini, ekspresi reguler digunakan untuk menangani teks dalam "bahasa alay" dan mengembalikannya ke bentuk alfabetik asli.

### Requirement

- .NET Framework
- Docker

### Instalasi

1. **.NET Framework**:

   - Pastikan .NET Framework sudah terinstall di sistem Anda. Untuk menginstall .NET Framework, kunjungi [situs resmi .NET](https://dotnet.microsoft.com/download/dotnet-framework).

2. **Docker**:

   - Docker dan Docker Compose diperlukan untuk setup database. Untuk menginstall Docker, kunjungi [situs resmi Docker](https://www.docker.com/get-started).

3. **Database Setup**:
   - Gunakan Docker Compose untuk mengatur database dan mengimport file SQL ke dalam database.

## Langkah Penggunaan
- Langkah-langkah untuk menjalankan aplikasi pencocokan sidik jari, yaitu sebagai berikut.
1. Pertama, pastikan memiliki kode daripada aplikasi ini, dengan melakukan cloning dari github yang terdapat pada lampiran laporan ini. Dapat dilakukan dengan command sebagai berikut pada terminal, seperti berikut.
```
git clone https://github.com/mybajwk/Tubes3_Selesaikan.git
```

2. Masuk ke folder source code, yaitu folder `src`. Dengan melakukan command berikut setelah melakukan _repository clone_
```
cd Tubes3_Selesaikan
cd src
```

3. Kemudian, lakukan `docker compose up` pada root folder `src` yang mengandung file docker.compose.yaml. Pastikan docker desktop sudah dihidupkan terlebih dahulu sebelum menjalankan aplikasi ini.
4. Setelah itu, pastikan docker sudah berjalan dengan baik dengan memastikan pada `http://localhost:8080/` terdapat Adminer yang telah aktif untuk memudahkan perlakukan import SQL pada database. Dengan adanya Adminer ini, pengguna akan lebih mudah berinteraksi dengan basis data. Untuk mengakses Adminer, akan diminta data-data untuk verifikasi untuk mengakses basis data yang hendak diakses, dengan **credentials** sebagai berikut.
- System : MySQL
- Server : mysql-service.selesaikan.local
- Username : selesaikan
- Password : selesaikan
- Database : selesaikan-db

5. Pada Adminer yang terdapat pada `http://localhost:8080/`, klik tombol “import” dan pilih file `selesaikan-db.sql` dan kemudian tombol “**execute**”. Maka, dengan itu maka semua basis data yang dibutuhkan aplikasi sudah ter-setup dengan baik beserta dengan data-data dummy untuk relative path gambar sidik jari beserta dengan nama orang pemiliknya.
6. Terlebih dahulu ingat, untuk menyimpan foto-foto sidik jari dengan folder name `test` pada folder `Selesaikan` sejajar dengan `MainWindow.xaml`. Di dalam folder _test_ ini dapat disimpan gambar gambar sidik jari. Path ini dapat disesuaikan secara relatif dengan data sidik jari yang tersimpan pada basis data Sidik Jari.

7. Lakukan run aplikasi dengan melakukan command berikut untuk masuk ke folder `Selesaikan` setelah memasuki folder `src`. Sebelum itu, pastikan bahwa file `config.json` memiliki `isEncrypted: false` jika basis data Biodata belum pernah dienkripsi. Jika sudah pernah, seharusnya `isEncrypted` sudah otomatis menjadi true.
```
cd Selesaikan
dotnet build
dotnet run
```
7. Pengguna dapat memilih terlebih dahulu dengan menekan tombol “KMP” atau “BM” untuk menemukan jenis algoritma apa yang hendak yang digunakan untuk melakukan pencocokan sidik jari. Klik tombol “Load Image” untuk memilih foto yang hendak dibandingkan dengan sidik jari yang terdapat pada basis data. Berdasarkan gambar sidik jari yang di-load oleh pengguna, aplikasi akan mencari sidik jari yang paling matching berdasarkan algoritma yang dipilih.

8. Pada akhirnya, setelah seluruh langkah sebelumnya dilakukan secara prosedural, maka akan muncul output gambar sidik jari yang match dengan gambar sidik jari yang dimasukkan oleh pengguna. Selain itu, juga terdapat informasi pemilik sidik jari yang match tersebut di sampingnya. Juga terdapat informasi lama pencarian sidik jari tersebut dalam format ms (milisekon) jika waktu eksekusi < 1000ms, dan format s (sekon) jika hasil dalam >1000ms. Kemudian, terdapat juga informasi similarity percentage (persentase kemiripan) yang didapatkan dari hasil hamming distance dibagi dengan panjang string yang diambil dari sebuah sidik jari.

**NOTE UNTUK PENGGUNA DEBUGGER IDE** 
- Jika pengguna menggunakan debugger yang terdapat pada IDE, dapat melakukan _copy_ terhadap `config.json` ke folder `bin` dan set menjadi _false_. Serta, folder _test_ atau folder untuk menyimpan gambar dapat disimpan pada folder `bin` pula karena debugger akan memiliki relative path yang dimulai dari `bin/Debug/net8.0-windows`. Aturan `config.json` disesuaikan sesuai dengan yang telah dijelaskan seperti penggunaan dengan _build manual_.
  
## Identitas
1. 13522029 Ignatius Jhon Hezkiel Chan
2. 13522045 Elbert Chailes
3. 13522077 Enrique Yanuar
