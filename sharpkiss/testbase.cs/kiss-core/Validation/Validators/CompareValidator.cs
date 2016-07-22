using System;
using System.ComponentModel;
using System.Reflection;

namespace Kiss.Validation.Validators
{
    /// <summary>
    /// The operator the Compare Validator uses.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// A comparison for equality.  
        /// </summary>
        [Description("等于")]
        Equal,
        /// <summary>
        /// A comparison for greater than.  
        /// </summary>
        [Description("大于")]
        GreaterThan,
        /// <summary>
        /// A comparison for greater than or equal to. 
        /// </summary>
        [Description("大于等于")]
        GreaterThanOrEqual,
        /// <summary>
        /// A comparison for less than.  
        /// </summary>
        [Description("小于")]
        LessThan,
        /// <summary>
        /// A comparison for less than or equal to.  
        /// </summary>
        [Description("小于等于")]
        LessThanOrEqual,
        /// <summary>
        /// A comparison for inequality.  
        /// </summary>
        [Description("不等于")]
        NotEqual
    }

    /// <summary>
    /// Performs validation based on a value comparison.
    /// </summary>
    public class CompareValidator : Validator
    {
        /// <summary>
        /// Gets the comparison operator.
        /// </summary>
        /// <value>The comparison operator.</value>
        public ComparisonOperator ComparisonOperator { get; private set; }

        /// <summary>
        /// Gets the value to compare.
        /// </summary>
        /// <value>The value to compare.</value>
        public object ValueToCompare { get; private set; }

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
        /// Initializes a new instance of the <see cref="CompareValidator"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        public CompareValidator(PropertyInfo propertyInfo, ComparisonOperator comparisonOperator, object valueToCompare)
            : this(null, propertyInfo, comparisonOperator, valueToCompare)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        public CompareValidator(string errorMessage, PropertyInfo propertyInfo, ComparisonOperator comparisonOperator, object valueToCompare)
            : base(errorMessage, propertyInfo)
        {
            this.ComparisonOperator = comparisonOperator;
            this.ValueToCompare = valueToCompare;
            if (string.IsNullOrEmpty(errorMessage))
                this.ErrorMessage = string.Format("{0}必须{1}{2}.", getPropertyName(propertyInfo), this.ComparisonOperator.ToString(), this.ValueToCompare);
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
            IComparable comp = value as IComparable;

            object valueToCompare = Convert.ChangeType(this.ValueToCompare, value.GetType());

            return TestComparisonResult(comp.CompareTo(valueToCompare));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tests the comparison result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private bool TestComparisonResult(int result)
        {
            switch (this.ComparisonOperator)
            {
                case ComparisonOperator.Equal:
                    return result == 0;
                case ComparisonOperator.NotEqual:
                    return result != 0;
                case ComparisonOperator.GreaterThan:
                    return result > 0;
                case ComparisonOperator.GreaterThanOrEqual:
                    return result >= 0;
                case ComparisonOperator.LessThan:
                    return result < 0;
                case ComparisonOperator.LessThanOrEqual:
                    return result <= 0;
            }
            return false;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareValidator"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns></returns>
        public static CompareValidator CreateValidator<T>(string propertyName, ComparisonOperator comparisonOperator, object valueToCompare)
        {
            return CreateValidator(typeof(T), propertyName, comparisonOperator, valueToCompare);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns></returns>
        public static CompareValidator CreateValidator(Type type, string propertyName, ComparisonOperator comparisonOperator, object valueToCompare)
        {
            return new CompareValidator(Validator.GetPropertyInfo(type, propertyName), comparisonOperator, valueToCompare);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns></returns>
        public static CompareValidator CreateValidator<T>(string errorMessage, string propertyName, ComparisonOperator comparisonOperator, object valueToCompare)
        {
            return CreateValidator(typeof(T), errorMessage, propertyName, comparisonOperator, valueToCompare);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns></returns>
        public static CompareValidator CreateValidator(Type type, string errorMessage, string propertyName, ComparisonOperator comparisonOperator, object valueToCompare)
        {
            return new CompareValidator(errorMessage, Validator.GetPropertyInfo(type, propertyName), comparisonOperator, valueToCompare);
        }

        #endregion
    }
}
