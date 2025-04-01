using Project8.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebBanSach.Controllers;
using WebBanSach.Models.Data;

namespace WebBanSach
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // Initialize dependencies
            var context = new BSDBContext();
            var orderFactory = new OrderFactory(context);

            // Set custom controller factory
            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory(context, orderFactory));
        }
        public class CustomControllerFactory : DefaultControllerFactory
        {
            private readonly BSDBContext _context;
            private readonly IOrderFactory _orderFactory;

            public CustomControllerFactory(BSDBContext context, IOrderFactory orderFactory)
            {
                _context = context;
                _orderFactory = orderFactory;
            }

            protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
            {
                if (controllerType == null)
                {
                    return base.GetControllerInstance(requestContext, controllerType);
                }

                if (controllerType == typeof(CartController))
                {
                    return new CartController(_context);
                }

                // Fallback to default factory for other controllers
                return base.GetControllerInstance(requestContext, controllerType);
            }
        }
    }
}
