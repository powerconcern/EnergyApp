using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EnergyApp.Data;

namespace EnergyApp.Pages_CMPAssignments
{
    public class CreateModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public CreateModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ChargerID"] = new SelectList(_context.Chargers, "ID", "ID");
        ViewData["MeterID"] = new SelectList(_context.Meters, "ID", "ID");
        ViewData["PartnerID"] = new SelectList(_context.Partners, "ID", "ID");
            return Page();
        }

        [BindProperty]
        public CMPAssignment CMPAssignment { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.CMPAssignments.Add(CMPAssignment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}