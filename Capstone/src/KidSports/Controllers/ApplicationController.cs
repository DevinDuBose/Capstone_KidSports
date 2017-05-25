﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using KidSports.Models.ViewModels;
using KidSports.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using KidSports.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KidSports.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private IHostingEnvironment _environment;
        private IApplicationRepo appRepo;
        private IUserRepo userRepo;
        private IStateRepo stateRepo;
        private ISportRepo sportRepo;
        private IAreaRepo areaRepo;
        private ISchoolRepo schoolRepo;
        private IGradeRepo gradeRepo;
        private IExpRepo expRepo;
        private IAppStatusRepo appStatusRepo;

        UserManager<User> userManager;

        public ApplicationController(UserManager<User> UM, IHostingEnvironment environment, IApplicationRepo apprepo, IUserRepo userrepo, IStateRepo staterepo, ISportRepo sportrepo, IAreaRepo arearepo, ISchoolRepo schoolrepo, IGradeRepo graderepo, IExpRepo exprepo, IAppStatusRepo appstatusrepo)
        {
            userManager = UM;
            _environment = environment;
            appRepo = apprepo;
            userRepo = userrepo;
            stateRepo = staterepo;
            sportRepo = sportrepo;
            areaRepo = arearepo;
            schoolRepo = schoolrepo;
            gradeRepo = graderepo;
            expRepo = exprepo;
            appStatusRepo = appstatusrepo;
        }

        #region Home Page
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IndexViewModel ivm = new IndexViewModel();
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
                ivm.UserFirstName = user.FirstName;

                if (user.currentYearApp != null)
                {
                    int id = user.currentYearApp.ApplicationID;
                    ivm.ApplicationID = id;
                    return View(ivm);
                }
            }

            ivm.ApplicationID = 0;
            return View(ivm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(IndexViewModel ivm)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            if (ivm.ApplicationID == 0)
            {
                Application app = new Application();
                appRepo.CreateApp(app);

                ApplicationStatus appstatus = new ApplicationStatus();
                appstatus.AppStartDate = DateTime.Now;
                appstatus.ApplicationID = app.ApplicationID;
                appStatusRepo.Create(appstatus);

                ivm.ApplicationID = app.ApplicationID;
                if (user != null)
                {
                    user.currentYearApp = app;
                    userRepo.Update(user);
                }
                return RedirectToAction("CoachInfo", new { AppID = ivm.ApplicationID });
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (user.currentYearApp.ApplicationID == ivm.ApplicationID || User.IsInRole("Admin") || User.IsInRole("SportsManager"))
            {
                if (ivm.ApplicationID != 0)
                {
                    /* Use app id to do stuff */
                    if (ivm.PageName == "CoachInfo")
                        return RedirectToAction("CoachInfo", new { AppID = ivm.ApplicationID });
                    if (ivm.PageName == "CoachInterests")
                        return RedirectToAction("CoachInterests", new { AppID = ivm.ApplicationID });
                    if (ivm.PageName == "CoachPledge")
                        return RedirectToAction("CoachPledge", new { AppID = ivm.ApplicationID });
                    if (ivm.PageName == "ConcussionCourse")
                        return RedirectToAction("ConcussionCourse", new { AppID = ivm.ApplicationID });
                    if (ivm.PageName == "PcaCourse")
                        return RedirectToAction("PcaCourse", new { AppID = ivm.ApplicationID });
                    if (ivm.PageName == "ID")
                        return RedirectToAction("ID", new { AppID = ivm.ApplicationID });
                    if (ivm.PageName == "Badge")
                        return RedirectToAction("Badge", new { AppID = ivm.ApplicationID });
                    return View(ivm);
                }
            }
            else
                return RedirectToAction("AccessDenied", "Account");

            return View(ivm);
        }

        #endregion

        #region SportsManager Views
        [HttpGet]
        [Authorize(Roles = "SportsManager")]
        public IActionResult Applications()
        {
            //Get the sports manager and put their area in the search model by default.
            ApplicationSearchModel asm = new ApplicationSearchModel();
            asm.filteredApps = new List<Application>();
            asm.filteredUsers = new List<User>();
            asm.filteredAppStatus = new List<ApplicationStatus>();

            List<Application> filteredApps = appRepo.GetFilteredApplications(asm).ToList();
            List<User> filteredUsers = new List<User>();
            List<ApplicationStatus> filteredAppStatus = new List<ApplicationStatus>();

            foreach (Application a in filteredApps)
            {
                User u = userRepo.GetUserByID(a.ApplicationID);
                ApplicationStatus appstatus = appStatusRepo.GetAppStatusByID(a.ApplicationID);

                filteredUsers.Add(u);
                filteredAppStatus.Add(appstatus);
            }

            if (filteredApps != null)
                asm.filteredApps.AddRange(filteredApps);
            else
                asm.filteredApps = new List<Application>();

            if (filteredUsers != null)
                asm.filteredUsers.AddRange(filteredUsers);
            else
                asm.filteredUsers = new List<User>();

            if (filteredAppStatus != null)
                asm.filteredAppStatus.AddRange(filteredAppStatus);
            else
                asm.filteredAppStatus = new List<ApplicationStatus>();

            return View(asm);
        }
        
        [HttpPost]
        [Authorize(Roles = "SportsManager")]
        public IActionResult Applications(ApplicationSearchModel asm = null)
        {
            if (asm == null)
               asm = new ApplicationSearchModel();
            asm.filteredApps = new List<Application>();
            asm.filteredUsers = new List<User>();
            asm.filteredAppStatus = new List<ApplicationStatus>();

            List<Application> filteredApps = appRepo.GetFilteredApplications(asm).ToList();
            List<User> filteredUsers = new List<User>();
            List<ApplicationStatus> filteredAppStatus = new List<ApplicationStatus>();

            foreach (Application a in filteredApps)
            {
                User u = userRepo.GetUserByID(a.ApplicationID);
                ApplicationStatus appstatus = appStatusRepo.GetAppStatusByID(a.ApplicationID);

                filteredUsers.Add(u);
                filteredAppStatus.Add(appstatus);
            }

            if (filteredApps != null)
                asm.filteredApps.AddRange(filteredApps);
            else
                asm.filteredApps = new List<Application>();

            if (filteredUsers != null)
                asm.filteredUsers.AddRange(filteredUsers);
            else
                asm.filteredUsers = new List<User>();

            if (filteredAppStatus != null)
                asm.filteredAppStatus.AddRange(filteredAppStatus);
            else
                asm.filteredAppStatus = new List<ApplicationStatus>();
            return View(asm);
        }


        [HttpGet]
        [Authorize(Roles = "SportsManager")]
        public IActionResult ApplicantDetails(int ApplicantID)
        {
            //Do security stuff
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
        public async Task<IActionResult> CoachInfo(int AppID)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == AppID)
            {
                //Get the coaches current app
                Application currentApp = appRepo.GetApplicationByID(AppID);


                CoachInfoViewModel civm = new CoachInfoViewModel();

                #region Bind application to view model if pre-exisiting info
                //If any information exists, bind it to the view model.
                civm.ApplicationID = AppID;

                //if (!User.IsInRole("Admin") && !User.IsInRole("Sports Manager"))
                //{
                if (!User.IsInRole("SportsManager") && !User.IsInRole("Admin"))
                {

                    if (user.FirstName != null) civm.FirstName = user.FirstName;
                    if (user.MiddleName != null) civm.MiddleName = user.MiddleName;
                    if (user.LastName != null) civm.LastName = user.LastName;
                    if (user.PreviousLastName1 != null) civm.PreviousLastName1 = user.PreviousLastName1;
                    if (user.PreviousLastName2 != null) civm.PreviousLastName2 = user.PreviousLastName2;
                    if (user.PreviousLastName3 != null) civm.PreviousLastName3 = user.PreviousLastName3;
                    if (user.PhoneNumber != null) civm.CellPhone = user.PhoneNumber;
                    if (user.AlternatePhone != null) civm.AlternatePhone = user.AlternatePhone;
                    if (user.PreferredName != null) civm.PreferredName = user.PreferredName;
                
                }
                else
                {
                    User appuser = userRepo.GetUserByID(AppID);
                    civm.FirstName = appuser.FirstName;
                    civm.MiddleName = appuser.MiddleName;
                    civm.LastName = appuser.LastName;
                    civm.PreviousLastName1 = appuser.PreviousLastName1;
                    civm.PreviousLastName2 = appuser.PreviousLastName2;
                    civm.PreviousLastName3 = appuser.PreviousLastName3;
                    civm.PreferredName = appuser.PreferredName;
                    civm.CellPhone = appuser.PhoneNumber;
                    civm.AlternatePhone = appuser.AlternatePhone;
                }
           

            if (currentApp.DOB != null) civm.DOB = currentApp.DOB;
                if (currentApp.YearsLivedInOregon != -1) civm.YearsLivingInOregon = currentApp.YearsLivedInOregon;
                if (currentApp.Address != null) civm.Address = currentApp.Address;
                if (currentApp.City != null) civm.City = currentApp.City;
                if (currentApp.State != null) civm.State = currentApp.State; else civm.State = new State();
                if (currentApp.ZipCode != null) civm.Zip = currentApp.ZipCode;
                if (currentApp.LivedOutsideUSA != false) civm.HasLivedOutsideUSA = currentApp.LivedOutsideUSA;
                
                if (currentApp.City != null) civm.City = currentApp.City;
                if (currentApp.State != null) civm.State = currentApp.State;
                if (currentApp.Address != null) civm.Address = currentApp.Address;
                if (currentApp.CurrentEmployer != null) civm.CurrentEmployer = currentApp.CurrentEmployer;
                if (currentApp.JobTitle != null) civm.JobTitle = currentApp.JobTitle;
                civm.YearsLivingInOregon = currentApp.YearsLivedInOregon;

                civm.AllStates = stateRepo.GetAllStates();
                if (currentApp.StatesLived != null)
                {
                    foreach (AppStateJoin a in currentApp.StatesLived)
                        civm.PreviousStates.Add(stateRepo.GetStateByID(a.StateID));
                } else
                {
                    civm.PreviousStates = new List<State>();
                }
                #endregion

                //Display the view.
                return View(civm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> CoachInfo(CoachInfoViewModel civm)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == civm.ApplicationID)
            {
                //Process all data that is in the view model. If anything is new or changed,
                //update the coaches current application.
                Application currentApp = appRepo.GetApplicationByID(civm.ApplicationID);
                #region Bind VM to application
                if (civm.FirstName != null) user.FirstName = civm.FirstName;
                if (civm.LastName != null) user.LastName = civm.LastName;
                if (civm.MiddleName != null) user.MiddleName = civm.MiddleName;
                if (civm.PreviousLastName1 != null) user.PreviousLastName1 = civm.PreviousLastName1;
                if (civm.PreviousLastName2 != null) user.PreviousLastName2 = civm.PreviousLastName2;
                if (civm.PreviousLastName3 != null) user.PreviousLastName3 = civm.PreviousLastName3;
                if (civm.PreferredName != null) user.PreferredName = civm.PreferredName;
                if (civm.CellPhone != null) user.PhoneNumber = civm.CellPhone;
                if (civm.AlternatePhone != null) user.AlternatePhone = civm.AlternatePhone;
                currentApp.LivedOutsideUSA = civm.HasLivedOutsideUSA;
                if (civm.newPickedStateID != -1) currentApp.State = stateRepo.GetStateByID(civm.newPickedStateID);
                if (civm.PreviousStates != null)
                {
                    foreach (State s in civm.PreviousStates)
                        currentApp.StatesLived.Add(new AppStateJoin() { ApplicationID = currentApp.ApplicationID, StateID = s.StateID });
                }
                if (civm.DOB != new DateTime()) currentApp.DOB = civm.DOB; 
                if (civm.Address != null) currentApp.Address = civm.Address;
                if (civm.City != null) currentApp.City = civm.City;
                if (civm.Zip != null) currentApp.ZipCode = civm.Zip;
                if (civm.CurrentEmployer != null) currentApp.CurrentEmployer = civm.CurrentEmployer;
                if (civm.JobTitle != null) currentApp.JobTitle = civm.JobTitle;
                
                if (civm.YearsLivingInOregon != -1) currentApp.YearsLivedInOregon = civm.YearsLivingInOregon;
                appRepo.Update(currentApp);
             
                #endregion
                userRepo.Update(user);

                if (civm.Direction == "approve")
                {
                    return RedirectToAction("CoachInterests", new { AppID = civm.ApplicationID });
                }

                if (civm.Direction == "deny")
                {
                    User appuser = userRepo.GetUserByID(civm.ApplicationID);
                    return RedirectToAction("ApplicantDetails", new { ApplicantID = appuser.Id });
                }
                //Decide which direction is being taken (this page only has next).
                if (civm.Direction == "next")
                    return RedirectToAction("CoachInterests", new { AppID = civm.ApplicationID });

                //if all else fails, post back to the same page.
                return View(civm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }
        #endregion

        #region Application Coach Interests
        [HttpGet]
        public async Task<IActionResult> CoachInterests(int AppID)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == AppID)
            {
                //Get the coaches current app
                Application currentApp = appRepo.GetApplicationByID(AppID);
                CoachInterestViewModel civm = new CoachInterestViewModel();

                #region Bind application to view model if pre-exisiting info
                //If any information exists, bind it to the view model.
                civm.ApplicationID = AppID;
                civm.AllSports = sportRepo.GetAllSports();
                civm.AllAreas = areaRepo.GetAllAreas();
                if (civm.Area != null)
                    civm.SchoolsByArea = schoolRepo.GetSchoolsByArea(civm.Area);
                else civm.SchoolsByArea = schoolRepo.GetAllSchools();
                civm.AllGrades = gradeRepo.GetAllGrades();
                civm.AllExperience = expRepo.GetAllExperiences();

                if (currentApp.AppArea != null) civm.Area = currentApp.AppArea; else civm.Area = new Area();
                if (currentApp.IsHeadCoach != false) civm.IsHeadCoach = currentApp.IsHeadCoach;
                if (currentApp.IsAssistantCoach != false) civm.IsAssistantCoach = currentApp.IsAssistantCoach;
                if (currentApp.AppSport != null) civm.Sport = currentApp.AppSport;
                if (currentApp.AppGender != null) civm.Gender = currentApp.AppGender;
                if (currentApp.AppSchool != null) civm.School = currentApp.AppSchool; else civm.School = new School();
                if (currentApp.AppGrade != null) civm.GradePreference = currentApp.AppGrade; 
                if (currentApp.NameOfChild != null) civm.ChildTeam = currentApp.NameOfChild;
                if (currentApp.YearsExperience != -1) civm.YearsExperience = currentApp.YearsExperience;

                if (civm.PreviousExperience != null)
                {
                    foreach (Experience e in civm.PreviousExperience)
                        currentApp.PreviousExperience.Add(new AppExpJoin() { ApplicationID = currentApp.ApplicationID, ExperienceID = e.ExperienceID });
                }
                else if (civm.PreviousExperience == null & currentApp.PreviousExperience != null)
                    foreach (AppExpJoin ae in currentApp.PreviousExperience)
                       civm.PreviousExperience.Add(expRepo.GetExperienceByID(ae.ExperienceID));
                else
                    civm.PreviousExperience = new List<Experience>();
                #endregion

                //Display the view.
                return View(civm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> CoachInterests(CoachInterestViewModel civm)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == civm.ApplicationID)
            {
                Application currentApp = appRepo.GetApplicationByID(civm.ApplicationID);
                if (civm.newPickedAreaID != -1) currentApp.AppArea = areaRepo.GetAreaByID(civm.newPickedAreaID);
                if (civm.Gender != null) currentApp.AppGender = civm.Gender;
                if (civm.GradePreference != null) currentApp.AppGrade = civm.GradePreference;
                if (civm.ChildTeam != null) currentApp.NameOfChild = civm.ChildTeam;
                currentApp.IsHeadCoach = civm.IsHeadCoach;
                currentApp.IsAssistantCoach = civm.IsAssistantCoach;
                if (civm.newPickedSchoolID != -1) currentApp.AppSchool = schoolRepo.GetSchoolByID(civm.newPickedSchoolID);
                if (civm.newPickedSportID != -1) currentApp.AppSport = sportRepo.GetSportsByID(civm.newPickedSportID);
                currentApp.YearsExperience = civm.YearsExperience;
                appRepo.Update(currentApp);

                //Process all data that is in the view model. If anything is new or changed,
                //update the coaches current application.


                //Decide which direction is being taken.
                if (civm.Direction == "approve")
                {
                    return RedirectToAction("CoachPledge", new { AppID = civm.ApplicationID });
                }

                if (civm.Direction == "deny")
                {
                    User appuser = userRepo.GetUserByID(civm.ApplicationID);
                    return RedirectToAction("ApplicantDetails", new { ApplicantID = appuser.Id });
                }

                if (civm.Direction == "previous")
                    return RedirectToAction("CoachInfo", new { AppID = civm.ApplicationID });
                if (civm.Direction == "next")
                    return RedirectToAction("CoachPledge", new { AppID = civm.ApplicationID });

                //if all else fails, post back to the same page.
                return View(civm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }
        #endregion

        #region Application Coach Pledge
        [HttpGet]
        public async Task<IActionResult> CoachPledge(int AppID)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == AppID)
            {
                //Get the coaches current app
                Application currentApp = appRepo.GetApplicationByID(AppID);
                CoachPledgeViewModel cpvm = new CoachPledgeViewModel();

                //If any information exists, bind it to the view model.
                cpvm.ApplicationID = AppID;
                if (currentApp.PledgeName != null) cpvm.Name = currentApp.PledgeName;
                if (currentApp.PledgeInitials != null) cpvm.Initials = currentApp.PledgeInitials;
                if (currentApp.pledgeIsInAgreement != false) cpvm.IsInAgreement = currentApp.pledgeIsInAgreement;
                if (currentApp.PledgeSubmissionDate != new DateTime()) cpvm.PledgeSubmissionDate = currentApp.PledgeSubmissionDate;

                //Display the view.
                return View(cpvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> CoachPledge(CoachPledgeViewModel cpvm)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == cpvm.ApplicationID)
            {
                //Process all data that is in the view model. If anything is new or changed,
                //update the coaches current application.
                Application currentApp = appRepo.GetApplicationByID(cpvm.ApplicationID);
                if (cpvm.Name != null) currentApp.PledgeName = cpvm.Name;
                if (cpvm.Initials != null) currentApp.PledgeInitials = cpvm.Initials;
                if (cpvm.IsInAgreement != false) currentApp.pledgeIsInAgreement = cpvm.IsInAgreement;
                if (cpvm.PledgeSubmissionDate != new DateTime()) currentApp.PledgeSubmissionDate = cpvm.PledgeSubmissionDate;
                appRepo.Update(currentApp);

                //Decide which direction is being taken.
                if (cpvm.Direction == "approve")
                {
                    return RedirectToAction("ConcussionCourse", new { AppID = cpvm.ApplicationID });
                }

                if (cpvm.Direction == "deny")
                {
                    User appuser = userRepo.GetUserByID(cpvm.ApplicationID);
                    return RedirectToAction("ApplicantDetails", new { ApplicantID = appuser.Id });
                }
                if (cpvm.Direction == "previous")
                    return RedirectToAction("CoachInterests", new { AppID = cpvm.ApplicationID });
                if (cpvm.Direction == "next")
                    return RedirectToAction("ConcussionCourse", new { AppID = cpvm.ApplicationID });

                //if all else fails, post back to the same page.
                return View(cpvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }
        #endregion

        #region Application Concussion Course
        [HttpGet]
        public async Task<IActionResult> ConcussionCourse(int AppID)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == AppID)
            {
                //Get the coaches current app
                Application currentApp = appRepo.GetApplicationByID(AppID);
                ConcussionCourseViewModel ccvm = new ConcussionCourseViewModel();
                ccvm.ApplicationID = AppID;
                //If any information exists, bind it to the view model.
               // if (currentApp.ConcussionCourseSubmissionDate != new DateTime()) ccvm.ConcussionCourseSubmissionDate = currentApp.ConcussionCourseSubmissionDate;
                if (currentApp.NfhsPath != null) ccvm.NfhsPath = currentApp.NfhsPath; else ccvm.NfhsPath = "";
                if (currentApp.ConcussionCourseSubmissionDate != new DateTime()) ccvm.ConcussionCourseSubmissionDate = currentApp.ConcussionCourseSubmissionDate;

                //Display the view.
                return View(ccvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> ConcussionCourse(ConcussionCourseViewModel ccvm)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == ccvm.ApplicationID)
            {
                //Process all data that is in the view model. If anything is new or changed,
                //update the coaches current application.
                Application currentApp = appRepo.GetApplicationByID(ccvm.ApplicationID);
                if (ccvm.ConcussionCourseSubmissionDate != new DateTime()) currentApp.ConcussionCourseSubmissionDate = ccvm.ConcussionCourseSubmissionDate;
                if (currentApp.NfhsPath != null) { 
                    //get the image and put it in the view model.
                }

                try
                {
                    var uploads = Path.Combine(_environment.WebRootPath, "Images", "ConcussionCourse");
                    if (ccvm.File.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, ccvm.ApplicationID.ToString() + ".jpg"), FileMode.Create))
                        {
                            await ccvm.File.CopyToAsync(fileStream);
                            currentApp.NfhsPath = "..\\Images\\ConcussionCourse\\" + currentApp.ApplicationID.ToString() + ".jpg";
                        }
                    }
                }
                catch (Exception e)
                {
                    //Add model state error? validation?
                }
                appRepo.Update(currentApp);

                //Decide which direction is being taken.
                if (ccvm.Direction == "approve")
                {
                    if (currentApp.IsHeadCoach == true)
                        return RedirectToAction("PcaCourse", new { AppID = ccvm.ApplicationID });
                    else
                        return RedirectToAction("ID", new { AppID = ccvm.ApplicationID });
                }

                if (ccvm.Direction == "deny")
                {
                    User appuser = userRepo.GetUserByID(ccvm.ApplicationID);
                    return RedirectToAction("ApplicantDetails", new { ApplicantID = appuser.Id });
                }
                if (ccvm.Direction == "previous")
                    return RedirectToAction("CoachPledge", new { AppID = ccvm.ApplicationID });
                if (ccvm.Direction == "next")
                {
                    if (currentApp.IsHeadCoach == true)
                        return RedirectToAction("PcaCourse", new { AppID = ccvm.ApplicationID });
                    else
                        return RedirectToAction("ID", new { AppID = ccvm.ApplicationID });
                }

                //if all else fails, post back to the same page.
                return View(ccvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }
        #endregion

        #region Application Pca Course
        [HttpGet]
        public async Task<IActionResult> PcaCourse(int AppID)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == AppID)
            {
                //Get the coaches current app
                Application currentApp = appRepo.GetApplicationByID(AppID);
                PcaCourseViewModel pcvm = new PcaCourseViewModel();
                pcvm.ApplicationID = AppID;
                //If any information exists, bind it to the view model.
                if (currentApp.PcaPath != null) pcvm.PcaPath = currentApp.PcaPath; else pcvm.PcaPath = "";

                //Display the view.
                return View(pcvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> PcaCourse(PcaCourseViewModel pcvm)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == pcvm.ApplicationID)
            {
                Application currentApp = appRepo.GetApplicationByID(pcvm.ApplicationID);

                //Process all data that is in the view model. If anything is new or changed,
                //update the coaches current application.
                var uploads = Path.Combine(_environment.WebRootPath, "Images", "PCACourse");
                try
                {
                    if (pcvm.File.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, pcvm.ApplicationID.ToString() + ".jpg"), FileMode.Create))
                        {
                            await pcvm.File.CopyToAsync(fileStream);
                            currentApp.PcaPath = "..\\Images\\PcaCourse\\" + currentApp.ApplicationID.ToString() + ".jpg";
                        }
                    }
                }
                catch (Exception e)
                {

                }
            appRepo.Update(currentApp);

                //Decide which direction is being taken.
                if (pcvm.Direction == "approve")
                {
                    return RedirectToAction("ID", new { AppID = pcvm.ApplicationID });
                }

                if (pcvm.Direction == "deny")
                {
                    User appuser = userRepo.GetUserByID(pcvm.ApplicationID);
                    return RedirectToAction("ApplicantDetails", new { ApplicantID = appuser.Id });
                }
                if (pcvm.Direction == "previous")
                    return RedirectToAction("CoachPledge", new { AppID = pcvm.ApplicationID });
                if (pcvm.Direction == "next")
                    return RedirectToAction("ID", new { AppID = pcvm.ApplicationID });

                //if all else fails, post back to the same page.
                return View(pcvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }
        #endregion

        #region Application ID
        [HttpGet]
        public async Task<IActionResult> ID(int AppID)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == AppID)
            {
                //Get the coaches current app
                Application currentApp = appRepo.GetApplicationByID(AppID);
                IDViewModel idvm = new IDViewModel();
                idvm.ApplicationID = AppID;
                //If any information exists, bind it to the view model.
                if (currentApp.DlPath != null) idvm.IDPath = currentApp.DlPath; else idvm.IDPath = "";

                //Display the view.
                return View(idvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> ID(IDViewModel idvm)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == idvm.ApplicationID)
            {
                Application currentApp = appRepo.GetApplicationByID(idvm.ApplicationID);

                //Process all data that is in the view model. If anything is new or changed,
                //update the coaches current application.
                var uploads = Path.Combine(_environment.WebRootPath, "Images", "ID");
                try { 
                    if (idvm.File.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, idvm.ApplicationID.ToString() + ".jpg"), FileMode.Create))
                        {
                            await idvm.File.CopyToAsync(fileStream);
                            currentApp.DlPath = "..\\Images\\ID\\" + currentApp.ApplicationID.ToString() + ".jpg";
                        }
                    }
                }
                catch (Exception e)
                {

                }
                appRepo.Update(currentApp);


                //Decide which direction is being taken.
                if (idvm.Direction == "approve")
                {
                    return RedirectToAction("Badge", new { AppID = idvm.ApplicationID });
                }

                if (idvm.Direction == "deny")
                {
                    User appuser = userRepo.GetUserByID(idvm.ApplicationID);
                    return RedirectToAction("ApplicantDetails", new { ApplicantID = appuser.Id });
                }
                if (idvm.Direction == "previous")
                {
                    if (currentApp.IsHeadCoach == true)
                        return RedirectToAction("PcaCourse", new { AppID = idvm.ApplicationID });
                    else
                        return RedirectToAction("ConcussionCourse", new { AppID = idvm.ApplicationID });
                }
                if (idvm.Direction == "next")
                    return RedirectToAction("Badge", new { AppID = idvm.ApplicationID });

                //if all else fails, post back to the same page.
                return View(idvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }
        #endregion

        #region Application Badge
        [HttpGet]
        public async Task<IActionResult> Badge(int AppID)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == AppID)
            {
                //Get the coaches current app
                Application currentApp = appRepo.GetApplicationByID(AppID);
                BadgeViewModel bvm = new BadgeViewModel();
                bvm.ApplicationID = AppID;
                //If any information exists, bind it to the view model.
                if (currentApp.BadgePath != null) bvm.BadgePath = currentApp.BadgePath; else bvm.BadgePath = "";

                //Display the view.
                return View(bvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Badge(BadgeViewModel bvm)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user = userRepo.GetDetailedUser(user);
            }

            //This will probably need to include all of your past application id's as well. so users can view their own past apps.
            if (User.IsInRole("Admin") || User.IsInRole("SportsManager") || user.currentYearApp.ApplicationID == bvm.ApplicationID)
            {
                Application currentApp = appRepo.GetApplicationByID(bvm.ApplicationID);

                //Process all data that is in the view model. If anything is new or changed,
                //update the coaches current application.
                var uploads = Path.Combine(_environment.WebRootPath, "Images", "Badge");
                try
                {
                    if (bvm.File.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, bvm.ApplicationID.ToString() + ".jpg"), FileMode.Create))
                        {
                            await bvm.File.CopyToAsync(fileStream);
                            currentApp.BadgePath = "..\\Images\\Badge\\" + currentApp.ApplicationID.ToString() + ".jpg";
                        }
                    }
                } catch(Exception e)
                {

                }
                appRepo.Update(currentApp);


                //Decide which direction is being taken.
                if (bvm.Direction == "approve")
                {
                    User appuser = userRepo.GetUserByID(bvm.ApplicationID);
                    return RedirectToAction("ApplicantDetails", new { ApplicantID = appuser.Id });
                }

                if (bvm.Direction == "deny")
                {
                    User appuser = userRepo.GetUserByID(bvm.ApplicationID);
                    return RedirectToAction("ApplicantDetails", new { ApplicantID = appuser.Id });
                }
                if (bvm.Direction == "previous")
                    return RedirectToAction("ID", new { AppID = bvm.ApplicationID });
                //This is going to need to check if the user is an admin/sportsmanager since they have a different "index"
                if (bvm.Direction == "next")
                {
                    if (User.IsInRole("SportsManager") || User.IsInRole("Admin"))
                    {
                        User appuser = userRepo.GetUserByID(bvm.ApplicationID);
                        return RedirectToAction("ApplicantDetails", new { ApplicantID = appuser.Id });
                    }
                    return RedirectToAction("Index");
                }

                //if all else fails, post back to the same page.
                return View(bvm);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }
        #endregion
        

    }
}
