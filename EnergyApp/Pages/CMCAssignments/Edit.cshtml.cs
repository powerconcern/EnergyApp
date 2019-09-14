using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Pages_CMCAssignments
{
    public class EditModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public EditModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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
           ViewData["ChargerID"] = new SelectList(_context.Chargers, "ID", "ID");
           ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "ID");
           ViewData["MeterID"] = new SelectList(_context.Meters, "ID", "ID");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CMCAssign).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CMCAssignExists(CMCAssign.ChargerID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CMCAssignExists(int id)
        {
            return _context.CMCAssigns.Any(e => e.ChargerID == id);
        }
    }
}
