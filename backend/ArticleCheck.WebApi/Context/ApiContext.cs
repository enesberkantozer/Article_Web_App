using ArticleCheck.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArticleCheck.WebApi.Context
{
    public class ApiContext:DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Verification> Verifications { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ArticleApiDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;");
        }
    }
}
