using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Dapper;
using WebApiCore.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiCore.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public ValuesController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        private string Baglanti()
        {
            return _configuration["ConnectionStrings:DefaultConnection"];
        }


        [HttpGet("get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "" };
        }

        [HttpGet("cari")]
        public IEnumerable<CariHesap> GetCari()
        {
            var cnn = new SqlConnection(Baglanti());
            return cnn.Query<CariHesap>(@"select cari_kodu,	cari_ad_unvan from tbl_cari_hesap  ORDER BY cari_ad_unvan    ");
        }

        [HttpGet("anagrup")]
        public IEnumerable<AnaGrup> GetAnaGrup()
        {
            var cnn = new SqlConnection(Baglanti());
            return cnn.Query<AnaGrup>(@"Select kod, tanim from tbl_iskonusu_tanim order by tanim   ");
            
        }

        [HttpGet("altgrup/{anaGrupAdi}")]
        public IEnumerable<AnaGrup> GetAltGrupAll(string anaGrupAdi)
        {
            if (anaGrupAdi.Trim() == "") { anaGrupAdi = ".621.09.099.00."; }
            var cnn = new SqlConnection(Baglanti());
            return cnn.Query<AnaGrup>(@"Select kod, tanim from tbl_mlz_grubu where kod='" + anaGrupAdi.Trim() + "' order by tanim  ");
        }

        [HttpGet("getstok/{AltGrupAdi}")]
        public IEnumerable<MlzStok> GetStok(string AltGrupAdi)
        {
            var cnn = new SqlConnection(Baglanti());
            return cnn.Query<MlzStok>(@"Select ref_no,mam_mlz_grubu,grubu,mlz_kodu,mlz_tanim,satis_fiyat1 from tbl_mlz_stok where mam_mlz_grubu = '" + AltGrupAdi.Trim() + "' ");
            
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
