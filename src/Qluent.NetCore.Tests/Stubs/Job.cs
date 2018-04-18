using System;

namespace Qluent.NetCore.Tests.Stubs
{
    [Serializable]
    public class Job
    {
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Company { get; set; }

        public static Job Create()
        {
            return new Job()
            {
                JobTitle = "Developer",
                Department = "IT Dept.",
                Company = "ACME Corp."
            };
        }
    }

}