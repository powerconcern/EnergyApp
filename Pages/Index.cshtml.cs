using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using powerconcern.mqtt.services;

namespace EnergyApp.Pages
{
    public class IndexModel : PageModel
    {
        private IHostedService _mqttsvc;
        
        public IndexModel(IHostedService mqttsvc) {
            _mqttsvc=mqttsvc;
        }

        [BindProperty]
        public string userId {get;set;}

        [BindProperty]
        public float fMeanCurrent {get;set;}

        public void OnGet()
        {
            userId =  User.FindFirst(ClaimTypes.NameIdentifier).Value;
            fMeanCurrent=((MQTTService)_mqttsvc).fMeanCurrent[1];
        }
    }
}
