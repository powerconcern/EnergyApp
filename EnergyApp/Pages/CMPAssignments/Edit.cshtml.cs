using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Pages_CMPAssignments
{
    public class EditModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public EditModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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
           ViewData["ChargerID"] = new SelectList(_context.Chargers, "ID", "ID");
           ViewData["MeterID"] = new SelectList(_context.Meters, "ID", "ID");
           ViewData["PartnerID"] = new SelectList(_context.Partners, "ID", "ID");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CMPAssignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CMPAssignmentExists(CMPAssignment.ChargerID))
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

        private bool CMPAssignmentExists(int id)
        {
            return _context.CMPAssignments.Any(e => e.ChargerID == id);
        }
    }
}
