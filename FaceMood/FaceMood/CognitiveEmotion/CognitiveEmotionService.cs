using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms.Internals;

namespace FaceMood.CognitiveEmotion
{
    public class CognitiveEmotionService
    {
        //static byte[] GetImageAsByteArray(string imageFilePath)
        //{
        //    FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
        //    BinaryReader binaryReader = new BinaryReader(fileStream);
        //    return binaryReader.ReadBytes((int)fileStream.Length);
        //}

        public async Task<CognitiveEmotionResult[]> GetImageEmotions(byte[] imageBytes)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "ee573352ecdb432bb49bc5149ef72a0c");

            // NOTE: You must use the same region in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westcentralus, replace "westus" in the 
            //   URI below with "westcentralus".
            string uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";
            HttpResponseMessage response;
            string responseContent;

            // Request body. Try this sample with a locally stored JPEG image.
            //byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(imageBytes))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                responseContent = response.Content.ReadAsStringAsync().Result;
            }

            var result = JsonConvert.DeserializeObject<CognitiveEmotionResult[]>(responseContent);

            return result;
        }
    }
}
