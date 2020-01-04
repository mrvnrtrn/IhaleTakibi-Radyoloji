using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiCore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApiCore.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Cors;

namespace WebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        private string Baglanti()
        {
            return _configuration["ConnectionStrings:DefaultConnection"];
        }

        [HttpGet("get")]
        // public IEnumerable<string> Get()
        public ActionResult Get()
        {
            return Ok(new string[] { "Servis çalışıyor !!" });
        }

        //ali gül şehir için buradan
        public class TableMusteri
        {
            public string tanim { get; set; }
        }
        [HttpGet("musteri")]
        public ActionResult Musteri()
        {
            try
            {
                var cnn = new SqlConnection(Baglanti());
                var musteriler = cnn.Query<TableSehir>(@"SELECT cari_ad_unvan as tanim FROM dbo.tbl_cari_hesap");
                return Ok(musteriler);
            }
            catch (Exception xp1)
            {
                return BadRequest(new { message = xp1.Message });
            }
        }

        public class TableSehir
        {
            public string tanim { get; set; }
        }
        [HttpGet("sehir")]
        public ActionResult Sehir()
        {
            try
            {
                var cnn = new SqlConnection(Baglanti());
                var sehirler = cnn.Query<TableSehir>(@"SELECT tanim FROM dbo.tbl_il_tanim");
                if (sehirler == null)
                {
                    return BadRequest(new { message = "Hiç kayıt yok " });
                }
                return Ok(sehirler);
            }
            catch (Exception xp1)
            {
                return BadRequest(new { message = xp1.Message });
            }
        }

        public class TableIlce
        {
            public string tanim { get; set; }
        }

        [HttpGet("ilce")]
        public ActionResult Ilce()
        {
            var cnn = new SqlConnection(Baglanti());
            var tb = cnn.Query<TableIlce>(@"SELECT tanim FROM dbo.tbl_ilce_tanim");
            return Ok(tb);
        }
        //mlz birim fiyatı başlangıcı
        public class StokBirimFiyat
        {
            public string alis_fiyat { get; set; }
        }
        [HttpGet("mlzbirimfiyat")]
        public ActionResult MlzBirimFiyat(string mlz_kodu)
        {
            try
            {
                string AramaSQL = "";
                string SQL = @"SELECT alis_fiyat FROM dbo.tbl_mlz_stok ";
                if (mlz_kodu == null) { mlz_kodu = ""; }
                if (mlz_kodu.Trim() != "") { AramaSQL = " where mlz_kodu = '" + mlz_kodu.Trim() + "' "; }
                var cnn = new SqlConnection(Baglanti());
                var birimfiyat = cnn.Query<StokBirimFiyat>(SQL + AramaSQL).FirstOrDefault();
                return Ok(birimfiyat);
            }
            catch (Exception xp1)
            {
                return BadRequest(new
                {
                    message = xp1.Message
                });
            }
        }

        //mlz birim fiyat için buraya
        //doviz kuru
        public class MuhDovizKuru
        {
            public string kur_tutari { get; set; }
        }
        [HttpGet("dovizkuru")]
        public ActionResult DovizKuru(string kur_adi)
        {
            try
            {
                string AramaSQL = "";
                string SQL = @"SELECT kur_tutari FROM dbo.tbl_doviz_kuru ";
                if (kur_adi == null) { kur_adi = ""; }
                if (kur_adi.Trim() != "") { AramaSQL = " where doviz_adi = '" + kur_adi.Trim() + "' "; }
                var cnn = new SqlConnection(Baglanti());
                var kurfiyati = cnn.Query<MuhDovizKuru>(SQL + AramaSQL).FirstOrDefault();
                return Ok(kurfiyati);

            }
            catch (Exception xp1)
            {
                return BadRequest(new
                {
                    message = xp1.Message
                });
            }
        }

        //doviz kuru için buraya

        [HttpPost("hesapkaydet")]
        public ActionResult HesapKaydet([FromBody] ModulerSuDeposu model)
        {
            try
            {
                if (model != null)
                {
                    var cnn = new SqlConnection(Baglanti());
                    String SQL = @"INSERT INTO tbl_moduler_su_deposu
           (
            tarih
           ,cari_kodu
           ,cari_ad_unvan
           ,depo_tipi
           ,depo_en
           ,depo_boy
           ,depo_kat_sayisi
           ,depo_hacim
           ,depo_taban_kalinlik
           ,depo_tavan_kalinlik
           ,depo_1_kalinlik
           ,depo_15_kalinlik
           ,depo_2_kalinlik
           ,depo_25_kalinlik
           ,depo_3_kalinlik
           ,depo_35_kalinlik
           ,depo_4_kalinlik
           ,manson_giris_cap
           ,manson_giris_adet
           ,manson_cikis_cap
           ,manson_cikis_adet
           ,manson_bosaltma_cap
           ,manson_bosaltma_adet
           ,manson_tasma_cap
           ,manson_tasma_adet
           ,manson_ekstra_cap
           ,manson_ekstra_adet
           ,satis_temsilcisi
           ,il_adi
           ,ulke_adi
           ,depo_tutari
           ,doviz_cinsi
           ,doviz_kur
           ,dovizli_tutar
           ,aciklama
           ,kayit_tarihi
           ,kayit_yapan
           ,sac_birim_fiyat
           ,merdiven_birim_fiyat
           ,manson_birim_fiyat
           ,ust_kapak_birim_fiyat
           ,seviye_gosterge_birim_fiyat
           ,tdk_birim_fiyat
           ,cdk_birim_fiyat
           ,gergi_birim_fiyat
           ,dikme_birim_fiyat
           ,civata_birim_fiyat
           ,silikon_birim_fiyat
           ,yapistirici_birim_fiyat
           ,lastik_birim_fiyat
           ,modul_sayisi
           ,yari_modul_sayisi
           ,taban_sayisi
           ,tavan_sayisi
           ,tdk_sayisi
           ,cdk_sayisi
           ,gergi_sayisi
           ,dikme_sayisi
           ,civata_sayisi
           ,silikon_sayisi
           ,yapistirici_sayisi
           ,lastik_sayisi
           ,modul_agirlik
           ,yari_modul_agirlik
           ,taban_agirlik
           ,tavan_agirlik
           ,tdk_agirlik
           ,cdk_agirlik
           ,gergi_agirlik
           ,dikme_agirlik
           ,modul_tutari
           ,yari_modul_tutari
           ,taban_tutari
           ,tavan_tutari
           ,tdk_tutari
           ,cdk_tutari
           ,gergi_tutari
           ,dikme_tutari
           ,civata_tutari
           ,silikon_tutari
           ,yapistirici_tutari
           ,lastik_tutari
           ,manson_tutari
           ,nakliye_tutari
           ,iscilik_tutari
           ,kalinliklar_toplami
           ,ortalama_modul_kalinlik
           ,parca_sayisi
        )
     VALUES
        (
            @tarih
           ,@cari_kodu
           ,@cari_ad_unvan
           ,@depo_tipi
           ,@depo_en
           ,@depo_boy
           ,@depo_kat_sayisi
           ,@depo_hacim
           ,@depo_taban_kalinlik
           ,@depo_tavan_kalinlik
           ,@depo_1_kalinlik
           ,@depo_15_kalinlik
           ,@depo_2_kalinlik
           ,@depo_25_kalinlik
           ,@depo_3_kalinlik
           ,@depo_35_kalinlik
           ,@depo_4_kalinlik
           ,@manson_giris_cap
           ,@manson_giris_adet
           ,@manson_cikis_cap
           ,@manson_cikis_adet
           ,@manson_bosaltma_cap
           ,@manson_bosaltma_adet
           ,@manson_tasma_cap
           ,@manson_tasma_adet
           ,@manson_ekstra_cap
           ,@manson_ekstra_adet
           ,@satis_temsilcisi
           ,@il_adi
           ,@ulke_adi
           ,@depo_tutari
           ,@doviz_cinsi
           ,@doviz_kur
           ,@dovizli_tutar
           ,@aciklama
           ,@kayit_tarihi
           ,@kayit_yapan
           ,@sac_birim_fiyat
           ,@merdiven_birim_fiyat
           ,@manson_birim_fiyat
           ,@ust_kapak_birim_fiyat
           ,@seviye_gosterge_birim_fiyat
           ,@tdk_birim_fiyat
           ,@cdk_birim_fiyat
           ,@gergi_birim_fiyat
           ,@dikme_birim_fiyat
           ,@civata_birim_fiyat
           ,@silikon_birim_fiyat
           ,@yapistirici_birim_fiyat
           ,@lastik_birim_fiyat
           ,@modul_sayisi
           ,@yari_modul_sayisi
           ,@taban_sayisi
           ,@tavan_sayisi
           ,@tdk_sayisi
           ,@cdk_sayisi
           ,@gergi_sayisi
           ,@dikme_sayisi
           ,@civata_sayisi
           ,@silikon_sayisi
           ,@yapistirici_sayisi
           ,@lastik_sayisi
           ,@modul_agirlik
           ,@yari_modul_agirlik
           ,@taban_agirlik
           ,@tavan_agirlik
           ,@tdk_agirlik
           ,@cdk_agirlik
           ,@gergi_agirlik
           ,@dikme_agirlik
           ,@modul_tutari
           ,@yari_modul_tutari
           ,@taban_tutari
           ,@tavan_tutari
           ,@tdk_tutari
           ,@cdk_tutari
           ,@gergi_tutari
           ,@dikme_tutari
           ,@civata_tutari
           ,@silikon_tutari
           ,@yapistirici_tutari
           ,@lastik_tutari
           ,@manson_tutari
           ,@nakliye_tutari
           ,@iscilik_tutari
           ,@kalinliklar_toplami
           ,@ortalama_modul_kalinlik
           ,@parca_sayisi
            )";
                    if (model.ref_no > 0)
                    {
                        SQL = @"UPDATE tbl_moduler_su_deposu
   SET 
        tarih = @tarih
      ,cari_kodu = @cari_kodu
      ,cari_ad_unvan = @cari_ad_unvan
      ,depo_tipi = @depo_tipi
      ,depo_en = @depo_en
      ,depo_boy = @depo_boy
      ,depo_kat_sayisi = @depo_kat_sayisi
      ,depo_hacim = @depo_hacim
      ,depo_taban_kalinlik = @depo_taban_kalinlik
      ,depo_tavan_kalinlik = @depo_tavan_kalinlik
      ,depo_1_kalinlik = @depo_1_kalinlik
      ,depo_15_kalinlik = @depo_15_kalinlik
      ,depo_2_kalinlik = @depo_2_kalinlik
      ,depo_25_kalinlik = @depo_25_kalinlik
      ,depo_3_kalinlik = @depo_3_kalinlik
      ,depo_35_kalinlik = @depo_35_kalinlik
      ,depo_4_kalinlik = @depo_4_kalinlik
      ,manson_giris_cap = @manson_giris_cap
      ,manson_giris_adet = @manson_giris_adet
      ,manson_cikis_cap = @manson_cikis_cap
      ,manson_cikis_adet = @manson_cikis_adet
      ,manson_bosaltma_cap = @manson_bosaltma_cap
      ,manson_bosaltma_adet = @manson_bosaltma_adet
      ,manson_tasma_cap = @manson_tasma_cap
      ,manson_tasma_adet = @manson_tasma_adet
      ,manson_ekstra_cap = @manson_ekstra_cap
      ,manson_ekstra_adet = @manson_ekstra_adet
      ,satis_temsilcisi = @satis_temsilcisi
      ,il_adi = @il_adi
      ,ulke_adi = @ulke_adi
      ,depo_tutari = @depo_tutari
      ,doviz_cinsi = @doviz_cinsi
      ,doviz_kur = @doviz_kur
      ,dovizli_tutar = @dovizli_tutar
      ,aciklama = @aciklama
      ,kayit_tarihi = @kayit_tarihi
      ,kayit_yapan = @kayit_yapan
      ,sac_birim_fiyat = @sac_birim_fiyat
      ,merdiven_birim_fiyat = @merdiven_birim_fiyat
      ,manson_birim_fiyat = @manson_birim_fiyat
      ,ust_kapak_birim_fiyat = @ust_kapak_birim_fiyat
      ,seviye_gosterge_birim_fiyat = @seviye_gosterge_birim_fiyat
      ,tdk_birim_fiyat = @tdk_birim_fiyat
      ,cdk_birim_fiyat = @cdk_birim_fiyat
      ,gergi_birim_fiyat = @gergi_birim_fiyat
      ,dikme_birim_fiyat = @dikme_birim_fiyat
      ,civata_birim_fiyat = @civata_birim_fiyat
      ,silikon_birim_fiyat = @silikon_birim_fiyat
      ,yapistirici_birim_fiyat = @yapistirici_birim_fiyat
      ,lastik_birim_fiyat = @lastik_birim_fiyat
      ,modul_sayisi = @modul_sayisi
      ,yari_modul_sayisi = @yari_modul_sayisi
      ,taban_sayisi = @taban_sayisi
      ,tavan_sayisi = @tavan_sayisi
      ,tdk_sayisi = @tdk_sayisi
      ,cdk_sayisi = @cdk_sayisi
      ,gergi_sayisi = @gergi_sayisi
      ,dikme_sayisi = @dikme_sayisi
      ,civata_sayisi = @civata_sayisi
      ,silikon_sayisi = @silikon_sayisi
      ,yapistirici_sayisi = @yapistirici_sayisi
      ,lastik_sayisi = @lastik_sayisi
      ,modul_agirlik = @modul_agirlik
      ,yari_modul_agirlik = @yari_modul_agirlik
      ,taban_agirlik = @taban_agirlik
      ,tavan_agirlik = @tavan_agirlik
      ,tdk_agirlik = @tdk_agirlik
      ,cdk_agirlik = @cdk_agirlik
      ,gergi_agirlik = @gergi_agirlik
      ,dikme_agirlik = @dikme_agirlik
      ,modul_tutari = @modul_tutari
      ,yari_modul_tutari = @yari_modul_tutari
      ,taban_tutari = @taban_tutari
      ,tavan_tutari = @tavan_tutari
      ,tdk_tutari = @tdk_tutari
      ,cdk_tutari = @cdk_tutari
      ,gergi_tutari = @gergi_tutari
      ,dikme_tutari = @dikme_tutari
      ,civata_tutari = @civata_tutari
      ,silikon_tutari = @silikon_tutari
      ,yapistirici_tutari = @yapistirici_tutari
      ,lastik_tutari = @lastik_tutari
      ,manson_tutari = @manson_tutari
      ,nakliye_tutari = @nakliye_tutari
      ,iscilik_tutari = @iscilik_tutari
      ,kalinliklar_toplami = @kalinliklar_toplami
      ,ortalama_modul_kalinlik = @ortalama_modul_kalinlik
      ,parca_sayisi = @parca_sayisi
 WHERE  ref_no = @ref_no";
                    }
                    var prm = new
                    {
                        tarih = model.tarih
,
                        cari_kodu = model.cari_kodu
,
                        cari_ad_unvan = model.cari_ad_unvan
,
                        depo_tipi = model.depo_tipi
,
                        depo_en = model.depo_en
,
                        depo_boy = model.depo_boy
,
                        depo_kat_sayisi = model.depo_kat_sayisi
,
                        depo_hacim = model.depo_hacim
,
                        depo_taban_kalinlik = model.depo_taban_kalinlik
,
                        depo_tavan_kalinlik = model.depo_tavan_kalinlik
,
                        depo_1_kalinlik = model.depo_1_kalinlik
,
                        depo_15_kalinlik = model.depo_15_kalinlik
,
                        depo_2_kalinlik = model.depo_2_kalinlik
,
                        depo_25_kalinlik = model.depo_25_kalinlik
,
                        depo_3_kalinlik = model.depo_3_kalinlik
,
                        depo_35_kalinlik = model.depo_35_kalinlik
,
                        depo_4_kalinlik = model.depo_4_kalinlik
,
                        manson_giris_cap = model.manson_giris_cap
,
                        manson_giris_adet = model.manson_giris_adet
,
                        manson_cikis_cap = model.manson_cikis_cap
,
                        manson_cikis_adet = model.manson_cikis_adet
,
                        manson_bosaltma_cap = model.manson_bosaltma_cap
,
                        manson_bosaltma_adet = model.manson_bosaltma_adet
,
                        manson_tasma_cap = model.manson_tasma_cap
,
                        manson_tasma_adet = model.manson_tasma_adet
,
                        manson_ekstra_cap = model.manson_ekstra_cap
,
                        manson_ekstra_adet = model.manson_ekstra_adet
,
                        satis_temsilcisi = model.satis_temsilcisi
,
                        il_adi = model.il_adi
,
                        ulke_adi = model.ulke_adi
,
                        depo_tutari = model.depo_tutari
,
                        doviz_cinsi = model.doviz_cinsi
,
                        doviz_kur = model.doviz_kur
,
                        dovizli_tutar = model.dovizli_tutar
,
                        aciklama = model.aciklama
,
                        kayit_tarihi = model.kayit_tarihi
,
                        kayit_yapan = model.kayit_yapan
,
                        sac_birim_fiyat = model.sac_birim_fiyat
,
                        merdiven_birim_fiyat = model.merdiven_birim_fiyat
,
                        manson_birim_fiyat = model.manson_birim_fiyat
,
                        ust_kapak_birim_fiyat = model.ust_kapak_birim_fiyat
,
                        seviye_gosterge_birim_fiyat = model.seviye_gosterge_birim_fiyat
,
                        tdk_birim_fiyat = model.tdk_birim_fiyat
,
                        cdk_birim_fiyat = model.cdk_birim_fiyat
,
                        gergi_birim_fiyat = model.gergi_birim_fiyat
,
                        dikme_birim_fiyat = model.dikme_birim_fiyat
,
                        civata_birim_fiyat = model.civata_birim_fiyat
,
                        silikon_birim_fiyat = model.silikon_birim_fiyat
,
                        yapistirici_birim_fiyat = model.yapistirici_birim_fiyat
,
                        lastik_birim_fiyat = model.lastik_birim_fiyat
,
                        modul_sayisi = model.modul_sayisi
,
                        yari_modul_sayisi = model.yari_modul_sayisi
,
                        taban_sayisi = model.taban_sayisi
,
                        tavan_sayisi = model.tavan_sayisi
,
                        tdk_sayisi = model.tdk_sayisi
,
                        cdk_sayisi = model.cdk_sayisi
,
                        gergi_sayisi = model.gergi_sayisi
,
                        dikme_sayisi = model.dikme_sayisi
,
                        civata_sayisi = model.civata_sayisi
,
                        silikon_sayisi = model.silikon_sayisi
,
                        yapistirici_sayisi = model.yapistirici_sayisi
,
                        lastik_sayisi = model.lastik_sayisi
,
                        modul_agirlik = model.modul_agirlik
,
                        yari_modul_agirlik = model.yari_modul_agirlik
,
                        taban_agirlik = model.taban_agirlik
,
                        tavan_agirlik = model.tavan_agirlik
,
                        tdk_agirlik = model.tdk_agirlik
,
                        cdk_agirlik = model.cdk_agirlik
,
                        gergi_agirlik = model.gergi_agirlik
,
                        dikme_agirlik = model.dikme_agirlik
,
                        modul_tutari = model.modul_tutari
,
                        yari_modul_tutari = model.yari_modul_tutari
,
                        taban_tutari = model.taban_tutari
,
                        tavan_tutari = model.tavan_tutari
,
                        tdk_tutari = model.tdk_tutari
,
                        cdk_tutari = model.cdk_tutari
,
                        gergi_tutari = model.gergi_tutari
,
                        dikme_tutari = model.dikme_tutari
,
                        civata_tutari = model.civata_tutari
,
                        silikon_tutari = model.silikon_tutari
,
                        yapistirici_tutari = model.yapistirici_tutari
,
                        lastik_tutari = model.lastik_tutari
,
                        manson_tutari = model.manson_tutari
,
                        nakliye_tutari = model.nakliye_tutari
,
                        iscilik_tutari = model.iscilik_tutari
,
                        kalinliklar_toplami = model.kalinliklar_toplami
,
                        ortalama_modul_kalinlik = model.ortalama_modul_kalinlik
,
                        parca_sayisi = model.parca_sayisi
,
                        ref_no = model.ref_no
                    };

                    var affectRows = cnn.Execute(SQL, prm);
                    return Ok("ok");
                }

                else
                {
                    return BadRequest("boş");
                }
            }
            catch (Exception xp1)
            {
                return BadRequest(xp1.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> InsertUser([FromBody] RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                return Ok(new { Username = user.UserName });
            }
            else
                return BadRequest(new { Username = user.UserName });

        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            string kullanicitipi = "Customer";

            model = getUserKontrol(model);
            if (model != null)
            {
                var userData = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Username,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                if (model.Email == "engin@engin.com") { kullanicitipi = "Admin"; }

                var silmeok = await _userManager.DeleteAsync(userData);
                var result = await _userManager.CreateAsync(userData, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userData, kullanicitipi);
                }

                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null /*&& await _userManager.CheckPasswordAsync(user, model.Password)*/  )
                {
                    var claim = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.UserName) };
                    var signinKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

                    int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

                    var token = new JwtSecurityToken(
                      issuer: _configuration["Jwt:Site"],
                      audience: _configuration["Jwt:Site"],
                      expires: DateTime.UtcNow.AddDays(expiryInMinutes),  //  Günlük
                      signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                    );

                    return Ok(
                      new
                      {
                          token = new JwtSecurityTokenHandler().WriteToken(token),
                          expiration = token.ValidTo,
                          username = user.UserName,
                          usertype = kullanicitipi,
                          model = CurrentUserInfo(model.Username,model.Password)
                      });;
                }
            }

            return Unauthorized();
        }

        private LoginViewModel getUserKontrol(LoginViewModel model)
        {
            var cnn = new SqlConnection(Baglanti());

            var usermodel = cnn.Query<UserModel>(" SELECT ref_no, ad_soyad, e_mail,sifre, kadro, departman, yetki, depo_kodu " +
                  " FROM tbl_personel  WHERE ad_soyad='" + model.Username.ToString().Trim() + "' AND sifre='" + model.Password.ToString().Trim() + "' ").FirstOrDefault();
            if (usermodel != null)
            {
                model.Email = usermodel.e_mail.Trim();
                return model;
            }
            else return null;
        }

        public IActionResult CurrentUserInfo(string Username, string Password)
        {
            try
            {
                var cnn = new SqlConnection(Baglanti());
                string SQL = @"SELECT * FROM vw_personel WHERE ad_soyad='" + Username.ToString().Trim() + "' AND sifre='" + Password.ToString().Trim() + "' ";
                var db_rows = cnn.Query<WebApiCore.Data.RadyolojiTakip.vw_personel>(SQL);
                return Ok(db_rows);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}