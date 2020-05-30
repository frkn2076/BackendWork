using DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Model {
    public class SurveyViewModel : BaseModel {
        public List<Survey> surveys { get; set; }
    }
   
}
