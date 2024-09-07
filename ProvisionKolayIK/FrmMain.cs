using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using RenksanadaSms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProvisionKolayIK
{
    public partial class FrmMain : DevExpress.XtraEditors.XtraForm
    {
        SqlBaglanti bgl = new SqlBaglanti();
        int selectedMonthNumber;
        string filePath;
        public FrmMain()
        {
            InitializeComponent();
        }

        void loadGrid()
        {

            string sqlCommand = $"SELECT pt_pkod,per_adi,pt_maliyil,pt_tah_ay FROM PERSONEL_TAHAKKUKLARI " +
                $"left outer join PERSONELLER ON per_kod=pt_pkod " +
                $"where pt_tah_ay={selectedMonthNumber} AND pt_maliyil={cmbYear.SelectedItem} AND pt_kesin='1' ";
            using (SqlCommand comand = new SqlCommand(sqlCommand, bgl.baglan()))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(comand);
                DataTable dataTable = new DataTable();

                dataAdapter.Fill(dataTable);

                Invoke(new MethodInvoker(delegate
                {
                    gridControl1.DataSource = dataTable;
                }));

            }

        }
        void loadCombobox()
        {
            for (int i = 1; i <= 12; i++)
            {
                comboBox1.Items.Add(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));
            }

        }

        void loadComboboxYear()
        {
            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < 20; i++)
            {
                int year = currentYear - i;
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedItem = currentYear;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            loadCombobox();
            loadComboboxYear();

            string dosyaIcerigi = File.ReadAllText("dosya_yolu.txt");
            filePath = dosyaIcerigi;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMonthName = comboBox1.SelectedItem.ToString();
            selectedMonthNumber = DateTime.ParseExact(selectedMonthName, "MMMM", CultureInfo.CurrentCulture).Month;


            if (comboBox1.SelectedItem != null)
            {
                loadGrid();
            }


        }

        void ShowWaitForm()
        {
            // SplashScreenManager ile WaitForm'u göster
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
        }

        void CloseWaitForm()
        {
            // SplashScreenManager ile WaitForm'u kapat
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SplashScreenManager.CloseForm(false)));
            }
            else
            {
                SplashScreenManager.CloseForm(false);
            }
        }
        private void btnAktar_Click(object sender, EventArgs e)
        {
            ShowWaitForm();
            pdfFileNameChange();
            Task.Run(async () =>
            {
                await UploadFileToApi();
            });

            CloseWaitForm();
        }

        void pdfFileNameChange()
        {
            
            string[] pdfFiles = Directory.GetFiles(filePath, "*.pdf");
            foreach (var pdfFile in pdfFiles)
            {
                
                string querry = $"select per_orjdildeadisoyadi from PERSONELLER where per_kod='{Path.GetFileNameWithoutExtension(pdfFile)}'";
                using (SqlCommand command = new SqlCommand(querry,bgl.baglan()))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string per_orjdildeadisoyadi = reader.GetString(0);
                            string newFileName = per_orjdildeadisoyadi+".pdf";
                            File.Move(pdfFile, Path.Combine(filePath, newFileName));
                        }

                    }
                }
                
            }
        }

       

        private async Task UploadFileToApi()
        {
            try
            {
                var client = new HttpClient();
                var apiUrl = "https://api.kolayik.com/v2/person/upload-file";

                var accessToken = "your_bearer_token";

                // Her bir personId için bir PDF dosyası işle
                var pdfFiles = Directory.GetFiles(filePath, "*.pdf");

                foreach (var pdfFile in pdfFiles)
                {
                    var fileName = Path.GetFileName(pdfFile);

                    // API'ye gönderilecek isteği oluşturun
                    using (var request = new HttpRequestMessage(HttpMethod.Post, apiUrl))
                    {
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                        using (var content = new MultipartFormDataContent())
                        {
                            // API'nin beklediği parametreleri ekleyin
                            content.Add(new StringContent($"{Path.GetFileNameWithoutExtension(pdfFile)}"), "personId");
                            content.Add(new StringContent("Payroll"), "folderName");

                            // Dosyayı içeren StreamContent ekleyin
                            using (var fileStream = File.OpenRead(pdfFile))
                            {
                                content.Add(new StreamContent(fileStream), "file", fileName);
                            }

                            // İsteğe içeriği ekleyin
                            request.Content = content;

                            // İsteği API'ye gönderin
                            var response = await client.SendAsync(request);

                            // Başarı durumunu kontrol edin
                            response.EnsureSuccessStatusCode();

                            //MessageBox.Show(await response.Content.ReadAsStringAsync(), "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    
                }
                MessageBox.Show("Tüm dosyalar başarıyla yüklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            pdfFileNameChange();
        }
    }
}
