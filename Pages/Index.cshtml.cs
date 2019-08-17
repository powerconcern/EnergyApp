using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using powerconcern.mqtt.services;
using EnergyApp.Data;

namespace EnergyApp.Pages
{
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
        public Customer Customer { get; set; }

        [BindProperty]
        public string userId {get;set;}

        [BindProperty]
        public float fMeanCurrent {get;set;}

        [BindProperty]
        public float fCurrent {get;set;}
        public void OnGet()
        {
            //Get user id
            userId =  User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Customer=_context.Customers.FirstOrDefault(c => c.CustomerNumber == userId);

            Customer = _context.Customers
                    .Include(cust => cust.Chargers)
//                        .ThenInclude(e => e.Course)
                    .AsNoTracking()
                    .FirstOrDefault(m => m.CustomerNumber == userId);


/*            var chargers=   from charger in _context.Chargers
                            join meter in _context.Meters on charger.ID equals meter.ChargerID
                            join outlet in _context.Outlets on charger.OutletID equals outlet.ID
                            where meter.ID
                            select outlet;
   */                                              
            //Get all chargers and outlets for the user
            
            fMeanCurrent=((MQTTService)_mqttsvc).fMeanCurrent[1];
        }
    }
}
