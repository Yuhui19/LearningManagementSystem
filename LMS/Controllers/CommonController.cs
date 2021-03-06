﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
  public class CommonController : Controller
  {

    /*******Begin code to modify********/

    // TODO: Uncomment and change 'X' after you have scaffoled

    
    protected Team14LMSContext db;

    public CommonController()
    {
      db = new Team14LMSContext();
    }
    

    /*
     * WARNING: This is the quick and easy way to make the controller
     *          use a different LibraryContext - good enough for our purposes.
     *          The "right" way is through Dependency Injection via the constructor 
     *          (look this up if interested).
    */

    // TODO: Uncomment and change 'X' after you have scaffoled
    
    public void UseLMSContext(Team14LMSContext ctx)
    {
      db = ctx;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
    



    /// <summary>
    /// Retreive a JSON array of all departments from the database.
    /// Each object in the array should have a field called "name" and "subject",
    /// where "name" is the department name and "subject" is the subject abbreviation.
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetDepartments()
    {
            // TODO: Do not return this hard-coded array.
            
            var query = from d in db.Departments
                        select new { name = d.DName, subject = d.Subject };
            return Json(query.ToArray());
                //return Json(new[] { new { name = "None", subject = "NONE" } });
    }



    /// <summary>
    /// Returns a JSON array representing the course catalog.
    /// Each object in the array should have the following fields:
    /// "subject": The subject abbreviation, (e.g. "CS")
    /// "dname": The department name, as in "Computer Science"
    /// "courses": An array of JSON objects representing the courses in the department.
    ///            Each field in this inner-array should have the following fields:
    ///            "number": The course number (e.g. 5530)
    ///            "cname": The course name (e.g. "Database Systems")
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetCatalog()
    {
            var query = from d in db.Departments
                        select new
                        {
                            subject = d.Subject,
                            dname = d.DName,
                            courses = from c in db.Courses
                                        where d.Subject == c.Subject
                                        select new { number = c.CNum, cname = c.CName }
                        };
            return Json(query.ToArray());
                //return Json(null);
    }

    /// <summary>
    /// Returns a JSON array of all class offerings of a specific course.
    /// Each object in the array should have the following fields:
    /// "season": the season part of the semester, such as "Fall"
    /// "year": the year part of the semester
    /// "location": the location of the class
    /// "start": the start time in format "hh:mm:ss"
    /// "end": the end time in format "hh:mm:ss"
    /// "fname": the first name of the professor
    /// "lname": the last name of the professor
    /// </summary>
    /// <param name="subject">The subject abbreviation, as in "CS"</param>
    /// <param name="number">The course number, as in 5530</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetClassOfferings(string subject, int number)
    {
            var query = from co in db.Courses
                        where co.Subject == subject && co.CNum == number
                        join cl in db.Classes on co.CatalogId equals cl.CatalogId into join1
                        from j1 in join1.DefaultIfEmpty()
                        join u in db.Users on j1.UId equals u.UId into join2

                        from j2 in join2
                        select new { season = j1.Season, year = j1.Year, location = j1.Location, start = j1.Start, end = j1.End, fname = j2.FirstName, lname = j2.LastName };
            return Json(query.ToArray());
                //return Json(null);
    }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <returns>The assignment contents</returns>
    public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
    {
            var query = from co in db.Courses
                        where co.Subject == subject && co.CNum == num
                        join cl in db.Classes on co.CatalogId equals cl.CatalogId into join1
                        from j1 in join1.DefaultIfEmpty()
                        where j1.Season == season && j1.Year == year
                        join ac in db.AssignmentCategories on j1.ClassId equals ac.ClassId into join2
                        from j2 in join2.DefaultIfEmpty()
                        where j2.AcName == category
                        join a in db.Assignments on j2.AcId equals a.AcId into join3

                        from j3 in join3.DefaultIfEmpty()
                        where j3.AName == asgname
                        select j3;
            //return query == null ? null : Content(query.ToString());
            return query.Count() == 0 ? null : Content(query.ToArray()[0].Contents);

                //return Content("");
    }


    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment submission.
    /// Returns the empty string ("") if there is no submission.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <param name="uid">The uid of the student who submitted it</param>
    /// <returns>The submission text</returns>
    public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
    {
            var query = from co in db.Courses
                        where co.Subject == subject && co.CNum == num
                        join cl in db.Classes on co.CatalogId equals cl.CatalogId into join1
                        from j1 in join1.DefaultIfEmpty()
                        where j1.Season == season && j1.Year == year
                        join ac in db.AssignmentCategories on j1.ClassId equals ac.ClassId into join2
                        from j2 in join2.DefaultIfEmpty()
                        where j2.AcName == category
                        join a in db.Assignments on j2.AcId equals a.AcId into join3
                        from j3 in join3.DefaultIfEmpty()
                        where j3.AName == asgname
                        join s in db.Submission on j3.AId equals s.AId into join4

                        from j4 in join4.DefaultIfEmpty()
                        where j4.UId == uid
                        select j4;
            //return query == null ? Content("") : Content(query.ToString());
            return query.Count() == 0 ? Content("") : Content(query.ToArray()[0].Solution);
            //return query.Count() == 0 ? Content("") : Content(query.First().Solution);
                //return Content("");
    }


    /// <summary>
    /// Gets information about a user as a single JSON object.
    /// The object should have the following fields:
    /// "fname": the user's first name
    /// "lname": the user's last name
    /// "uid": the user's uid
    /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
    ///               If the user is a Professor, this is the department they work in.
    ///               If the user is a Student, this is the department they major in.    
    ///               If the user is an Administrator, this field is not present in the returned JSON
    /// </summary>
    /// <param name="uid">The ID of the user</param>
    /// <returns>
    /// The user JSON object 
    /// or an object containing {success: false} if the user doesn't exist
    /// </returns>
    public IActionResult GetUser(string uid)
    {
            var query1 = from u in db.Users
                         where u.UId == uid
                         join p in db.Professors on u.UId equals p.UId
                         join d in db.Departments on p.Subject equals d.Subject
                         select new { fname = u.FirstName, lname = u.LastName, uid, department = d.DName };
            if (query1.Count() != 0)
            {
                return Json(query1.ToArray()[0]);
            }

            var query2 = from u in db.Users
                             //where u.UId == uid
                         join s in db.Students on u.UId equals s.UId
                         join d in db.Departments on s.Subject equals d.Subject
                         where u.UId == uid
                         select new { fname = u.FirstName, lname = u.LastName, uid, department = d.DName };
            if (query2.Count() != 0)
            {
                return Json(query2.ToArray()[0]);
            }

            var query3 = from u in db.Users
                         where u.UId == uid
                         join a in db.Administrators on u.UId equals a.UId
                         select new { fname = u.FirstName, lname = u.LastName, uid };
            if (query3.Count() != 0)
            {
                return Json(query3.ToArray()[0]);
            }

            return Json(new { success = false });

            //return Json(new { success = false });
        }


        /*******End code to modify********/

    }
}