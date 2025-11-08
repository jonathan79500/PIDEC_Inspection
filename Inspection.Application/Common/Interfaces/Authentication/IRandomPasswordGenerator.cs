using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Common.Interfaces.Authentication
{
    public interface IRandomPasswordGenerator
    {
        public string GenerateSecurePassword(int length=12);
    }
}
