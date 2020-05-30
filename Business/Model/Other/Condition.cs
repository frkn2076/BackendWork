using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Model.Other {
    internal class Condition {
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public List<string> City { get; set; }
        public List<string> Job { get; set; }
    }
}
