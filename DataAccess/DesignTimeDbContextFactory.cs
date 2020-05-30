using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccess {
    //Bu class ı db ye migration yapabilmek için ekledim, devamında services ekleyebilirsem Backendside dan services a appdbcontext eklemeyi kaldırabilirsin
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDBContext> {
        public AppDBContext CreateDbContext(string[] args) {
            var builder = new DbContextOptionsBuilder<AppDBContext>();
            //MyDB58 db'si => Data Source=den1.mssql7.gear.host;Initial Catalog=mydb58;User ID=mydb58;Password=Tz24zr-c~lHw
            //Server=DESKTOP-6PRRUQM;Database=FurkanDB;Trusted_Connection=True;MultipleActiveResultSets=true
            /*azure*/
            var connectionString = "Server = tcp:backendd.database.windows.net,1433; Initial Catalog = backend; Persist Security Info = False; User ID = frkn2076; Password =-F.b.r.01994; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;";
            builder.UseSqlServer(connectionString);
            return new AppDBContext(builder.Options);
        }
    }
}
