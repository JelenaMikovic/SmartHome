namespace nvt_back.Services.Interfaces
{
    public interface IImageService
    {
        string SaveImage(string imageBase64);
        string GetBase64StringFromImage(string fileName);
    }
}
