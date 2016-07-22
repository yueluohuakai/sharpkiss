using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Kiss.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class Principal : IPrincipal
    {
        private IIdentity _user;

        public Principal(IIdentity user)
        {
            _user = user;
        }

        /// <summary>
        /// 检查当前用户是否有指定的权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermission(string permission)
        {
            return ServiceLocator.Instance.Resolve<IUserService>().HasPermission(_user, permission);
        }

        /// <summary>
        /// 检查当前用户是否有指定的权限，如果没有权限，则抛出权限拒绝事件
        /// </summary>
        /// <param name="permission"></param>
        public void CheckPermission(string permission)
        {
            if (!HasPermission(permission))
                OnPermissionDenied(new PermissionDeniedEventArgs(permission));
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _user; }
        }

        public bool IsInRole(string role)
        {
            return ServiceLocator.Instance.Resolve<IUserService>().IsInRole(Identity, role);
        }

        public bool IsInSite(string siteId)
        {
            return ServiceLocator.Instance.Resolve<IUserService>().IsInSite(Identity, siteId);
        }

        #endregion

        private IUser _info = null;

        public IUser Info
        {
            get
            {
                if (_info == null)
                {
                    _info = ServiceLocator.Instance.Resolve<IUserService>().GetUserInfo(Identity);
                }

                return _info;
            }
        }

        #region events

        public static event EventHandler<PermissionDeniedEventArgs> PermissionDenied;

        public void OnPermissionDenied(PermissionDeniedEventArgs e)
        {
            EventHandler<PermissionDeniedEventArgs> handler = PermissionDenied;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }

    public interface IUser
    {
        string Id { get; }
        string UserName { get; }
        string DisplayName { get; set; }
        string Email { get; set; }
        string Mobile { get; set; }
        bool IsValid { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        string CitizenID { get; set; }
        string Password { get; }
        string DefaultUrl { get; }

        string OrgId { get; set; }

        string AvatorUrl { get; set; }

        int UserType { get; set; }

        string Prop1 { get; set; }
        string Prop2 { get; set; }
        string Prop3 { get; set; }
        string Prop4 { get; set; }
        string Prop5 { get; set; }
        string Prop6 { get; set; }
        string Prop7 { get; set; }
        string Prop8 { get; set; }
        string Prop9 { get; set; }
        string Prop10 { get; set; }
        string Prop11 { get; set; }
        string Prop12 { get; set; }
        string Prop13 { get; set; }
        string Prop14 { get; set; }
        string Prop15 { get; set; }

        string[] GetsRoleId();
        IRole[] GetsRole();

        string[] GetsDeptId();
        IDept[] GetsDept();

        string[] GetsDeptId(bool inherit);
        IDept[] GetsDept(bool inherit);

        string this[string prop] { get; set; }

        int GetPermissionLevel(string moduleId, string instance, string action);

        Dictionary<int, List<string>> GetsRelation();

        ExtendedAttributes ExtAttrs { get; }

        void UpdatePassword(string password);

        void SerializeExtAttrs();
    }

    public interface IPermission
    {
        string Id { get; }

        string ModuleId { get; set; }

        string Instance { get; set; }
        long Flag { get; set; }

        string ResId { get; set; }
        int ResType { get; set; }

        string SiteId { get; set; }
        /// <summary>
        /// 权限继承级别
        /// </summary>
        int Level { get; set; }

        bool Check(long flag);

        string Title { get; }
    }

    public interface IModule
    {
        string Id { get; }
        string Name { get; }
        string Title { get; }
        string PermissionTitles { get; }
        string PermissionNames { get; }
        string PermissionFlags { get; }

        List<string> Titles { get; }
        List<string> Names { get; }
        List<string> Flags { get; }

        int TotalFlag { get; }
    }

    public interface IRole
    {
        string Id { get; }
        string Title { get; }
        string DefaultUrl { get; }

        int SortOrder { get; }

        void SerializeExtAttrs();
    }

    public interface IDept
    {
        string Id { get; }
        string OrgId { get; set; }
        string Title { get; }
        string ParentId { get; }

        string C0 { get; set; }
        string C1 { get; set; }
        string C2 { get; set; }
        string C3 { get; set; }
        string C4 { get; set; }
        string C5 { get; set; }
        string C6 { get; set; }
        string C7 { get; set; }
        string C8 { get; set; }
        string C9 { get; set; }
        string C10 { get; set; }
        string C11 { get; set; }
        string C12 { get; set; }
        string C13 { get; set; }
        string C14 { get; set; }
        string C15 { get; set; }
        string C16 { get; set; }
        string C17 { get; set; }
        string C18 { get; set; }
        string C19 { get; set; }
        string C20 { get; set; }

        string this[string prop] { get; set; }
        ExtendedAttributes ExtAttrs { get; }

        void SerializeExtAttrs();
    }

    public interface IGroup
    {
        string Id { get; }
        string OrgId { get; set; }
        string Title { get; set; }
        string CreatorId { get; set; }
        int ResType { get; set; }
        int SecurityType { get; set; }
        int? Status { get; set; }

        string C0 { get; set; }
        string C1 { get; set; }
        string C2 { get; set; }
        string C3 { get; set; }
        string C4 { get; set; }
        string C5 { get; set; }
        string C6 { get; set; }
        string C7 { get; set; }
        string C8 { get; set; }
        string C9 { get; set; }
        string C10 { get; set; }
        string C11 { get; set; }
        string C12 { get; set; }
        string C13 { get; set; }
        string C14 { get; set; }
        string C15 { get; set; }
        string C16 { get; set; }
        string C17 { get; set; }
        string C18 { get; set; }
        string C19 { get; set; }
        string C20 { get; set; }

        string this[string prop] { get; set; }
        ExtendedAttributes ExtAttrs { get; }

        void SerializeExtAttrs();
    }

    public interface ISite
    {
        string Id { get; }
        string OrgId { get; set; }
        string CreatorId { get; set; }
        string Title { get; set; }
        string Authority { get; }

        string C0 { get; set; }
        string C1 { get; set; }
        string C2 { get; set; }
        string C3 { get; set; }
        string C4 { get; set; }
        string C5 { get; set; }
        string C6 { get; set; }
        string C7 { get; set; }
        string C8 { get; set; }
        string C9 { get; set; }
        string C10 { get; set; }
        string C11 { get; set; }
        string C12 { get; set; }
        string C13 { get; set; }
        string C14 { get; set; }
        string C15 { get; set; }
        string C16 { get; set; }
        string C17 { get; set; }
        string C18 { get; set; }
        string C19 { get; set; }
        string C20 { get; set; }

        string this[string prop] { get; set; }

        ExtendedAttributes ExtAttrs { get; }

        void SerializeExtAttrs();
    }

    public interface IUserRelation
    {
        string Id { get; }
        string UserId { get; set; }
        string ResId { get; set; }
        int ResType { get; set; }
        int SortOrder { get; set; }
        bool IsDirect { get; set; }
        string RefTo { get; set; }
        int UserType { get; set; }
        int UserType1 { get; set; }
    }

    /// <summary>
    /// PermissionDenied EventArgs
    /// </summary>
    public class PermissionDeniedEventArgs : EventArgs
    {
        public static readonly new PermissionDeniedEventArgs Empty = new PermissionDeniedEventArgs();

        public string Permission { get; private set; }

        public PermissionDeniedEventArgs()
        {
        }

        public PermissionDeniedEventArgs(string permission)
        {
            Permission = permission;
        }
    }
}
