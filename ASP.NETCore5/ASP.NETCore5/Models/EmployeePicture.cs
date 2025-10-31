using System;
using System.Collections.Generic;

#nullable disable

namespace ASP.NETCore5.Models
{
    public partial class EmployeePicture
    {
        public int EmpId { get; set; }
        public string ImageInfo { get; set; }
        public string ImageType { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime Created { get; set; }

        public virtual Employee Emp { get; set; }
    }
}
