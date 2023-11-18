﻿using nvt_back.Services.Interfaces;

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

        public string SaveImage(string imageBase64)
        {
            imageBase64 = this.extractBase64String(imageBase64);
            byte[] imageBytes = Convert.FromBase64String(imageBase64);
            string extension = this.getImageFileTypeFromBase64(imageBytes);
            string fileName = this.generateUuid() + extension;
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
            string filePath = Path.Combine(directoryPath, fileName);

            try
            {
                File.WriteAllBytes(filePath, imageBytes);
                
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception("Error saving image: Writing to file failed.");
                    
            }
        }
}
}