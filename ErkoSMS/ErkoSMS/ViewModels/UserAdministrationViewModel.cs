using ErkoSMS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ErkoSMS.ViewModels
{
    public class UserAdministrationViewModel
    {
        public List<UserViewModel> Users { get; set; }
    }

    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}