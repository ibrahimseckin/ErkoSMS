﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Model
{
    public class Exporter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TradeRegisterNo { get; set; }
        public string VatNo { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
    }
}
