using dotless.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebOptimizer.Dotless
{
    /// <summary>
    /// Compiles Sass files
    /// </summary>
    /// <seealso cref="WebOptimizer.IProcessor" />
    public class Compiler : IProcessor
    {
        /// <summary>
        /// Gets the custom key that should be used when calculating the memory cache key.
        /// </summary>
        public string CacheKey(HttpContext context) => string.Empty;

        /// <summary>
        /// Executes the processor on the specified configuration.
        /// </summary>
        public Task ExecuteAsync(IAssetContext context)
        {
            var content = new Dictionary<string, byte[]>();
            var env = (IHostingEnvironment)context.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));
            IFileProvider fileProvider = context.Asset.GetFileProvider(env);

            foreach (string route in context.Content.Keys)
            {
                IFileInfo file = fileProvider.GetFileInfo(route);

                var css = Less.Parse(context.Content[route].AsString(), new dotless.Core.configuration.DotlessConfiguration()
                {
                });


                //var settings = new ScssOptions { InputFile = file.PhysicalPath };

                //ScssResult result = Scss.ConvertToCss(context.Content[route].AsString(), settings);

                //content[route] = result.Css.AsByteArray();

                content[route] = System.Text.Encoding.UTF8.GetBytes(css);
            }

            context.Content = content;

            return Task.CompletedTask;
        }
    }
}
