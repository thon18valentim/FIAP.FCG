using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace FIAP.FCG.WebApi.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
                                               .CreateLogger("GlobalExceptionHandler");

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var ex = feature?.Error;

                    // Log básico
                    logger.LogError(ex, "Unhandled exception on {Path}", context.Request?.Path.Value);

                    var problem = new ProblemDetails
                    {
                        Title = "Erro ao processar a requisição",
                        Instance = context.Request.Path,
                    };

                    switch (ex)
                    {
                        case ValidationException ve:
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            problem.Status = context.Response.StatusCode;
                            problem.Detail = ve.Message;
                            break;

                        case DuplicateNameException dne:
                            context.Response.StatusCode = StatusCodes.Status409Conflict;
                            problem.Status = context.Response.StatusCode;
                            problem.Detail = dne.Message;
                            break;

                        case DbUpdateException dbEx:
                            context.Response.StatusCode = StatusCodes.Status409Conflict;
                            problem.Status = context.Response.StatusCode;
                            problem.Detail = "Violação de restrição no banco de dados.";
                            if (env.IsDevelopment())
                                problem.Extensions["exception"] = dbEx.GetBaseException().Message;
                            break;

                        case KeyNotFoundException knf:
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            problem.Status = context.Response.StatusCode;
                            problem.Detail = knf.Message;
                            break;

                        case UnauthorizedAccessException uae:
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            problem.Status = context.Response.StatusCode;
                            problem.Detail = string.IsNullOrWhiteSpace(uae.Message)
                                ? "Não autorizado."
                                : uae.Message;
                            break;

                        default:
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            problem.Status = context.Response.StatusCode;
                            problem.Detail = env.IsDevelopment()
                                ? ex?.ToString()
                                : "Ocorreu um erro inesperado. Tente novamente mais tarde.";
                            break;
                    }

                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsJsonAsync(problem);
                });
            });

            return app;
        }
    }
}
