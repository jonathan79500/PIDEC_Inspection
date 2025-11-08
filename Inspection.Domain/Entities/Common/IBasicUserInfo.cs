using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public interface IBasicUserInfo
    {
         string Name { get; set; }
        string UserName { get; set; } 
        int DomainId { get; set; }
        List<Project> projects { get; set; }  
        List<Order> orders { get; set; }
    }
}
