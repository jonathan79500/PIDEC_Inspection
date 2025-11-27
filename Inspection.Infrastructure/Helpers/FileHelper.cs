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
            string ext = "";
        var name = Guid.NewGuid().ToString().Replace("-","");
            var lastIndex = fileName.LastIndexOf('.');
            if (lastIndex > 0)
                ext = fileName.Substring(lastIndex);
            else
                ext = fileName;
            return name + ext;
        }
    }
}
