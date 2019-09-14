using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace RelationTest.Pages_Chargers
{
    public class DetailsModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DetailsModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Charger Charger { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Charger = await _context.Charger.FirstOrDefaultAsync(m => m.ID == id);

            if (Charger == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
