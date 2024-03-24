namespace Common.Authorization;


// Bu sınıf, temel eylemleri temsil eder.
// Bu eylemler, genellikle belirli bir özelliğe(örneğin, kullanıcıları yönetme) ilişkilendirilen eylemleri içerir. 
// 'Read', 'Create', 'Delete' ve 'Update' gibi eylemler, genellikle veri işleme operasyonlarını temsil eder.

public static class AppAction
{
    public const string Read = nameof(Read);
    public const string Create = nameof(Create);
    public const string Delete = nameof(Delete);
    public const string Update = nameof(Update);
} 
