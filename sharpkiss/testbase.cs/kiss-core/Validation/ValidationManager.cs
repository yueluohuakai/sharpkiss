using Kiss.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ValidatorCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Kiss.Validation.Validators.Validator>>;

namespace Kiss.Validation
{
    /// <summary>
    /// Manages the validation for a type and instance.
    /// </summary>
    public class ValidationManager
    {
        #region Private Static Methods

        /// <summary>
        /// Stores the Validators for a particular type.
        /// </summary>
        private static readonly Dictionary<Type, ValidatorCollection> typeValidators = new Dictionary<Type, ValidatorCollection>();

        #endregion

        #region Private Static Events

        private static event EventHandler<GlobalValidatorsChangedEventArgs> GlobalValidatorsChanged;

        #endregion

        #region Private Fields

        protected Type targetType;
        private IDictionary<string, string> validationErrors = new Dictionary<string, string>();
        private ValidatorCollection validators;
        private ValidatorCollection instanceValidators;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the validators.
        /// </summary>
        /// <value>The validators.</value>
        public ValidatorCollection Validators
        {
            get { return GetValidators(); }
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>The validation errors.</value>
        public IDictionary<string, string> ValidationErrors
        {
            get { return this.validationErrors; }
        }

        #endregion

        #region Constructor(s)

        public ValidationManager(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.targetType = type;

            GlobalValidatorsChanged += new EventHandler<GlobalValidatorsChangedEventArgs>(OnGlobalValidatorsChanged);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the validator to the instance validators.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public void AddValidator(Validator validator)
        {
            string key = validator.PropertyInfo.Name;
            //Add it the instance validators first.
            //
            if (this.instanceValidators == null)
                this.instanceValidators = new Dictionary<string, List<Validator>>();
            if (!this.instanceValidators.ContainsKey(key))
                this.instanceValidators[key] = new List<Validator>();
            this.instanceValidators[key].Add(validator);

            //Force a refresh of the validators cache...
            //
            this.validators = null;
        }

        /// <summary>
        /// Determines whether the specified property is valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is property valid] [the specified property name]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPropertyValid(object instance, string propertyName)
        {
            ValidatorCollection validators = this.Validators;
            this.validationErrors.Remove(propertyName);
            DoIsValid(instance, validators[propertyName]);
            return this.validationErrors.ContainsKey(propertyName);
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(object instance)
        {
            this.validationErrors.Clear();
            ValidatorCollection validators = this.Validators;
            foreach (List<Validator> vl in validators.Values)
            {
                DoIsValid(instance, vl);
            }

            return this.validationErrors.Count == 0;
        }

        public string GetValidationErrorContent()
        {
            StringBuilder txt = new StringBuilder();

            foreach (var item in ValidationErrors)
            {
                txt.AppendLine(item.Value);
            }

            return txt.ToString();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Does the validation.
        /// </summary>
        private void DoIsValid(object instance, List<Validator> validators)
        {
            if (validators == null)
                return;

            for (int i = 0; i < validators.Count; i++)
            {
                if (!validators[i].IsValid(instance))
                {
                    this.validationErrors[validators[i].PropertyInfo.Name] = validators[i].ErrorMessage;
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the validators.
        /// </summary>
        /// <returns></returns>
        private ValidatorCollection GetValidators()
        {
            if (this.validators == null)
                this.validators = ValidationManager.FindValidatorAttributes(this.targetType);

            if (this.instanceValidators != null)
            {
                foreach (KeyValuePair<string, List<Validator>> kvp in this.instanceValidators)
                    this.validators[kvp.Key].AddRange(kvp.Value);
            }

            return this.validators;
        }

        /// <summary>
        /// Called when [global validators changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnGlobalValidatorsChanged(object sender, GlobalValidatorsChangedEventArgs e)
        {
            if (this.targetType == e.Type)
                this.validators = null;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Adds the validator globally.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public static void AddGlobalValidator<T>(Validator validator)
        {
            AddGlobalValidator(typeof(T), validator);
        }

        /// <summary>
        /// Adds the validator globally.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="validator">The validator.</param>
        public static void AddGlobalValidator(Type type, Validator validator)
        {
            if (!typeValidators.ContainsKey(type))
                typeValidators[type] = new ValidatorCollection();

            ValidatorCollection validators = typeValidators[type];

            string key = validator.PropertyInfo.Name;
            if (!validators.ContainsKey(key))
                validators[key] = new List<Validator>();

            validators[key].Add(validator);

            if (GlobalValidatorsChanged != null)
                GlobalValidatorsChanged(null, new GlobalValidatorsChangedEventArgs(type));
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Finds the validator attributes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A ValidatorCollection.</returns>
        private static ValidatorCollection FindValidatorAttributes(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!typeValidators.ContainsKey(type))
            {
                lock (typeValidators)
                {
                    if (!typeValidators.ContainsKey(type))
                    {
                        ValidatorCollection validators = new ValidatorCollection();

                        foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                        {
                            ValidatorAttribute[] vas = (ValidatorAttribute[])pi.GetCustomAttributes(typeof(ValidatorAttribute), true);
                            List<ValidatorAttribute> vasList = new List<ValidatorAttribute>(vas);
                            vasList.Sort(
                                delegate(ValidatorAttribute va1, ValidatorAttribute va2)
                                {
                                    return va1.Order.CompareTo(va2.Order);
                                });

                            List<Validator> list = vasList.ConvertAll<Validator>(
                                delegate(ValidatorAttribute va)
                                {
                                    return va.GetValidator(pi);
                                });


                            if (list.Count > 0)
                                validators.Add(pi.Name, list);
                        }

                        typeValidators.Add(type, validators);
                    }
                }
            }

            return typeValidators[type];
        }

        #endregion

        #region Private Objects

        /// <summary>
        /// Contains the type that was changed.
        /// </summary>
        private class GlobalValidatorsChangedEventArgs : EventArgs
        {

            private Type type;

            /// <summary>
            /// Gets the type.
            /// </summary>
            /// <value>The type.</value>
            public Type Type
            {
                get { return this.type; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GlobalValidatorsChangedEventArgs"/> class.
            /// </summary>
            /// <param name="type">The type.</param>
            public GlobalValidatorsChangedEventArgs(Type type)
            {
                this.type = type;
            }

        }

        #endregion
    }

    public class ValidationManager<T> : ValidationManager
    {
        public ValidationManager()
            : base(typeof(T))
        {
        }

        public bool IsValid(T t)
        {
            return base.IsValid(t);
        }
    }
}