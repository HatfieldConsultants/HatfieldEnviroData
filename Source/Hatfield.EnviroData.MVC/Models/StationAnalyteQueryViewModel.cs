﻿using Hatfield.EnviroData.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hatfield.EnviroData.MVC.Models
{
    public class StationAnalyteQueryViewModel
    {
        public string Variable { get; set; }
        public DateTime? ResultDateTime { get; set; }
        public double DataValue { get; set; }
        public string UnitsName { get; set; }
        public string UnitsTypeCV { get; set; }
    }
}