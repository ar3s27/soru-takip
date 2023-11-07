using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SoruTakip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=AFRODIT\SQLEXPRESS;Initial Catalog=SoruTakip;Integrated Security=True");

        private int GetCurrentDatabaseValue(string ders)
        {
            int value = 0;
            baglanti.Open();
            SqlCommand selectCommand = new SqlCommand("SELECT CozulenSoru FROM DersSoru WHERE Dersler = @ders", baglanti);
            selectCommand.Parameters.AddWithValue("@ders", ders);
            SqlDataReader reader = selectCommand.ExecuteReader();
            if (reader.Read())
            {
                value = reader.GetInt32(0);
            }
            baglanti.Close();
            return value;
        }

        private void UpdateDatabase(string ders, int value)
        {
            baglanti.Open();

            // Update the database using parameterized queries
            SqlCommand updateCommand = new SqlCommand("UPDATE DersSoru SET CozulenSoru = @value WHERE Dersler = @ders", baglanti);
            updateCommand.Parameters.AddWithValue("@value", value);
            updateCommand.Parameters.AddWithValue("@ders", ders);
            updateCommand.ExecuteNonQuery();

            baglanti.Close();
        }



        private void RefreshChart()
        {
            chart1.Series["Ders"].Points.Clear(); // Clear existing data

            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select Dersler, CozulenSoru from DersSoru", baglanti);
            SqlDataReader oku = komut.ExecuteReader();
            while (oku.Read())
            {
                chart1.Series["Ders"].Points.AddXY(oku[0].ToString(), oku[1].ToString());
            }
            baglanti.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select Dersler,CozulenSoru from DersSoru", baglanti);
            SqlDataReader oku = komut.ExecuteReader();
            while (oku.Read())
            {
                chart1.Series["Ders"].Points.AddXY(oku[0].ToString(), oku[1].ToString());
            }
            baglanti.Close();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // Get the values from textboxes
            int matValue = int.Parse(matTxt.Text);
            int geoValue = int.Parse(geoTxt.Text);
            int turValue = int.Parse(turTxt.Text);

            // Get the current values from the database
            int currentMatValue = GetCurrentDatabaseValue("Matematik");
            int currentGeoValue = GetCurrentDatabaseValue("Geometri");
            int currentTurValue = GetCurrentDatabaseValue("Turkce");

            // Calculate the new values by adding the entered values to the current values
            int updatedMatValue = currentMatValue + matValue;
            int updatedGeoValue = currentGeoValue + geoValue;
            int updatedTurValue = currentTurValue + turValue;

            // Update the database with the new values
            UpdateDatabase("Matematik", updatedMatValue);
            UpdateDatabase("Geometri", updatedGeoValue);
            UpdateDatabase("Turkce", updatedTurValue);

            // Refresh the chart
            RefreshChart();
        }
    }
}
