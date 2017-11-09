using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using WebOptimizer;
using Microsoft.Extensions.DependencyInjection;

namespace WebOptimizer.Dotless.Test
{
    public class AssetExtensionsTest
    {
        [Fact]
        public void Compile_single_Success()
        {
            var asset = GenerateAssets(1).First();

            asset.CompileLess();

            Assert.Equal(1, asset.Processors.Count);
            Assert.Contains(asset.Processors, p => p is Compiler);
        }

        [Fact]
        public void Compile_Multiple_Success()
        {
            var assets = GenerateAssets(5).ToArray();

            assets.CompileLess();

            foreach (IAsset asset in assets)
            {
                Assert.Equal(1, asset.Processors.Count);
                Assert.Contains(asset.Processors, p => p is Compiler);
            }
        }

        private IEnumerable<IAsset> GenerateAssets(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                var asset = new Mock<IAsset>();
                asset.SetupGet(a => a.Processors)
                     .Returns(new List<IProcessor>());

                yield return asset.Object;
            }
        }
    }
}
