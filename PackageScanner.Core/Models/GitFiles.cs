using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageScanner.Core.Models
{
    public class GitHub_content
    {
        public string name { get; set; }
        public string path { get; set; }
        public string sha { get; set; }
        public long size { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string git_url { get; set; }
        public string download_url { get; set; }
        public string type { get; set; }
        public GitHub_content_links _links { get; set; }

    }
    public class GitHub_content_links
    {
        public string self { get; set; }
        public string git { get; set; }
        public string html { get; set; }
    }
}
