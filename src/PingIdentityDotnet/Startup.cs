using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using IdentityModel.AspNetCore.AccessTokenManagement;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace PingIdentityDotnet
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
            services.AddHttpClient("client")
                .AddHttpMessageHandler<UserAccessTokenHandler>();

            services.AddAccessTokenManagement()
                .ConfigureBackchannelHttpClient(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

            services.AddControllers();
            services.AddApiVersioning(options => {
                options.ReportApiVersions = true;

                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1,0);
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            var openApiInfo = new OpenApiInfo();
            Configuration.Bind("OpenApiInfo", openApiInfo);
            services.AddSingleton(openApiInfo);
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                //options.SwaggerDoc("v1", openApiInfo);
                options.OperationFilter<SwaggerDefaultValues>();
            });

            // Cookie configuration for HTTP to support cookies with SameSite=None
            services.ConfigureSameSiteNoneCookies();

            // Add authentication services

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("cookies", options =>
            {
                options.AccessDeniedPath = "/account/denied";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = Configuration["AuthorizationServer:Authority"];
                options.ClientId = Configuration["AuthorizationServer:ClientId"];
                options.ClientSecret = Configuration["AuthorizationServer:ClientSecret"];
                options.ResponseType = OpenIdConnectResponseType.Code;

                // Also ensure that you have added the URL as an Allowed Callback URL in PingFederate
                options.CallbackPath = new PathString(Configuration["AuthorizationServer:CallbackPath"]);

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                // Configure the scope
                options.Scope.Clear();
                var scopeArray = Configuration["AuthorizationServer:Scopes"].Split(',');
                foreach (var scope in scopeArray)
                    options.Scope.Add(scope);

                options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnAuthorizationCodeReceived = (context) =>
                    {
                        Console.WriteLine(context.JwtSecurityToken);
                        return Task.CompletedTask;
                    }
                };
            });

            // Add framework services.
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwaggerUI(c =>
                {
                    foreach(var description in provider.ApiVersionDescriptions)
                        c.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName);
                }
            );

            app.UseCors(builder => builder.AllowAnyMethod()
                                          .AllowAnyOrigin()
                                          .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
