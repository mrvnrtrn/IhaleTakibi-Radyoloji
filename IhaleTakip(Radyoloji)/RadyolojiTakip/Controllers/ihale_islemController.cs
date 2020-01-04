using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RadyolojiDataService;

namespace RadyolojiTakip.Controllers
{
    [Route("api/ihale-islem")]
    [Produces("application/json")]
    //[ApiController]
    public class ihale_islemController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _enviroment;

        public ihale_islemController(IConfiguration configuration, IHostingEnvironment enviroment)
        {
            this._configuration = configuration;
            this._enviroment = enviroment;
        }

        private string Baglanti()
        {
            try
            {
                return _configuration["ConnectionStrings:DefaultConnection"];
            }
            catch
            {
                return "Yapılandırma ayarları okuma hatası.";
            }
        }

        [HttpGet("getIhaleIslem")]
        public IActionResult GetIhaleIslem()
        {
            string SQL = @"SELECT * FROM tbl_ihale_islem WHERE islem_durumu = 0 ORDER BY ref_no DESC";
            var conn = new SqlConnection(Baglanti());
            try
            {
                var db_rows = conn.Query<ihale_islem>(SQL);
                return Ok(db_rows);
            }
            catch (Exception xp1)
            {
                return BadRequest(xp1.Message);
            }
        }

        public static DateTime SQLServerDate(SqlConnection AConn)
        {
            DateTime DateTime1 = DateTime.Now;
            string SQL = @"SELECT GETDATE() AS datefield";
            var db_rows = AConn.Query<SingleField>(SQL);
            foreach (SingleField item in db_rows)
            {
                DateTime1 = item.datefield;
                break;
            }
            return (DateTime1);
        }

        [HttpGet("getIhaleIslemGunluk")]
        public IActionResult GetIhaleIslemGunluk(DateTime cekim_tarihi)
        {
            try
            {
                if (cekim_tarihi != null)
                {
                    string SQL = @"SELECT * FROM tbl_ihale_islem WHERE cekim_tarihi=@cekim_tarihi AND islem_durumu = 0 ORDER BY ref_no DESC";
                    var conn = new SqlConnection(Baglanti());
                    var prm = new { cekim_tarihi = cekim_tarihi };
                    var db_rows = conn.Query<ihale_islem>(SQL, prm);
                    return Ok(db_rows);
                }
                else
                {
                    return BadRequest("Veri yok");
                }
            }
            catch (Exception xp1)
            {
                return BadRequest(xp1.Message);
            }

        }

        [HttpGet("getIhaleAdi")]
        public String GetIhaleAdi(int sicil_no)
        {
            if (sicil_no > 0)
            {
                string ihale_ad = "";
                string SQL = @"SELECT 
	                               ih.ihale_cari_ad_unvan 
                               FROM 
	                               tbl_ihale_tanim ih
                               LEFT OUTER JOIN tbl_personel p 
                               ON
	                               ih.ihale_cari_kodu = p.masraf_yeri
                               WHERE
	                               p.sicil_no = @sicil_no";

                var conn = new SqlConnection(Baglanti());
                var prm = new { sicil_no = @sicil_no };
                var db_rows = conn.Query<ihale_tanim>(SQL, prm);
                foreach (ihale_tanim item in db_rows)
                {
                    ihale_ad = item.ihale_cari_ad_unvan;
                    break;
                }
                return (ihale_ad);
            }
            else
            {
                return ("İhale adi bulunamadi.");
            }
        }

