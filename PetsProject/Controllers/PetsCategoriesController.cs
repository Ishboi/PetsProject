using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetsProject.Data;
using PetsProject.Models;

namespace PetsProject.Controllers
{
    public class PetsCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetsCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PetsCategories
        public async Task<IActionResult> Index()
        {
              return View(await _context.PetsCategories.ToListAsync());
        }

        // GET: PetsCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PetsCategories == null)
            {
                return NotFound();
            }

            var petsCategories = await _context.PetsCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (petsCategories == null)
            {
                return NotFound();
            }

            return View(petsCategories);
        }

        // GET: PetsCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PetsCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] PetsCategories petsCategories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(petsCategories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(petsCategories);
        }

        [HttpPost]
        public void AddCategoryToPet([Bind("Id, PetId, CategoryId")] PetsCategories petsCategories)
        {
            var test = petsCategories;
            Console.WriteLine("ok");

        }

        // GET: PetsCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PetsCategories == null)
            {
                return NotFound();
            }

            var petsCategories = await _context.PetsCategories.FindAsync(id);
            if (petsCategories == null)
            {
                return NotFound();
            }
            return View(petsCategories);
        }

        // POST: PetsCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] PetsCategories petsCategories)
        {
            if (id != petsCategories.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(petsCategories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetsCategoriesExists(petsCategories.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(petsCategories);
        }

        // GET: PetsCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PetsCategories == null)
            {
                return NotFound();
            }

            var petsCategories = await _context.PetsCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (petsCategories == null)
            {
                return NotFound();
            }

            return View(petsCategories);
        }

        // POST: PetsCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PetsCategories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PetsCategories'  is null.");
            }
            var petsCategories = await _context.PetsCategories.FindAsync(id);
            if (petsCategories != null)
            {
                _context.PetsCategories.Remove(petsCategories);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PetsCategoriesExists(int id)
        {
          return _context.PetsCategories.Any(e => e.Id == id);
        }
    }
}
