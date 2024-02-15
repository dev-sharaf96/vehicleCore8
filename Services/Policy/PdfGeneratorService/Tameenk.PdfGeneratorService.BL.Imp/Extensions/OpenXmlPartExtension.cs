using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Tameenk.PdfGeneratorService.BL.Imp.Extensions
{
    public static class OpenXmlPartExtension
    {
        public static XDocument GetXDocument(this OpenXmlPart part)
        {
            XDocument partXDocument = part.Annotation<XDocument>();
            if (partXDocument != null)
                return partXDocument;
            using (Stream partStream = part.GetStream())
            using (XmlReader partXmlReader = XmlReader.Create(partStream))
                partXDocument = XDocument.Load(partXmlReader);
            part.AddAnnotation(partXDocument);
            return partXDocument;
        }
    }
}
