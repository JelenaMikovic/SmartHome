using nvt_back.Services.Interfaces;

namespace nvt_back.Services
{
    public class ImageService : IImageService
    {
        private string generateUuid()
        {
            return Guid.NewGuid().ToString();
        }

        private string getImageFileTypeFromBase64(byte[] decodedBytes)
        {

            if (decodedBytes.Length >= 2)
            {
                if (decodedBytes[0] == 0xFF && decodedBytes[1] == 0xD8)
                {
                    return ".jpg";
                }
                else if (decodedBytes[0] == 0x89 && decodedBytes[1] == 0x50)
                {
                    return ".png";
                }
            }
            throw new Exception("Error saving image: Image type not supported.");
        }

        private string extractBase64String(string dataUri)
        {
            string[] parts = dataUri.Split(',');
            
            if (parts.Length > 1)
            {
                return parts[1];
            }

            throw new Exception("Error saving image: Invalid base64 format.");
        }

        private string getFilePath(byte[] imageBytes)
        {
            string extension = this.getImageFileTypeFromBase64(imageBytes);
            string fileName = this.generateUuid() + extension;
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "images");

            return Path.Combine(directoryPath, fileName);
        }

        public string SaveImage(string imageBase64)
        {
            imageBase64 = this.extractBase64String(imageBase64);
            byte[] imageBytes = Convert.FromBase64String(imageBase64);
            string filePath = this.getFilePath(imageBytes);

            try
            {
                File.WriteAllBytes(filePath, imageBytes);
                
                return "images\\" + filePath.Split('\\').Last();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception("Error saving image: Writing to file failed.");
                    
            }
        }

        public string GetBase64StringFromImage(string fileName)
        {
            try
            {
                using (FileStream fs = File.OpenRead(fileName))
                {
                    byte[] imageBytes = new byte[fs.Length];
                    fs.Read(imageBytes, 0, imageBytes.Length);
                    return "data:image/jpg;base64," + Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception("Error loading image: Reading from file failed.");

            }
        }

    }

        /*public string GetBase64StringFromImage(string fileName)
        {
            byte[] imageBytes = File.ReadAllBytes(fileName);
            string base64String = Convert.ToBase64String(imageBytes);
            // imgElement.src = 'data:image/jpeg;base64,' + base64String;
            return base64String;
        }*/
    //}
}
