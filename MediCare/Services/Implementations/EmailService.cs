using MediCareApi.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace MediCareApi.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _fromEmail = "capritech00@gmail.com";
        private readonly string _password = "urlq jwqa opah ihot";

        private void Send(string to, string subject, string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            using var smtp = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_fromEmail, _password),
                EnableSsl = true
            };

            smtp.Send(message);
        }

        public void SendOtp(string toEmail, string otp)
        {
            var subject = "Your OTP Code";
            var body = OtpTemplate(otp);
            Send(toEmail, subject, body);
        }

        public void SendResetPassword(string toEmail, string resetLink)
        {
            var subject = "Reset Your Password";
            var body = ResetPasswordTemplate(resetLink);
            Send(toEmail, subject, body);
        }

        public void SendForgotPassword(string toEmail, string resetLink)
        {
            var subject = "Forgot Password Request";
            var body = ForgotPasswordTemplate(resetLink);
            Send(toEmail, subject, body);
        }

        public void SendRegistrationLink(string toEmail, string registerLink)
        {
            var subject = "Complete Your Registration";
            var body = RegisterTemplate(registerLink);
            Send(toEmail, subject, body);
        }

        public void SendInvite(string toEmail, string inviteLink)
        {
            var subject = "You're Invited!";
            var body = InviteTemplate(inviteLink);
            Send(toEmail, subject, body);
        }

        public void SendAdminApprovalRequest(string adminEmail, string userEmail, string role)
        {
            var approveLink = $"https://medicare.com/api/admin/approve/{userEmail}";

            var subject = "New User Approval Required";
            var body = AdminApprovalTemplate(userEmail, role, approveLink);

            Send(adminEmail, subject, body);
        }


        // templates
        public static string OtpTemplate(string otp)
        {
            return $@"
        <h2>Your OTP Code</h2>
        <p>Use the following OTP to continue:</p>
        <h1>{otp}</h1>
        <p>This code expires in 5 minutes.</p>";
        }

        public static string ResetPasswordTemplate(string link)
        {
            return $@"
        <h2>Reset Password</h2>
        <p>Click the link below to reset your password:</p>
        <a href='{link}'>Reset Password</a>
        <p>If you didn’t request this, ignore this email.</p>";
        }

        public static string ForgotPasswordTemplate(string link)
        {
            return $@"
        <h2>Forgot Password</h2>
        <p>We received a request to reset your password.</p>
        <a href='{link}'>Reset Password</a>";
        }

        public static string RegisterTemplate(string link)
        {
            return $@"
        <h2>Welcome!</h2>
        <p>Please complete your registration:</p>
        <a href='{link}'>Register Now</a>";
        }

        public static string InviteTemplate(string link)
        {
            return $@"
        <h2>You are Invited!</h2>
        <p>Click below to join our platform:</p>
        <a href='{link}'>Accept Invitation</a>";
        }

        public static string AdminApprovalTemplate(string userEmail, string role, string approveLink)
        {
            return $@"
            <h2>Approval Required</h2>
            <p>A new <strong>{role}</strong> has registered and requires approval.</p>

            <p><strong>User Email:</strong> {userEmail}</p>

            <p>
                <a href='{approveLink}'
                   style='padding:10px 15px; background:#28a745; color:white; text-decoration:none;'>
                   Approve User
                </a>
            </p>

            <p>If you did not expect this request, please ignore.</p>";
        }

    }
}
