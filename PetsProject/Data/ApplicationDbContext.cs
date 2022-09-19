using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetsProject.Models;

namespace PetsProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<PetsProject.Models.Categories> Categories { get; set; }
        public DbSet<PetsProject.Models.Pets> Pets { get; set; }
        public DbSet<PetsProject.Models.PetsCategories> PetsCategories { get; set; }
    }
}