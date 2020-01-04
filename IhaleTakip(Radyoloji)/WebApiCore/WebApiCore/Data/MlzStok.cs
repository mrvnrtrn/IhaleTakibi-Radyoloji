using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiCore.Data
{
    public class MlzStok
    {
        [Display(Name = "ref_no")]
        public Int64 ref_no { get; set; }

        [Display(Name = "mam_mlz_grubu")]
        public string mam_mlz_grubu { get; set; }

        [Display(Name = "grubu")]
        public string grubu { get; set; }

        [Display(Name = "mlz_kodu")]
        public string mlz_kodu { get; set; }

        [Display(Name = "mlz_tanim")]
        public string mlz_tanim { get; set; }

        [Display(Name = "fiyat1")]
        public decimal satis_fiyat1 { get; set; }
    }
}