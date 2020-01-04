using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore.Data
{
    public class SepetDetay
    {
        public int id { get; set; }
        public string username { get; set; }
        public int islemler_ref_no { get; set; }
        public DateTime tarih { get; set; }
        public int islem_tipi { get; set; }
        public string cari_kodu { get; set; }
        public string stok_kodu { get; set; }
        public string birim { get; set; }
        public decimal isk1 { get; set; }
        public decimal isk2 { get; set; }
        public decimal isk3 { get; set; }
        public decimal mevcut_stok { get; set; }
        public decimal miktar { get; set; }
        public decimal kdv_oran { get; set; }
        public decimal birim_fiyat { get; set; }
        public decimal tutar { get; set; }
        public decimal isk_toplam1 { get; set; }
        public decimal isk_toplam2 { get; set; }
        public decimal isk_toplam3 { get; set; }
        public decimal ara_toplam { get; set; }
        public decimal kdv_tutar { get; set; }
        public decimal toplam_tutar { get; set; }
        public int onay { get; set; }
        public int deleted { get; set; }
        public string cari_unvan { get; set; }
        public string stok_tanim { get; set; }

    }
}
