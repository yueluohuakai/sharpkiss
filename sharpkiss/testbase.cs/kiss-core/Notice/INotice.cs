using Kiss.Security;
using System;
using System.Collections.Generic;

namespace Kiss.Notice
{
    /// <summary>
    /// notice interface
    /// </summary>
    public interface INotice
    {
        /// <summary>
        /// send notify
        /// </summary>
        void Send(string title, string content, IUser from, IUser[] to, params string[] msgKey);

        /// <summary>
        /// send notify using template
        /// </summary>
        void Send(string templateId, Dictionary<string, object> param, IUser from, IUser[] to, params string[] msgKey);

        /// <summary>
        /// send notify
        /// </summary>
        void Send(string title, string content, string from, string[] to, params string[] msgKey);

        /// <summary>
        /// send notify using template
        /// </summary>
        void Send(string templateId, Dictionary<string, object> param, string from, string[] to, params string[] msgKey);
    }

    /// <summary>
    /// notice config interface
    /// </summary>
    public interface INoticeConfig
    {
        /// <summary>
        /// get specified user's valid channel of specified msg type
        /// </summary>
        /// <returns></returns>
        Dictionary<string, List<string>> GetsValidChannel(string templateId, params string[] userIds);

        Dictionary<string, List<string>> GetsValidTemplate(string userid);

        List<string> GetsValidTemplate(string userid, string channel);

        /// <summary>
        /// save users' notice config
        /// </summary>
        void SaveConfig(string userId, List<Tuple<string, string, bool>> config);

        List<string> GetsConfigableTemplate(string channelname);
    }
}
