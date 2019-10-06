using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZermeloFunction.Models
{
    public class ZermeloResult
    {
        public TheResponse response { get; set; }
    }

    public class TheResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public int eventId { get; set; }
        public int startRow { get; set; }
        public int endRow { get; set; }
        public int totalRows { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public int startTimeSlot { get; set; }
        public int endTimeSlot { get; set; }
        public string startTimeSlotName { get; set; }
        public string endTimeSlotName { get; set; }
        public bool optional { get; set; }
        public bool teacherChanged { get; set; }
        public bool groupChanged { get; set; }
        public bool locationChanged { get; set; }
        public bool timeChanged { get; set; }
        public bool valid { get; set; }
        public bool cancelled { get; set; }
        public bool modified { get; set; }
        public bool moved { get; set; }
        public bool hidden { get; set; }
        public bool _new { get; set; }
        public object content { get; set; }
        public string schedulerRemark { get; set; }
        public string remark { get; set; }
        public string changeDescription { get; set; }
        public string branch { get; set; }
        public int branchOfSchool { get; set; }
        public int created { get; set; }
        public int appointmentInstance { get; set; }
        public string type { get; set; }
        public int lastModified { get; set; }
        public int appointmentLastModified { get; set; }
        public string[] subjects { get; set; }
        public string[] teachers { get; set; }
        public int[] groupsInDepartments { get; set; }
        public string[] groups { get; set; }
        public int[] locationsOfBranch { get; set; }
        public string[] locations { get; set; }
    }
}
