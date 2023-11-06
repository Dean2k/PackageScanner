using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageScanner.Core.Models
{
    public class HashList
    {
        public string KnownFileName { get; set; }

        public string FileHash1 { get; set; }

        public bool Malicious { get; set; }

        public DateTime DateChecked { get; set; }
    }
}
