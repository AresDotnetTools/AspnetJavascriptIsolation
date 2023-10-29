namespace AspnetJavascriptIsolation;
public class JavascriptIsolationOptions
{
	public bool UseModule { get; set; } = false;
	public string RootFolder { get; set; } = "Pages";
	public string RouteHandler { get; set; } = "jspage";
	public int CacheDurationInHours { get; set; } = 24;
	public string RootPath { get; set; } = default!;
}
