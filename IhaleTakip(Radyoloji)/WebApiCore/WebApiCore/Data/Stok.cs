using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore.Data
{
    public class Stok
    {
        public int ref_no { get; set; }
        public string mlz_kodu { get; set; }
        public string mlz_tanim { get; set; }
        public string grubu { get; set; }
        public string mam_mlz_grubu { get; set; }
        public decimal fiyatTur { get; set; }
        public string birim1 { get; set; }
        public decimal satis_fiyat1 { get; set; }
        public decimal satis_fiyat2 { get; set; }
        public decimal satis_fiyat3 { get; set; }
        public decimal kdv_orani { get; set; }
        public string resimUrl { get; set; }
        public decimal mevcut_stok { get; set; }
        public decimal isk1 { get; set; }
        public decimal isk2 { get; set; }


        public decimal satis_miktar { get; set; }
        public decimal satis_birimfiyat { get; set; }

        public decimal satis_ara_toplam { get; set; }
        public decimal satis_kdv_tutar { get; set; }
        public decimal satis_iskonto_toplam { get; set; }
        public decimal toplam_tutar { get; set; }


    }
}
