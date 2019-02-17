using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bachelor_Gr4_Chatbot_MVC.Data;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Bachelor_Gr4_Chatbot_MVC.Services;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Bachelor_Gr4_Chatbot_MVC.Hubs;
using Microsoft.Bot.Connector;
using Microsoft.AspNetCore.HttpOverrides;

namespace Bachelor_Gr4_Chatbot_MVC
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
            var msAppIdKey = Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
            var msAppPwdKey = Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppPasswordKey)?.Value;

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //  options.UseMySql(Configuration.GetConnectionString("AllanConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                // Require users to confirm email
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
      
            
            // Options
            services.Configure<EmailSenderOptions>(Configuration);
            services.Configure<RoleOptions>(Configuration);
            services.Configure<ChatbotKeywordOptions>(Configuration);



            // Add application services.

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IChatBot, QnAChatBot>();
            services.AddTransient<IAdminRepository, EFAdminRepository>();
            services.AddTransient<IChatRepository, EFChatRepository>();
            services.AddTransient<IQnARepository, EFQnARepository>();
            services.AddTransient<IChatbotRepository, EFChatbotRepository>();



            services.AddCors();
            services.AddSignalR();
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseCors(builder =>
                // TODO: These settings MUST be fixed, potensial SAFETY RISK!!!!! Only testcode to see if we get CORS running
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader() 
                           .AllowAnyMethod()
                           .AllowCredentials()
                    );

            app.UseStaticFiles();

            // TODO: .... Configure proxy server.
            /*app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });*/
            // TODO: /

            app.UseAuthentication();

            app.UseSession();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("chathub");
            });
            


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            
        }
    }
}
