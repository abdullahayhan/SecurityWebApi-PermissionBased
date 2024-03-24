namespace Common.Authorization;

// Bu sınıf, projedeki belirli özellikleri temsil eder.
// Örneğin, 'Employees' özelliği çalışan verilerine erişim yetkisini, 'Users' kullanıcı verilerine erişim yetkisini eder. 
// Bu özelliklerin belirtilmesi, erişim kontrolünü ve yetkilendirme işlemlerini yönetmeye yardımcı olur.


public static class AppFeature
{
    public const string Employees = nameof(Employees);
    public const string Users = nameof(Users);
    public const string Roles = nameof(Roles);
    public const string UserRoles = nameof(UserRoles); 
    public const string RoleClaims = nameof(RoleClaims);
}
