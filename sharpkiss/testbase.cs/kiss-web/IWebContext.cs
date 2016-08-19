﻿#region File Comment
//+-------------------------------------------------------------------+
//+ FileName: 	    IWebContext.cs
//+ File Created:   20090827
//+-------------------------------------------------------------------+
//+ Purpose:        
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090827        ZHLI Comment Created
//+-------------------------------------------------------------------+
#endregion

using Kiss.Utils;
using System.Collections;
using System.Security.Principal;
using System.Web;

namespace Kiss.Web
{
    /// <summary>
    /// A mockable interface for operations that targets the the http context.
    /// </summary>
    public interface IWebContext
    {
        /// <summary>Gets wether there is a web context availabe.</summary>
        bool IsWeb { get; }

        /// <summary>Gets a dictionary of request scoped items.</summary>
        IDictionary RequestItems { get; }

        /// <summary>Gets the current user principal (may be null).</summary>
        IPrincipal User { get; }

        /// <summary>The current request object.</summary>
        HttpRequest Request { get; }

        /// <summary>The current response object.</summary>
        HttpResponse Response { get; }

        /// <summary>The handler associated with this request.</summary>
        IHttpHandler Handler { get; }

        /// <summary>The physical path on disk to the requested page.</summary>
        string PhysicalPath { get; }

        /// <summary>The local part of the requested path.</summary>
        Url Url { get; }

        /// <summary>Closes any endable resources at the end of the request.</summary>
        void Close ( );

        /// <summary>Converts a virtual path to an an absolute path. E.g. ~/hello.aspx -> /MyVirtualDirectory/hello.aspx.</summary>
        /// <param name="virtualPath">The virtual url to make absolute.</param>
        /// <returns>The absolute url.</returns>
        string ToAbsolute ( string virtualPath );

        /// <summary>Converts an absolute url to an app relative path. E.g. /MyVirtualDirectory/hello.aspx -> ~/hello.aspx.</summary>
        /// <param name="virtualPath">The absolute url to convert.</param>
        /// <returns>An app relative url.</returns>
        string ToAppRelative ( string virtualPath );

        /// <summary>Maps a virtual path to a physical disk path.</summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        string MapPath ( string path );

        /// <summary>Rewrites the request to the given path.</summary>
        /// <param name="path">The path to the template that will handle the request.</param>
        void RewritePath ( string path );

        /// <summary>Transferes the request to the given path.</summary>
        /// <param name="path">The path to the template that will handle the request.</param>
        void TransferRequest ( string path );
    }
}
