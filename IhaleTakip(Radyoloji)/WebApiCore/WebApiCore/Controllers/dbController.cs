using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApiCore.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Data;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiCore.Controllers
{

    [Produces("application/json")]
    [Route("db")]
    [ApiController]
    public class dbController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public dbController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        private string Baglanti()
        {
            return _configuration["ConnectionStrings:DefaultConnection"];
        }

        [Authorize]
        [HttpGet("get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "Guvenlik servis çalışıyor !!" };
        }

        // [Authorize]
        [HttpGet("cari")]
        public IEnumerable<CariHesap> GetCari(string username, string cariunvan, string sehir)
        {
            string aramaSQL = "";

            if (cariunvan == null) { cariunvan = ""; }
            if (sehir == null) { sehir = ""; }
            if (sehir == "null") { sehir = ""; }
            if (cariunvan != "") { aramaSQL += " AND cari.cari_ad_unvan LIKE '" + cariunvan.Trim() + "%'"; }
            if (sehir != "") { aramaSQL += " AND cari.il='" + sehir.Trim() + "'"; }
            var cnn = new SqlConnection(Baglanti());
              return cnn.Query<CariHesap>(@" SELECT cari.cari_kodu, cari.cari_ad_unvan, cari.il, ISNULL(bky.TL,0) TL,
                                                        ISNULL(bky.USD,0) USD, ISNULL(bky.STOK,0) STOK,
                                              ISNULL((SELECT COUNT(*) adet  FROM dbo.tbl_islemler_detay_sepet      
                                                 WHERE deleted=0 and onay=0 and islemler_ref_no=0  and username='"+ username + @"' 
                                                   and cari_kodu=cari.cari_kodu COLLATE Turkish_CI_AS ),0) sepet_adet  ,
                                              ISNULL((SELECT SUM(toplam_tutar) toplam_tutar  FROM dbo.tbl_islemler_detay_sepet      
                                                 WHERE deleted=0 and onay=0 and islemler_ref_no=0  and username='"+ username + @"' 
                                                   and cari_kodu=cari.cari_kodu COLLATE Turkish_CI_AS ),0) sepet_tutar 
                                            FROM dbo.tbl_cari_hesap cari
                                            LEFT OUTER JOIN GET_CARI_BAKIYE bky ON bky.ch_kodu=cari.cari_kodu
                                            WHERE (1=1) " + aramaSQL + " ORDER BY cari.cari_ad_unvan    ");
              

         
        }

        //  [Authorize]
        [HttpGet("anagrup")]
        public IEnumerable<AnaGrup> GetAnaGrup()
        {
            var cnn = new SqlConnection(Baglanti());
            return cnn.Query<AnaGrup>(@"Select kod, tanim from tbl_iskonusu_tanim order by tanim   ");

        }

        //  [Authorize]
        [HttpGet("altgrup")]
        public IEnumerable<AnaGrup> GetAltGrupAll(string anaGrupAdi)
        {
            if (anaGrupAdi == null) { anaGrupAdi = ""; }
            if (anaGrupAdi.Trim() == "") { anaGrupAdi = ".621.09.099.00."; }
            var cnn = new SqlConnection(Baglanti());
            return cnn.Query<AnaGrup>(@"Select kod, tanim from tbl_mlz_grubu where kod='" + anaGrupAdi.Trim() + "' order by tanim  ");
        }

        // [Authorize]
        [HttpGet("getstok")]
        public IEnumerable<Stok> GetStok(string AltGrupAdi, string carikodu)
        {
            if (AltGrupAdi == null) { AltGrupAdi = ""; }
            if (carikodu == null) { carikodu = ""; }
            var arama = "";
            var carifiltre = " 0 as isk1,  0 as isk2 ";
            if (carikodu.Trim() != "")
            {
                carifiltre = @" isnull((Select top 1 isnull(p.isk1, 0) From tbl_promosyon p
                                        Where p.mlz_kodu = s.mlz_kodu and p.cari_kodu = '" + carikodu.Trim() + @"'),0) as isk1, 
                                  isnull((Select top 1 isnull(p.isk2, 0) From tbl_promosyon p
                                     Where p.mlz_kodu = s.mlz_kodu and p.cari_kodu = '" + carikodu.Trim() + @"'),0) as isk2 ";
            }
            if (AltGrupAdi.Trim() != "") { arama += " and s.grubu='" + AltGrupAdi.Trim() + "' "; }


            var cnn = new SqlConnection(Baglanti());
            //  return cnn.Query<MlzStok>(@"Select ref_no,mam_mlz_grubu,grubu,mlz_kodu,mlz_tanim,satis_fiyat1 from tbl_mlz_stok where mam_mlz_grubu = '" + AltGrupAdi.Trim() + "' ");
            string sql = @"Select  s.ref_no, s.mlz_kodu, s.mlz_tanim,
                          s.grubu,s.mam_mlz_grubu,  -1 as fiyatTur, 
                          s.mlz_birim as birim1, 
                          s.satis_fiyat1, s.satis_fiyat2, 
                          s.satis_fiyat3, s.kdv_orani, '' as resimUrl, 
                          s.mevcut_stok, " + carifiltre + @"
                              , 0 as satis_miktar, 0 as satis_birimfiyat, 0 as satis_ara_toplam, 0 as satis_kdv_tutar, 0 as satis_iskonto_toplam, 0 as toplam_tutar 
                          From tbl_mlz_stok s
                          Where(1 = 1) " + arama;

            return cnn.Query<Stok>(sql);
        }


        // [Authorize]
        [HttpGet("sehir")]
        public IEnumerable<AnaGrup> GetSehir()
        {
            var cnn = new SqlConnection(Baglanti());
            return cnn.Query<AnaGrup>(@"SELECT kod, tanim FROM dbo.tbl_il_tanim order by tanim    ");

        }

        [HttpGet("sepetget")]
        public IEnumerable<SepetDetay> SepetGet(string username, string carikodu)
        {
            var cnn = new SqlConnection(Baglanti());
            try
            {
                var cmd = new SqlCommand("WebislemlerSepetGuncelle", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username.Trim();
                cnn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                cnn.Close();
            }
            catch
            {

            }

            string SQL = @"SELECT id,username,islemler_ref_no,tarih,islem_tipi,cari_kodu,stok_kodu
                              ,birim ,isk1,isk2 ,isk3,mevcut_stok ,miktar,kdv_oran,birim_fiyat
                              ,tutar ,isk_toplam1,isk_toplam2,isk_toplam3,ara_toplam,kdv_tutar
                              ,toplam_tutar ,onay ,deleted ,cari_unvan ,stok_tanim  
                FROM dbo.tbl_islemler_detay_sepet WHERE deleted=0 and onay=0 and islemler_ref_no=0 and username='" + username.Trim() + "' and cari_kodu='" + carikodu.Trim() + "' ";

            var sepetdetay = cnn.Query<SepetDetay>(SQL);

            return sepetdetay;
        }

        [HttpGet("sepetliste")]
        public IEnumerable<SepetListe> SepetListe(string username, string tarih)
        {
            string Aranan = "";
            if (username == null) { username = "-123456789abc"; }
            if (username == "") { username = "-123456789abc"; }
            if (tarih == null) { tarih = ""; }
            if (tarih.Trim()!="") { Aranan += "and tarih='"+ tarih.Trim() + "'"; }
            if (username.Trim() != "") { username += "and username='" + username.Trim() + "'"; }

            var cnn = new SqlConnection(Baglanti());
            try
            {
                var cmd = new SqlCommand("WebislemlerSepetGuncelle", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username.Trim();
                cnn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                cnn.Close();
            }
            catch
            {

            }

          
                string SQL = @"SELECT islemler_ref_no,  MAX(cari_kodu) cari_kodu, MAX(cari_unvan) cari_unvan,
                               REPLACE( CONVERT(VARCHAR(10),  MAX(tarih),102),'.','-') tarih,
                               SUM(toplam_tutar) toplam_tutar
                            FROM dbo.tbl_islemler_detay_sepet 
                            WHERE 1=1 " + Aranan + @"
                               and deleted=0 
                               and onay=1 
                               and islemler_ref_no>0 
                               and islem_tipi=5
                            GROUP BY islemler_ref_no ";

                var sepetliste = cnn.Query<SepetListe>(SQL);

                return sepetliste;
           
        }


        [HttpGet("siparisgunluktoplam")]
        public IEnumerable<SiparisGunlukToplam> SiparisGunlukToplam(string username, string tarih)
        {
            string Aranan = "";
            if (username == null) { username = "-123456789abc"; }
            if (username == "") { username = "-123456789abc"; }
            if (tarih == null) { tarih = ""; }
            if (tarih.Trim() != "") { Aranan += "and tarih='" + tarih.Trim() + "'"; }
            if (username.Trim() != "") { username += "and username='" + username.Trim() + "'"; }

            var cnn = new SqlConnection(Baglanti());

            string SQL = @"SELECT ISNULL(count(*),0) siparisadet, ISNULL(sum(toplam_tutar),0) toplam_tutar
                   FROM  
                    ( SELECT islemler_ref_no,  MAX(cari_kodu) cari_kodu, MAX(cari_unvan) cari_unvan,
                               REPLACE( CONVERT(VARCHAR(10),  MAX(tarih),102),'.','-') tarih,
                               SUM(toplam_tutar) toplam_tutar
                            FROM dbo.tbl_islemler_detay_sepet 
                            WHERE 1=1 " + Aranan + @"
                               and deleted=0 
                               and onay=1 
                               and islemler_ref_no>0 
                               and islem_tipi=5
                            GROUP BY islemler_ref_no
                       ) tmp ";

            var sepetliste = cnn.Query<SiparisGunlukToplam>(SQL);

            return sepetliste;

        }


        [HttpGet("sepettoplam")]
        public IEnumerable<SiparisGunlukToplam> SepetToplam(string username, string tarih)
        {

            string Aranan = "";
            if (username == null) { username = "-123456789abc"; }
            if (username == "") { username = "-123456789abc"; }
            if (tarih == null) { tarih = ""; }
            if (tarih.Trim() != "") { Aranan += "and tarih BETWEEN '"+ tarih.Trim() + " 00:00:00' AND '"+ tarih.Trim() + " 23:59:59'"; }
            if (username.Trim() != "") { username += "and username='" + username.Trim() + "'"; }

            var cnn = new SqlConnection(Baglanti());

            string SQL = @" SELECT ISNULL(count(*),0) siparisadet, ISNULL(sum(toplam_tutar),0) toplam_tutar 
              FROM (
                SELECT id,username,islemler_ref_no,tarih,islem_tipi,cari_kodu,stok_kodu
                              ,birim ,isk1,isk2 ,isk3,mevcut_stok ,miktar,kdv_oran,birim_fiyat
                              ,tutar ,isk_toplam1,isk_toplam2,isk_toplam3,ara_toplam,kdv_tutar
                              ,toplam_tutar ,onay ,deleted ,cari_unvan ,stok_tanim  
                FROM dbo.tbl_islemler_detay_sepet 
                WHERE deleted=0 and onay=0 and islemler_ref_no=0 " + Aranan.Trim() + @" 
                ) tmp ";

            var sepetdetay = cnn.Query<SiparisGunlukToplam>(SQL);

            return sepetdetay;
        }


        [HttpPost("sepetekle")]
        public ActionResult SepetEkle([FromBody] Sepet model)
        {
            try
            {
                var cnn = new SqlConnection(Baglanti());
                var cmd = new SqlCommand("WebislemlerSepet", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = model.username.Trim();
                cmd.Parameters.Add("@islem_tipi", SqlDbType.Int).Value = model.islem_tipi;
                cmd.Parameters.Add("@cari_kodu", SqlDbType.VarChar).Value = model.cari_kodu.Trim();
                cmd.Parameters.Add("@stok_kodu", SqlDbType.VarChar).Value = model.stok_kodu.Trim();
                cmd.Parameters.Add("@birim", SqlDbType.VarChar).Value = model.birim.Trim();
                cmd.Parameters.Add("@isk1", SqlDbType.Decimal).Value = model.isk1;
                cmd.Parameters.Add("@isk2", SqlDbType.Decimal).Value = model.isk2;
                cmd.Parameters.Add("@isk3", SqlDbType.Decimal).Value = model.isk3;
                cmd.Parameters.Add("@mevcut_stok", SqlDbType.Decimal).Value = model.mevcut_stok;
                cmd.Parameters.Add("@miktar", SqlDbType.Decimal).Value = model.miktar;
                cmd.Parameters.Add("@kdv_oran", SqlDbType.Decimal).Value = model.kdv_orani;
                cmd.Parameters.Add("@birim_fiyat", SqlDbType.Decimal).Value = model.birim_fiyat;
                cnn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                cnn.Close();

                return Ok("ok");
            }
            catch
            {
                return BadRequest() ;
            }
        }


        [HttpPost("sepeturunsil")]
        public ActionResult SepetUrunSil(int id)
        {
            try
            {
                var cnn = new SqlConnection(Baglanti());
                var cmd = new SqlCommand("UPDATE dbo.tbl_islemler_detay_sepet SET deleted=1 WHERE deleted=0 and onay=0 and islemler_ref_no=0 and id=@id", cnn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cnn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                cnn.Close();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("sepetmusteribosalt")]
        public ActionResult SepetMusteriBosalt(string username, string cari_kodu)
        {
            try
            {
                var cnn = new SqlConnection(Baglanti());
                var cmd = new SqlCommand("UPDATE dbo.tbl_islemler_detay_sepet SET deleted=1 WHERE deleted=0 and onay=0 and islemler_ref_no=0 and username=@username and cari_kodu=@cari_kodu", cnn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@cari_kodu", SqlDbType.VarChar).Value = cari_kodu;
                cnn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                cnn.Close();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }


        [HttpPost("sepettumunubosalt")]
        public ActionResult SepetTumunuBosalt(string username)
        {
            try
            {
                var cnn = new SqlConnection(Baglanti());
                var cmd = new SqlCommand("UPDATE dbo.tbl_islemler_detay_sepet SET deleted=1 WHERE deleted=0 and onay=0 and islemler_ref_no=0 and username=@username ", cnn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                cnn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                cnn.Close();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }


        [HttpPost("sepetonayla")]
        public ActionResult SepetOnayla([FromBody] SepetOnayForm model)
        {
        
            try
            {
                var cnn = new SqlConnection(Baglanti());
                var cmd = new SqlCommand("WebislemlerSepetOnayla", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = model.username.Trim();
                cmd.Parameters.Add("@carikodu", SqlDbType.VarChar).Value = model.carikodu.Trim();
                cmd.Parameters.Add("@cariunvan", SqlDbType.VarChar).Value = model.cariunvan.Trim();
                cmd.Parameters.Add("@islem_tipi", SqlDbType.Int).Value = model.islem_tipi;
                cmd.Parameters.Add("@aciklama", SqlDbType.VarChar).Value = model.aciklama.Trim();
               
                cnn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                cnn.Close();

                return Ok(rowsAffected);
            }
            catch
            {
                return BadRequest("0");
            }
        }


        [HttpPost("sipariscikar")]
        public ActionResult SiparisCikar(string username, int refno)
        {
            try
            {
                var cnn = new SqlConnection(Baglanti());
                var cmd = new SqlCommand("WebislemlerSiparisSil", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username.Trim();
                cmd.Parameters.Add("@ref_no", SqlDbType.Int).Value = refno;
                cnn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                cnn.Close();
                return Ok(rowsAffected);
            }
            catch
            {
                return BadRequest("0");
            }
        }

    }
}
