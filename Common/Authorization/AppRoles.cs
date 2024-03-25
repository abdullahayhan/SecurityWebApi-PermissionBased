using System.Collections.ObjectModel;

namespace Common.Authorization;

// Bu sınıf, projedeki belirli rolleri temsil eder. 
// 'Admin' ve 'Basic' gibi roller, farklı kullanıcı tiplerine veya yetkilendirme seviyelerine atanan rolleri ifade edebilir. 
// Roller, genellikle belirli özelliklere veya eylemlere erişim düzeylerini kontrol etmek için kullanılır.

public static class AppRoles
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);
    public const string EmployeeRead = nameof(EmployeeRead);

    // bir koleksiyonun salt okunur (read-only) bir görünümünü sağlar.
    public static IReadOnlyList<string> DefaultRoles { get; }
        = new ReadOnlyCollection<string>(new[]
    {
        Admin,
        Basic,
        EmployeeRead
    });

    public static bool IsDefault(string roleName)
        => DefaultRoles.Any(r => r == roleName);
}
