namespace Admin.Services.Endpoints
{
    public class ApiEndpoints
    {
        public string BaseApiUrl { get; set; }
        public UserEndpoints UserEndpoints { get; set; }
        public RoleEndpoints RoleEndpoints { get; set; }
        public TokenEndpoints TokenEndpoints { get; set; }
    }

    public class UserEndpoints
    {
        public string Register { get; set; }
        public string GetById { get; set; }
        public string GetAll { get; set; }
        public string Update { get; set; }
        public string ChangePassword { get; set; }
        public string ChangeStatus { get; set; }
        public string GetRoles { get; set; }
        public string UpdateRoles { get; set; }

    }

    public class RoleEndpoints
    {
        public string Create { get; set; }
        public string Update { get; set; }
        public string Delete { get; set; }
        public string GetById { get; set; }
        public string GetAll { get; set; }
        public string GetPermissions { get; set; }
        public string UpdatePermissions { get; set; }
    }

    public class TokenEndpoints
    {
        public string GetToken { get; set; }
        public string GetRefreshToken { get; set; }
    }

}