        [HttpGet("getPersonelBilgileri")]
        public IActionResult GetPersonelBilgileri(string masraf_yeri, string departman)
        {
            try
            {
                if ((masraf_yeri != "") && (departman != ""))
                {
                    string SQL = @"SELECT * 
                                   FROM 
	                                   tbl_personel p
                                   LEFT OUTER JOIN tbl_ihale_hastane ih 
                                   ON
                                       ih.ihale_cari_kodu = p.masraf_yeri AND ih.ihale_hastane_cari_kodu = p.departman
                                   WHERE
                                       masraf_yeri = @masraf_yeri AND departman = @departman";

                    var conn = new SqlConnection(Baglanti());
                    var prm = new { masraf_yeri = @masraf_yeri, departman = @departman };
                    var db_rows = conn.Query<vw_personel>(SQL, prm);
                    return Ok(db_rows);
                }
                else
                {
                    return BadRequest("Personel Bilgisi Bulunamadı");
                }
            }
            catch
            {
                return BadRequest("Personel Bilgisi Bulunamadı");
            }
        }

        private object SQLServerDate(Func<string> baglanti)
        {
            throw new NotImplementedException();
        }

        [HttpGet("getIhaleIslemFilter")]
        public IActionResult GetIhaleIslemFilter(DateTime start, DateTime end)
        {
            try
            {
                if (start != null || end != null)
                {
                    string SQL = @"SELECT * 
                                   FROM tbl_ihale_islem 
                                   WHERE cekim_tarihi 
                                   BETWEEN @start AND @end 
                                   AND islem_durumu = 0 
                                   ORDER BY ref_no DESC";
                    var conn = new SqlConnection(Baglanti());
                    var prm = new { start = @start, end = @end };
                    var db_rows = conn.Query<ihale_islem>(SQL, prm);
                    return Ok(db_rows);
                }
                else
                {
                    return BadRequest("Veri yok");
                }
            }
            catch (Exception xp1)
            {
                return BadRequest(xp1.Message);
            }

        }

        [HttpGet("getIhaleIslemSatiri")] //rec_id ile satır çekme 
        public IActionResult GetIhaleIslemSatiri(int ref_no)
        {
            try
            {
                if (ref_no > 0)
                {
                    var conn = new SqlConnection(Baglanti());
                    string SQL = @"SELECT * 
                                   FROM tbl_ihale_islem 
                                   WHERE ref_no = @ref_no";
                    var prm = new { ref_no = ref_no };
                    try
                    {
                        var db_rows = conn.Query<ihale_islem>(SQL, prm);
                        return Ok(db_rows);
                    }
                    catch (Exception xp1)
                    {
                        return BadRequest(xp1.Message);
                    }
                }
                else
                {
                    return BadRequest("Veriler Görüntülenemedi. Eksik Veri Gönderildi.");
                }
            }
            catch (Exception xp1)
            {
                return BadRequest(xp1.Message);
            }
        }

