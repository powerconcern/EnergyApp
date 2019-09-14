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
    public class DeleteModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DeleteModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Meter = await _context.Meter.FindAsync(id);

            if (Meter != null)
            {
                _context.Meter.Remove(Meter);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
