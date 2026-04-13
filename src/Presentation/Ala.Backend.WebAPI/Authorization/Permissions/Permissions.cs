namespace Ala.Backend.WebAPI.Authorization.Permissions
{
    public static class Permissions
    {
        public static class Users
        {
            public const string View = "Users.View";
            public const string List = "Users.List";
            public const string Create = "Users.Create";
            public const string Update = "Users.Update";
            public const string Delete = "Users.Delete";
            public const string Lock = "Users.Lock";
            public const string Unlock = "Users.Unlock";
            public const string Reactivate = "Users.Reactivate";
        }

        public static class Roles
        {
            public const string View = "Roles.View";
            public const string List = "Roles.List";
            public const string Create = "Roles.Create";
            public const string Update = "Roles.Update";
            public const string Delete = "Roles.Delete";
        }
    }
}