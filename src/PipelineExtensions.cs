using System.Collections.Generic;
using WebOptimizer;
using WebOptimizer.Dotless;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions methods for registrating the Less compiler on the Asset Pipeline.
    /// </summary>
    public static class PipelineExtensions
    {
        /// <summary>
        /// Compile Less files on the asset pipeline.
        /// </summary>
        public static IAsset CompileLess(this IAsset asset)
        {
            asset.Processors.Add(new Compiler());
            return asset;
        }

        /// <summary>
        /// Compile Less files on the asset pipeline.
        /// </summary>
        public static IEnumerable<IAsset> CompileLess(this IEnumerable<IAsset> assets)
        {
            var list = new List<IAsset>();

            foreach (IAsset asset in assets)
            {
                list.Add(asset.CompileLess());
            }

            return list;
        }

        /// <summary>
        /// Compile Less files on the asset pipeline.
        /// </summary>
        /// <param name="pipeline">The asset pipeline.</param>
        /// <param name="route">The route where the compiled .css file will be available from.</param>
        /// <param name="sourceFiles">The path to the .less source files to compile.</param>
        public static IAsset AddLessBundle(this IAssetPipeline pipeline, string route, params string[] sourceFiles)
        {
            return pipeline.AddBundle(route, "text/css; charset=UTF-8", sourceFiles).EnforceFileExtensions(".less")
                           .CompileLess()
                           .AdjustRelativePaths()
                           .Concatenate()
                           .FingerprintUrls()
                           .AddResponseHeader("X-Content-Type-Options", "nosniff")
                           .MinifyCss();
        }

        /// <summary>
        /// Compiles .less files into CSS and makes them servable in the browser.
        /// </summary>
        /// <param name="pipeline">The asset pipeline.</param>
        public static IEnumerable<IAsset> CompileLessFiles(this IAssetPipeline pipeline)
        {
            return pipeline.CompileLessFiles("**/*.less");
        }

        /// <summary>
        /// Compiles the specified .less files into CSS and makes them servable in the browser.
        /// </summary>
        /// <param name="pipeline">The pipeline object.</param>
        /// <param name="sourceFiles">A list of relative file names of the sources to compile.</param>
        public static IEnumerable<IAsset> CompileLessFiles(this IAssetPipeline pipeline, params string[] sourceFiles)
        {
            return pipeline.AddFiles("text/css; charset=UFT-8", sourceFiles).EnforceFileExtensions(".less")
                           .CompileLess()
                           .FingerprintUrls()
                           .AddResponseHeader("X-Content-Type-Options", "nosniff")
                           .MinifyCss();
        }
    }
}