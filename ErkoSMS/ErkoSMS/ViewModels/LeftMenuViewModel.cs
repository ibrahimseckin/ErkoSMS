using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ErkoSMS.ViewModels
{
    public class LeftMenuViewModel
    {
        public List<LeftMenuItem> Navigations { get; set; }
        public LeftMenuViewModel()
        {
            Navigations = new List<LeftMenuItem>();
        }
        public LeftMenuViewModel(List<LeftMenuItem> navigationItems)
        {
            Navigations = navigationItems;
        }
    }

    public class LeftMenuItem
    {
        public string DisplayText { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public LeftMenuItem(string displayText, string controller, string action)
        {
            this.DisplayText = displayText;
            this.Controller = controller;
            this.Action = action;
        }
    }
}