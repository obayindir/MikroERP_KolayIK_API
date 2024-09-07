using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisionKolayIK
{
    public class SqlBaglanti
    {
        string baglantiadresi;
        public SqlConnection baglan()
        {
            //StreamReader oku = new StreamReader(@"C:\SmsIsemri\database.ini");
            StreamReader oku = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.ini"));
            string satir = oku.ReadLine();
            while (satir != null)
            {
                baglantiadresi = satir;
                satir = oku.ReadLine();
            }
            SqlConnection baglanti = new SqlConnection(baglantiadresi);
            baglanti.Open();
            return baglanti;
        }

        public SqlConnection bglkapat()
        {
            //StreamReader oku2 = new StreamReader(@"C:\SmsIsemri\database.ini");
            StreamReader oku2 = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.ini"));
            string satir = oku2.ReadLine();
            while (satir != null)
            {
                baglantiadresi = satir;
                satir = oku2.ReadLine();
            }
            SqlConnection baglanti1 = new SqlConnection(baglantiadresi);
            baglanti1.Close();
            return baglanti1;
        }
    }
}
