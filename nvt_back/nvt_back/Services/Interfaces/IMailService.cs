namespace nvt_back.Services.Interfaces
{
    public interface IMailService
    {
        void SendPropertyApprovedEmail(string email, string name, string propertyName);
        void SendPropertyDeniedEmail(string email, string name1, string name2, string reason);
    }
}
