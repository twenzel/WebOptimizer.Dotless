# A LESS compiler for ASP.NET Core using dotless

[![NuGet](https://img.shields.io/nuget/v/codeessentials.WebOptimizer.Dotless.svg)](https://nuget.org/packages/codeessentials.WebOptimizer.Dotless/)

This package compiles LESS files into CSS by hooking into the [LigerShark.WebOptimizer](https://github.com/ligershark/WebOptimizer) pipeline.

## Versions
Master (Version >= 3.0) is being updated for ```ASP.NET Core 3.x```.

For ```ASP.NET Core 2.x```, use the latest 1.0.10 [Tag](https://github.com/twenzel/WebOptimizer.Dotless/tree/v1.0.10) or [NuGet Package](https://www.nuget.org/packages/codeessentials.WebOptimizer.Dotless/1.0.10).

## Install
Add the NuGet package [codeessentials.WebOptimizer.Dotless](https://nuget.org/packages/codeessentials.WebOptimizer.Dotless/) to any ASP.NET Core project supporting .NET Standard 2.0 or higher.

> &gt; dotnet add package codeessentials.WebOptimizer.Dotless

### Versions
Version|Support
-|-
&gt;= 3.x|ASP.Net Core 3.0 and above
&lt;= 1.0.10|netstandard2.0 (ASP.NET Core 2.x)


## Usage
Here's an example of how to compile `a.less` and `b.less` from inside the wwwroot folder and bundle them into a single .css file called `/all.css`:

In **Startup.cs**, modify the *ConfigureServices* method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddWebOptimizer(pipeline =>
    {
        pipeline.AddLessBundle("/all.css", "a.less", "b.less");
    });
}
```

...and add `app.UseWebOptimizer()` to the `Configure` method anywhere before `app.UseStaticFiles`, like so:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseWebOptimizer();

    app.UseStaticFiles();
    app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });
}
```

Now the path *`http://domain/all.css`* will return a compiled, bundled and minified CSS document based on the two source files.

You can also reference any .less files directly in the browser (*`http://domain/a.less`*) and a compiled and minified CSS document will be served. To set that up, do this:

```csharp
services.AddWebOptimizer(pipeline =>
{
    pipeline.CompileLessFiles();
});
```

Or if you just want to limit what .less files will be compiled, do this:

```csharp
services.AddWebOptimizer(pipeline =>
{
    pipeline.CompileLessFiles("/path/file1.less", "/path/file2.less");
});
```

## Setup TagHelpers
In `_ViewImports.cshtml` register the TagHelpers by adding `@addTagHelper *, WebOptimizer.Core` to the file. It may look something like this:

```text
@addTagHelper *, WebOptimizer.Core
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
