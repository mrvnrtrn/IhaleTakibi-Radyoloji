using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore.Data
{
    public class SepetListe
    {
        public int islemler_ref_no { get; set; }
        public string cari_kodu { get; set; }
        public string cari_unvan { get; set; }

        public string tarih { get; set; }

        public decimal toplam_tutar { get; set; }


    }
}
