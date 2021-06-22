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
using ManageProfile.Web.Models;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;

namespace ManageProfile.Web.Controllers
{
    public class HomeController : Controller
    {
        private AzureServiceTokenProvider _azureServiceTokenProvider = null;
        private KeyVaultClient _keyVaultClient = null;

        public HomeController()
        {
            _azureServiceTokenProvider = new AzureServiceTokenProvider();
            _keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    _azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            UserPrincipal userPrincipal;
            UserModel userModel = null;

            // Parse SAM account name from UPN
            string samAccountName = this.User.Identity.Name.Substring(0, this.User.Identity.Name.IndexOf("@"));

            // Retrieve user principal for Active Directory
            userPrincipal = GetUserPrincipal(samAccountName);

            if (userPrincipal != null)
            {
                userModel = new UserModel()
                {
                    SamAccountName = samAccountName,
                    UPN = userPrincipal.UserPrincipalName,
                    EmailAddress = userPrincipal.EmailAddress,
                    DisplayName = userPrincipal.DisplayName,
                    GivenName = userPrincipal.GivenName,
                    Surname = userPrincipal.Surname,
                    Description = userPrincipal.Description
                };
            }

            return View(userModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public ActionResult IndexPost(UserModel model)
        {
            if (ModelState.IsValid)
            {
                UserPrincipal userPrincipal = GetUserPrincipal(model.SamAccountName);
                if (userPrincipal != null)
                {
                    userPrincipal.EmailAddress = model.EmailAddress;
                    userPrincipal.DisplayName = model.DisplayName;
                    userPrincipal.GivenName = model.GivenName;
                    userPrincipal.Surname = model.Surname;
                    userPrincipal.Description = model.Description;
                    userPrincipal.Save();
                    TempData["Message"] = "Changed saved!";
                }
            }

            return RedirectToAction("Index");
        }

        private UserPrincipal GetUserPrincipal(string samAccountName)
        {
            PrincipalContext ctx = null;
            UserPrincipal userPrincipal = null;

            // Retrieve administrative level credentials from Azure Key Vault
            SecretBundle usernameBundle = _keyVaultClient.GetSecretAsync(ConfigurationManager.AppSettings["KeyVaultEndpoint"], "domainUserName").Result;
            SecretBundle passwordBundle = _keyVaultClient.GetSecretAsync(ConfigurationManager.AppSettings["KeyVaultEndpoint"], "domainPassword").Result;

            // Connect to the domain controller
            try
            {
                ctx = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["DomainName"], ConfigurationManager.AppSettings["Container"], ContextOptions.SimpleBind, usernameBundle.Value, passwordBundle.Value);
                //ctx = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["DomainName"]);
            }
            catch
            {
                ViewBag.Message = $"Unable to connect to domain controller {ConfigurationManager.AppSettings["DomainName"]}";
            }

            if (ctx != null)
            {
                try
                {
                    // Retrieve the user by their SAM account name from the domain
                    userPrincipal = UserPrincipal.FindByIdentity(ctx, samAccountName);
                }
                catch
                {
                    ViewBag.Message = $"User {samAccountName} could not be found in the directory. It may be a cloud only account.";
                }
                if (userPrincipal == null)
                {
                    ViewBag.Message = $"User {samAccountName} could not be found in the directory. It may be a cloud only account.";
                }
            }

            return userPrincipal;
        }
    }
}