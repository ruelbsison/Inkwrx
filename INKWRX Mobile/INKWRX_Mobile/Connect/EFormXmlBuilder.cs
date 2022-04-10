using INKWRX_Mobile.Database.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect
{
    public static class EFormXmlBuilder
    {

        #region Stroke Page

        public static XElement StrokeForm ()
        {
            return new XElement(penDataNS + "rawformdata",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName)
                );
        }

        public static XElement StrokesSubForm(Form form, DateTime startDate, DateTime sentDate, int strokeCount)
        {
            var startDateUTCTicks = (long)Math.Floor(startDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            var sentDateUTCTicks = (long)Math.Floor(sentDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            return new XElement(penDataNS + "form",
                new XAttribute("firststroketime", startDateUTCTicks.ToString()),
                new XAttribute("laststroketime", sentDateUTCTicks.ToString()),
                new XAttribute("appinstancekey", form.FormIdentifier),
                new XAttribute("formtranid", "1"),
                new XAttribute("strokecount", strokeCount.ToString()),
                new XAttribute("tad", "0")
            );
        }

        public static XElement StrokesPages ()
        {
            return new XElement(penDataNS + "pages");
        }

        public static XElement StrokesPage(int pageNumber)
        {
            return new XElement(penDataNS + "page", new XAttribute("address", "0"), new XAttribute("pageno", pageNumber.ToString()));
        }

        public static XElement StrokesStrokes()
        {
            return new XElement(penDataNS + "strokes");
        }

        public static XElement StrokesUnassigned()
        {
            return new XElement(penDataNS + "unassigned");
        }

        public static XElement StrokeFieldContainer(string fieldname)
        {
            return new XElement(penDataNS + "field",
                new XAttribute("fieldid", fieldname),
                new XAttribute("fieldtype", "c"));
        }

        public static XElement StrokeField(string fieldName, double minX, double minY, double maxX, double maxY)
        {
            return new XElement(penDataNS + "stroke",
                new XAttribute("start", "0"),
                new XAttribute("duration", "0"),
                new XAttribute("color", "0"),
                new XAttribute("linewidth", "1"),
                new XAttribute("minx", minX.ToString()),
                new XAttribute("miny", minY.ToString()),
                new XAttribute("maxx", maxX.ToString()),
                new XAttribute("maxy", maxY.ToString()),
                new XAttribute("fieldid", fieldName));
        }

        public static XElement StrokeSample(double x, double y)
        {
            return new XElement(penDataNS + "sample",
                new XAttribute("x", x.ToString()),
                new XAttribute("y", y.ToString()),
                new XAttribute("force", "0"),
                new XAttribute("timestamp", "0")
                );
        }

        #endregion

        #region Form Page

        public static XElement ProcForm()
        {
            return new XElement(procDataNS + "procformdata",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName)
                );
        }

        public static XElement ProcFormHeader(Form form, DateTime startDate, DateTime sentDate)
        {
            var startDateStr = startDate.ToString(DateFormat);
            var sentDateStr = sentDate.ToString(DateFormat);
            return new XElement(procDataNS + "header",
                new XElement(procDataNS + "formInfo",
                    new XAttribute("appinstance", form.FormIdentifier),
                    new XAttribute("devrecv", startDateStr),
                    new XAttribute("devsent", sentDateStr),
                    new XAttribute("sysrecv", sentDateStr)),
                new XElement(procDataNS + "action",
                    new XAttribute("key", "43"),
                    new XAttribute("datetime", sentDateStr),
                    new XAttribute("id", "1"),
                    new XAttribute("type", "entry"),
                    new XAttribute("userforename", "a"),
                    new XAttribute("usersurname", "b"),
                    new XAttribute("startdatetime", startDateStr))
                );
        }

        public static XElement ProcSubForm()
        {
            return new XElement(procDataNS + "form",
                new XAttribute("minrs", "0"),
                new XAttribute("minnrs", "0"));
        }

        public static XElement ProcPage(int pageNumber)
        {
            return new XElement(procDataNS + "page",
                new XAttribute("pageno", pageNumber.ToString()));
        }

        public static XElement ProcFields()
        {
            return new XElement(procDataNS + "fields");
        }

        public static XElement ProcField(string fieldId, string value, bool tickable, bool ticked, string val)
        {
            var valueElem = new XElement(procDataNS + "value",
                new XAttribute("actionid", "1"),
                new XAttribute("actiontype", "entry"),
                value
                );
            if (tickable)
            {
                valueElem.Add(new XAttribute("ticked", ticked ? "true" : "false"));
            }
            if (val != null)
            {
                valueElem.Add(new XAttribute("val", val));
            }

            return new XElement(procDataNS + "field",
                new XAttribute("fieldid", fieldId),
                valueElem);

        }

        #endregion

        private static XNamespace xsi = @"http://www.w3.org/2001/XMLSchema-instance";
        private static XNamespace penDataNS = @"http://destiny.com/xml/penData";
        private static XNamespace xsd = @"http://www.w3.org/2001/XMLSchema";
        private static XNamespace procDataNS = @"http://destiny.com/xml/procformdata";
        private static string DateFormat = "yyyy-MM-dd'T'HH:mm:ss.ffff";
    }
}
