using System;
using System.Collections;
using System.Reflection;

namespace Kiss.Validation.Validators
{
    /// <summary>
    /// Performs validation based on a value falling within length restrictions.
    /// </summary>
    public class LengthValidator : Validator
    {
        /// <summary>
        /// Gets the length of the maximum.
        /// </summary>
        /// <value>The length of the maximum.</value>
        public int MaxLength { get; private set; }

        /// <summary>
        /// Gets the length of the minimum.
        /// </summary>
        /// <value>The length of the minimum.</value>
        public int MinLength { get; private set; }

        #region Protected Properties

        /// <summary>
        /// Gets the valid property types.
        /// </summary>
        /// <value>The valid property types.</value>
        protected override Type[] ValidPropertyTypes
        {
            get { return new Type[] { typeof(ICollection), typeof(string) }; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthValidator"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="minLength">Length of the min.</param>
        /// <param name="maxLength">Length of the max.</param>
        public LengthValidator(PropertyInfo propertyInfo, int minLength, int maxLength)
            : this(null, propertyInfo, minLength, maxLength)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="minLength">Length of the min.</param>
        /// <param name="maxLength">Length of the max.</param>
        public LengthValidator(string errorMessage, PropertyInfo propertyInfo, int minLength, int maxLength)
            : base(errorMessage, propertyInfo)
        {
            this.MinLength = minLength;
            this.MaxLength = maxLength;

            if (string.IsNullOrEmpty(errorMessage))
                this.ErrorMessage = string.Format("{0} 的长度必须在{1} - {2}之间.", getPropertyName(propertyInfo), this.MinLength, this.MaxLength);
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

            ICollection col = value as ICollection;
            if (col != null)
                return IsValidLength((uint)col.Count);

            string s = value as string;
            if (s != null)
                return IsValidLength((uint)s.Length);

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether length falls within the restrictions.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid length] [the specified length]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidLength(uint length)
        {
            return this.MinLength <= length && length <= this.MaxLength;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthValidator"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="minLength">The minimum value.</param>
        /// <param name="maxLength">The maximum value.</param>
        /// <returns></returns>
        public static LengthValidator CreateValidator<T>(string propertyName, int minLength, int maxLength)
        {
            return CreateValidator(typeof(T), propertyName, minLength, maxLength);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="minLength">The minimum value.</param>
        /// <param name="maxLength">The maximum value.</param>
        /// <returns></returns>
        public static LengthValidator CreateValidator(Type type, string propertyName, int minLength, int maxLength)
        {
            return new LengthValidator(Validator.GetPropertyInfo(type, propertyName), minLength, maxLength);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="minLength">The minimum value.</param>
        /// <param name="maxLength">The maximum value.</param>
        /// <returns></returns>
        public static LengthValidator CreateValidator<T>(string errorMessage, string propertyName, int minLength, int maxLength)
        {
            return CreateValidator(typeof(T), errorMessage, propertyName, minLength, maxLength);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="minLength">The minimum value.</param>
        /// <param name="maxLength">The maximum value.</param>
        /// <returns></returns>
        public static LengthValidator CreateValidator(Type type, string errorMessage, string propertyName, int minLength, int maxLength)
        {
            return new LengthValidator(errorMessage, Validator.GetPropertyInfo(type, propertyName), minLength, maxLength);
        }

        #endregion

    }
}
