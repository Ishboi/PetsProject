namespace PetsProject.Models
{
    public class Categories
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public IList<Pets> Pets { get; set; } = new List<Pets>();
        public Categories()
        {

        }
    }
}
