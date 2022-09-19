using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsProject.Models
{
    public class PetsCategories
    {
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey(nameof(Pets))]
        public Guid PetId { get; set; }
        public Pets Pets { get; set; }
        [Required]
        [ForeignKey(nameof(Categories))]
        public Guid CategoryId { get; set; }
        public Categories Categories { get; set; }
        public PetsCategories()
        {

        }
    }
}
