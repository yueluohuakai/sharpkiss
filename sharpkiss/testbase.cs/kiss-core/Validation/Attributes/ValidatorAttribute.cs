using System;
using System.Reflection;
using Kiss.Validation.Validators;

namespace Kiss.Validation
{
    /// <summary>
    /// The base validator attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Serializable]
    public abstract class ValidatorAttribute : Attribute
    {
        #region Private Fields

        private string errorMessage;
        private int _order;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get { return this.errorMessage; }
            set { this.errorMessage = value; }
        }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order
        {
            get { return this._order; }
            set { this._order = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public abstract Validator GetValidator(PropertyInfo propertyInfo);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorAttribute"/> class.
        /// </summary>
        public ValidatorAttribute()
        {
            this.errorMessage = null;
            this._order = 0;
        }

        #endregion

    }
}
