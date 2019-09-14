using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace RelationTest.Pages_Meters
{
    public class IndexModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public IndexModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Meter> Meter { get;set; }

        public async Task OnGetAsync()
        {
            Meter = await _context.Meter.ToListAsync();
        }
    }
}
