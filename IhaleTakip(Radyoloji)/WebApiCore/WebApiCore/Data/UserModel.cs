using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore.Data
{
    public class UserModel
    {
        [Display(Name = "ref_no")]
        public Int64 ref_no { get; set; }

        [Display(Name = "ad_soyad")]
        public string ad_soyad { get; set; }

        [Display(Name = "e_mail")]
        public string e_mail { get; set; }

        [Display(Name = "sifre")]
        public string sifre { get; set; }

        [Display(Name = "kadro")]
        public string kadro { get; set; }

        [Display(Name = "departman")]
        public string departman { get; set; }

        [Display(Name = "yetki")]
        public string yetki { get; set; }

        [Display(Name = "depo_kodu")]
        public string depo_kodu { get; set; }
    }
}
