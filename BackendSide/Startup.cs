using Business;
using Business.Authorization;
using Business.Crypto;
using Business.Job;
using Business.KPS;
using DataAccess;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Notification;
using Serilog;
using System;
using System.Text;

namespace BackendSide {
    public class Startup {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddCors(); //authorization için ekledim.

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // cache in memory
            services.AddMemoryCache();

            //MyDB58 db'si => "Data Source=den1.mssql7.gear.host;Initial Catalog=mydb58;User ID=mydb58;Password=Tz24zr-c~lHw"
            //Server=DESKTOP-6PRRUQM;Database=FurkanDB;Trusted_Connection=True;MultipleActiveResultSets=true
            /*azure*/
            services.AddDbContext<AppDBContext>(options => options./*UseLazyLoadingProxies().*/UseSqlServer("Server = tcp:backendd.database.windows.net,1433; Initial Catalog = backend; Persist Security Info = False; User ID = frkn2076; Password = -F.b.r.01994; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;"));
            services.AddSingleton<IEncryptor, Encryptor>();
            services.AddSingleton<IKPSService, KPSService>();
            services.AddScoped<IMainManager, MainManager>();
            services.AddSingleton<IMailSender, MailSender>();
            services.AddSingleton<IAuthorization, Authorization>();

            //Aşağıdakilerin hepsini authorization için ekledim
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // JWT authentication Aayarlaması
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
            /////////////////////////////////////////////////////////////////


            //Hangfire, job için aşağıdakiler
            services.AddHangfire(c => c.UseMemoryStorage()); //Db'ye bağlayabilirsin kendi oto db tablolarını oluşturuyor ilk ayağa kalkarken. DB sunucu aldığında dbye bağlarsın

            //Burayı job ı kullanırken açıcam. ScheduledJob üzerinden yöneticem job işlemlerini.
            services.AddScoped<IScheduledJob, ScheduledJob>();

            //
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseHsts();
            }


            app.UseHttpsRedirection();

            //Authentication için ekledim aşağıdakileri
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            ////////////////////////////////////////////


            //Hangfire için aşağısı
            app.UseHangfireServer();

            app.UseHangfireDashboard(); // .../hangfire dediğinde hangfire dashboarduna yönlendiriyor.
            //

            app.UseMvc();


        }
    }
}
