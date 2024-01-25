namespace pdfsharp_online_backend.Helpers
{
    public class EmailBody
    {

        public static string EmailStringBody(string email, string emailToken)
        {
            return $@"
            <html>
            <head>          
            </head>
            <body>
                <h1>Reset your password</h1>
                <a href=""http://localhost:4200/reset?email={email}&code={emailToken}""> Reset Password </a>
            </body>
            </html>
            ";
        }
    }
}
