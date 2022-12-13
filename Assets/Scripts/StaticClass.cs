public static class StaticClass
{
    private static float influenceValue = 0.5f;
    private static string apiURL = "http://141.145.207.63:3000/api/ladder";
    private static string nameUser = "Unknown user";

    public static float InfluenceValue { get => influenceValue; set => influenceValue = value; }
    public static string NameUser { get => nameUser; set => nameUser = value; }
    public static string ApiURL { get => apiURL; }
}
