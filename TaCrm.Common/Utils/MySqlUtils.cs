namespace TaCrm.Common.Utils;

public static class MySqlUtils
{
    public static string CreateMySqlDapperColSelect(Type dapperTableMap)
    {
        var properties = dapperTableMap.GetProperties();
        return String.Join(", ", properties.Select(propertyInfo => $"`{propertyInfo.Name}`"));
    }
}