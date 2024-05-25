using System.Text;
using MySql.Data.MySqlClient;
using Selesaikan.Models;

namespace Selesaikan.Database
{
    public class Database
    {
        private readonly string _connectionString;

        public Database()
        {
            Console.WriteLine("aaa");
            
            AppConfig loadedConfig = Config.Config.LoadConfiguration("config.json");
            Console.WriteLine(loadedConfig.DatabaseConnectionString);
            _connectionString = loadedConfig.DatabaseConnectionString;
            Console.WriteLine("bbb");
        }

        public List<SidikJari> GetSidikJari()
        {
            List<SidikJari> dataSidikJari = new List<SidikJari>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT nama, berkas_citra FROM sidik_jari";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataSidikJari.Add(new SidikJari()
                            {
                                Nama = reader.GetString(0),
                                BerkasCitra = reader.GetString(1),
                            });
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle exceptions related to SQL here
                Console.WriteLine("A SQL error occurred: " + e.Message);
            }
            catch (Exception e)
            {
                // Handle other types of exceptions here
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return dataSidikJari;
        }

        public Biodata GetBiodata(string name)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan FROM biodata WHERE nama = @Name";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Name", name);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new Biodata()
                            {
                                Nik = reader.GetString(0),
                                Nama = reader.GetString(1),
                                TempatLahir = reader.GetString(2),
                                TanggalLahir = reader.GetDateTime(3),
                                JenisKelamin = reader.GetString(4),
                                GolonganDarah = reader.GetString(5),
                                Alamat = reader.GetString(6),
                                Agama = reader.GetString(7),
                                StatusPerkawinan = reader.GetString(8),
                                Pekerjaan = reader.GetString(9),
                                Kewarganegaraan = reader.GetString(10),
                            };
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle exceptions related to SQL here
                Console.WriteLine("A SQL error occurred: " + e.Message);
            }
            catch (Exception e)
            {
                // Handle other types of exceptions here
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return new Biodata();
        }
        public List<Biodata> GetAllBiodata()
        {
            List<Biodata> dataBiodata = new List<Biodata>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan FROM biodata";
                    MySqlCommand command = new MySqlCommand(sql, connection);;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataBiodata.Add(new Biodata()
                            {
                                Nik = reader.GetString(0),
                                Nama = reader.GetString(1),
                                TempatLahir = reader.GetString(2),
                                TanggalLahir = reader.GetDateTime(3),
                                JenisKelamin = reader.GetString(4),
                                GolonganDarah = reader.GetString(5),
                                Alamat = reader.GetString(6),
                                Agama = reader.GetString(7),
                                StatusPerkawinan = reader.GetString(8),
                                Pekerjaan = reader.GetString(9),
                                Kewarganegaraan = reader.GetString(10),
                            });
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle exceptions related to SQL here
                Console.WriteLine("A SQL error occurred: " + e.Message);
            }
            catch (Exception e)
            {
                // Handle other types of exceptions here
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return dataBiodata;
        }

        public void TruncateBiodata()
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // SQL command to truncate the table
                        string sql = "TRUNCATE TABLE biodata";
                        MySqlCommand command = new MySqlCommand(sql, connection);
                        command.ExecuteNonQuery();

                        // Commit the transaction
                        transaction.Commit();
                        Console.WriteLine("Table 'biodata' has been truncated successfully.");
                    }
                    catch (MySqlException e)
                    {
                        // Handle exceptions related to SQL here
                        Console.WriteLine("A SQL error occurred: " + e.Message);
                        transaction.Rollback(); // Rollback the transaction on error
                    }
                    catch (Exception e)
                    {
                        // Handle other types of exceptions here
                        Console.WriteLine("An error occurred: " + e.Message);
                        transaction.Rollback(); // Rollback the transaction on error
                    }
                }
            }

        }

        public void SaveAllBiodata(List<Biodata> biodataList)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                AppConfig loadedConfig = Config.Config.LoadConfiguration("config.json");
                byte[] key = Encoding.UTF8.GetBytes(loadedConfig.Key);
                Blowfish blowfish = new Blowfish(key);
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var biodata in biodataList)
                        {
                            string encryptedNik = blowfish.Encrypt(biodata.Nik);
                            string encryptedNama = blowfish.Encrypt(biodata.Nama);
                            string encryptedTempatLahir = blowfish.Encrypt(biodata.TempatLahir);
                            string encryptedKewarganegaraan = blowfish.Encrypt(biodata.Kewarganegaraan);
                            // string encryptedJenisKelamin= blowfish.Encrypt(biodata.JenisKelamin);
                            string encryptedGolonganDarah = blowfish.Encrypt(biodata.GolonganDarah);
                            string encryptedAlamat = blowfish.Encrypt(biodata.Alamat);
                            string encryptedAgama = blowfish.Encrypt(biodata.Agama);
                            // string encryptedStatusPerkawinan= blowfish.Encrypt(biodata.StatusPerkawinan);
                            string encryptedPekerjaan = blowfish.Encrypt(biodata.Pekerjaan);

                            string sql =
                                "INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) " +
                                "VALUES (@NIK, @Nama, @TempatLahir, @TanggalLahir, @JenisKelamin, @GolonganDarah, @Alamat, @Agama, @StatusPerkawinan, @Pekerjaan, @Kewarganegaraan)";

                            MySqlCommand command = new MySqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@NIK", encryptedNik);
                            command.Parameters.AddWithValue("@Nama", encryptedNama);
                            command.Parameters.AddWithValue("@TempatLahir", encryptedTempatLahir);
                            command.Parameters.AddWithValue("@TanggalLahir", biodata.TanggalLahir);
                            command.Parameters.AddWithValue("@JenisKelamin", biodata.JenisKelamin);
                            command.Parameters.AddWithValue("@GolonganDarah", encryptedGolonganDarah);
                            command.Parameters.AddWithValue("@Alamat", encryptedAlamat);
                            command.Parameters.AddWithValue("@Agama", encryptedAgama);
                            command.Parameters.AddWithValue("@StatusPerkawinan", biodata.StatusPerkawinan);
                            command.Parameters.AddWithValue("@Pekerjaan", encryptedPekerjaan);
                            command.Parameters.AddWithValue("@Kewarganegaraan", encryptedKewarganegaraan);

                            command.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        // Handle exceptions related to SQL here
                        Console.WriteLine("A SQL error occurred: " + e.Message);
                    }
                    catch (Exception e)
                    {
                        // Handle other types of exceptions here
                        Console.WriteLine("An error occurred: " + e.Message);
                    }
                }
            }
        }

    }
}
