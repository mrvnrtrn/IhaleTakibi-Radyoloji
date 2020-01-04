using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using WebApiCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Cors.Internal;

namespace WebApiCore
{
    
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            
        }

     
        readonly string rootUrl = @"http://localhost:4200";

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string[] baseurl = null;
            try
            {
                baseurl = Configuration.GetSection("ConnectionStrings:BaseUrl").Value.Split("|");
                if (baseurl==null)
                {
                    baseurl = new string[] { "http://localhost:50597", "http://176.42.6.113:24322", "http://10.0.0.241:24322" };
                }
            }
            catch(Exception)
            {
                baseurl = new string[] { "http://localhost:50597", "http://176.42.6.113:24322", "http://10.0.0.241:24322" };
            }

            services.AddDbContext<ApplicationDbContext>(
                option => option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(
                 option =>
                 {
                     option.Password.RequireDigit = false;
                     option.Password.RequiredLength = 1;
                     option.Password.RequireNonAlphanumeric = false;
                     option.Password.RequireUppercase = false;
                     option.Password.RequireLowercase = false;
                 }

                )
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(option => {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Site"],
                    ValidIssuer = Configuration["Jwt:Site"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"]))
                };
            });



            //services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            //{
            //    builder.WithOrigins("http://localhost:4200", "http://localhost:50597").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            //}));


            services.AddCors(options => options.AddPolicy("ApiCorsPolicy",
                   builder =>
                   {
                       builder.WithOrigins(baseurl)
                       .AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
                   }));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // app.UseHttpsRedirection();

            // app.UseAuthentication();


            app.UseCors("ApiCorsPolicy");

            // app.UseCors();

            app.UseMvc();

             


        }
    }
}
