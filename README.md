# A LESS compiler for ASP.NET Core using dotless

[![Build status](https://ci.appveyor.com/api/projects/status/8mh9woxgm5x8l4uw?svg=true)](https://ci.appveyor.com/project/twenzel/weboptimizer-dotless)
[![NuGet](https://img.shields.io/nuget/v/codeessentials.WebOptimizer.Dotless.svg)](https://nuget.org/packages/codeessentials.WebOptimizer.Dotless/)

This package compiles LESS files into CSS by hooking into the [LigerShark.WebOptimizer](https://github.com/ligershark/WebOptimizer) pipeline.

## Caution
Due to the lack of .NET Core/Standard support of the dotless package this extension for the [LigerShark.WebOptimizer](https://github.com/ligershark/WebOptimizer) pipeline is currently running on .NET Framework only.

## Install
Add the NuGet package [codeessentials.WebOptimizer.Dotless](https://nuget.org/packages/codeessentials.WebOptimizer.Dotless/) to any ASP.NET Core project supporting .NET Standard 2.0 or higher.

> &gt; dotnet add package codeessentials.WebOptimizer.Dotless

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