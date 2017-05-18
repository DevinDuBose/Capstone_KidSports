﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KidSports.Models
{
    public class ApplicationSearchModel
    {
        public int ApplicationID { get; set; }
        public List<Application> filterableApps { get; set; }
        //public List<Tuple<int, string, ApplicationStatus>> keyValStatus { get; set; }
        public bool IsChecked { get; set; }
        public string Area { get; set; }
        public string School { get; set; }
        public string Sport { get; set; }
        public string Gender { get; set; }
        public string Grade { get; set; }
    }
}
