using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Catalog()
    {
      return View();
    }

    public IActionResult Class(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }


    public IActionResult ClassListings(string subject, string num)
    {
      System.Diagnostics.Debug.WriteLine(subject + num);
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }


    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of the classes the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester
    /// "year" - The year part of the semester
    /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
            var query = from e in db.Enrollment
                        join cl in db.Classes on e.ClassId equals cl.ClassId
                        join c in db.Courses on cl.CatalogId equals c.CatalogId
                        where e.UId == uid
                        select new { subject = c.Subject, number = c.CNum, name = c.CName, season = cl.Season, year = cl.Year, grade = e == null ? "--" : e.Grade };
            return Json(query.ToArray());
   
      //return Json(null);
    }

    /// <summary>
    /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The category name that the assignment belongs to
    /// "due" - The due Date/Time
    /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="uid"></param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
    {
            var query1 = from co in db.Courses
                         join cl in db.Classes on co.CatalogId equals cl.CatalogId
                         where co.Subject == subject && co.CNum == num && cl.Season == season && cl.Year == year
                         join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                         join a in db.Assignments on ac.AcId equals a.AcId
                         select a;

            var query2 = from q in query1
                         join s in db.Submission
                         on new { A = q.AId, B = uid } equals new { A = s.AId, B = s.UId }
                         into join1
                         from j1 in join1.DefaultIfEmpty()
                         select new { aname = q.AName, cname = q.Ac.AcName, due = q.Due, score = j1 == null ? null : j1.Score };
            return Json(query2.ToArray());

            

      //return Json(null);
    }



    /// <summary>
    /// Adds a submission to the given assignment for the given student
    /// The submission should use the current time as its DateTime
    /// You can get the current time with DateTime.Now
    /// The score of the submission should start as 0 until a Professor grades it
    /// If a Student submits to an assignment again, it should replace the submission contents
    /// and the submission time (the score should remain the same).
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="uid">The student submitting the assignment</param>
    /// <param name="contents">The text contents of the student's submission</param>
    /// <returns>A JSON object containing {success = true/false}</returns>
    public IActionResult SubmitAssignmentText(string subject, int num, string season, int year, 
      string category, string asgname, string uid, string contents)
    {
            var query = from c in db.Courses
                        join cl in db.Classes on c.CatalogId equals cl.CatalogId
                        join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.AcId equals a.AcId
                        where c.Subject == subject && c.CNum == num && cl.Season == season && cl.Year == year && ac.AcName == category && a.AName == asgname
                        select a;

            if (query.Count() == 0)
            {
                return Json(new { success = false });
            }
            uint asID = query.ToArray()[0].AId;

            var query1 = from s in db.Submission
                         where s.AId == asID && uid == s.UId
                         select s;
            if (query1.Count() == 1)
            {
                query1.ToArray()[0].Time = DateTime.Now;
                query1.ToArray()[0].Solution = contents;
                query1.ToArray()[0].Score = 0;
            }
            else
            {
                Submission sub = new Submission();
                sub.UId = uid;
                sub.AId = asID;
                sub.Time = DateTime.Now;
                sub.Solution = contents;
                sub.Score = 0;
                db.Submission.Add(sub);
            }


            db.SaveChanges();

            return Json(new { success = true });
     
      //return Json(new { success = false });
    }

    
    /// <summary>
    /// Enrolls a student in a class.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing {success = {true/false}. 
    /// false if the student is already enrolled in the class, true otherwise.</returns>
    public IActionResult Enroll(string subject, int num, string season, int year, string uid)
    {
            var query = from e in db.Enrollment
                        join cl in db.Classes on e.ClassId equals cl.ClassId
                        join c in db.Courses on cl.CatalogId equals c.CatalogId
                        where c.Subject == subject && c.CNum == num && cl.Season == season && cl.Year == year
                        select e;
            foreach(var x in query)
            {
                if (x.UId == uid)
                {
                    return Json(new { success = false });
                }
            }

            Enrollment en = new Enrollment();
            en.UId = uid;
            en.ClassId = query.ToArray()[0].ClassId;
            en.Grade = "--"; //Or null
            db.Enrollment.Add(en);
            db.SaveChanges();

            return Json(new { success = true });

      //return Json(new { success = false });
    }



    /// <summary>
    /// Calculates a student's GPA
    /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
    /// Assume all classes are 4 credit hours.
    /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
    /// If a student is not enrolled in any classes, they have a GPA of 0.0.
    /// Otherwise, the point-value of a letter grade is determined by the table on this page:
    /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
    public IActionResult GetGPA(string uid)
    {
            var query = from e in db.Enrollment
                        where e.UId == uid
                        select e;
            double gpa = 0.0;
            int num = query.Count();
            if (num == 0)
            {
                return Json(new { gpa });
            }
            
            foreach(var x in query)
            {
                switch (x.Grade)
                {
                    case "A":
                        gpa += 4.0;
                        break;
                    case "A-":
                        gpa += 3.7;
                        break;
                    case "B+":
                        gpa += 3.3;
                        break;
                    case "B":
                        gpa += 3.0;
                        break;
                    case "B-":
                        gpa += 2.7;
                        break;
                    case "C+":
                        gpa += 2.3;
                        break;
                    case "C":
                        gpa += 2.0;
                        break;
                    case "C-":
                        gpa += 1.7;
                        break;
                    case "D+":
                        gpa += 1.3;
                        break;
                    case "D":
                        gpa += 1.0;
                        break;
                    case "D-":
                        gpa += 0.7;
                        break;
                    case "E":
                        gpa += 0.0;
                        break;
                    case "--":
                        num--;
                        break;
                }
            }
            gpa = gpa / num;
            return Json(new { gpa });

      //return Json(null);
    }

    /*******End code to modify********/

  }
}