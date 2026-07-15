namespace Application.Constants
{
    public static class ApiRoutes
    {
        public const string Base = "api";

        public static class Products
        {
            public const string GetAll = Base + "/products";

            public const string GetById = Base + "/products/{id:int}";

            public const string Create = Base + "/products";

            public const string Update = Base + "/products/{id:int}";

            public const string Delete = Base + "/products/{id:int}";
        }

        public static class Auth
        {
            public const string Register = Base + "/auth/register";

            public const string Login = Base + "/auth/login";
        }
    }
}