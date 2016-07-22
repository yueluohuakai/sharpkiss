using System;
using System.Reflection;

namespace Kiss.Validation.Validators
{
    /// <summary>
    /// Performs validation based on a value not being null.
    /// </summary>
    /// <remarks>
    /// While this validator is legitimate for a value type, it will always return true
    /// because a value type cannot be null.  The exception is a <see cref="string" /> which
    /// will behave properly.
    /// </remarks>
    public class RequiredValidator : Validator
    {

        #region Protected Properties

        /// <summary>
        /// Gets the valid property types.
        /// </summary>
        /// <value>The valid property types.</value>
        protected override Type[] ValidPropertyTypes
        {
            get { return null; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidator"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        public RequiredValidator(PropertyInfo propertyInfo)
            : this(null, propertyInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyInfo">The property info.</param>
        public RequiredValidator(string errorMessage, PropertyInfo propertyInfo)
            : base(errorMessage, propertyInfo)
        {
            if (string.IsNullOrEmpty(errorMessage))
                this.ErrorMessage = string.Format("{0} ²»ÄÜÎª¿Õ.", getPropertyName(propertyInfo));
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
            return value != null;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidator"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns></returns>
        public static RequiredValidator CreateValidator<T>(string propertyName)
        {
            return CreateValidator(typeof(T), propertyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns></returns>
        public static RequiredValidator CreateValidator(Type type, string propertyName)
        {
            return new RequiredValidator(Validator.GetPropertyInfo(type, propertyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns></returns>
        public static RequiredValidator CreateValidator<T>(string errorMessage, string propertyName)
        {
            return CreateValidator(typeof(T), errorMessage, propertyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns></returns>
        public static RequiredValidator CreateValidator(Type type, string errorMessage, string propertyName)
        {
            return new RequiredValidator(errorMessage, Validator.GetPropertyInfo(type, propertyName));
        }

        #endregion

    }
}
