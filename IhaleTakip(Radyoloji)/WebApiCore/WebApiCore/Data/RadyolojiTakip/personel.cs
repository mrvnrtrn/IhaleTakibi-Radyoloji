using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore.Data.RadyolojiTakip
{
    public class personel
    {
        public long ref_no { get; set; }
        public string ad_soyad { get; set; }
        public string baba_adi { get; set; }
        public string ana_adi { get; set; }
        public string milliyeti { get; set; }
        public string telefon { get; set; }
        public string e_mail { get; set; }
        public string ev_adresi { get; set; }
        public string satis_primi { get; set; }
        public string hedef_primi { get; set; }
        public string beden { get; set; }
        public string ayakabi_no { get; set; }
        public string sifre { get; set; }
        public string acil_ad_soyad { get; set; }
        public string okul_adi { get; set; }
        public string aciklama { get; set; }
        public string isten_cikis_nedeni { get; set; }
        public string cari_kodu { get; set; }
        public string departman { get; set; }
        public string kadro { get; set; }
        public string dogum_yeri { get; set; }
        public string medeni_durum { get; set; }
        public string cinsiyeti { get; set; }
        public string fiyat_grubu { get; set; }
        public string kan_grubu { get; set; }
        public string ehliyet { get; set; }
        public string yetki { get; set; }
        public string depo_kodu { get; set; }
        public string yabanci_dil { get; set; }
        public string egitim_durumu { get; set; }
        public string masraf_yeri { get; set; }
        public int? sicil_no { get; set; }
        public long? ssk_no { get; set; }
        public float? yemek_parasi { get; set; }
        public float? asgari_gecim_indirimi { get; set; }
        public int? cocuk_sayisi { get; set; }
        public decimal? ucret1 { get; set; }
        public decimal? ucret2 { get; set; }
        public DateTime? dogum_tarih { get; set; }
        public DateTime? ise_giris_tarih { get; set; }
        public DateTime? isten_cikis_tarih { get; set; }
   }
}
