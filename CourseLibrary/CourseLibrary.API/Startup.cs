using System;
using System.Net;
using AutoMapper;
using CourseLibrary.Persistence.EFCore;
using CourseLibrary.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CourseLibrary.API
{
    public class Startup
    {
        private const string ApplicationProblemJson = "application/problem+json";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(configure => configure.ReturnHttpNotAcceptable = true)
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(ApiBehaviorOptionsSetupAction());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(
                options => options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;"));
        }

        private static Action<ApiBehaviorOptions> ApiBehaviorOptionsSetupAction()
        {
            return setupAction => setupAction.InvalidModelStateResponseFactory = context =>
            {
                var httpContext = context.HttpContext;
                var problemDetailsFactory = httpContext.RequestServices
                    .GetRequiredService<ProblemDetailsFactory>();

                var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                    httpContext,
                    context.ModelState);
                problemDetails.Detail = "See errors section for details";
                problemDetails.Instance = httpContext.Request.Path;

                var containsUnparsedArguments = ((ActionExecutingContext)context).ActionArguments.Count
                    != context.ActionDescriptor.Parameters.Count;

                if (!containsUnparsedArguments && !context.ModelState.IsValid)
                {
                    problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                    problemDetails.Title = "One or more validation errors occurred.";
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7807";
                }

                return new UnprocessableEntityObjectResult(problemDetails)
                {
                    ContentTypes =
                    {
                        ApplicationProblemJson
                    }
                };
            };
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
                app.UseExceptionHandler(
                    configure => configure.Run(
                        async context =>
                        {
                            context.Response.StatusCode = 500;
                            await context.Response.WriteAsync("An unexpected fault happened. Please try again later");
                        }));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}