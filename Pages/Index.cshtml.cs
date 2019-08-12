using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using powerconcern.mqtt.services;

namespace EnergyApp.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private IHostedService _mqttsvc;

        public IndexModel(IHostedService mqttsvc) {
            _mqttsvc=mqttsvc;
        }
        public void OnGet()
        {
            var test=((MQTTService)_mqttsvc).fMeanCurrent[1];
            Console.WriteLine(test); 
        }
    }
}
