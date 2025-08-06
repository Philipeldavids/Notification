using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.UtilityHelpers
{
    public class GetTemplate
    {
        public static string GetEmailTemplate(string templateName)
        {
            var baseDir = Directory.GetCurrentDirectory();
            string folderName = "/StaticFiles/";
            var path = Path.Combine(baseDir + folderName, templateName);
            return File.ReadAllText(path);
        }
    }
}
