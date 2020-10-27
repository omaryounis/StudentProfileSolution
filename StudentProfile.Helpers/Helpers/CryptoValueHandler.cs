using StudentProfile.Components.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StudentProfile.Components.Helpers
{
    public class CryptoValueProvider : IValueProvider
    {
        RouteData routeData = null;
        Dictionary<string, string> dictionary = null;

        public CryptoValueProvider(RouteData routeData)
        {
            this.routeData = routeData;
        }

        public bool ContainsPrefix(string prefix)
        {
            if (this.routeData.Values["id"] == null && HttpContext.Current.Request.QueryString.ToString() == "")
            {
                return false;
            }
            string query = this.routeData.Values["id"] != null ? this.routeData.Values["id"].ToString() : HttpContext.Current.Request.QueryString.ToString();
            if(EncryptDecrypt.IsBase64Encoded(query))
            this.dictionary = EncryptDecrypt.Decrypt(query, false);

            return this.dictionary !=null? this.dictionary.ContainsKey(prefix):false;
        }

        public ValueProviderResult GetValue(string key)
        {
            ValueProviderResult result = null;
            if (this.dictionary != null && this.dictionary.Keys.Contains(key))
            {
                result = new ValueProviderResult(this.dictionary[key],
                    this.dictionary[key], CultureInfo.CurrentCulture);
            }
            return result;
        }
    }
    public class CryptoValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new CryptoValueProvider(controllerContext.RouteData);
        }
    }
    public class CryptoValueProviderAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            filterContext.Controller.ValueProvider = new CryptoValueProvider(filterContext.RouteData);
        }
    }
}
