﻿using System;

namespace Kiss.Web.Mvc
{
    /// <summary>
    /// indicate an assembly contains mvc related code
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class MvcAttribute : Attribute
    {
    }
}
