using System;
using System.Collections;
using System.Reflection;

namespace Kiss.Validation.Validators
{
    /// <summary>
    /// Performs validation based on a value not being null or empty.
    /// </summary>
    public class NotNullValidator : Validator
    {
        #region Protected Properties

        /// <summary>
        /// Gets the valid property types.
        /// </summary>
        /// <value>The valid property types.</value>
        protected override Type[] ValidPropertyTypes
        {
            get { return new Type[] { typeof(IEnumerable) }; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyValidator"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        public NotNullValidator(PropertyInfo propertyInfo)
            : this(null, propertyInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyInfo">The property info.</param>
        public NotNullValidator(string errorMessage, PropertyInfo propertyInfo)
            : base(errorMessage, propertyInfo)
        {
            if (string.IsNullOrEmpty(errorMessage))
                this.ErrorMessage = string.Format("{0} ²»ÄÜÎªNULL.", getPropertyName(propertyInfo));
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
            if (value == null)
                return false;

            IEnumerable ienum = value as IEnumerable;
            if (ienum != null)
                return ienum.GetEnumerator().MoveNext();

            return false;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyValidator"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns></returns>
        public static NotNullValidator CreateValidator<T>(string propertyName)
        {
            return CreateValidator(typeof(T), propertyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns></returns>
        public static NotNullValidator CreateValidator(Type type, string propertyName)
        {
            return new NotNullValidator(Validator.GetPropertyInfo(type, propertyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns></returns>
        public static NotNullValidator CreateValidator<T>(string errorMessage, string propertyName)
        {
            return CreateValidator(typeof(T), errorMessage, propertyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns></returns>
        public static NotNullValidator CreateValidator(Type type, string errorMessage, string propertyName)
        {
            return new NotNullValidator(errorMessage, Validator.GetPropertyInfo(type, propertyName));
        }

        #endregion

    }
}
