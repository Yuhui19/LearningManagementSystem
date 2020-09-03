using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Students(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
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

    public IActionResult Categories(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
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

    public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      ViewData["uid"] = uid;
      return View();
    }

    /*******Begin code to modify********/


    /// <summary>
    /// Returns a JSON array of all the students in a class.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "dob" - date of birth
    /// "grade" - the student's grade in this class
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
    {
            var query = from c in db.Courses
                        join cl in db.Classes on c.CatalogId equals cl.CatalogId
                        join e in db.Enrollment on cl.ClassId equals e.ClassId
                        join u in db.Users on e.UId equals u.UId
                        where c.Subject == subject && c.CNum == num && cl.Season == season && cl.Year == year
                        select new { fname = u.FirstName, lname = u.LastName, uid = u.UId, dob = u.Dob, grade = e.Grade };
            return Json(query.ToArray());
      
      //return Json(null);
    }



    /// <summary>
    /// Returns a JSON array with all the assignments in an assignment category for a class.
    /// If the "category" parameter is null, return all assignments in the class.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The assignment category name.
    /// "due" - The due DateTime
    /// "submissions" - The number of submissions to the assignment
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class, 
    /// or null to return assignments from all categories</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
    {
            var query = from c in db.Courses
                        join cl in db.Classes on c.CatalogId equals cl.CatalogId
                        join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.AcId equals a.AcId
                        where c.Subject == subject && c.CNum == num && cl.Season == season && cl.Year == year
                        select new { ac, a };
            if (category == null)
            {
                var query1 = from q in query
                             select new
                             {
                                 aname = q.a.AName,
                                 cname = q.ac.AcName,
                                 due = q.a.Due,
                                 submissions = (from s in db.Submission
                                                where s.AId == q.a.AId
                                                select s).Count()
                             };
                return Json(query1.ToArray());
            }
            else
            {
                var query1 = from q in query
                             where q.ac.AcName == category
                             select new
                             {
                                 aname = q.a.AName,
                                 cname = q.ac.AcName,
                                 due = q.a.Due,
                                 submissions = (from s in db.Submission
                                                where s.AId == q.a.AId
                                                select s).Count()
                             };
                return Json(query1.ToArray());
            }
            
      //return Json(null);
    }


    /// <summary>
    /// Returns a JSON array of the assignment categories for a certain class.
    /// Each object in the array should have the folling fields:
    /// "name" - The category name
    /// "weight" - The category weight
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
    {
            var query = from c in db.Courses
                        join cl in db.Classes on c.CatalogId equals cl.CatalogId
                        join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                        where c.Subject == subject && c.CNum == num && cl.Season == season && cl.Year == year
                        select new { name = ac.AcName, weight = ac.GradingWeight };
            return Json(query.ToArray());

      //return Json(null);
    }

    /// <summary>
    /// Creates a new assignment category for the specified class.
    /// If a category of the given class with the given name already exists, return success = false.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The new category name</param>
    /// <param name="catweight">The new category weight</param>
    /// <returns>A JSON object containing {success = true/false} </returns>
    public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
    {
            var query = from co in db.Courses
                        join cl in db.Classes on co.CatalogId equals cl.CatalogId
                        where co.Subject == subject && co.CNum == num && cl.Season == season && cl.Year == year
                        select cl;
            uint classID = query.ToArray()[0].ClassId;

            var query1 = from ac in db.AssignmentCategories
                         where ac.ClassId == classID && ac.AcName == category
                         select ac;
            if (query1.Count() > 0)
            {
                return Json(new { success = false });
            }

            AssignmentCategories acg = new AssignmentCategories();
            acg.ClassId = classID;
            acg.GradingWeight = (uint)catweight;
            acg.AcName = category;
            db.AssignmentCategories.Add(acg);
            db.SaveChanges();
            return Json(new { success = true });

      //return Json(new { success = false });
    }

    /// <summary>
    /// Creates a new assignment for the given class and category.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="asgpoints">The max point value for the new assignment</param>
    /// <param name="asgdue">The due DateTime for the new assignment</param>
    /// <param name="asgcontents">The contents of the new assignment</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
    {

            var query = from co in db.Courses
                        join cl in db.Classes on co.CatalogId equals cl.CatalogId
                        where co.Subject == subject && co.CNum == num && cl.Season == season && cl.Year == year
                        select cl.ClassId;
            var classID = query.ToArray()[0];

            var allCategoryID = (from a in db.Assignments
                                 select a.AId).ToArray();
            var query1 = from ac in db.AssignmentCategories
                         where ac.ClassId == classID && allCategoryID.Contains(ac.AcId)
                         select new { ac };
            uint totalWeight = 0;
            foreach (var i in query1)
            {
                totalWeight += i.ac.GradingWeight;
            }

            var quer = from ac in db.AssignmentCategories
                         where ac.ClassId == classID
                         select new { ac };
            var qury = from q in quer
                       where q.ac.AcName == category
                       select q.ac.AcId;
            var qury1 = from a in db.Assignments
                        where a.AcId == qury.ToArray()[0] && a.AName == asgname
                        select a;
            if (qury1.Count() > 0)
            {
                return Json(new { success = false });
            }
            Assignments a1 = new Assignments();
            a1.AcId = qury.ToArray()[0];
            a1.AName = asgname;
            a1.MaxPoint = asgpoints;
            a1.Due = asgdue;
            a1.Contents = asgcontents;
            db.Assignments.Add(a1);
            db.SaveChanges();

            var quey = from e in db.Enrollment
                       where e.ClassId == classID
                       select e.UId;
            foreach (var uid in quey.ToArray())
            {
                var query3 = from q in query1
                             join a in db.Assignments on q.ac.AcId equals a.AcId
                             select new { q, a };
                var query4 = from q in query3
                             join s in db.Submission on q.a.AId equals s.AId into join1
                             from j1 in join1.DefaultIfEmpty()
                             where j1.UId == uid || j1.UId == null
                             select new
                             {
                                 category = q.q.ac.AcName,
                                 weight = q.q.ac.GradingWeight,
                                 asgname = q.a.AName,
                                 score = j1.Score == null ? 0 : j1.Score,
                                 total = (from s1 in query3
                                          where s1.q.ac.AcName == q.q.ac.AcName
                                          select s1.a.MaxPoint).Sum()
                             };
                double sum = 0.0;
                foreach (var q in query4.ToArray())
                {
                    sum += (double)q.weight * (double)q.score / (double)q.total;
                }
                sum = sum * 100 / (double)totalWeight;

                var query5 = from e in db.Enrollment
                             where e.UId == uid && e.ClassId == classID
                             select e;
                query5.ToArray()[0].Grade = ScoreToGrade(sum);
            }

            db.SaveChanges();
            return Json(new { success = true });
    }


    /// <summary>
    /// Gets a JSON array of all the submissions to a certain assignment.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "time" - DateTime of the submission
    /// "score" - The score given to the submission
    /// 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
    {
            var query = from c in db.Courses
                        join cl in db.Classes on c.CatalogId equals cl.CatalogId
                        join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.AcId equals a.AcId
                        where c.Subject == subject && c.CNum == num && cl.Season == season && cl.Year == year && ac.AcName == category && a.AName == asgname
                        select a;
            uint assignmentID = query.ToArray()[0].AId;
            var query1 = from su in db.Submission
                         join s in db.Students on su.UId equals s.UId
                         join u in db.Users on s.UId equals u.UId
                         where su.AId == assignmentID
                         select new { fname = u.FirstName, lname = u.LastName, uid = u.UId, time = su.Time, score = su.Score };
            return Json(query1.ToArray());
     
      //return Json(null);
    }


    /// <summary>
    /// Set the score of an assignment submission
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <param name="uid">The uid of the student who's submission is being graded</param>
    /// <param name="score">The new score for the submission</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
    {

            var query = from co in db.Courses
                        join cl in db.Classes on co.CatalogId equals cl.CatalogId
                        where co.Subject == subject && co.CNum == num && cl.Season == season && cl.Year == year
                        select cl.ClassId;
            var classID = query.ToArray()[0];

            var allCategoryID = (from a in db.Assignments
                                 select a.AcId).ToArray();
            var query1 = from ac in db.AssignmentCategories
                         where ac.ClassId == classID && allCategoryID.Contains(ac.AcId)
                         select new { ac };

            uint totalWeight = 0;
            foreach (var i in query1)
            {
                totalWeight += i.ac.GradingWeight;
            }

            var query2 = from q in query1
                         join a in db.Assignments on q.ac.AcId equals a.AcId
                         join s in db.Submission on a.AId equals s.AId
                         where q.ac.AcName == category && a.AName == asgname && s.UId == uid
                         select s;
            query2.ToArray()[0].Score = score;
            db.SaveChanges();

            var query3 = from q in query1
                         join a in db.Assignments on q.ac.AcId equals a.AcId
                         select new { q, a };

            var query4 = from q in query3
                         join s in db.Submission on q.a.AId equals s.AId into join1
                         from j1 in join1.DefaultIfEmpty()
                         where j1.UId == uid || j1.UId == null
                         select new
                         {
                             category = q.q.ac.AcName,
                             weight = q.q.ac.GradingWeight,
                             asgname = q.a.AName,
                             score = j1.Score == null ? 0 : j1.Score,
                             total = (from s1 in query3
                                      where s1.q.ac.AcName == q.q.ac.AcName
                                      select s1.a.MaxPoint).Sum()
                         };
            double sum = 0.0;
            foreach (var q in query4.ToArray())
            {
                sum += q.weight * (double)q.score / q.total;
            }
            sum = sum * 100 / totalWeight;

            var query5 = from e in db.Enrollment
                         where e.UId == uid && e.ClassId == classID
                         select e;
            query5.ToArray()[0].Grade = ScoreToGrade(sum);

            db.SaveChanges();
            return Json(new { success = true });
    }
        private string ScoreToGrade(double score)
        {
            if (score >= 93)
                return "A";
            else if (score >= 90)
                return "A-";
            else if (score >= 87)
                return "B+";
            else if (score >= 83)
                return "B";
            else if (score >= 80)
                return "B-";
            else if (score >= 77)
                return "C+";
            else if (score >= 73)
                return "C";
            else if (score >= 70)
                return "C-";
            else if (score >= 67)
                return "D+";
            else if (score >= 63)
                return "D";
            else if (score >= 60)
                return "D-";
            else
                return "E";
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
    {
            var query = from cl in db.Classes
                        join c in db.Courses on cl.CatalogId equals c.CatalogId
                        where cl.UId == uid
                        select new { subject = c.Subject, number = c.CNum, name = c.CName, season = cl.Season, year = cl.Year };
            return Json(query.ToArray());

      //return Json(null);
    }


    /*******End code to modify********/

  }
}