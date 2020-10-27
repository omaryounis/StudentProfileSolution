
using System;
using System.Configuration;

using System.Web.Mvc;

namespace StudentProfile.Components
{
    public static class HtmlHelperExtensions
    {
        public enum Theme { Red, White, BlackGlass, Clean }
        public static MvcHtmlString GenerateCaptcha(this HtmlHelper helper, Theme theme, string callBack = null)
        {
            const string htmlInjectString = @"<div id=""recaptcha_div""></div>
<script type=""text/javascript"">
    Recaptcha.create(""{0}"", ""recaptcha_div"", {{ theme: ""{1}"" {2}}});
</script>";

            var publicKey = ConfigurationManager.AppSettings["ReCaptcha.PublicKey"];

            if (string.IsNullOrWhiteSpace(publicKey))
                throw new InvalidKeyException("ReCaptcha.PublicKey missing from appSettings");

            if (!string.IsNullOrWhiteSpace(callBack))
                callBack = string.Concat(", callback: ", callBack);

            var html = string.Format(htmlInjectString, publicKey, theme.ToString().ToLower(), callBack);
            return MvcHtmlString.Create(html);
        }
    }
    //[Serializable]
    //public class InvalidKeyException : ApplicationException
    //{
    //    public InvalidKeyException() { }
    //    public InvalidKeyException(string message) : base(message) { }
    //    public InvalidKeyException(string message, Exception inner) : base(message, inner) { }
    //}
}
