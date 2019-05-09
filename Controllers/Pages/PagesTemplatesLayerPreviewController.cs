using System;
using System.Linq;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Core.Utils;

namespace SS.Poll.Controllers.Pages
{
    [RoutePrefix("pages/templatesLayerPreview")]
    public class PagesTemplatesLayerPreviewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, PollUtils.PluginId)) return Unauthorized();

                var formInfoList = PollManager.GetPollInfoList(siteId, 0);

                var type = request.GetQueryString("type");
                var name = request.GetQueryString("name");
                var templateInfoList = TemplateManager.GetTemplateInfoList(type);
                var templateInfo =
                    templateInfoList.FirstOrDefault(x => PollUtils.EqualsIgnoreCase(name, x.Name));

                return Ok(new
                {
                    Value = templateInfo,
                    PollInfoList = formInfoList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
