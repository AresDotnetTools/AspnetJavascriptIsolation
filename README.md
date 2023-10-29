# AspnetJavascriptIsolation

This component allows you to associate a JavaScript file with an MVC view or a Razor page to isolate the JavaScript calls to that page only. If the file is present and is marked as "Content", the component automatically adds a script tag in the body with a link to this file.

## Installation

Install the package from NuGet:

```powershell
Install-Package AspnetJavascriptIsolation
```

## Usage

![Project](/doc/Project.png)

![Source](/doc/Source.png)

![Javascript](/doc/Javascript.png)

### MVC or Razor Pages

In your Program.cs before builder.Build(), add the following line:

```csharp
builder.AddJavascriptIsolation(options =>
{
	options.UseModule = true;
	if (builder.Environment.IsProduction())
	{
		options.CacheDurationInHours = 24;
	}
	else
	{
		options.CacheDurationInHours = 0;
		options.RootPath = builder.Environment.ContentRootPath;
	}
});
```

If you using MVC configure the option :

```csharp
options.RootFolder = "Views";
```

In your Program.cs, add the following line:

```csharp
app.UseJavascriptIsolation();
```



