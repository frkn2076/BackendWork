using BackendSide.RequestModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendSide.ExceptionHandling {
    public class ExceptionHandler : ActionFilterAttribute {
        public ServiceEnum serviceEnum;
        //2 after
        public override void OnActionExecuted(ActionExecutedContext context) {
            //do your staff here
        }
        //1 before
        public override void OnActionExecuting(ActionExecutingContext context) {
            //şimdilik servis bazlı yapıyorum
            switch(serviceEnum) {
                case ServiceEnum.register:
                    var model = context.ActionArguments["model"] as LoginRequestModel;
                    //if(string.IsNullOrWhiteSpace(model.email) || string.IsNullOrWhiteSpace(model.password))
                        //return ü hallet
                    break;
                default:
                    break;
            }
        }
        public override void OnResultExecuted(ResultExecutedContext context) {
            //do your staff here
        }
        public override void OnResultExecuting(ResultExecutingContext context) {
            //do your staff here
        }
    }

    public enum ServiceEnum {
        register = 0
    }
}
