using Kiss.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kiss
{
    /// <summary>
    /// extendable item
    /// </summary>
    public interface IExtendable
    {
        string PropertyName { get; set; }
        string PropertyValue { get; set; }

        string this[string key] { get; set; }

        void SerializeExtAttrs();
    }

    /// <summary>
    /// base model class
    /// </summary>
    [Serializable]
    public abstract class Obj<t> : QueryObject
    {
        #region props

        /// <summary>
        /// Id
        /// </summary>
        public virtual t Id { get; set; }

        #endregion

        #region equality overrides

        /// <summary>
        /// A uniquely key to identify this particullar instance of the class
        /// </summary>
        /// <returns>A unique integer value</returns>
        public override int GetHashCode()
        {
            if (Id == null)
                return string.Empty.GetHashCode();

            return Id.GetHashCode();
        }

        /// <summary>
        /// Comapares this object with another
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the two objects as equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() == this.GetType())
            {
                return obj.GetHashCode() == this.GetHashCode();
            }

            return false;
        }

        /// <summary>
        /// Checks to see if two business objects are the same.
        /// </summary>
        public static bool operator ==(Obj<t> first, Obj<t> second)
        {
            if (Object.ReferenceEquals(first, second))
            {
                return true;
            }

            if ((object)first == null || (object)second == null)
            {
                return false;
            }

            return first.GetHashCode() == second.GetHashCode();
        }

        /// <summary>
        /// Checks to see if two business objects are different.
        /// </summary>
        public static bool operator !=(Obj<t> first, Obj<t> second)
        {
            return !(first == second);
        }

        #endregion
    }

    /// <summary>
    /// dict schema
    /// </summary>
    [Serializable]
    [OriginalName("gDictSchema")]
    public class DictSchema : QueryObject<DictSchema, string>, IComparable<DictSchema>, IExtendable
    {
        [PK(AutoGen = false)]
        public override string Id { get { return base.Id; } set { base.Id = value; } }

        [Length(50)]
        public string SiteId { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; }

        public int Depth { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public bool HasChild { get; set; }
        public bool IsValid { get; set; }
        public DateTime DateCreated { get; set; }

        public string Prop1 { get; set; }
        public string Prop2 { get; set; }
        public string Prop3 { get; set; }
        public string Prop4 { get; set; }
        public string Prop5 { get; set; }

        [Validation.Length(Int32.MaxValue)]
        public string Text1 { get; set; }
        [Validation.Length(Int32.MaxValue)]
        public string Text2 { get; set; }
        [Validation.Length(Int32.MaxValue)]
        public string Text3 { get; set; }
        [Validation.Length(Int32.MaxValue)]
        public string Text4 { get; set; }
        [Validation.Length(Int32.MaxValue)]
        public string Text5 { get; set; }

        public DateTime DateTime1 { get; set; }
        public DateTime DateTime2 { get; set; }
        public DateTime DateTime3 { get; set; }
        public DateTime DateTime4 { get; set; }
        public DateTime DateTime5 { get; set; }

        public int Int1 { get; set; }
        public int Int2 { get; set; }
        public int Int3 { get; set; }
        public int Int4 { get; set; }
        public int Int5 { get; set; }

        [Ignore]
        public DictSchema Parent { get; set; }
        [Ignore]
        public List<DictSchema> Children { get; set; }

        public override string ToString()
        {
            return Title;
        }

        public int CompareTo(DictSchema other)
        {
            if (other == null)
                return 1;
            if (Id == other.Id)
                return 0;

            int result = Depth.CompareTo(other.Depth);
            if (result == 0)
            {
                DictSchema p1 = this;
                DictSchema p2 = other;
                while (p1.Parent != p2.Parent)
                {
                    p1 = p1.Parent;
                    p2 = p2.Parent;
                }
                result = p1.SortOrder.CompareTo(p2.SortOrder);
            }
            else
            {
                DictSchema s = result > 0 ? this : other;
                int d = Math.Abs(Depth - other.Depth);
                while (d-- > 0)
                {
                    s = s.Parent;
                }
                if (result > 0)
                {
                    if (s.Id != other.Id)
                        result = s.CompareTo(other);
                }
                else
                {
                    if (s.Id != Id)
                        result = this.CompareTo(s);
                }
            }

            return result;
        }

        [Length(2000)]
        public string PropertyName { get; set; }

        [Length(Int32.MaxValue)]
        public string PropertyValue { get; set; }

        private ExtendedAttributes _extAttrs;
        [Ignore]
        public ExtendedAttributes ExtAttrs
        {
            get
            {
                if (_extAttrs == null)
                {
                    _extAttrs = new ExtendedAttributes();
                    _extAttrs.SetData(PropertyName, PropertyValue);
                }
                return _extAttrs;
            }
        }

        [Ignore]
        public string this[string key]
        {
            get
            {
                if (ExtAttrs.ExtendedAttributesCount == 0)
                    ExtAttrs.SetData(PropertyName, PropertyValue);
                return ExtAttrs.GetExtendedAttribute(key);
            }
            set
            {
                ExtAttrs.SetExtendedAttribute(key, value);
            }
        }

        public void SerializeExtAttrs()
        {
            SerializerData sd = ExtAttrs.GetSerializerData();

            PropertyName = sd.Keys;
            PropertyValue = sd.Values;
        }

        #region Api

        public static DictSchema GetByName(string type, string name)
        {
            return (from q in CreateContext()
                    where q.Type == type && q.Name == name
                    select q).FirstOrDefault();
        }

        public static void DeleteByCategory(string siteId, string type, string category)
        {
            DictSchema.Where("SiteId='{0}'", siteId).Where("Type='{0}'", type).Where("Category='{0}'", category).Delete();
        }

        public static DictSchemas GetsByCategory(ILinqContext<DictSchema> cx, string type, string category)
        {
            List<DictSchema> list = (from q in cx
                                     where q.Type == type && q.Category == category
                                     orderby q.SortOrder ascending
                                     select q).ToList();

            // re group            
            List<DictSchema> result = list.FindAll(delegate(DictSchema s)
            {
                return string.IsNullOrEmpty(s.ParentId);
            });

            foreach (DictSchema s in result)
            {
                regroup(s, list);
            }

            return new DictSchemas(result);
        }

        public static DictSchemas GetsByCategory(string type, string category)
        {
            return GetsByCategory(DictSchema.CreateContext(), type, category);
        }

        public static DictSchemas GetsByCategory(ILinqContext<DictSchema> cx, string siteid, string type, string category)
        {
            List<DictSchema> list = (from q in cx
                                     where q.SiteId == siteid && q.Type == type && q.Category == category
                                     orderby q.SortOrder ascending
                                     select q).ToList();

            // re group            
            List<DictSchema> result = list.FindAll(delegate(DictSchema s)
            {
                return string.IsNullOrEmpty(s.ParentId);
            });

            foreach (DictSchema s in result)
            {
                regroup(s, list);
            }

            return new DictSchemas(result);
        }

        public static DictSchemas GetsByCategory(string siteId, string type, string category)
        {
            return GetsByCategory(DictSchema.CreateContext(), siteId, type, category);
        }

        public static List<DictSchema> GetsByParentId(string id)
        {
            return (from q in CreateContext()
                    where q.ParentId == id
                    orderby q.SortOrder ascending
                    select q).ToList();
        }

        #endregion

        #region schema

        public static List<string> Categories
        {
            get
            {
                List<string> list = new List<string>();

                foreach (DataRow row in DictSchema.Where("type='schema'").Select("distinct category").Rows)
                {
                    list.Add(Convert.ToString(row[0]));
                }

                return list;
            }
        }

        public static List<DictSchema> GetsByCategory(string category)
        {
            return (from q in CreateContext()
                    where q.Type == "schema" && q.Category == category && q.ParentId == string.Empty
                    orderby q.SortOrder ascending
                    select q).ToList();
        }

        #endregion

        #region helper
        private static void regroup(DictSchema s, List<DictSchema> list)
        {
            if (s.HasChild)
            {
                s.Children = list.FindAll(delegate(DictSchema schema)
                {
                    return schema.ParentId == s.Id;
                });

                foreach (DictSchema schema in s.Children)
                {
                    schema.Parent = s;
                    regroup(schema, list);
                }
            }
        }
        #endregion

        #region event

        public class FilterEventArgs : EventArgs
        {
            public static readonly new FilterEventArgs Empty = new FilterEventArgs();

            public List<DictSchema> SchemaList { get; set; }

            public string Type { get; set; }
        }

        public static event EventHandler<FilterEventArgs> Filter;

        public static void OnFilter(string type, List<DictSchema> list)
        {
            FilterEventArgs e = new FilterEventArgs();
            e.SchemaList = list;
            e.Type = type;

            EventHandler<FilterEventArgs> handler = Filter;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        #endregion
    }

    public class DictSchemas : List<DictSchema>
    {
        public DictSchemas()
            : base()
        {
        }

        public DictSchemas(int capacity)
            : base(capacity)
        {
        }

        public DictSchemas(IEnumerable<DictSchema> collection)
            : base(collection)
        {
        }

        public DictSchema this[string name]
        {
            get
            {
                return Find((d) => { return string.Equals(d.Name, name, StringComparison.InvariantCultureIgnoreCase); });
            }
        }
    }
}
