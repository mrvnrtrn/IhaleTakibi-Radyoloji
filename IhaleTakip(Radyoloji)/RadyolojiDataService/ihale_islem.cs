using System;

namespace RadyolojiDataService
{
    public class ihale_islem
    {
        public long ref_no { get; set; }
        public string ihale_cari_kodu { get; set; }
        public string ihale_cari_ad_unvan { get; set; }
        public string ihale_hastane_cari_kodu { get; set; }
        public string ihale_hastane_cari_ad_unvan { get; set; }
        public string hasta_kabul_no { get; set; }
        public string hasta_tc_no { get; set; }
        public string hasta_adi { get; set; }
        public string hasta_soyadi { get; set; }
        public DateTime? cekim_tarihi { get; set; }
        public string cekim_saati { get; set; }
        public string islem_kodu { get; set; }
        public string islem_adi { get; set; }
        public float? islem_puani { get; set; }
        public string cihaz_adi { get; set; }
        public string islem_durumu { get; set; }
        public DateTime? kayit_tarihi { get; set; }
        public string kayit_yapan { get; set; }
        public DateTime? rapor_yazim_tarihi { get; set; }
        public string rapor_yazan_cari_kodu { get; set; }
        public string rapor_yazan_cari_ad_unvan { get; set; }
        public DateTime? rapor_onay_tarihi { get; set; }
        public string rapor_onay_cari_kodu { get; set; }
        public string rapor_onay_cari_ad_unvan { get; set; }
    }
}
