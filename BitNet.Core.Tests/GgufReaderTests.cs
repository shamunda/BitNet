using Xunit;
using BitNet.Core;
using System.IO;
using System.Collections.Generic;

namespace BitNet.Core.Tests
{
    public class GgufReaderTests
    {
        [Fact]
        public void ReadModel_ShouldReadHeaderAndMetadata()
        {
            // Arrange
            var modelPath = "../../../ggml-model-i2_s.gguf";

            // Act
            var model = GgufReader.ReadModel(modelPath);

            // Assert
            Assert.NotNull(model);
            Assert.Equal(0x46554747, model.Header.Magic); // GGUF in little-endian
            Assert.Equal((uint)3, model.Header.Version);
            Assert.True(model.Metadata.Count > 0);

            // Optional: Print metadata for manual verification
            foreach(var kvp in model.Metadata)
            {
                System.Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }
    }
}
