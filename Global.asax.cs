using site.classesHelp;
using System;
using System.Reflection;
using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace site
{
    public class Global : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var route = routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { action = System.Web.Http.RouteParameter.Optional, id = System.Web.Http.RouteParameter.Optional }
            );
            route.RouteHandler = new MyHttpControllerRouteHandler();
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["opacityStart"] = "ok";
            //переменная для включения стиля выпадающего верхнего меню в консоли управления(только при первом запуске)
            Session["menuAdmTopShow"] = "ok";
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex.InnerException != null) {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Ссылка: " + Request.Url.AbsoluteUri + ". Текст исключения: " + ex.InnerException.Message + ". Строка: " + ex.InnerException.StackTrace);
            }

            if (ex is HttpException)
            {
                if (((HttpException)(ex)).GetHttpCode() == 404)
                    Server.Transfer("~/404.html");
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        public void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {

        }
    }

    // Create two new classes
    public class MyHttpControllerHandler
        : HttpControllerHandler, IRequiresSessionState
    {
        public MyHttpControllerHandler(RouteData routeData) : base(routeData)
        { }
    }
    public class MyHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(
            RequestContext requestContext)
        {
            return new MyHttpControllerHandler(requestContext.RouteData);
        }
    }
}