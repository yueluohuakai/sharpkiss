﻿using System.IO;
using System.Web.UI;

namespace Kiss.Web.Controls
{
    public static class Util
    {
        public delegate void RenderFunc(HtmlTextWriter writer);

        public static string Render(RenderFunc func)
        {
            string content = string.Empty;

            using (MemoryStream ms = new MemoryStream())
            {
                using (HtmlTextWriter htmlWriter = new HtmlTextWriter(new StreamWriter(ms)))
                {
                    func(htmlWriter);
                    htmlWriter.Flush();
                }

                using (StreamReader rdr = new StreamReader(ms))
                {
                    rdr.BaseStream.Position = 0;
                    content = rdr.ReadToEnd();
                }
            }


            ITemplateEngine te = ServiceLocator.Instance.Resolve<ITemplateEngine>();

            using (StringWriter sw = new StringWriter())
            {
                te.Process(JContext.Current.ViewData, string.Empty, sw, content);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
