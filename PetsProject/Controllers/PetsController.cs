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
using PetsProject.Controllers;
using Microsoft.AspNetCore.Http.Extensions;

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
            Pets resultPet = await Base64ImageToPetModel();
            //line below just testing
            resultPet.Categories = _context.Categories.ToList();
            resultPet.SelectedCategories = _context.PetsCategories.Where(pc => pc.PetId.Equals(resultPet.Id)).ToList();
            ViewBag.Categories = _context.Categories;
            ViewBag.Pets = _context.Pets;
            ViewBag.CategoriesWithNoPet = _context.Categories;

            if (_context.PetsCategories is not null) CategoriesAlreadyAssociatedWithPets();

            var exists = ReturnPetIfImageWasSaved(resultPet.Base64Image);
            if (exists is not null)
            {
                //If saved pet image doesn't doesn't have any category yet return model to View
                if (!resultPet.SelectedCategories.Any())
                {
                    return View(resultPet);
                }
                await PetCategories(resultPet.Id);
            }

            return View(resultPet);
        }


        public void CategoriesAlreadyAssociatedWithPets()
        {
            var existingPets = _context.Pets.ToList();
            var existingCategories = _context.Categories.ToList();
            var existingPetsCategories = new List<PetsCategories>();
            var resultCategories = new List<Categories>();

            //var test = await _context.PetsCategories.ForEachAsync(c => c.CategoryId.Equals());
            foreach (var category in existingCategories)
            {
                existingPetsCategories.Add(_context.PetsCategories.Where(c => c.CategoryId.Equals(category.Id)).Distinct().FirstOrDefault());
            }
            existingPetsCategories.RemoveAll(item => item is null);
            foreach (var existingCategory in existingPetsCategories)
            {
                resultCategories.Add(existingCategories.Find(c => c.Id.Equals(existingCategory.CategoryId)));
                ViewData["CategoryId"] = new SelectList(resultCategories, "Id", existingCategory.CategoryId.ToString());
            }


            ViewBag.ExistingPetsCategories = resultCategories;


        }

        // Get categories associated with pet image
        public async Task<IActionResult> PetCategories(Guid petId)
        {
            ViewBag.Categories = _context.Categories;
            ViewBag.Pets = _context.Pets;


            var foundPets = new Pets();
            var categoriesModel = new Categories();
            var petsCategoriesModel = new PetsCategories();

            var categories = _context.PetsCategories.Where(p => p.PetId.Equals(petId));
            ViewBag.CategoriesWithPetAssociated = categories;
            foundPets = await _context.Pets.Where(p => p.Id.Equals(petId)).FirstOrDefaultAsync();
            if (!categories.Any())
            {
                petsCategoriesModel.Pets = foundPets;
            }
            if (foundPets is not null)
            {
                foreach (PetsCategories item in categories)
                {
                    foundPets = await _context.Pets.Where(p => p.Id.Equals(item.PetId)).FirstOrDefaultAsync();
                    petsCategoriesModel.Pets = item.Pets;
                    petsCategoriesModel.PetId = item.PetId;

                    categoriesModel = await _context.Categories.Where(c => c.Id.Equals(item.CategoryId)).FirstOrDefaultAsync();
                    if (categoriesModel is not null && foundPets is not null)
                    {

                        categoriesModel.Id = item.Categories.Id;
                        petsCategoriesModel.CategoryId = item.CategoryId;

                        foundPets.Categories.Add(categoriesModel);
                    }

                }

            }

            //Categories not associated with pet
            CategoriesWithNoPet(foundPets);

            //Set existing categories for filtering

            if (_context.PetsCategories is not null) CategoriesAlreadyAssociatedWithPets();



            ViewBag.PetCategoriesSaved = foundPets.Categories.Count > 0 ? foundPets.Categories : null;


            return View("Index", petsCategoriesModel.Pets);
        }

        public void CategoriesWithNoPet(Pets foundPets)
        {
            var allCategories = _context.Categories.ToList();
            var categoriesExcluded = allCategories.Except(foundPets.Categories.ToList());
            ViewBag.CategoriesWithNoPet = categoriesExcluded;
        }

        public async Task<IActionResult> FilterByCategory(Guid petId, Guid categoryId, string petCategoryId)
        {
            var categories = _context.Categories.Where(c => c.Id.Equals(categoryId)).ToList();
            ViewBag.CategoryFiltered = categories[0].Name;

            var petCategories = await _context.PetsCategories.Where(pc => pc.CategoryId.Equals(categoryId)).ToListAsync();
            if (!categories.Any() || !petCategories.Any())
            {
                return NotFound("Category isn't associated with any pet");
            }
            var pets = new List<Pets>();
            foreach (var item in petCategories)
            {
                pets.Add(_context?.Pets.Find(item.PetId));
            }

            foreach (var item in pets)
            {
                Console.WriteLine($"Pet id is:{item.Id}");
            }


            return View("FilteredPetsByCategory", pets);
        }


        // GET: Pets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Base64Image")] Pets pets)
        public async Task<IActionResult> Create(Pets pets)
        {
            if (ReturnPetIfImageWasSaved(pets.Base64Image) is not null) return RedirectToAction(nameof(Index));
            _context.Add(pets);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //return View(pets);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryToPet(Guid petId, Guid categoryId, string base64Image)
        {
            if (ReturnPetIfImageWasSaved(base64Image) is null)
            {
                var newPet = new Pets()
                {
                    Id = petId,
                    Base64Image = base64Image
                };
                await Create(newPet);
            }

            var petsCategories = new PetsCategories()
            {
                PetId = petId,
                CategoryId = categoryId,
            };
            _context.Add(petsCategories);
            await _context.SaveChangesAsync();

            return Redirect($"PetCategories?petId={petId}");

        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoryFromPet(Guid petId, Guid categoryId)
        {
            if (_context.PetsCategories is null)
            {
                return Problem("Currently no Categories associated to any pets.");
            }
            var petsCategories = await _context.PetsCategories.Where(x => (x.PetId.Equals(petId) &&
                         (x.CategoryId.Equals(categoryId)))).FirstOrDefaultAsync();
            if (petsCategories != null)
            {
                _context.PetsCategories.Remove(petsCategories);
            }
            await _context.SaveChangesAsync();
            return Redirect($"PetCategories?petId={petId}");


        }

        // GET: Pets/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Base64Image")] Pets pets)
        {
            if (id != pets.Id)
            {
                return NotFound();
            }


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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
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

        //Checks if image has already been saved by the user
        public Pets ReturnPetIfImageWasSaved(string base64Image)
        {
            if (_context.Pets is null) return null;
            Pets? petSaved = _context.Pets.Where(p => p.Base64Image.Equals(base64Image)).FirstOrDefault();
            return petSaved ?? null;
        }


        //Creates pet model based on base64 string and Guid
        private async Task<Pets> Base64ImageToPetModel()
        {
            var image = await client.GetStringAsync("http://shibe.online/api/shibes");
            var thirdRandomImage = image.Substring(2, image.Length - 4);
            var randImage = new[] { "https://cataas.com/cat", "https://place.dog/300/200", thirdRandomImage };
            var randomResult = new Random();
            var randUrl = randImage[randomResult.Next(0, 3)];
            var imageStream = await client.GetStreamAsync(randUrl);


            //Conver to base64
            var imageBase64 = ConvertToBase64(imageStream);

            var existingPet = ReturnPetIfImageWasSaved(imageBase64);
            if (existingPet is not null)
            {
                return existingPet;
            }
            var resultPet = new Pets()
            {
                Id = Guid.NewGuid(),
                Base64Image = imageBase64

            };
            return resultPet;
        }
        //Converts stream to base64 string
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
