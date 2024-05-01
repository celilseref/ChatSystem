using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Data;
using ServerApp.models;
using ServerApp.Models;


namespace ServerApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
      
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ChatContext>(options =>
    options.UseSqlServer("Data Source=34.133.177.71;Initial Catalog=ChatDb1;User ID=altunarali;Password=361330258Aa"));
// uygulamayı kesmek icin ctrl c kullan
             services.AddIdentity<User,Role>().AddEntityFrameworkStores<ChatContext>();
             services.AddScoped<IUserRepository,UserRepository>();
             services.AddScoped<IMessageRepository,MessageRepository>();
              services.AddScoped<IGroupRepository,GroupRepository>();
            services.Configure<IdentityOptions>(options=>{
                options.Password.RequireDigit=true;
                options.Password.RequireLowercase=true;
                options.Password.RequiredLength=6;
                options.Password.RequireNonAlphanumeric = false;

                options.User.AllowedUserNameCharacters="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail=true;
            });
              services.AddAutoMapper(typeof(Startup));
            services.AddControllers().AddNewtonsoftJson(options =>{
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

  services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", builder =>
        {
            builder.WithOrigins("https://chatf.netlify.app") // İzin vermek istediğiniz domain adresini buraya ekleyin
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
    });

            services.AddAuthentication(x=>{
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(x =>{
               x.RequireHttpsMetadata=false;
               x.SaveToken=true;
               x.TokenValidationParameters=new TokenValidationParameters{
                ValidateIssuerSigningKey=true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes( Configuration.GetSection("AppSettings:Secret").Value)),
                ValidateIssuer=false,
                ValidateAudience=false
                
               };

             });
             
             services.AddHttpContextAccessor();


            services.AddControllers();
             services.AddSignalR();
             services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 60000000; // 60MB
});
 services.Configure<KestrelServerOptions>(options =>
    {
        options.Limits.MaxRequestBodySize = 104857600; // 100 MB in bytes
    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
{
    
    FileProvider = new PhysicalFileProvider(
        Path.Combine(env.ContentRootPath, "wwwroot")),
    RequestPath = "/wwwroot"
});

            app.UseRouting();
             app.UseCors("CorsPolicy");

              app.UseAuthentication(); 

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                 endpoints.MapHub<ChatHub>("/ChatHub"); 
            });
        }
    }
}
