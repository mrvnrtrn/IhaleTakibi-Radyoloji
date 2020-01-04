using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadyolojiDataService
{
    public class ihale_tanim
    {
        public long ref_no { get; set; }
        public string ihale_cari_kodu { get; set; }
        public string ihale_cari_ad_unvan { get; set; }
        public DateTime? ihale_bas_tarih { get; set; }
        public DateTime? ihale_bit_tarih { get; set; }
        public float? toplam_ihale_puani { get; set; }
        public float? toplam_ihale_rapor_adedi { get; set; }
        public float? ihale_sozlesme_bedeli { get; set; }
        public float? birim_ihale_puan_bedeli { get; set; }
        public float? birim_ihale_rapor_bedeli { get; set; }
        public float? cekilen_puan { get; set; }
        public float? cekilen_rapor_adedi { get; set; }
        public float? fatura_edilen_tutar { get; set; }
        public string temlik_cari_kodu { get; set; }
        public string temlik_cari_ad_unvan { get; set; }
    }
}
