﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KidSports.Models.ViewModels
{
    public class CoachInterestViewModel
    {
        public int ApplicationID { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public bool IsHeadCoach { get; set; }
        public bool IsAssistantCoach { get; set; }
        public Sport Sport { get; set; }
       
        public Area Area { get; set; }
        public School School { get; set; }

        public string Gender { get; set; }
        public List<int> GradePreference{ get; set; }
        public string ChildTeam { get; set; }
        public List<PreviousYearsCoached> PreviousYearsCoached { get; set; }
        public List<PreviousGradesCoached> PreviousGradesCoached { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public List<Area> AllAreas { get; set; }
        public List<School> SchoolsByArea { get; set; }
        public List<Sport> AllSports { get; set; }
        public List<PreviousYearsCoached> AllYears { get; set; }
        public List<PreviousGradesCoached> AllGrades { get; set; }
        public int YearsExperience { get; set; }









    }
}
