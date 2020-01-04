using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore.Data
{
    public class Sepet
    {
        public string username { get; set; }
        public int islem_tipi { get; set; }
        public string cari_kodu { get; set; }
        public string stok_kodu { get; set; }
        public string birim { get; set; }
        public decimal isk1 { get; set; }
        public decimal isk2 { get; set; }
        public decimal isk3 { get; set; }
        public decimal mevcut_stok { get; set; }
        public decimal miktar { get; set; }
        public decimal kdv_orani { get; set; }
        public decimal birim_fiyat { get; set; }
    }
}
