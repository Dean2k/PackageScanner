using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageScanner.Core.Models
{
    public class FileStats
    {
        public int SafeFiles { get; set; }
        public int UnknownFiles { get; set; }
        public int BadFiles { get; set; }
        public int UrlDelete { get; set; }
        public int NoDelete { get; set; }
        public int OtherDelete { get; set; }
    }
}
