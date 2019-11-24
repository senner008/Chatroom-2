using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace app {
    public static class Helper 
    {
        public static double ToMiliseconds (DateTime date) 
        {
            var mili =  date.ToUniversalTime ().Subtract (new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return Math.Round(mili,2);
        }

        public static string GuidToBigInt (Guid guid) 
        {
           return (new BigInteger(guid.ToByteArray())).ToString();   
        }
    }

    public class Benchmark : IDisposable 
    {
        private readonly Stopwatch timer = new Stopwatch();
        private readonly string benchmarkName;

        public Benchmark(string benchmarkName)
        {
            this.benchmarkName = benchmarkName;
            timer.Start();
        }

        public void Dispose() 
        {
            timer.Stop();
            Console.WriteLine($"{benchmarkName} {timer.ElapsedMilliseconds}");
        }
    }

    public class MyChatHubException : Exception
    {
        public MyChatHubException()
        {
        }

        public MyChatHubException(string message)
            : base(message)
        {
        }

        public MyChatHubException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class EnsureListMinimumOne : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            System.Console.WriteLine("dfsfsdfdsfdsfdsfdsfdsfdsfs");
            var list = value as IList;
            if (list != null)
            {
                if (list.Count < 1) return false;
            }
            return true;
        }
    }

    public class EnsureMaxTenUsers : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                if (list.Count > 10) return false;
            }
            return true;
        }
    }

    public class EnsureUserNamesAreStringsAndMaxLength30 : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IList;
            foreach( var user in list) {
                if (!(user is string)) return false;
                if (Convert.ToString(user).Length > 30) return false;
            }
            return true;
        }
    }

      public class HTMLSanitizerError : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            HtmlSanitizer sanitizer  = new HtmlSanitizer();
            var sanitized = sanitizer.Sanitize(value.ToString());
            if(value.ToString() != sanitized) return false;
            return true;
        }
    }

    public class ModelStateValidator 
    {
        public List<ValidationResult> validationResults { get; set; }
        public bool ValidatePost<T>(T post)
        {
            var context = new ValidationContext(post, serviceProvider: null, items: null);
            validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(post, context, validationResults, true);
        }

    }
    public class ModelStateValidationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {    
           
                  System.Console.WriteLine("in action filter...");
                var modelState = actionContext.ModelState;

                var modelStateMessage = actionContext.ModelState.FirstOrDefault().Value?.Errors?.FirstOrDefault()?.ErrorMessage;

                if (!modelState.IsValid)
                    actionContext.Result = new ContentResult()
                    {
                        Content = modelStateMessage,
                        StatusCode = 400
                    };
                 
                base.OnActionExecuting(actionContext);

                System.Console.WriteLine(modelState.ErrorCount);
               

               // TODO : only in production
                if (modelState.ErrorCount > 0) {
                    System.Console.WriteLine(actionContext.ModelState.FirstOrDefault().Key);
                     if ( actionContext.HttpContext.Request.Method.Contains("GET") ) {

                          System.Console.WriteLine("redirecting");

                           actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { 
                                action = "404", 
                                controller = "Error", 
                                area = "" 
                            }));
                    }
                      
                }
 
        }
    }
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext exceptionContext)
        {
            System.Console.WriteLine("EXCEPTION!");

                   if ( exceptionContext.HttpContext.Request.Method.Contains("GET") ) {
                    exceptionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { 
                            action = "Error", 
                            controller = "Home", 
                            area = "" 
                        }));
                   }
                   else {
                       
                exceptionContext.Result = new ContentResult()
                    {
                        Content = "Home/Error",
                        StatusCode = 302
                    };
                }
        }
    }
    
}