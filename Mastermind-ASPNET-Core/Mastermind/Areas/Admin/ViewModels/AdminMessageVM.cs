using Mastermind.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Demo_ASPNET_Core.Areas.Admin.ViewModels
{
    public class AdminMessageVM
    {
        public string Message { get; set; } = "";
        public string Area { get; set; } = "Admin";
        public string Controller { get; set; } = "Home";
        public string Action { get; set; } = "Index";
        public string Id { get; set; } = "";
        public string BackToMsg { get; set; } = Resource.BackToHomepage;
    }
}
