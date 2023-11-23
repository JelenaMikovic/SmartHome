namespace nvt_back.DTOs
{
    public class PropertyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumOfFloors { get; set; }
        public double Area { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
        public ReturnedAddressDTO Address { get; set; }
        public UserDTO? Owner { get; set; }
    }
}
