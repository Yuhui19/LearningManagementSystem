using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enrollment = new HashSet<Enrollment>();
        }

        public uint ClassId { get; set; }
        public uint CatalogId { get; set; }
        public uint Year { get; set; }
        public string Season { get; set; }
        public string Semester { get; set; }
        public string Location { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string UId { get; set; }

        public virtual Courses Catalog { get; set; }
        public virtual Professors U { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
    }
}
