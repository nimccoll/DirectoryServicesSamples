//===============================================================================
// Microsoft FastTrack for Azure
// Directory Services Samples
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;

namespace ManageProfile.Web
{
    public partial class Startup
    {
        private readonly string _clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private readonly string _authority = string.Format("{0}/{1}/", ConfigurationManager.AppSettings["ida:Authority"], ConfigurationManager.AppSettings["ida:Tenant"]);
        private readonly string _redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                CookieName = "ManageProfile",
                CookieManager = new Microsoft.Owin.Host.SystemWeb.SystemWebChunkingCookieManager()
            });

            app.UseOpenIdConnectAuthentication(
                 new OpenIdConnectAuthenticationOptions
                 {
                     ClientId = _clientId,
                     Authority = _authority,
                     RedirectUri = _redirectUri
                 });
        }
    }
}
