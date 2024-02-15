using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tameenk.PdfGeneratorService.BL.Core.Exceptions;
using Tameenk.PdfGeneratorService.BL.Imp.Extensions;
using Tameenk.PdfGeneratorService.DAL.Core.Repository;

namespace Tameenk.PdfGeneratorService.BL.Imp.Helpers
{
    class DocxGeneratorHelper
    {
        private static readonly XNamespace w;

        static DocxGeneratorHelper()
        {
            w = RepositoryConstants.DocxWSchema;
        }

        public static async Task<string> GenerateDocxReport(string reportTemplateFilePath, string reportType, string reportDataAsJsonString)
        {
            try
            {
                if (!File.Exists(reportTemplateFilePath))
                    throw new TemplateNotFoundException(reportType);

                JObject reportData = JObject.Parse(reportDataAsJsonString);

                var generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                    Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15), reportType,
                    DateTime.Now.ToString("HHmmss"), RepositoryConstants.DocxExtension);
                string generatedReportDirPath = RepositoryConstants.GeneratedReportsBasePath;
                string generatedReportFilePath = Path.Combine(generatedReportDirPath, generatedReportFileName);
                if (!Directory.Exists(generatedReportDirPath))
                    Directory.CreateDirectory(generatedReportDirPath);

                File.Copy(reportTemplateFilePath, generatedReportFilePath, false);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(generatedReportFilePath, true))
                {
                    XDocument mainXDocumentPart = wordDoc.MainDocumentPart.GetXDocument();

                    //doBinding(mainXDocumentPart, reportData);
                    doBinding(mainXDocumentPart, reportData, wordDoc);

                    mainXDocumentPart.Save(wordDoc.MainDocumentPart.GetStream(FileMode.Create));
                    wordDoc.Close();
                    wordDoc.Dispose();
                }
                return generatedReportFilePath;
            }
            catch (Exception ex)
            {
                throw new DocxGenerationFailureException(ex);
            }
        }

        private static List<XElement> getInnerContentControls(XContainer container)
        {
            return container.Descendants(w + "sdt").ToList();
        }

        private static XElement deepCopyXElement(XElement original)
        {
            return XElement.Parse(original.ToString());
        }

        //private static void doBinding(XContainer doc, JObject reportDataAsJson, bool replaceIdOfCC = false)
        //{
        //    foreach (var contentControlXElement in getInnerContentControls(doc))
        //    {
        //        if (isContainerOfOtherElements(contentControlXElement, reportDataAsJson))
        //        {
        //            var contentDataAsArray = getContentControlArrayFromJson(contentControlXElement, reportDataAsJson);
        //            doArrayBinding(contentControlXElement, contentDataAsArray);
        //        }
        //        else
        //        {
        //            doContentControlBinding(contentControlXElement, reportDataAsJson, replaceIdOfCC);
        //        }
        //    }
        //}

        //private static void doArrayBinding(XElement container, JArray reportDataAsJsonArray)
        //{
        //    try
        //    {
        //        var generatedElements = new List<XElement>();
        //        for (int i = 0; i < reportDataAsJsonArray.Count; i++)
        //        {
        //            var clonnedContainer = deepCopyXElement(container);
        //            replaceContentControlId(clonnedContainer);
        //            var reportDataAsJson = reportDataAsJsonArray[i] as JObject;
        //            if (reportDataAsJson != null)
        //            {
        //                reportDataAsJson.Add("Index", (i + 1).ToString());
        //                doBinding(clonnedContainer, reportDataAsJson, true);
        //                generatedElements.Add(clonnedContainer);
        //            }
        //            else
        //            {
        //                reportDataAsJson = new JObject();
        //                reportDataAsJson.Add("Index", " - " + (i + 1).ToString());
        //                reportDataAsJson.Add("Data", reportDataAsJsonArray[i].ToString());
        //                doBinding(clonnedContainer, reportDataAsJson, true);
        //                generatedElements.Add(clonnedContainer);
        //            }
        //        }

        //        container.ReplaceWith(generatedElements);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private static void doBinding(XContainer doc, JObject reportDataAsJson, WordprocessingDocument worddoc, bool replaceIdOfCC = false, bool arr = false)
        {
            var controls = getInnerContentControls(doc);
            int index = 0;
            foreach (var contentControlXElement in controls)
            {
                var res = isContainerOfOtherElements(contentControlXElement, reportDataAsJson, arr);
                if (res == 1)
                {
                    var contentDataAsArray = getContentControlArrayFromJson(contentControlXElement, reportDataAsJson);
                    doArrayBinding(contentControlXElement, contentDataAsArray);
                }
                else if (res == 2)
                    doContentControlBinding(contentControlXElement, reportDataAsJson, replaceIdOfCC, arr);
                else if (res == 4)
                    doCheckBoxContentControlBinding(contentControlXElement, reportDataAsJson, replaceIdOfCC, arr);
            }
        }

        private static void doArrayBinding(XElement container, JArray reportDataAsJsonArray)
        {
            try
            {
                var generatedElements = new List<XElement>();
                for (int i = 0; i < reportDataAsJsonArray.Count; i++)
                {
                    var clonnedContainer = deepCopyXElement(container);
                    replaceContentControlId(clonnedContainer);
                    var reportDataAsJson = reportDataAsJsonArray[i] as JObject;
                    if (reportDataAsJson != null)
                    {
                        reportDataAsJson.Add("Index", (i + 1).ToString());
                        doBinding(clonnedContainer, reportDataAsJson, null, true, false);
                        generatedElements.Add(clonnedContainer);
                    }
                    else
                    {
                        reportDataAsJson = new JObject();
                        reportDataAsJson.Add("Index", " - " + (i + 1).ToString());
                        reportDataAsJson.Add("Data", reportDataAsJsonArray[i].ToString());
                        doBinding(clonnedContainer, reportDataAsJson, null, true, false);
                        generatedElements.Add(clonnedContainer);
                    }
                }

                container.ReplaceWith(generatedElements);
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        //private static void doContentControlBinding(XElement ccAsXelement, JObject reportDataAsJson, bool replaceIdOfCC)
        //{
        //    try
        //    {
        //        // parse value from json
        //        string value = getContentControlValueFromJson(ccAsXelement, reportDataAsJson);
        //        if (replaceIdOfCC)
        //            replaceContentControlId(ccAsXelement);

        //        // replace content control with value
        //        var ccContent = (ccAsXelement.Descendants(w + "sdtContent")).FirstOrDefault();
        //        if (ccContent != null)
        //        {
        //            var contentChildren = ccContent.Elements().ToList();
        //            //XElement finalData = null;
        //            foreach (var child in contentChildren)
        //            {
        //                var textXElement = (child.Descendants(w + "t")).FirstOrDefault();
        //                if (textXElement != null)
        //                {
        //                    if (string.IsNullOrEmpty(value))
        //                    {
        //                        textXElement.Value = " ";
        //                    }
        //                    else
        //                    {
        //                        textXElement.Value = value;
        //                    }
        //                    //finalData = child;
        //                    break;
        //                }
        //            }

        //            //if (finalData != null)
        //            //{
        //            //    ccAsXelement.Remove();
        //            //    parentXElement.Add(finalData);
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private static void doContentControlBinding(XElement ccAsXelement, JObject reportDataAsJson, bool replaceIdOfCC, bool arr)
        {
            try
            {
                string value = getContentControlValueFromJson(ccAsXelement, reportDataAsJson, arr);
                if (replaceIdOfCC)
                    replaceContentControlId(ccAsXelement);

                // replace content control with value
                var ccContent = (ccAsXelement.Descendants(w + "sdtContent")).FirstOrDefault();

                if (ccContent == null)
                    return;

                var contentChildren = ccContent.Elements().ToList();
                //XElement finalData = null;
                foreach (var child in contentChildren)
                {
                    var textXElement = (child.Descendants(w + "t")).FirstOrDefault();
                    if (textXElement != null)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            textXElement.Value = " ";
                        }
                        else
                        {
                            textXElement.Value = value;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // throw ex;
            }
        }

        private static string getContentControlTagValue(XElement ccAsXelement)
        {
            try
            {
                var sdtPrElement = ccAsXelement.Element(w + "sdtPr");
                if (sdtPrElement != null)
                {
                    var tagElement = sdtPrElement.Element(w + "tag");
                    if (tagElement != null && tagElement.HasAttributes)
                    {
                        return tagElement.FirstAttribute.Value;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private static string getContentControlValueFromJson(XElement ccAsXelement, JObject reportDataAsJson)
        //{
        //    string propName = getContentControlTagValue(ccAsXelement);
        //    if (!string.IsNullOrEmpty(propName) && reportDataAsJson.ContainsKey(propName))
        //        return (string)reportDataAsJson[propName];

        //    return " ";
        //}

        private static string getContentControlValueFromJson(XElement ccAsXelement, JObject reportDataAsJson, bool arr = false)
        {
            string propName = string.Empty;
            if (!arr)
                propName = getContentControlTagValue(ccAsXelement);
            else
                propName = getContentControlTagValueForTable(ccAsXelement);

            if (reportDataAsJson.ContainsKey(propName))
                return (string)reportDataAsJson[propName];

            return " ";
        }

        //private static bool isContainerOfOtherElements(XElement ccAsXelement, JObject reportDataAsJson)
        //{
        //    try
        //    {
        //        string propName = getContentControlTagValue(ccAsXelement);
        //        if (!string.IsNullOrEmpty(propName) && reportDataAsJson.ContainsKey(propName))
        //        {
        //            return reportDataAsJson[propName].Type == JTokenType.Array;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private static int isContainerOfOtherElements(XElement ccAsXelement, JObject reportDataAsJson, bool arr = false)
        {
            try
            {
                string propName = string.Empty;
                if (!arr)
                {
                    propName = getContentControlTagValue(ccAsXelement);
                    if (reportDataAsJson.ContainsKey(propName))
                    {
                        if (reportDataAsJson[propName].Type == JTokenType.Array)
                            return 1;
                        if (reportDataAsJson[propName].Type == JTokenType.Boolean)
                            return 4;
                    }
                }
                else
                {
                    propName = getContentControlTagValueForTable(ccAsXelement);
                    return 2;
                }
                return 2;
            }
            catch (Exception ex)
            {
                return -1;
                //throw ex;
            }
        }

        private static string getContentControlTagValueForTable(XElement ccAsXelement)
        {
            try
            {
                string ProbName = string.Empty;
                var Elements = ccAsXelement.Element(w + "sdtContent").Element(w + "tr").Elements(w + "tc");
                int index = 0;
                foreach (var ele in Elements)
                {
                    if (index <= 0)
                        continue;

                    var sdtPrElement = ele.Element(w + "p");
                    if (sdtPrElement == null)
                        sdtPrElement = ele.Element(w + "sdt").Element(w + "sdtPr");
                    else
                        sdtPrElement = sdtPrElement.Element(w + "sdt").Element(w + "sdtPr");
                    if (sdtPrElement != null)
                    {
                        var tagElement = sdtPrElement.Element(w + "tag");
                        if (tagElement != null && tagElement.HasAttributes)
                        {
                            return tagElement.FirstAttribute.Value;
                        }
                    }

                    index++;
                }
                return ProbName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static JArray getContentControlArrayFromJson(XElement ccAsXelement, JObject reportDataAsJson)
        {
            try
            {
                string propName = getContentControlTagValue(ccAsXelement);
                if (reportDataAsJson.ContainsKey(propName))
                    return reportDataAsJson[propName] as JArray;

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void replaceContentControlId(XElement ccAsXelement)
        {
            var sdtPrElement = ccAsXelement.Element(w + "sdtPr");
            if (sdtPrElement != null)
            {
                var idElement = sdtPrElement.Element(w + "id");
                if (idElement != null && idElement.HasAttributes)
                {
                    Random rnd = new Random(System.Environment.TickCount);
                    idElement.FirstAttribute.Value = rnd.Next(int.MinValue, int.MaxValue).ToString();
                }
            }
        }

        private static void doCheckBoxContentControlBinding(XElement ccAsXelement, JObject reportDataAsJson, bool replaceIdOfCC, bool arr)
        {
            try
            {
                string value = getContentControlValueFromJson(ccAsXelement, reportDataAsJson, arr);
                if (replaceIdOfCC)
                    replaceContentControlId(ccAsXelement);
                var ccContent = (ccAsXelement.Descendants(w + "sdtContent")).FirstOrDefault();

                if (ccContent == null)
                    return;

                var contentChildren = ccContent.Elements().ToList();
                foreach (var child in contentChildren)
                {
                    var textXElement = (child.Descendants(w + "t")).FirstOrDefault();
                    if (textXElement == null)
                        continue;

                    if (string.IsNullOrEmpty(value))
                    {
                        textXElement.Value = " ";
                    }
                    else
                    {
                        if (value.ToLower() == "false")
                        {
                            textXElement.Value = "☐";
                        }
                        else if (value.ToLower() == "true")
                        {
                            textXElement.Value = "☑";
                        }
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                // throw ex;
            }
        }
    }
}
