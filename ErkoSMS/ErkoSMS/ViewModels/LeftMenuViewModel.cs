using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ErkoSMS.ViewModels
{
    public class LeftMenuViewModel
    {
        public List<LeftMenu> Navigations { get; set; }
        public LeftMenuViewModel()
        {
            Navigations = new List<LeftMenu>();
        }
        public LeftMenuViewModel(List<LeftMenu> navigationItems)
        {
            Navigations = navigationItems;
        }
    }

    public class LeftMenu
    {
        public LeftMenuItem menuItem { get; set; }
        public List<LeftMenuItem> subMenuItems { get; set; }
        public LeftMenu()
        {
            menuItem = new LeftMenuItem();
            subMenuItems = new List<LeftMenuItem>();
        }

        public LeftMenu(LeftMenuItem menuItem, List<LeftMenuItem> subMenuItems )
        {
            this.menuItem = menuItem;
            this.subMenuItems = subMenuItems != null ? subMenuItems : new List<LeftMenuItem>() ;
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
        public LeftMenuItem()
        {

        }
    }
}