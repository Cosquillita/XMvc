using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;

namespace XScaffolding.Helpers
{
    public class TextFileUtilities
    {
        public string ReadTextFile(string fileName)
        {
            string result = null;
            result = File.ReadAllText(fileName);
            return result;
        }

        public bool WriteTextFile(string fileName, string fileContent)
        {
            bool result = true;
            try
            {
                using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(fileName))
                {
                    outfile.Write(fileContent);
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}