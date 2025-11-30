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

namespace Test_Sefer_Seyahat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
  
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-0NNHPCV\SQLEXPRESS;Initial Catalog=TestYolcuBilet;Integrated Security=True");
        void seferlistesi()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select * from TBLSEFERBILGI", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }
        void KoltukKirmiziYap(string koltukNo)
        {
            foreach (Control c in groupBox1.Controls)
            {
                if (c is Button && c.Text == koltukNo)
                {
                    c.BackColor = Color.Red;
                }
            }
        }
        void DoluKoltuklariBoyama(string seferNo)
        {
            baglanti.Open();

            SqlCommand komut = new SqlCommand(
                "SELECT koltuk FROM TBLSEFERDETAY WHERE seferno=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", seferNo);

            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                KoltukKirmiziYap(dr["koltuk"].ToString());
            }

            baglanti.Close();
        }
        private void BtnKoltuk_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            // Eğer koltuk doluysa engelle
            if (btn.BackColor == Color.Red)
            {
                MessageBox.Show("Bu koltuk dolu!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Önce sarı olan varsa resetle
            foreach (Control c in groupBox1.Controls)
            {
                if (c is Button b && b.BackColor == Color.Yellow)
                    b.BackColor = Color.LightGray;
            }

            // Seçili koltuğu sarı yap ve textbox'a yaz
            btn.BackColor = Color.Yellow;
            TxtKoltukNo.Text = btn.Text;
        }

        // Travego 16 2+1 koltuk düzeni (dinamik oluşturma örneği)
        // Bu kodu Form1.cs içine, KoltuklariYukle() yerine kullanabilirsiniz.
        private void KaptanKoltugu_Click(object sender, EventArgs e)
        {
            if (TxtSeferNo.Text == "")
            {
                MessageBox.Show("Önce bir sefer seçmeniz gerekiyor!", "Uyarı");
                return;
            }

            baglanti.Open();

            // Seçili sefere ait kaptan numarasını çek
            SqlCommand komut1 = new SqlCommand(
                "SELECT kaptan FROM TBLSEFERBILGI WHERE seferno=@p1", baglanti);
            komut1.Parameters.AddWithValue("@p1", TxtSeferNo.Text);

            object kaptanNoObj = komut1.ExecuteScalar();

            if (kaptanNoObj == null)
            {
                baglanti.Close();
                MessageBox.Show("Bu sefere ait kaptan bulunamadı!");
                return;
            }

            string kaptanNo = kaptanNoObj.ToString();
            TxtKaptanNo.Text = kaptanNo;  // Otomatik doldur

            // Şimdi kaptan bilgilerini çek
            SqlCommand komut2 = new SqlCommand(
                "SELECT adsoyad, telefon FROM tblkaptan WHERE kaptanno=@p1", baglanti);
            komut2.Parameters.AddWithValue("@p1", kaptanNo);

            SqlDataReader dr = komut2.ExecuteReader();

            if (dr.Read())
            {
                TxtAdSoyad.Text = dr["adsoyad"].ToString();
                MskTelefonKaptan.Text = dr["telefon"].ToString();
            }
            dr.Close();

            baglanti.Close();

            MessageBox.Show("Kaptan bilgileri yüklendi!", "Bilgi");
        }

        void KoltuklariYukle()
        {
            groupBox1.Controls.Clear();

            int solX = 30;      // Sol tekli koltuk
            int sagX = 120;     // Sağ ikili başlangıcı
            int startY = 60;
            int aralikY = 45;

            int koltukNo = 1;

            // --- KAPTAN KOLTUĞU ---
            Button kaptan = new Button();
            kaptan.Text = "K";
            kaptan.Width = 50;
            kaptan.Height = 40;
            kaptan.Location = new Point(90, 10);
            kaptan.BackColor = Color.LightBlue;
            kaptan.Click += KaptanKoltugu_Click;  // <-- tıklama olayı eklendi
            groupBox1.Controls.Add(kaptan);


            // --- 1 → 38 ARASI 2+1 KOLTUKLAR ---
            for (int i = 0; i < 13; i++)  // toplam 13 sıra
            {
                if (koltukNo > 38) break;

                // SOL TEKLİ
                Button sol = new Button();
                sol.Text = koltukNo.ToString();
                sol.Width = 50;
                sol.Height = 40;
                sol.Location = new Point(solX, startY + (i * aralikY));
                sol.Click += BtnKoltuk_Click;
                groupBox1.Controls.Add(sol);
                koltukNo++;
                if (koltukNo > 38) break;

                // SAĞ İKİLİ (1. koltuk)
                Button sag1 = new Button();
                sag1.Text = koltukNo.ToString();
                sag1.Width = 50;
                sag1.Height = 40;
                sag1.Location = new Point(sagX, startY + (i * aralikY));
                sag1.Click += BtnKoltuk_Click;
                groupBox1.Controls.Add(sag1);
                koltukNo++;
                if (koltukNo > 38) break;

                // SAĞ İKİLİ (2. koltuk)
                Button sag2 = new Button();
                sag2.Text = koltukNo.ToString();
                sag2.Width = 50;
                sag2.Height = 40;
                sag2.Location = new Point(sagX + 55, startY + (i * aralikY));
                sag2.Click += BtnKoltuk_Click;
                groupBox1.Controls.Add(sag2);
                koltukNo++;
            }
        }



        void DoluKoltuklariBoyat()
        {
            SqlCommand komut = new SqlCommand(
                "SELECT d.KOLTUK, y.CINSIYET " +
                "FROM TBLSEFERDETAY d " +
                "INNER JOIN TBLYOLCUBILGI y ON d.YOLCUTC = y.TC " +
                "WHERE d.SEFERNO = @p1", baglanti);
            komut.Parameters.AddWithValue("@p1", TxtSeferNo.Text);

            baglanti.Open();
            SqlDataReader dr = komut.ExecuteReader();

            while (dr.Read())
            {
                string koltukNo = dr["KOLTUK"].ToString();
                string cinsiyet = dr["CINSIYET"].ToString();

                foreach (Control c in groupBox1.Controls)
                {
                    if (c is Button && c.Text == koltukNo)
                    {
                        if (cinsiyet == "ERKEK")
                            c.BackColor = Color.CadetBlue;

                        else if (cinsiyet == "KADIN")
                            c.BackColor = Color.HotPink;
                    }
                }
            }

            dr.Close();
            baglanti.Close();
        }





        private void Form1_Load(object sender, EventArgs e)
        {
            seferlistesi();
            KoltuklariYukle();
        }
       
        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            /*baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into TBLYOLCUBILGI (ad,soyad,telefon,tc,cınsıyet,maıl) values (@p1,@p2,@p3,@p4,@p5,@p6)", baglanti);
            komut.Parameters.AddWithValue("@p1", TxtAd.Text);
            komut.Parameters.AddWithValue("@p2", TxtSoyad.Text);
            komut.Parameters.AddWithValue("@p3", MskTelefon.Text);
            komut.Parameters.AddWithValue("@p4", MskTC.Text);
            komut.Parameters.AddWithValue("@p5", CmbCinsiyet.Text);
            komut.Parameters.AddWithValue("@p6", TxtMail.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Yolcu Bilgisi Sisteme kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);*/
            Button btn = sender as Button;

            // Dolu koltuk kontrol
            if (btn.BackColor == Color.Red)
            {
                MessageBox.Show("Bu koltuk dolu!", "Uyarı");
                return;
            }

            TxtKoltukNo.Text = btn.Text;
        }

        private void BtnKaptan_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into tblkaptan(kaptanno,adsoyad,telefon) values (@p1,@p2,@p3)", baglanti);
            komut.Parameters.AddWithValue("@p1", TxtKaptanNo.Text);
            komut.Parameters.AddWithValue("@p2", TxtAdSoyad.Text);
            komut.Parameters.AddWithValue("@p3", MskTelefonKaptan.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Kaptan Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void BtnSeferOlustur_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into TBLSEFERBILGI (kalkıs,varıs,tarıh,saat,kaptan,fıyat) values (@p1,@p2,@p3,@p4,@p5,@p6)", baglanti);
            komut.Parameters.AddWithValue("@p1", TxtKalkis.Text);
            komut.Parameters.AddWithValue("@p2", TxtVaris.Text);
            komut.Parameters.AddWithValue("@p3", MskTarih.Text);
            komut.Parameters.AddWithValue("@p4", MskSaat.Text);
            komut.Parameters.AddWithValue("@p5", MskSeferKaptan.Text);
            komut.Parameters.AddWithValue("@p6", TxtFiyat.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Sefer Oluşturuldu", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            seferlistesi();

        }
        void KoltuklariTemizle()
        {
            foreach (Control c in groupBox1.Controls)
            {
                if (c is Button && c.Text != "K") // kaptan hariç
                {
                    c.BackColor = Color.LightGray;
                }
            }
        }



        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            TxtSeferNo.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();

            // 1️⃣ Önce koltukları temizle
            KoltuklariTemizle();

            // 2️⃣ Kaptanı getir
            string kaptanNo = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
            TxtKaptanNo.Text = kaptanNo;

            baglanti.Open();
            SqlCommand komut = new SqlCommand(
                "SELECT adsoyad, telefon FROM tblkaptan WHERE kaptanno=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", kaptanNo);

            SqlDataReader dr = komut.ExecuteReader();

            if (dr.Read())
            {
                TxtAdSoyad.Text = dr["adsoyad"].ToString();
                MskTelefonKaptan.Text = dr["telefon"].ToString();
            }

            dr.Close();
            baglanti.Close();

            // 3️⃣ Yeni seferin dolu koltuklarını kırmızıya boya
            DoluKoltuklariBoyat();
        }


        private void Btn1_Click(object sender, EventArgs e)
        {
            TxtKoltukNo.Text = "1";
        }

        private void Btn2_Click(object sender, EventArgs e)
        {
            TxtKoltukNo.Text = "2";
        }

        private void Btn3_Click(object sender, EventArgs e)
        {
            TxtKoltukNo.Text = "3";
        }

        private void Btn4_Click(object sender, EventArgs e)
        {
            TxtKoltukNo.Text = "4";
        }

        private void Btn5_Click(object sender, EventArgs e)
        {
            TxtKoltukNo.Text = "5";
        }

        private void Btn6_Click(object sender, EventArgs e)
        {
            TxtKoltukNo.Text = "6";
        }

        private void Btn7_Click(object sender, EventArgs e)
        {
            TxtKoltukNo.Text = "7";
        }

        private void Btn8_Click(object sender, EventArgs e)
        {
            TxtKoltukNo.Text = "8";
        }

        private void Btn9_Click(object sender, EventArgs e)
        {
            TxtKoltukNo.Text = "9";
        }

        private void BtnRezervasyon_Click(object sender, EventArgs e)
        {
            if (TxtSeferNo.Text == "" || MskYolcuTC.Text == "" || TxtKoltukNo.Text == "")
            {
                MessageBox.Show("Lütfen tüm bilgileri doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            baglanti.Open();

            // 1) Koltuk dolu mu kontrol et
            SqlCommand kontrol = new SqlCommand(
                "SELECT * FROM TBLSEFERDETAY WHERE seferno=@p1 AND koltuk=@p2", baglanti);
            kontrol.Parameters.AddWithValue("@p1", TxtSeferNo.Text);
            kontrol.Parameters.AddWithValue("@p2", TxtKoltukNo.Text);

            SqlDataReader dr = kontrol.ExecuteReader();

            if (dr.Read())
            {
                // Dolu koltuk
                dr.Close();
                baglanti.Close();
                MessageBox.Show("Bu koltuk zaten dolu!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dr.Close();

            // 2) Koltuk boş -> rezervasyon ekle
            SqlCommand komut = new SqlCommand(
                "INSERT INTO TBLSEFERDETAY (seferno, yolcutc, koltuk) VALUES (@p1, @p2, @p3)", baglanti);

            komut.Parameters.AddWithValue("@p1", TxtSeferNo.Text);
            komut.Parameters.AddWithValue("@p2", MskYolcuTC.Text);
            komut.Parameters.AddWithValue("@p3", TxtKoltukNo.Text);

            komut.ExecuteNonQuery();
            baglanti.Close();

            MessageBox.Show("Rezervasyon başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 3) Koltuğun arka plan rengini kırmızı yap (istenirse)
            KoltukKirmiziYap(TxtKoltukNo.Text);
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
