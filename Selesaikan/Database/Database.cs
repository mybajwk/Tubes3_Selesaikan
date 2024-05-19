using MySql.Data.MySqlClient;
using System.Configuration;
using Selesaikan.Models;

namespace Selesaikan.Database
{
    public class Database
    {
        private readonly string _connectionString;

        public Database()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
            Console.WriteLine(_connectionString);
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

        public List<Biodata> GetBiodata(string name)
        {
            List<Biodata> dataBiodata = new List<Biodata>();
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
                            dataBiodata.Add(new Biodata()
                            {
                                Nik = reader.GetString(0),
                                Nama = reader.GetString(1),
                                TempatLahir = reader.GetString(2),
                                TanggalLahir = reader.GetString(3),
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
    }
}
