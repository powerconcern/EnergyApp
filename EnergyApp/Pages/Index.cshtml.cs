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
        public async void OnGet()
        {
            //Init cache
            meterCacheList=new List<MeterCache>();

            //Get user id
            userId =  User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var customer=await _context.Partners
                .Include(cmp => cmp.CMPAssignments)
                    .ThenInclude(m => m.Meter)
                .Include(cmp => cmp.CMPAssignments)
                    .ThenInclude(c => c.Charger)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserReference == userId && p.Type == PartnerType.Kund);

            Partner=customer;
            
            foreach (var meter in customer.CMPAssignments)
            {
                //find metercache based on name
                MeterCache mc=(MeterCache)((MQTTService)_mqttsvc).GetBaseCache(meter.Meter.Name);
                meterCacheList.Add(mc);
            }
        }
    }
}
