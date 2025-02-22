namespace infoapi.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmail(string toemail,string otp);
    }
}
