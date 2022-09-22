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
            resultPet.SelectedCategories = _context.PetsCategories.Where(x => x.PetId.Equals(resultPet.Id)).ToList();
            ViewBag.Categories = _context.Categories;
            ViewBag.Pets = _context.Pets;
            ViewBag.CategoriesWithNoPet = _context.Categories;

            var exists = ImageWasSaved(resultPet.Id);
            if (exists)
            {
                //If saved pet image doesn't doesn't have any category yet return model to View
                if(!_context.PetsCategories.Where(p => p.PetId.Equals(resultPet.Id)).Any())
                {
                    return View(resultPet);
                }
                ViewBag.CategoriesAssociated = PetCategories(resultPet.Id);
                //ViewBag.CategoriesAssociated = PetCategories(Guid.Parse("15bed543-68bc-4b90-bcb8-1511f3d479d0"));
            }

            return View(resultPet);
            //return View(await _context.Pets.ToListAsync());
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
            ViewBag.CategoriesWithPets = categories;
            foundPets = await _context.Pets.Where(p => p.Id.Equals(petId)).FirstOrDefaultAsync();
            if (!categories.Any())
            {
                petsCategoriesModel.Pets = foundPets;
            }
            if(foundPets is not null)
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


            ViewBag.PetCategoriesSaved = foundPets.Categories.Count > 0 ? foundPets.Categories : null;


            return View("Index", petsCategoriesModel.Pets);
        }

        public void CategoriesWithNoPet(Pets foundPets)
        {
            var allCategories = _context.Categories.ToList();
            var categoriesExcluded = allCategories.Except(foundPets.Categories.ToList());
            ViewBag.CategoriesWithNoPet = categoriesExcluded;
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
            if(ImageWasSaved(pets.Id)) return RedirectToAction(nameof(Index));
            _context.Add(pets);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //return View(pets);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryToPet(Guid petId, Guid categoryId)
        {
            var petsCategories = new PetsCategories()
            {
                PetId = petId,
                CategoryId = categoryId,
            };
            _context.Add(petsCategories);
            await _context.SaveChangesAsync();
            var pet = _context.Pets.Find(petId);

            return RedirectToAction(nameof(Index));
            //Trying to found a better way for this
            //return Redirect($"PetCategories?petId={petId}"); //Doesn't reload, so the buttons aren't responsive to db changes
            //return RedirectToAction("Index", "Pets", pets.Id);
            //return RedirectToAction(nameof(Index), pets);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoryFromPet(Guid petId, Guid categoryId)
        {
            if(_context.PetsCategories is null)
            {
                return Problem("Currently no Categories associated to any pets.");
            }
            var petsCategories = await _context.PetsCategories.Where(x => (x.PetId.Equals(petId) &&
                         (x.CategoryId.Equals(categoryId)))).FirstOrDefaultAsync();
            if(petsCategories != null)
            {
                _context.PetsCategories.Remove(petsCategories);
            }
            await _context.SaveChangesAsync();
            return Redirect($"PetCategories?petId={petId}");
            return RedirectToAction(nameof(Index));
                

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

            //if (ModelState.IsValid)
            if(true)
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

        /* Testing
        //Edit Category
        public async Task<IActionResult> EditCategory(Guid categoryId,Pets pets)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            return View(category);

        }

        //Delete Category
        public async void DeleteCategory(Guid? id)
        {
            var category = new CategoriesController(_context);
            await category.Delete(id);
        }

        //Details Category
        public async void CategoryDetails(Guid? id)
        {
            var category = new CategoriesController(_context);
            await category.Details(id);
        }
        */


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
        public Boolean ImageWasSaved(Guid id)
        {
            var anyPets = _context.Pets.Where(p => p.Id.Equals(id));
            return anyPets.Count() > 0;
        }






        //Send to helper class


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
