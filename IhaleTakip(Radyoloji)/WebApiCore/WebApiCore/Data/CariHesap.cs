using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiCore.Data
{
    public class CariHesap
    {

        [Display(Name = "cari_kodu")]
        public string cari_kodu { get; set; }

        [Display(Name = "cari_ad_unvan")]
        public string cari_ad_unvan { get; set; }

        [Display(Name = "il")]
        public string il { get; set; }

        [Display(Name = "TL")]
        public string TL { get; set; }

        [Display(Name = "USD")]
        public string USD { get; set; }

        [Display(Name = "STOK")]
        public string STOK { get; set; }

        public int sepet_adet { get; set; }

        public decimal sepet_tutar { get; set; }

    }
}