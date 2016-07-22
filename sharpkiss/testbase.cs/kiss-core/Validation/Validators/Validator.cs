using System;
using System.ComponentModel;
using System.Reflection;

namespace Kiss.Validation.Validators
{
    /// <summary>
    /// The base class for all validators.
    /// </summary>
    public abstract class Validator
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// Gets the property info.
        /// </summary>
        /// <value>The property info.</value>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Gets the valid property types.
        /// </summary>
        /// <value>The valid property types.</value>
        protected abstract Type[] ValidPropertyTypes { get; }

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyInfo">The property info.</param>
        protected Validator(string errorMessage, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (string.IsNullOrEmpty(errorMessage))
                ErrorMessage = string.Format("{0} ÎÞÐ§.", getPropertyName(propertyInfo));
            else
                ErrorMessage = errorMessage;

            PropertyInfo = propertyInfo;

            ThrowIfInvalidPropertyType();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified instance is valid.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            object value = this.PropertyInfo.GetValue(instance, null);
            return DoIsValid(instance, value);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Does the validation.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected abstract bool DoIsValid(object instance, object value);

        /// <summary>
        /// Determines whether goodType is compatible with testType.
        /// </summary>
        /// <param name="goodType">Type of the good.</param>
        /// <param name="testType">Type of the test.</param>
        /// <returns></returns>
        protected bool AreCompatibleTypes(Type goodType, Type testType)
        {
            if (testType == goodType)
                return true;

            Type underTestType = Nullable.GetUnderlyingType(testType);
            if (underTestType != null && underTestType == goodType)
                return true;

            if (goodType.IsInterface)
            {
                foreach (Type iface in testType.GetInterfaces())
                    if (iface == goodType)
                        return true;
            }

            if (testType.IsSubclassOf(goodType))
                return true;

            return false;
        }

        /// <summary>
        /// Throws an exception if the property type this validator belongs to cannot be validated.
        /// </summary>
        protected virtual void ThrowIfInvalidPropertyType()
        {
            if (this.ValidPropertyTypes == null || this.ValidPropertyTypes.Length == 0)
                return;

            Type propType = this.PropertyInfo.PropertyType;
            foreach (Type type in this.ValidPropertyTypes)
            {
                if (AreCompatibleTypes(type, propType))
                    return;
            }

            throw new InvalidOperationException(string.Format("{0} cannot be applied to a(n) {1}.", this.GetType().FullName, propType.FullName));
        }

        #endregion

        #region Protected Static Methods

        /// <summary>
        /// Gets the property info.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            if (type == null)
                throw new ArgumentException("type");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName");

            return type.GetProperty(propertyName);
        }

        protected static string getPropertyName(PropertyInfo propertyInfo)
        {
            string prop = propertyInfo.Name;

            object[] attrs = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs.Length > 0)
                prop = (attrs[0] as DescriptionAttribute).Description;
            return prop;
        }

        #endregion
    }
}