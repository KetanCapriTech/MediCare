namespace MediCareApi.Services.Interfaces
{
    public interface IEmailService
    {
        void SendOtp(string toEmail, string otp);
        void SendResetPassword(string toEmail, string resetLink);
        void SendForgotPassword(string toEmail, string resetLink);
        void SendInvite(string toEmail, string inviteLink);
        void SendAdminApprovalRequest(string adminEmail, string userEmail, string role);

    }
}
