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
namespace ManageProfile.Web.Models
{
    public class UserModel
    {
        public string SamAccountName { get; set; }
        public string UPN { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Description { get; set; }
    }
}