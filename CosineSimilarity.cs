using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorGenAI
{
    public class CosineSimilarity
    {
        /// <summary>
        /// Calculates the cosine similarity between two vectors.
        /// </summary>
        /// <param name="vector1">The first vector as a list of floats.</param>
        /// <param name="vector2">The second vector as a list of floats.</param>
        /// <returns>The cosine similarity between the two vectors, or 0 if either vector is null or their lengths differ.</returns>
        public double GetCosineSimilarity(float[]? vector1, float[]? vector2)
        {
            if (vector1 == null || vector2 == null || vector1.Length != vector2.Length)
            {
                return 0;
            }

            int count = vector1.Length;
            double dotProduct = 0.0;
            double vector1Magnitude = 0.0;
            double vector2Magnitude = 0.0;

            for (int i = 0; i < count; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                vector1Magnitude += Math.Pow(vector1[i], 2);
                vector2Magnitude += Math.Pow(vector2[i], 2);
            }

            // Handle potential division by zero
            if (vector1Magnitude == 0 || vector2Magnitude == 0)
            {
                return 0;
            }

            return dotProduct / (Math.Sqrt(vector1Magnitude) * Math.Sqrt(vector2Magnitude));
        }

        public class EmbeddingData
        {
            public string? FileName { get; set; }
            public float[]? Embedding { get; set; }
        }

        const string OpenAIKey = "your-openai-key-here";
        const string OpenAIModel = "text-embedding-ada-002";

        public static async Task Main(string[] args)
        {
            List<EmbeddingData> embeddingDataList = new List<EmbeddingData>();
            EmbeddingData lastItem = new EmbeddingData();
            double highestCosineSimilarity = 0;

            // Uri oaiEndpoint = new ("https://YOUR_RESOURCE_NAME.openai.azure.com");
            // AzureKeyCredential credentials = new (oaiKey);
            // OpenAIClient openAIClient = new (oaiEndpoint, credentials);
            
            var openAIClient = new OpenAIClient(OpenAIKey, new OpenAIClientOptions());

            EmbeddingsOptions embeddingOptions = new()
            {
                DeploymentName = "text-embedding-ada-002",
                Input = { "熱輻射" },
            };

            var response = await openAIClient.GetEmbeddingsAsync(embeddingOptions);
            float[] embeddedData = ConvertReadOnlyMemoryToArray(response.Value.Data[0].Embedding);

            foreach (EmbeddingData embeddedItem in embeddingDataList)
            {
                double cosineSimilarity = new CosineSimilarity().GetCosineSimilarity(embeddedData, embeddedItem.Embedding);
                if (cosineSimilarity > highestCosineSimilarity)
                {
                    highestCosineSimilarity = cosineSimilarity;
                    lastItem = embeddedItem;
                }
            }

            Console.WriteLine($"The file with the highest cosine similarity is: {lastItem.FileName}");
        }

        public static float[] ConvertReadOnlyMemoryToArray(ReadOnlyMemory<float> readOnlyMemory)
        {
            // Get the span from the ReadOnlyMemory
            ReadOnlySpan<float> span = readOnlyMemory.Span;

            // Create a new float array with the same length as the span
            float[] array = new float[span.Length];

            // Copy the contents of the span to the new array
            span.CopyTo(array);

            return array;
        }
    }
}
