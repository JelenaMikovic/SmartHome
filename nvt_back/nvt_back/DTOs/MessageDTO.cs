namespace nvt_back.DTOs
{
    public class MessageDTO
    {
        public string Message { get; set; }

        public MessageDTO(string message)
        {
            this.Message = message;
        }
    }
}
