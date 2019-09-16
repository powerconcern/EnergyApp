using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Pages_CMPAssignments
{
    public class DetailsModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DetailsModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public CMPAssignment CMPAssignment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CMPAssignment = await _context.CMPAssignments
                .Include(c => c.Charger)
                .Include(c => c.Meter)
                .Include(c => c.Partner).FirstOrDefaultAsync(m => m.ChargerID == id);

            if (CMPAssignment == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
