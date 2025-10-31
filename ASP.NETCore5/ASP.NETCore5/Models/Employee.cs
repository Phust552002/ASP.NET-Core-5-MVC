using System;
using System.Collections.Generic;

#nullable disable

namespace ASP.NETCore5.Models
{
    public partial class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public DateTime Created { get; set; }

        public virtual EmployeePicture EmployeePicture { get; set; }
    }
}
