using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendSide.RequestModel {
    public class ConfirmMailRequestModel {
         public string email { get; set; }
         public string key { get; set; }
    }
}
