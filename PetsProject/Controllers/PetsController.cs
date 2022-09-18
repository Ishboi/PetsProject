using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetsProject.Data;
using PetsProject.Models;

namespace PetsProject.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static readonly HttpClient client = new HttpClient();

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pets
        public async Task<IActionResult> Index()
        {

            var image = await client.GetStringAsync("http://shibe.online/api/shibes");
            //image = HttpContext.
            var thirdRandomImage = image.Substring(2, image.Length - 4);
            var randImage = new[] { "https://cataas.com/cat", "https://place.dog/300/200", thirdRandomImage };
            var randomResult = new Random();
            var randUrl = randImage[randomResult.Next(0, 3)];
            var imageStream = await client.GetStreamAsync(randUrl);


            //Conver to base64
            var imageBase64 = ConvertToBase64(imageStream);
            var resultPet = new Pets()
                {
                    Id = Guid.NewGuid(),
                    Base64Image = imageBase64

                };
            return View(resultPet);
            //return View(await _context.Pets.ToListAsync());
        }

        // GET: Pets/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Pets == null)
            {
                return NotFound();
            }

            var pets = await _context.Pets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pets == null)
            {
                return NotFound();
            }

            return View(pets);
        }

        // GET: Pets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,BaseSixtyFourName")] Pets pets)
        public async Task<IActionResult> Create(Pets pets)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pets);
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pets);
        }

        // GET: Pets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pets == null)
            {
                return NotFound();
            }

            var pets = await _context.Pets.FindAsync(id);
            if (pets == null)
            {
                return NotFound();
            }
            return View(pets);
        }

        // POST: Pets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,BaseSixtyFourName")] Pets pets)
        {
            if (id != pets.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pets);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetsExists(pets.Id))
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
            return View(pets);
        }

        // GET: Pets/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Pets == null)
            {
                return NotFound();
            }

            var pets = await _context.Pets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pets == null)
            {
                return NotFound();
            }

            return View(pets);
        }

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Pets'  is null.");
            }
            var pets = await _context.Pets.FindAsync(id);
            if (pets != null)
            {
                _context.Pets.Remove(pets);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PetsExists(Guid id)
        {
          return _context.Pets.Any(e => e.Id == id);
        }


        //Make call 
        public string ReturnPetImage()
        {
            var debugIfRandom = GetRandomImageUrl().Result;
            return debugIfRandom;
        }

        public async Task<string> GetRandomImageUrl()
        {
            var image = await client.GetStringAsync("http://shibe.online/api/shibes");
            //image = HttpContext.
            var thirdRandomImage = image.Substring(2, image.Length - 4);
            var randImage = new[] { "https://cataas.com/cat", "https://place.dog/300/200", thirdRandomImage };
            var randomResult = new Random();
            var randUrl = randImage[randomResult.Next(0, 3)];
            var imageStream = await client.GetStreamAsync(randUrl);


            //Conver to base64
            var imageBase64 = ConvertToBase64(imageStream);

            //var imageBase64 = Convert.ToBase64String(imageBytes);

            return imageBase64;
            //return randImage[randomResult.Next(0, 3)];
        }

        //Send to helper class
        public string ConvertToBase64(Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
        }
    }
}
