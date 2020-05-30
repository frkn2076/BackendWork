using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess {
    public class AppDBContext : DbContext {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) {
        }
        public DbSet<Login> Login { get; set; }
        public DbSet<Survey> Survey { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<CompletedSurveys> CompletedSurveys { get; set; }
        public DbSet<Jobs> Job { get; set; }
    }
}
