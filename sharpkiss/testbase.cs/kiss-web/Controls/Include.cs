﻿using Kiss.Utils;
using System;
using System.IO;
using System.Web.UI;

namespace Kiss.Web.Controls
{
    public class Include : Control, IContextAwaredControl
    {
        public string Js { get; set; }
        public string Css { get; set; }
        public string Require { get; set; }
        public bool NoCombine { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientScriptProxy proxy = ClientScriptProxy.Current;

            proxy.Require(CurrentSite, Require);

            foreach (string css in StringUtil.Split(Css, ",", true, true))
            {
                if (css.Contains("|"))
                {
                    string[] array = StringUtil.Split(css, "|", true, true);
                    if (array.Length != 2) continue;

                    proxy.RegisterCssResource(array[1], array[0]);
                }
                else if (css.EndsWith(".css", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (css.StartsWith("~"))
                        proxy.RegisterCss(ServerUtil.ResolveUrl(css));
                    else if (css.StartsWith("."))
                        proxy.RegisterCss(StringUtil.CombinUrl(CurrentSite.VirtualPath, CurrentSite.ThemeRoot, MobileDetect.Instance.GetRealThemeName(CurrentSite), css.Substring(1)));
                    else
                        proxy.RegisterCss(StringUtil.CombinUrl(CurrentSite.VirtualPath, css));
                }
                else
                    proxy.RegisterCssResource(string.Format("Kiss.Web.jQuery.{0}.css", css));
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (StringUtil.IsNullOrEmpty(Js)) return;

            ClientScriptProxy proxy = ClientScriptProxy.Current;

            foreach (string js in StringUtil.Split(Js, ",", true, true))
            {
                if (js.Contains("|"))
                {
                    string[] array = StringUtil.Split(js, "|", true, true);
                    if (array.Length != 2) continue;

                    proxy.RegisterJsResource(array[1], array[0]);
                }
                else if (js.EndsWith(".js", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (js.Contains("*"))
                    {
                        string vp = "/";
                        var index = js.LastIndexOf('/');
                        if (index != -1)
                            vp = js.Substring(0, index + 1);

                        string path;
                        if (vp.StartsWith("."))
                            path = ServerUtil.MapPath(StringUtil.CombinUrl(CurrentSite.VirtualPath, CurrentSite.ThemeRoot, MobileDetect.Instance.GetRealThemeName(CurrentSite), vp.Substring(1)));
                        else
                            path = ServerUtil.MapPath(StringUtil.CombinUrl(CurrentSite.VirtualPath, vp));

                        if (!Directory.Exists(Path.GetDirectoryName(path)))
                            continue;

                        foreach (var item in Directory.GetFiles(Path.GetDirectoryName(path), js.Substring(index + 1), SearchOption.AllDirectories))
                        {
                            string relativePath = item.ToLower().Replace(path.ToLower(), string.Empty);
                            relativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/');

                            if (vp.StartsWith("~"))
                                proxy.RegisterJs(StringUtil.CombinUrl(ServerUtil.ResolveUrl(vp), relativePath), NoCombine);
                            else if (vp.StartsWith("."))
                                proxy.RegisterJs(StringUtil.CombinUrl(CurrentSite.VirtualPath,
                                    CurrentSite.ThemeRoot,
                                    MobileDetect.Instance.GetRealThemeName(CurrentSite),
                                    vp.Substring(1),
                                    relativePath), NoCombine);
                            else
                                proxy.RegisterJs(StringUtil.CombinUrl(CurrentSite.VirtualPath, StringUtil.CombinUrl(vp, relativePath)), NoCombine);
                        }
                    }
                    else
                    {
                        if (js.StartsWith("~"))
                            proxy.RegisterJs(ServerUtil.ResolveUrl(js), NoCombine);
                        else if (js.StartsWith("."))
                            proxy.RegisterJs(StringUtil.CombinUrl(CurrentSite.VirtualPath, CurrentSite.ThemeRoot, MobileDetect.Instance.GetRealThemeName(CurrentSite), js.Substring(1)), NoCombine);
                        else
                            proxy.RegisterJs(StringUtil.CombinUrl(CurrentSite.VirtualPath, js), NoCombine);
                    }
                }
                else
                    proxy.RegisterJsResource(
                        GetType(),
                        string.Format("Kiss.Web.jQuery.{0}.js", js), NoCombine);
            }
        }

        private IArea _site;
        public IArea CurrentSite { get { return _site ?? JContext.Current.Area; } set { _site = value; } }
    }
}
