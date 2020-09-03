using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Administrator")]
  public class AdministratorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Department(string subject)
    {
      ViewData["subject"] = subject;
      return View();
    }

    public IActionResult Course(string subject, string num)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }

    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of all the courses in the given department.
    /// Each object in the array should have the following fields:
    /// "number" - The course number (as in 5530)
    /// "name" - The course name (as in "Database Systems")
    /// </summary>
    /// <param name="subject">The department subject abbreviation (as in "CS")</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetCourses(string subject)
    {
            var query = from c in db.Courses
                        where c.Subject == subject
                        select new { number = c.CNum, name = c.CName };
            return Json(query.ToArray());
    }


    


    /// <summary>
    /// Returns a JSON array of all the professors working in a given department.
    /// Each object in the array should have the following fields:
    /// "lname" - The professor's last name
    /// "fname" - The professor's first name
    /// "uid" - The professor's uid
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetProfessors(string subject)
    {
            var query = from p in db.Professors
                        where p.Subject == subject
                        join u in db.Users on p.UId equals u.UId
                        select new { lname = u.LastName, fname = u.FirstName, uid = p.UId };
            return Json(query.ToArray());
    }



    /// <summary>
    /// Creates a course.
    /// A course is uniquely identified by its number + the subject to which it belongs
    /// </summary>
    /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
    /// <param name="number">The course number</param>
    /// <param name="name">The course name</param>
    /// <returns>A JSON object containing {success = true/false}.
    /// false if the course already exists, true otherwise.</returns>
    public IActionResult CreateCourse(string subject, int number, string name)
    {
            var query = from co in db.Courses
                        where co.Subject == subject && co.CNum == number
                        select co;
            if (query.Count() >= 1)
                return Json(new { success = false });

            //uint catalogID = 0;
            //var query1 = from co in db.Courses
            //            select co.CatalogId;
            //while (query1.Contains(catalogID))
            //{
            //    catalogID++;
            //}
            Courses c = new Courses();
            c.Subject = subject;
            c.CNum = (uint)number;
            c.CName = name;
            //c.CatalogId = catalogID;
            db.Courses.Add(c);
            db.SaveChanges();

            return Json(new { success = true });
      

      //return Json(new { success = false });
    }



    /// <summary>
    /// Creates a class offering of a given course.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="number">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="start">The start time</param>
    /// <param name="end">The end time</param>
    /// <param name="location">The location</param>
    /// <param name="instructor">The uid of the professor</param>
    /// <returns>A JSON object containing {success = true/false}. 
    /// false if another class occupies the same location during any time 
    /// within the start-end range in the same semester, or if there is already
    /// a Class offering of the same Course in the same Semester,
    /// true otherwise.</returns>
    public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
    {
            var query = from c in db.Courses
                        join cl in db.Classes on c.CatalogId equals cl.CatalogId
                        where cl.Season == season && cl.Year == year
                        select new { subject = c.Subject, num = c.CNum, loc = cl.Location, start = cl.Start, end = cl.End };
            foreach(var x in query)
            {
                if(x.subject.Equals(subject) && x.num == number)
                {
                    return Json(new { success = false });
                }
                else if (x.loc == location)
                {
                    if(DateTime.Compare(x.start, end) < 0 || DateTime.Compare(start, x.end) < 0)
                    {
                        return Json(new { success = false });
                    }
                }
            }

            var query2 = from c in db.Courses
                         where c.Subject == subject && c.CNum == number
                         select c.CatalogId;
            Classes cla = new Classes();
            cla.CatalogId = query2.ToArray()[0];
            cla.Season = season;
            cla.Year = (uint)year;
            cla.Start = start;
            cla.End = end;
            cla.Location = location;
            cla.UId = instructor;
            db.Classes.Add(cla);
            db.SaveChanges();

            return Json(new { success = true });
      
      //return Json(new { success = false });
    }


    /*******End code to modify********/

  }
}