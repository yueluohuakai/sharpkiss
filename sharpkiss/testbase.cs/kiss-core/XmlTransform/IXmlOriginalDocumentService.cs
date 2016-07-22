using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kiss.XmlTransform
{
    public interface IXmlOriginalDocumentService
    {
        XmlNodeList SelectNodes(string path, XmlNamespaceManager nsmgr);
    }
}
