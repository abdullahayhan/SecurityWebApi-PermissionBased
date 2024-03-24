namespace Common.Authorization;

// Bu sınıf, rolleri belirli gruplara ayırır.Örneğin, 'SystemAccess' bir sistem yöneticisi rolünü, 
// 'ManagementHierarchy' ise yönetim hiyerarşisi rollerini temsil edebilir.
// Bu gruplandırma, rol yönetimini ve roller arası ilişkileri düzenlemeyi kolaylaştırır.

public static class AppRoleGroup
{
    public const string SystemAccess = nameof(SystemAccess);
    public const string ManagementHierarchy = nameof(ManagementHierarchy);
}