        [HttpPost("saveIhaleIslem")]
        public IActionResult SaveIhaleIslem([FromBody] ihale_islem model)
        {
            bool returntcno = false;
            try
            {
                if (model != null)
                {
                    var conn = new SqlConnection(Baglanti());
                    // kontrol burda olacak stok kodu mukerrer olmayacak
                    //var ok = conn.ExecuteScalar(@"SELECT count(*) as adet FROM tbl_ihale_islem WHERE stk_kod='" + model.stk_kod + "'").ToString();
                    /*                    if (ok == null) { ok = "0"; }
                                        if (ok.Trim() == "") { ok = "0"; }
                                        if (ok != "0"){}
                                        else{}
                                        // bool xx = String.IsNullOrEmpty(ok.ToString());
                                        //                    if (Control == false){

                     */
                    if ((model.hasta_tc_no.Length == 11))
                    {
                        Int64 ATCNO, BTCNO, TcNo;
                        long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;

                        TcNo = Int64.Parse(model.hasta_tc_no);

                        ATCNO = TcNo / 100;
                        BTCNO = TcNo / 100;

                        C1 = ATCNO % 10; ATCNO = ATCNO / 10;
                        C2 = ATCNO % 10; ATCNO = ATCNO / 10;
                        C3 = ATCNO % 10; ATCNO = ATCNO / 10;
                        C4 = ATCNO % 10; ATCNO = ATCNO / 10;
                        C5 = ATCNO % 10; ATCNO = ATCNO / 10;
                        C6 = ATCNO % 10; ATCNO = ATCNO / 10;
                        C7 = ATCNO % 10; ATCNO = ATCNO / 10;
                        C8 = ATCNO % 10; ATCNO = ATCNO / 10;
                        C9 = ATCNO % 10; ATCNO = ATCNO / 10;
                        Q1 = ((10 - ((((C1 + C3 + C5 + C7 + C9) * 3) + (C2 + C4 + C6 + C8)) % 10)) % 10);
                        Q2 = ((10 - (((((C2 + C4 + C6 + C8) + Q1) * 3) + (C1 + C3 + C5 + C7 + C9)) % 10)) % 10);

                        returntcno = ((BTCNO * 100) + (Q1 * 10) + Q2 == TcNo);
                    }
                    else
                    {
                        return BadRequest("Girilen TC Kimlik Numarası Geçersiz");
                    }
                    if (returntcno == false)
                    {
                        return BadRequest("Girilen TC Kimlik Numarası Geçersiz");
                    }

                    string SQL = @" INSERT INTO 
                                      dbo.tbl_ihale_islem
                                    (
                                      ihale_cari_kodu,
                                      ihale_cari_ad_unvan,
                                      ihale_hastane_cari_kodu,
                                      ihale_hastane_cari_ad_unvan,
                                      hasta_kabul_no,
                                      hasta_tc_no,
                                      hasta_adi,
                                      hasta_soyadi,
                                      cekim_tarihi,
                                      cekim_saati,
                                      islem_kodu,
                                      islem_adi,
                                      islem_puani,
                                      cihaz_adi,
                                      islem_durumu,
                                      kayit_tarihi,
                                      kayit_yapan,
                                      rapor_yazim_tarihi,
                                      rapor_yazan_cari_kodu,
                                      rapor_yazan_cari_ad_unvan,
                                      rapor_onay_tarihi,
                                      rapor_onay_cari_kodu,
                                      rapor_onay_cari_ad_unvan
                                    ) 
                                    VALUES 
                                    (
                                      @ihale_cari_kodu,
                                      @ihale_cari_ad_unvan,
                                      @ihale_hastane_cari_kodu,
                                      @ihale_hastane_cari_ad_unvan,
                                      @hasta_kabul_no,
                                      @hasta_tc_no,
                                      @hasta_adi,
                                      @hasta_soyadi,
                                      @cekim_tarihi,
                                      @cekim_saati,
                                      @islem_kodu,
                                      @islem_adi,
                                      @islem_puani,
                                      @cihaz_adi,
                                      @islem_durumu,
                                      @kayit_tarihi,
                                      @kayit_yapan,
                                      @rapor_yazim_tarihi,
                                      @rapor_yazan_cari_kodu,
                                      @rapor_yazan_cari_ad_unvan,
                                      @rapor_onay_tarihi,
                                      @rapor_onay_cari_kodu,
                                      @rapor_onay_cari_ad_unvan
                                    )";
                    if (model.ref_no > 0)
                    {
                        SQL = @"UPDATE 
                                  dbo.tbl_ihale_islem  
                                SET 
                                  ihale_cari_kodu = @ihale_cari_kodu,
                                  ihale_cari_ad_unvan = @ihale_cari_ad_unvan,
                                  ihale_hastane_cari_kodu = @ihale_hastane_cari_kodu,
                                  ihale_hastane_cari_ad_unvan = @ihale_hastane_cari_ad_unvan,
                                  hasta_kabul_no = @hasta_kabul_no,
                                  hasta_tc_no = @hasta_tc_no,
                                  hasta_adi = @hasta_adi,
                                  hasta_soyadi = @hasta_soyadi,
                                  cekim_tarihi = @cekim_tarihi,
                                  cekim_saati = @cekim_saati,
                                  islem_kodu = @islem_kodu,
                                  islem_adi = @islem_adi,
                                  islem_puani = @islem_puani,
                                  cihaz_adi = @cihaz_adi,
                                  islem_durumu = @islem_durumu,
                                  kayit_tarihi = @kayit_tarihi,
                                  kayit_yapan = @kayit_yapan,
                                  rapor_yazim_tarihi = @rapor_yazim_tarihi,
                                  rapor_yazan_cari_kodu = @rapor_yazan_cari_kodu,
                                  rapor_yazan_cari_ad_unvan = @rapor_yazan_cari_ad_unvan,
                                  rapor_onay_tarihi = @rapor_onay_tarihi,
                                  rapor_onay_cari_kodu = @rapor_onay_cari_kodu,
                                  rapor_onay_cari_ad_unvan = @rapor_onay_cari_ad_unvan
                                WHERE 
                                  ref_no = @ref_no";
                    }
                    var prm = new
                    {
                        ref_no = model.ref_no,
                        ihale_cari_kodu = model.ihale_cari_kodu,
                        ihale_cari_ad_unvan = model.ihale_cari_ad_unvan,
                        ihale_hastane_cari_kodu = model.ihale_hastane_cari_kodu,
                        ihale_hastane_cari_ad_unvan = model.ihale_hastane_cari_ad_unvan,
                        hasta_kabul_no = model.hasta_kabul_no,
                        hasta_tc_no = model.hasta_tc_no,
                        hasta_adi = model.hasta_adi,
                        hasta_soyadi = model.hasta_soyadi,
                        cekim_tarihi = model.cekim_tarihi,
                        cekim_saati = model.cekim_saati,
                        islem_kodu = model.islem_kodu,
                        islem_adi = model.islem_adi,
                        islem_puani = model.islem_puani,
                        cihaz_adi = model.cihaz_adi,
                        kayit_tarihi = model.kayit_tarihi,
                        kayit_yapan = model.kayit_yapan,
                        rapor_yazim_tarihi = model.rapor_yazim_tarihi,
                        rapor_yazan_cari_kodu = model.rapor_yazan_cari_kodu,
                        rapor_yazan_cari_ad_unvan = model.rapor_yazan_cari_ad_unvan,
                        rapor_onay_tarihi = model.rapor_onay_tarihi,
                        rapor_onay_cari_kodu = model.rapor_onay_cari_kodu,
                        rapor_onay_cari_ad_unvan = model.rapor_onay_cari_ad_unvan,
                        islem_durumu = 0
                    };
                    var affectRows = conn.Execute(SQL, prm);
                    return Ok("ok");
                }
                else
                {
                    return BadRequest("Kayıt başarısız.");
                }
            }
            catch (Exception xp1)
            {
                return BadRequest("KAYIT HATASI: " + xp1.Message);
            }
        }

