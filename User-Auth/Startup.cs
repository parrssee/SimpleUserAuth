using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.Models;
using System;
using System.Text;
using User_Auth.Config;

namespace User_Auth {
    public class Startup {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.Configure<AuthorizeSettings>(Configuration.GetSection("AuthorizeSettings"));

            var key = Encoding.UTF8.GetBytes(Configuration["AuthorizeSettings:JWT_Secret"].ToString());
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                          .AddJwtBearer(options => {
                                options.RequireHttpsMetadata = false;
                                options.TokenValidationParameters = new TokenValidationParameters {
                                    ValidateIssuer = true,
                                    ValidIssuer = AuthorizeSettings.Issuer,
                                    ValidateAudience = true,
                                    ValidAudience = AuthorizeSettings.Audience,
                                    ValidateLifetime = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(key),
                                    ValidateIssuerSigningKey = true,
                                };
                          });

            services.AddSwaggerGen(x => {
                x.SwaggerDoc("v1", new OpenApiInfo() { Title = "Auth API", Version = "v1" });
                OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme() {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                };
                x.AddSecurityDefinition("jwt_auth", securityDefinition);
                OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme() {
                    Reference = new OpenApiReference() {
                        Id = "jwt_auth",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                OpenApiSecurityRequirement securityRequirements = new OpenApiSecurityRequirement() {
                    { securityScheme, new string[] { } },
                };
                x.AddSecurityRequirement(securityRequirements);
            });
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseHsts();
            }

            app.UseCors(builder =>
                builder.WithOrigins(Configuration["AuthorizeSettings:Client_URL"].ToString())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );

            var swaggerOps = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOps);

            app.UseSwagger(option => option.RouteTemplate = swaggerOps.JsonRoute);
            app.UseSwaggerUI(option => option.SwaggerEndpoint(swaggerOps.UIendpoint, swaggerOps.Description));

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
