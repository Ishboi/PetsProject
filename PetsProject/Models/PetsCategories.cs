using System.ComponentModel.DataAnnotations.Schema;

namespace PetsProject.Models
{
    public class PetsCategories
    {
        public int Id { get; set; }
        public IList<Pets> Pets { get; set; }
        public IList<Categories> Categories { get; set; }
        public PetsCategories()
        {

        }
    }
}
