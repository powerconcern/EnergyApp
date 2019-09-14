using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Pages_CMCAssignments
{
    public class DetailsModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DetailsModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public CMCAssign CMCAssign { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CMCAssign = await _context.CMCAssigns
                .Include(c => c.Charger)
                .Include(c => c.Customer)
                .Include(c => c.Meter).FirstOrDefaultAsync(m => m.ChargerID == id);

            if (CMCAssign == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
