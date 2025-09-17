using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TourismWebSite.Tests.Infrastructure
{
    public static class MvcTestHelpers
    {
        public static void SetUser(this Controller controller, string userId = "U1", bool authenticated = true)
        {
            var id = new ClaimsIdentity(authenticated ? "TestAuth" : null);
            if (authenticated) id.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
            var principal = new ClaimsPrincipal(id);

            var http = new FakeHttpContext(principal);
            controller.ControllerContext = new ControllerContext(new RequestContext(http, new RouteData()), controller);
            controller.TempData = new TempDataDictionary();
        }

        public static void InjectPrivateDb(object controller, object dbInstance)
        {
            var field = controller.GetType().GetField("db",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field == null) throw new InvalidOperationException("No private field named 'db' found.");
            field.SetValue(controller, dbInstance);
        }

        private class FakeHttpContext : HttpContextBase
        {
            private readonly IPrincipal _user;
            public FakeHttpContext(IPrincipal user) { _user = user; }
            public override IPrincipal User { get => _user; set { } }
            public override HttpRequestBase Request => new FakeRequest();
            public override HttpResponseBase Response => new FakeResponse();
        }
        private class FakeRequest : HttpRequestBase { public override string ApplicationPath => "/"; }
        private class FakeResponse : HttpResponseBase { public override string ApplyAppPathModifier(string virtualPath) => virtualPath; }
    }
}
