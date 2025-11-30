namespace infoapi.Entities
{
    public class OTPTable
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string Otp { get; set; }
        public DateTime ExpirationTime { get; set; }

        public virtual User User { get; set; }
    }
}
