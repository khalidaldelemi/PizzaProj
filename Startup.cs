﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PizzaOnLine.Data;
using PizzaOnLine.Models;
using PizzaOnLine.Services;
using Newtonsoft.Json;

namespace PizzaOnLine
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
        //    services.AddDbContext<ApplicationDbContext>(options =>
        //        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseInMemoryDatabase("DefaultConnection"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<UserManager<ApplicationUser>>();
            services.AddTransient<RoleManager<IdentityRole>>();
            services.AddTransient<IngredientService>();
            services.AddTransient<DishService>();
            services.AddTransient<CartService>();
            services.AddSession();




            services.AddMvc().AddJsonOptions(opt => { opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            UserManager<ApplicationUser> userManager, ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseSession();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            DbIntialize.Initialize(userManager, context, roleManager);
        }
    }
}
