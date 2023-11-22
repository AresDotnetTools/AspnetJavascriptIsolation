using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspnetJavascriptIsolation;

public static class StartupExtensions
{
	public static WebApplicationBuilder AddJavascriptIsolation(this WebApplicationBuilder builder, Action<JavascriptIsolationOptions>? options = null)
	{
		var opts = new JavascriptIsolationOptions();
		options?.Invoke(opts);
		builder.Services.AddSingleton(opts);

		if (string.IsNullOrWhiteSpace(opts.RootPath))
		{
			opts.RootPath = Path.GetDirectoryName(typeof(StartupExtensions).Assembly.Location)!;
		}

		builder.Services.AddHttpContextAccessor();
		builder.Services.AddTransient<ITagHelperComponent, ScriptTagHelperComponent>();

		return builder;
	}

	public static WebApplication UseJavascriptIsolation(this WebApplication app)
	{
		var globalSettings = app.Services.GetRequiredService<JavascriptIsolationOptions>();
		var logger = app.Services.GetRequiredService<ILogger<ScriptTagHelperComponent>>();
		var pattern = $"/{globalSettings.RouteHandler}/{{*page}}";

		var @delegate = (HttpContext ctx, string page) =>
		{
			if (string.IsNullOrWhiteSpace(page))
			{
				logger.LogWarning("No page specified");
				return Results.Content(string.Empty, "text/javascript");
			}

			DateTime? date = null;
			if (globalSettings.CacheDurationInHours > 0)
			{
				date = DateTime.Now.AddHours(globalSettings.CacheDurationInHours);
				var requestHeaders = ctx.Request.GetTypedHeaders();
				if (requestHeaders.IfModifiedSince.HasValue
					&& requestHeaders.IfModifiedSince.Value.ToUniversalTime() >= (DateTimeOffset)date.Value.ToUniversalTime())
				{
					return Results.StatusCode(StatusCodes.Status304NotModified);
				}
			}

			page = page.Replace(".js", "").Replace("/", "\\");
			var jsFileName = Path.Combine(globalSettings.RootPath, globalSettings.RootFolder, $"{page}.cshtml.js");
			if (!File.Exists(jsFileName))
			{
                logger.LogWarning($"js file name not found {jsFileName}", jsFileName);
                return Results.Content(string.Empty, "text/javascript");
			}

			if (date is not null)
			{
				ctx.Response.Headers.Add("Cache-Control", $"public, max-age={globalSettings.CacheDurationInHours * 3600}");
				ctx.Response.Headers.Add("Expires", date.Value.ToString("R"));
				ctx.Response.Headers.Add("Last-Modified", date.Value.ToString("R"));
			}
			else
			{
				// Ne pas mettre de cache
				ctx.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
			}

			var content = File.ReadAllText(jsFileName);
			return Results.Content(content, "text/javascript");
		};

		app.MapGet(pattern, @delegate);

		return app;
	}
}
