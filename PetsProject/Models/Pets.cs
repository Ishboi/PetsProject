namespace PetsProject.Models
{
    public class Pets
    {
        public Guid Id { get; set; }
        public string Base64Image { get; set; }

        public IList<Categories> Categories { get; set; } = new List<Categories>();
        public Pets()
        {

        }
    }
}
