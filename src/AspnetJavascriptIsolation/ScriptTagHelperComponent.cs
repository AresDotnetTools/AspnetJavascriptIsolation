using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace AspnetJavascriptIsolation;

internal class ScriptTagHelperComponent : TagHelperComponent
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly JavascriptIsolationOptions _globalSettings;

	public ScriptTagHelperComponent(IHttpContextAccessor httpContextAccessor,
		JavascriptIsolationOptions globalSettings)
	{
		_httpContextAccessor = httpContextAccessor;
		_globalSettings = globalSettings;
	}

	public override int Order => 1;

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		await Task.Yield();
		if (!context.TagName.Equals("body", StringComparison.InvariantCultureIgnoreCase))
		{
			return;
		}
		var pageName = _httpContextAccessor.HttpContext!.GetRouteValue("page");
		if (pageName is null)
		{
			return;
		}
		var jsFileName = $"{pageName}".TrimStart('/');
		jsFileName = Path.Combine(_globalSettings.RootPath, _globalSettings.RootFolder, $"{jsFileName}.cshtml.js");
		if (!System.IO.File.Exists(jsFileName))
		{
			return;
		}

		if (_globalSettings.UseModule)
		{
			output.PostContent.AppendHtml($"<script src=\"/{_globalSettings.RouteHandler}{pageName}.js\" type=\"module\"/></script>");
		}
		else
		{
			output.PostContent.AppendHtml($"<script src=\"/{_globalSettings.RouteHandler}{pageName}.js\"></script>");
		}
	}
}
