using EventsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;

namespace EventsApp.Controllers
{
    public class EventController : ApiController
    {
        List<Event> events = new List<Event>();
        public EventController()
        {
            ReadXML();
        }
        // GET: api/Event
        public IEnumerable<Event> Get()
        {
            return events;
        }

        // GET: api/Event/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Event
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Event/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Event/5
        public void Delete(int id)
        {
        }

        private void ReadXML()
        {
            XmlDocument doc1 = new XmlDocument();
            doc1.Load("http://agendalx.pt/rss");
            XmlElement root = doc1.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("/rss/channel/item");

            foreach (XmlNode node in nodes)
            {
                string link = null;
                link = node["link"].InnerText;
                if (link != null)
                {
                    GetInfo(link);
                }
            } 
        }

        public void GetInfo(String link)
        {
            WebClient client = new WebClient();
            String htmlCode = client.DownloadString(link);
            Event e = new Event();
            htmlCode = filterPage(htmlCode);
            e.Name = getName(htmlCode);
            e.Date = getDate(htmlCode);
            e.Category = getCategory(htmlCode);
            e.Description = getDescription(htmlCode);
            e.Image = getImage(htmlCode);
            events.Add(e);
        }

        private string filterPage(string htmlCode)
        {
            String res = htmlCode.Substring(htmlCode.IndexOf("<div class=\"page\""));
            return res;
        }

        private string getName(string htmlCode)
        {
            string start = "<h1 class=\"page-title\">";
            string end = "</h1>";
            return getStringFromHtml(htmlCode, start, end);
        }

        private string getDate(string htmlCode)
        {
            string start = "<span class=\"field-data-resumida\">";
            string end = "</span>";
            return getStringFromHtml(htmlCode, start, end);
        }

        private string getCategory(string htmlCode)
        {
            string start = "<span class=\"field-tipo-evento\">";
            string end = "</div>";
            return removeTags(getStringFromHtml(htmlCode, start, end));
        }

        private string getImage(string htmlCode)
        {
            string start = "img typeof=\"foaf:Image\" src=\"";
            string end = "\" width";
            return getStringFromHtml(htmlCode, start, end);
        }

        private string getDescription(string htmlCode)
        {
            string start = "<div class=\"field-body\">";
            string end = "</div>";
            return removeTags(getStringFromHtml(htmlCode, start, end));
        }

        //public decimal getPrice(string htmlCode) TODO
        //{
        //    string start = "<div class=\"field-informacoes-uteis\">";
        //    string end = "</p>";
        //    string aux = getStringFromHtml(htmlCode, start, end);
        //    if (aux.Contains("Preço"))
        //    {

        //    }

        //}

        public string getLocal(string htmlCode)//TODO
        {
            string start = "<div class=\"thoroughfare\">";
            string end = "</div>";
            return getStringFromHtml(htmlCode, start, end);
        }

        private string getStringFromHtml(String htmlCode, String start, String end){
            int startIdx, endIdx;
            startIdx = htmlCode.IndexOf(start) + start.Length;

            endIdx = htmlCode.Substring(startIdx).IndexOf(end);
            return htmlCode.Substring(startIdx, endIdx);
        }
        
        private string removeTags(string htmlCode)
        {
            htmlCode = htmlCode.Replace("<br />", "\n");
            htmlCode = htmlCode.Replace("<p>", "\n\n");
            htmlCode = htmlCode.Replace("</p>", "");
            htmlCode = htmlCode.Replace("<div>", "");
            htmlCode = htmlCode.Replace("</div", "");
            htmlCode = htmlCode.Replace("<strong>", "");
            htmlCode = htmlCode.Replace("</strong>", "");
            htmlCode = htmlCode.Replace("<em>", "");
            htmlCode = htmlCode.Replace("</em>", "");
            return htmlCode;
        }
    }
}
