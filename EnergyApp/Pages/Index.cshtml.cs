using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using powerconcern.mqtt.services;
using EnergyApp.Data;

namespace EnergyApp.Pages
{
//    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private IHostedService _mqttsvc;
        private EnergyApp.Data.ApplicationDbContext _context;
        
        public IndexModel(IHostedService mqttsvc, EnergyApp.Data.ApplicationDbContext context) {
            _mqttsvc=mqttsvc;
            _context=context;
        }

        [BindProperty]
        public Charger Charger { get; set; }

        [BindProperty]
        public Partner Partner { get; set; }

        [BindProperty]
        public string userId {get;set;}

        [BindProperty]
        public float fMeanCurrent {get;set;}

        [BindProperty]
        public List<MeterCache> meterCacheList {get;set;}
        public void OnGet()
        {
            //Init cache
            meterCacheList=new List<MeterCache>();

            //Get user id
            userId =  User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cmpAssigns=_context.CMPAssignments
                .Include(p => p.Partner)
                .Include(m => m.Meter)
                .Include(c => c.Charger)
                .AsNoTracking()
                .Where(c => c.Partner.UserReference ==userId && c.Partner.Type == PartnerType.Kund)
                .OrderBy(o => o.PartnerID);
            int PartnerID=0;

            //Get all chargers and outlets for the user
            foreach (var assign in cmpAssigns)
            {
                if(PartnerID==0||PartnerID==assign.PartnerID) {
                    //TODO show meters
                    ((MQTTService)_mqttsvc).GetBaseCache(assign.Meter.Name);
                    
                
                }
                PartnerID=assign.PartnerID;
            }
            
            //fMeanCurrent=((MQTTService)_mqttsvc).fMeanCurrent[1];
        }
    }
}
