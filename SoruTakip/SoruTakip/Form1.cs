using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;


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
            int matValue = int.Parse(matTxt.Text);
            int geoValue = int.Parse(geoTxt.Text);
            int turValue = int.Parse(turTxt.Text);

            int currentMatValue = GetCurrentDatabaseValue("Matematik");
            int currentGeoValue = GetCurrentDatabaseValue("Geometri");
            int currentTurValue = GetCurrentDatabaseValue("Turkce");

            int updatedMatValue = currentMatValue + matValue;
            int updatedGeoValue = currentGeoValue + geoValue;
            int updatedTurValue = currentTurValue + turValue;

            UpdateDatabase("Matematik", updatedMatValue);
            UpdateDatabase("Geometri", updatedGeoValue);
            UpdateDatabase("Turkce", updatedTurValue);

            RefreshChart();
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            Chart chart = (Chart)sender;
            HitTestResult hit = chart.HitTest(e.X, e.Y);

            if (hit.PointIndex >= 0 && hit.Series != null)
            {
                DataPoint dataPoint = chart.Series[hit.Series.Name].Points[hit.PointIndex];
                string value = $"{dataPoint.YValues[0]:N2}"; // Gerekirse değeri istediğiniz gibi biçimlendirin
                string seriesName = chart.Series[hit.Series.Name].Points[hit.PointIndex].AxisLabel;
                string tooltipText = $"{seriesName}: {value}";

                // ToolTip'i görüntüle
                chart.Series[hit.Series.Name].ToolTip = tooltipText;
            }
            else
            {
                // ToolTip'i temizle
                ToolTip tooltip = new ToolTip();
                tooltip.RemoveAll();
            }
        }
    }
}
