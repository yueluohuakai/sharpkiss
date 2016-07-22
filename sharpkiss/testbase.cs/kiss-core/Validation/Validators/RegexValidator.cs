using System;
using System.Reflection;

namespace Kiss.Validation.Validators
{
    /// <summary>
    /// Performs validation for strings based on a regular expression.
    /// </summary>
    public class RegexValidator : Validator
    {
        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        /// <value>The regular expression.</value>
        public string Regex { get; private set; }

        #region Protected Properties

        /// <summary>
        /// Gets the valid property types.
        /// </summary>
        /// <value>The valid property types.</value>
        protected override Type[] ValidPropertyTypes
        {
            get { return new Type[] { typeof(string) }; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexValidator"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="regex">The regex.</param>
        public RegexValidator(PropertyInfo propertyInfo, string regex)
            : this(null, propertyInfo, regex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="regex">The regex.</param>
        public RegexValidator(string errorMessage, PropertyInfo propertyInfo, string regex)
            : base(errorMessage, propertyInfo)
        {
            this.Regex = regex;
            if (string.IsNullOrEmpty(errorMessage))
                this.ErrorMessage = string.Format("{0} 的格式必须为{1}.", getPropertyName(propertyInfo), this.Regex);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Does the validation.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override bool DoIsValid(object instance, object value)
        {
            string s = value as string;
            return System.Text.RegularExpressions.Regex.IsMatch(s ?? "", this.Regex);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexValidator"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="regex">The regular expression.</param>
        /// <returns></returns>
        public static RegexValidator CreateValidator<T>(string propertyName, string regex)
        {
            return CreateValidator(typeof(T), propertyName, regex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="regex">The regular expression.</param>
        /// <returns></returns>
        public static RegexValidator CreateValidator(Type type, string propertyName, string regex)
        {
            return new RegexValidator(Validator.GetPropertyInfo(type, propertyName), regex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="regex">The regular expression.</param>
        /// <returns></returns>
        public static RegexValidator CreateValidator<T>(string errorMessage, string propertyName, string regex)
        {
            return CreateValidator(typeof(T), errorMessage, propertyName, regex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="regex">The regular expression.</param>
        /// <returns></returns>
        public static RegexValidator CreateValidator(Type type, string errorMessage, string propertyName, string regex)
        {
            return new RegexValidator(errorMessage, Validator.GetPropertyInfo(type, propertyName), regex);
        }

        #endregion

    }
}