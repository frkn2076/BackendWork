using DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Model {
    public class JobViewModel : BaseModel {
        public List<Jobs> Jobs { get; set; }
    }
}
