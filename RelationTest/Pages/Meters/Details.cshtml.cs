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
    public class DetailsModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DetailsModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Meter Meter { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Meter = await _context.Meter.FirstOrDefaultAsync(m => m.ID == id);

            if (Meter == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
