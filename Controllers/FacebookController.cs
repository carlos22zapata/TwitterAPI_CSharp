using Facebook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api_Tweet1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookController : ControllerBase
    {
        string API_Id = "882330132674715";
        string API_Secret = "5b35ce02050faa35ff33e3cf7bf2dffc";
        string Scope = "publish_stream,manage_pages";
        string Token = "EAAMieX4hZBJsBANZAsdiGAp98ut0Teei1ceKZB5CPZBo7kIjv2FwmmvKeP1QZCy3pKBwhngFi5TiRGDbroXjnpEfiwFh4ecNbXTLYw6ZBmpVaBe0BN857AJCOEYRMUcWi2O2RGwlmCkyZBjfsuxqWn77yej0UFfPIRw9knFTYB4AZBEA0SQOZALk0Xsk4g5WyabyuCEtVL88KBRP9WbN16CN7";

        string User_token = "EAAMieX4hZBJsBAHdQMW9Yda3wFxHSrapbVcT7ZCZAJ5V2O1hNZBjF79aEzIPPxK9s5pYx71Yz1cZBtu6DEIPSGYrHnVCGo6uIcGSpXZC1SUsnAPeimRws8m7xZATqMOGwJh3NWeAWPiCzec65FItutTenUNZBz75GjdgJ0QT2EZCXZC0TZBXcIuByq1";
                           //"EAAMieX4hZBJsBALB2gDeI26EhaTwiZBlhh2hbn5S88bibAGUMmRNtMgz2o5n0Ms2ANnZBLEl2EcSegxdmaYHJdiwkg5ZC2U426AHvydvDy9L0tswJjK30jQxyOt2WlSxoHaUuW7Nvx2Ebq7VwSPaZB6BQPrsALzV6PYES4GUHsQZA0GTfoHper";
        string App_token = "882330132674715|Bg7hwtI-yXQEQ7X2dm1VcT8xqWY";
        string User_Id = "10161343182349606";


        string urlx = "https://graph.facebook.com/v12.0/me?fields=id%2Cname&access_token=EAAMieX4hZBJsBANZAsdiGAp98ut0Teei1ceKZB5CPZBo7kIjv2FwmmvKeP1QZCy3pKBwhngFi5TiRGDbroXjnpEfiwFh4ecNbXTLYw6ZBmpVaBe0BN857AJCOEYRMUcWi2O2RGwlmCkyZBjfsuxqWn77yej0UFfPIRw9knFTYB4AZBEA0SQOZALk0Xsk4g5WyabyuCEtVL88KBRP9WbN16CN7";

        /*{
            "id": "10161343182349606",
            "name": "Carlos Zapata"
        }*/


        [HttpGet]
        public async Task<Object> Get()
        {
            string fields = "id,first_name,last_name,email,picture";
            string url = "https://graph.facebook.com/" + User_Id + "?access_token=" + App_token;

            //string url = "https://graph.facebook.com/" + User_Id + "/photos?access_token=" + User_token;
            
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);

            var result = await httpClient.GetAsync($"me?fields={fields}&access_token={User_token}");
            var msg = result.EnsureSuccessStatusCode();
            var res = await msg.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject(res);
        }


    }
}
