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

## Langkah menggunakan

1. docker compose up -d
2. buka http://localhost:8080 dan import file "selesaikan-db.sql" ke adminer jika belum ada
3. run "dotnet run"
4. masukkan image dan search sesuai dengan algoritma yang ingin digunakan

## Identitas

1. 13522029 Ignatius Jhon Hezkiel Chan
2. 13522045 Elbert Chailes
3. 13522077 Enrique Yanuar
