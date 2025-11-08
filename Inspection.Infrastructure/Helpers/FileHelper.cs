using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Infrastructure.Helpers
{
    public class FileHelper
    {
        public static string generateFileName(string fileName)
        { 
        var name = Guid.NewGuid().ToString().Replace("-","");
            var lastIndex = fileName.LastIndexOf('.');
            var ext = fileName.Substring(lastIndex);
            return name + ext;
        }
    }
}
