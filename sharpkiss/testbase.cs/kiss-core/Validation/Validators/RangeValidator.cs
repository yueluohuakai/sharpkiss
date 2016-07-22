using System;
using System.Reflection;

namespace Kiss.Validation.Validators
{
    /// <summary>
    /// Performs validation based on a value falling within a range.
    /// </summary>
    public class RangeValidator : Validator
    {
        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        public object MaxValue { get; private set; }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        public object MinValue { get; private set; }

        #region Protected Properties

        /// <summary>
        /// Gets the valid property types.
        /// </summary>
        /// <value>The valid property types.</value>
        protected override Type[] ValidPropertyTypes
        {
            get { return new Type[] { typeof(IComparable) }; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidator"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        public RangeValidator(PropertyInfo propertyInfo, object minValue, object maxValue)
            : this(null, propertyInfo, minValue, maxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        public RangeValidator(string errorMessage, PropertyInfo propertyInfo, object minValue, object maxValue)
            : base(errorMessage, propertyInfo)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            if (string.IsNullOrEmpty(errorMessage))
                this.ErrorMessage = string.Format("{0} 的值必须在{1} and {2}之间.", getPropertyName(propertyInfo), this.MinValue, this.MaxValue);
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
                return this.MinValue == null;

            object minValue = Convert.ChangeType(this.MinValue, value.GetType());
            object maxValue = Convert.ChangeType(this.MaxValue, value.GetType());

            IComparable val = value as IComparable;
            return val.CompareTo(minValue) >= 0 && val.CompareTo(maxValue) <= 0;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidator"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        public static RangeValidator CreateValidator<T>(string propertyName, object minValue, object maxValue)
        {
            return CreateValidator(typeof(T), propertyName, minValue, maxValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        public static RangeValidator CreateValidator(Type type, string propertyName, object minValue, object maxValue)
        {
            return new RangeValidator(Validator.GetPropertyInfo(type, propertyName), minValue, maxValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        public static RangeValidator CreateValidator<T>(string errorMessage, string propertyName, object minValue, object maxValue)
        {
            return CreateValidator(typeof(T), errorMessage, propertyName, minValue, maxValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        public static RangeValidator CreateValidator(Type type, string errorMessage, string propertyName, object minValue, object maxValue)
        {
            return new RangeValidator(errorMessage, Validator.GetPropertyInfo(type, propertyName), minValue, maxValue);
        }

        #endregion

    }
}
