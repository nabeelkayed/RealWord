using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using RealWord.Data;
using RealWord.Data.Repositories;
using RealWord.Core.Auth;
using RealWord.Core.Models;
using RealWord.Web.Validators;
using System;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using RealWord.Core.Services;

namespace RealWord.Web
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
            services.AddCors();

            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;

            }).AddXmlDataContractSerializerFormatters();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext =
                        actionContext as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    if (actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = Configuration["Jwt:Issuer"],
                      ValidAudience = Configuration["Jwt:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                  };
              });

            services.AddDbContext<RealWordDbContext>(options =>
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
                options.UseSqlServer(configuration.GetConnectionString("SQLServer"));
            });

            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IAuthentication, Authentication>();

            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICommentService, CommentService>();

            services.AddTransient<IValidator<UserForCreationDto>, UserForCreationValidator>();
            services.AddTransient<IValidator<UserForUpdateDto>, UserForUpdateValidator>();
            services.AddTransient<IValidator<CommentForCreationDto>, CommentForCreationValidator>();
            services.AddTransient<IValidator<ArticleForCreationDto>, ArticleForCreationValidator>();
            services.AddTransient<IValidator<ArticleForUpdateDto>, ArticleForUpdateValidator>();
            services.AddTransient<IValidator<UserLoginDto>, UserLoginValidator>();

            services.AddHttpContextAccessor();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers().AddFluentValidation();
            services.AddRazorPages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(options => options.WithOrigins("http://localhost:50001")
                                          .AllowAnyMethod()
                                          .AllowAnyHeader());
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
