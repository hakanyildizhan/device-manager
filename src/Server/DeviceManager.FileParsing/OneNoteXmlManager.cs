// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Xml;

namespace DeviceManager.FileParsing
{
    public class OneNoteXmlManager
    {
        private XmlDocument _doc;
        private XmlNamespaceManager _xmlnsManager;

        public OneNoteXmlManager(string schema)
        {
            _doc = new XmlDocument();
            _doc.LoadXml(schema);

            _xmlnsManager = new XmlNamespaceManager(_doc.NameTable);
            _xmlnsManager.AddNamespace("one", "http://schemas.microsoft.com/office/onenote/2013/onenote");
        }

        public bool PageExists(string pageName)
        {
            XmlNode pageNode = GetPageNode(pageName);
            return pageNode != null;
        }

        public string GetPageId(string pageName)
        {
            XmlNode pageNode = GetPageNode(pageName);
            string pageId = string.Empty;
            if (pageNode != null)
            {
                pageId = pageNode.Attributes["ID"].Value;
            }
            return pageId;
        }

        private XmlNode GetPageNode(string pageName)
        {
            XmlElement root = _doc.DocumentElement;
            XmlNodeList pageNodes = root.SelectNodes("//one:Page", _xmlnsManager);

            foreach (XmlNode pageNode in pageNodes)
            {
                if (pageNode.Attributes["name"].Value == pageName)
                {
                    return pageNode;
                }
            }

            return null;
        }
    }
}
