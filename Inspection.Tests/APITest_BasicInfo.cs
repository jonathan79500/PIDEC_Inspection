using Inspection.Domain.Entities.Orders;
using Inspection.Infrastructure.EDMS;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Inspection.Tests
{
    public class APITest_BasicInfo
    {
        [Fact]
        public void TestBasic_ReturnOk()
        {
            var edmsMock = new Mock<EMSService>();
            //edmsMock.Setup(service => service.GetProjectOrders())
            // .ReturnsAsync(new List<Order>());
            
                
        }
    }
}
