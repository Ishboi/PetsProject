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
            //line below just testing
            ViewBag.Categories = _context.Categories;
            ViewBag.Pets = _context.Pets;
            ViewBag.CategoriesWithNoPet = _context.Categories;
            Pets resultPet = await Base64ImageToPetModel();

            var exists = ImageWasSaved(resultPet.Id);
            if (exists)
            {
                ViewBag.CategoriesAssociated = PetCategories(resultPet.Id);
                //ViewBag.CategoriesAssociated = PetCategories(Guid.Parse("15bed543-68bc-4b90-bcb8-1511f3d479d0"));
            }

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

        // Get categories associated with pet image
        public async Task<IActionResult> PetCategories(Guid petId)
        {
            ViewBag.Categories = _context.Categories;
            ViewBag.Pets = _context.Pets;


            var foundPets = new Pets();
            var categoriesModel = new Categories();
            var petsCategoriesModel = new PetsCategories();

            var categories = _context.PetsCategories.Where(p => p.PetId.Equals(petId));

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

        // DELETE after setting it right
        public async Task<Pets> PetCategoriesDeleteThis(Guid petId)
        {
            ViewBag.Categories = _context.Categories;
            ViewBag.Pets = _context.Pets;

            //Need to put this into viewmodel
            var foundPets = new Pets();
            var categoriesModel = new Categories();
            var petsCategoriesModel = new PetsCategories();

            //pets.
            var categories = _context.PetsCategories.Where(p => p.PetId.Equals(petId));
            foreach (PetsCategories item in categories)
            {
                foundPets = await _context.Pets.Where(p => p.Id.Equals(item.PetId)).FirstOrDefaultAsync();
                petsCategoriesModel.Pets = item.Pets;
                petsCategoriesModel.PetId = item.PetId;

                categoriesModel = await _context.Categories.Where(c => c.Id.Equals(item.CategoryId)).FirstOrDefaultAsync();
                //categoriesModel.Name = foundCategories.Name;
                categoriesModel.Id = item.Categories.Id;
                //petsCategoriesModel.Categories = foundCategories;
                petsCategoriesModel.CategoryId = item.CategoryId;

                foundPets.Categories.Add(categoriesModel);
            }
            if (foundPets.Categories.Count > 0)
            {
                //Trying to solve this viewbag in order to show categories that are associated
                ViewBag.PetCategoriesSaved = foundPets.Categories;
            }

            return await Task.FromResult(petsCategoriesModel.Pets);

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
            _context.Add(pets);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //return View(pets);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryToPet(Guid petId, Guid categoryId, Pets pets)
        {
            //var pet = await _context.Pets.FindAsync(id);
            //pets.Base64Image = pet.Base64Image;
            var testCategory = _context.Categories.Find(Guid.Parse("92a58340-5b5f-4f24-ee94-08da99ca0cbf"));
            if (testCategory is not null)
            {
                if (pets.Categories.Count <= 0) new List<Categories>() { };
                pets.Categories.Add(testCategory);
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Id");
            var test = pets.Categories;
            Console.WriteLine("ok");
            return View(test);
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
