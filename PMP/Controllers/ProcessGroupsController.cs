using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace PMP.Controllers
{
    public class ProcessGroupsController : Controller
    {
        //
        // GET: /ProcessGroups/

        public ActionResult Index()
        {
            //string xmlFile = @"\Content\ProcessGroups.xml";

            //XDocument doc = XDocument.Load(xmlFile);
            //XElement testCfg = XElement.Load(xmlFile);
            //IEnumerable<XElement> test = doc.Elements();

            //XElement child = testCfg.Element("DownloadedEmailIdList");

            //IEnumerable<XElement> emailsList = child.Elements("i");
            return View();
        }

    }
}