        [HttpPost("deleteIhaleIslem")]
        public IActionResult DeleteIhaleIslem([FromBody] ihale_islem model)
        {
            try
            {
                if (model.ref_no > 0)
                {
                    var conn = new SqlConnection(Baglanti());
                    string SQL = @" UPDATE tbl_ihale_islem SET islem_durumu = 1 WHERE ref_no = @ref_no";
                    var prm = new
                    {
                        ref_no = model.ref_no,
                    };
                    var affectRows = conn.Execute(SQL, prm);
                    return Ok("Kayıt Başarıyla Silindi");
                }
                else
                {
                    return BadRequest("Kayıt silinemedi. Eksik Veri Gönderildi.");
                }
            }
            catch (Exception xp1)
            {
                return BadRequest(xp1.Message);
            }
        }
        /*
        [HttpPost("getYeniKayitOnDeger")]
        public IActionResult YeniKayitOnDeger()
        {
            try
            {
                string SQL = @"SELECT max(stk_kod) AS stk_kod FROM tbl_stok_karti WHERE stk_kod like '100.%'";
                var conn = new SqlConnection(GetConnection());
                try
                {
                    var db_rows = conn.Query<stokkarti>(SQL);
                    return Ok(db_rows);
                }
                catch (Exception xp1)
                {
                    return BadRequest(xp1.Message);
                }
            }
            catch (Exception xp1)
            {
                return BadRequest(xp1.Message);
            }
        }
        */
    }
}