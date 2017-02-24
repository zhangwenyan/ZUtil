using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eweb.info
{
    public class PageInfo<T>
    {
        private int _page;
        public int page { get { if (_page < 1) return 1; return _page; } set { _page = value; } }
        public int rows { get; set; }
        public int total { get; set; }

        public String timeStr { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public T query { get; set; }

    }
}
