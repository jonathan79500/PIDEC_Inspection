using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class BasicUserInfo : IBasicUserInfo
    {
        public string Name { get ; set ; }
        public string UserName { get; set; }
        public int DomainId { get; set; }
        public List<Project> projects { get ; set; }
        public List<Order> orders { get; set; }
    }
}
