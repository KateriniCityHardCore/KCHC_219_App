using KCHC.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace KCHC.Models
{
    public class MemberRole
    {
        public Person Member { get; set; }
        public Role Role { get; set; }
    }
}
