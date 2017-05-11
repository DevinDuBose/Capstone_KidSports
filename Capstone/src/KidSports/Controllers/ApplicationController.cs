﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using KidSports.Models.ViewModels;
using KidSports.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using KidSports.Models;

namespace KidSports.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private IHostingEnvironment _environment;
        //private IApplicationRepo repository;

        public ApplicationController(IHostingEnvironment environment)
        {
            _environment = environment;
        }


        //public ApplicationController(IApplicationRepo repo)
        //{
        //    repository = repo;
        //}

        #region Home Page
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Application Application)
        {

            Application app = new Application();
            return RedirectToAction("Application/CoachInfo", new { AppId = app.ApplicationID });
        }

        [HttpGet]
        public IActionResult NoApplication()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Appinprocess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CompletedApplication()
        {
            return View();
        }
        #endregion

        #region SportsManager Views
        [HttpGet]
        [Authorize(Roles = "SportsManager")]
        public IActionResult Applications()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "SportsManager")]
        public IActionResult ApplicantDetails()
        {
            return View();
        }
        #endregion

        #region Background Check
        [HttpGet]
        //List all background checks that have been processed by the CRIS API and are awaiting approval.
        public IActionResult BackgroundCheckResults()
        {
            return View();
        }

        //Display the result incidents of the specific background check.
        [HttpGet]
        public IActionResult BGCResultsDescription()
        {
            return View();
        }
        #endregion

        #region Application Coach Information
        [HttpGet]
        public IActionResult CoachInfo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CoachInfo(CoachInfoViewModel CInfoVm)
        {

           return View();
        }
        #endregion

        #region Application Coach Interests
        [HttpGet]
        public IActionResult CoachInterests()
        {
            return View();
        }

        //CoachInterests(int AppID, ViewModel vm)
        //Application a = repo.GetApplicationByID(AppID)
        //if (!IsNullOrEmpty(a.information)
        //    vm.information == a.information
        // check rest of app see what fields are already complete
        //
        // return view(vm);



        //[HttpPost]
        //public IActionResult CoachInterests()
        //{
        //    return View();
        //}
        #endregion

        #region Application Coach Pledge
        [HttpGet]
        public IActionResult CoachPledge()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult CoachPledge()
        //{
        //    return View();
        //}
        #endregion

        #region Application Concussion Course
        [HttpGet]
        public IActionResult ConcussionCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConcussionCourse(ConcussionCourseViewModel p4Vm)
        {
                if (p4Vm.File != null)
                {
                    var uploads = Path.Combine(_environment.WebRootPath);
                    if (p4Vm.File.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, p4Vm.File.FileName), FileMode.Create))
                        {
                            await p4Vm.File.CopyToAsync(fileStream);
                        }
                        //p4Vm.Application.NfhsPath = $"\\{p4Vm.File.FileName}";
                        return RedirectToAction("PcaCourse", "Application");
                    }
                    else return View(p4Vm);
                }
                else return View(p4Vm);
        }
        #endregion

        #region Application Pca Course
        [HttpGet]
        public IActionResult PcaCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PcaCourse(IDViewModel p5Vm)
        {
            var uploads = Path.Combine(_environment.WebRootPath);
            if (p5Vm.File.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, p5Vm.File.FileName), FileMode.Create))
                {
                    await p5Vm.File.CopyToAsync(fileStream);
                }
                //p5Vm.Application.PcaPath = $"\\{p5Vm.File.FileName}";
                return RedirectToAction("ID", "Application");
            }
            return View(p5Vm);
        }
        #endregion

        #region Application ID
        [HttpGet]
        public IActionResult ID()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ID(PcaCourseViewModel p6Vm)
        {
            var uploads = Path.Combine(_environment.WebRootPath);
            if (p6Vm.File.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, p6Vm.File.FileName), FileMode.Create))
                {
                    await p6Vm.File.CopyToAsync(fileStream);
                }
                //p6Vm.Application.DlPath = $"\\{p6Vm.File.FileName}";
                return RedirectToAction("Badge", "Application");
            }
            return View(p6Vm);
        }
        #endregion

        #region Application Badge
        [HttpGet]
        public IActionResult Badge()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Page7(BadgeViewModel p7Vm)
        {
            var uploads = Path.Combine(_environment.WebRootPath);
            if (p7Vm.File.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, p7Vm.File.FileName), FileMode.Create))
                {
                    await p7Vm.File.CopyToAsync(fileStream);
                }
                //p7Vm.Application.DlPath = $"\\{p7Vm.File.FileName}";
                return RedirectToAction("Appinprocess", "Application");
            }
            return View(p7Vm);
        }
        #endregion
        

    }
}
