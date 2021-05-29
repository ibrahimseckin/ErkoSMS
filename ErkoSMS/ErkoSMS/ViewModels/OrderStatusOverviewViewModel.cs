using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ErkoSMS.ViewModels
{
    public class OrderStatusOverviewViewModel
    {
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
        public List<string> BackgroundColors { get; set; }
    }
}