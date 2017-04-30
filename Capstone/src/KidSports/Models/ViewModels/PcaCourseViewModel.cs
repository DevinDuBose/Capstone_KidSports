﻿using KidSports.Models;
using Microsoft.AspNetCore.Http;
using System;

namespace KidSports.Models.ViewModels
{
    public class PcaCourseViewModel
    {
        public bool HasClicked { get; set; }
        public string VoucherCode { get; set; }
        public DateTime ConcussionCourseDate { get; set; }
        public IFormFile File { get; set; }

    }
}
