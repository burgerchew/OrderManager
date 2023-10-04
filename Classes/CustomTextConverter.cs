using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Classes
{
    public class CustomTextConverter
    {
        public static string Convert(string filePath)
        {
            return File.Exists(filePath) ? "File exists" : "File does not exist";
        }
    }

}
