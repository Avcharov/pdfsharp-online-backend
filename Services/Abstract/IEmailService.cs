namespace pdfsharp_online_backend.Services.Abstract
{
    public interface IEmailService
    {

        void SendEmail(EmailModel email);
    }
}
