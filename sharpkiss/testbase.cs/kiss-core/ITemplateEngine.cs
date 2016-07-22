using System;
using System.Collections;
using System.IO;

namespace Kiss
{
    /// <summary>
    /// Abstracts the underlying template engine being used.
    /// </summary>
    public interface ITemplateEngine
    {
        /// <summary>
        /// Implementors should process the template with
        /// data from the context.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="templateName"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool Process(IDictionary context, String templateName, TextWriter output);

        /// <summary>
        /// Implementors should process the input template with
        /// data from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="templateName">Name of the template.  Used only for information during logging</param>
        /// <param name="output">The output.</param>
        /// <param name="inputTemplate">The input template.</param>
        /// <returns></returns>
        bool Process(IDictionary context, String templateName, TextWriter output, String inputTemplate);

        /// <summary>
        /// Implementors should process the input template with
        /// data from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="templateName">Name of the template.  Used only for information during logging</param>
        /// <param name="output">The output.</param>
        /// <param name="inputTemplate">The input template.</param>
        /// <returns></returns>
        bool Process(IDictionary context, String templateName, TextWriter output, TextReader inputTemplate);

        /// <summary>
        /// Implementors should return <c>true</c> only if the 
        /// specified template exists and can be used
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        bool HasTemplate(String templateName);

        /// <summary>
        /// put context data
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        void PutContextData(string key, object obj);
    }
}
