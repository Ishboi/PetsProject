using PetsProject.Models;

namespace PetsProject.ViewModels
{
    public class PetCategoryViewModel
    {

        public int Id { get; set; }
        public Guid PetId { get; set; }
        public Guid CategoryId { get; set; }

        public PetCategoryViewModel CreatePetCategory(PetsCategories newPetCategory)
        {
            var petCategory = new PetCategoryViewModel()
            {
                Id = newPetCategory.Id,
                PetId = newPetCategory.PetId,
                CategoryId = newPetCategory.CategoryId,

            };

            return petCategory;
        }
        public IList<PetCategoryViewModel> CreatePetCategory(List<PetsCategories> petCategories)
        {

            var petCategoriesList = new List<PetCategoryViewModel>();
            foreach (var item in petCategories)
            {
                petCategoriesList.Add(CreatePetCategory(item));
            }
            return petCategoriesList;
        }
            



    }
}
