﻿using System;

namespace Kiss.Logging
{
    /// <summary>
    /// Manages the instantiation of <see cref="ILogger"/>s.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Creates a new logger, getting the logger name from the specified type.
        /// </summary>
        ILogger Create(Type type);

        /// <summary>
        /// Creates a new logger.
        /// </summary>
        ILogger Create(String name);
    }
}
