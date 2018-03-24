using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthrisationApi.Claims;
using AuthrisationApi.Database;
using AuthrisationApi.Models;
using AuthrisationApi.Options;
using AuthrisationApi.Services;
using AuthrisationApi.TokenProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace AuthrisationApi
{
    public class Startup
    {
        private readonly string _secretKey;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly TokenProviderOptions _tokenProvierOptions;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _secretKey = Configuration["Security:SigningKey"];
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
            _tokenProvierOptions = new TokenProviderOptions
            {
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256),
            };
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(
                options => options.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<IUserClaimsPrincipalFactory<User>, AppClaimsPrincipalFactory<User, Role>>();

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"])
            );
            services.AddSingleton(_tokenProvierOptions);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<ITokenProvider, JwtProvider>();
            services.AddTransient<UserService>();

            services.Configure<EmailClientOptions>(Configuration.GetSection("EmailOptions"));
            
            services.AddSwaggerGen(options =>
            {
                const string fileName = "WeddingPlannerApi.xml";
                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, fileName);
                options.SwaggerDoc("v1", new Info {Version = "v1", Title = "WeddingPlannerApi"});
                options.IncludeXmlComments(filePath);
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = "http://localhost:54632/";
                options.Authority = "http://localhost:54632/";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = true,
                    ValidIssuer = "ExampleIssuer",

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    ValidAudience = "ExampleAudience",

                    // Validate the token expiry
                    ValidateLifetime = true,

                    // If you want to allow a certain amount of clock drift, set that here:
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/{0}");
            }

            // JwtTokenAuthetication on [Authorized] Requests
            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvc();

            app.UseSwagger(setup =>
            {
                setup.RouteTemplate = "docs/{documentName}/swagger.json";
                setup.PreSerializeFilters.Add((document, request) => document.Host = request.Host.Value);
            });
            app.UseSwaggerUI(setup => { setup.SwaggerEndpoint("/docs/v1/swagger.json", "v1 Documentation"); });
        }
    }
}