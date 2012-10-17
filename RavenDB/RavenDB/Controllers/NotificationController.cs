using System;
using System.Web.Mvc;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using Raven.Client;
using RavenDB;

namespace ASP
{

    public class RavenController : Controller
    {
        protected IDocumentSession documentSession;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            documentSession = MvcApplication.DocumentStore.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using(documentSession)
            {
                if(filterContext.Exception==null)
                {
                    documentSession.SaveChanges();
                }
            }
        }
    }
    public class NotificationController : RavenController

    {
        public ActionResult NewOrder(string orderId)
        {
        
            documentSession.Store(
                new 
                    {
                        OrderId = orderId,
                        OrderDate = DateTime.Now
                    });
            return Json(new
                            {
                                Success = true
                            }, JsonRequestBehavior.AllowGet);
        }
         
    }
}