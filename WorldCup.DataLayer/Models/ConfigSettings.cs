namespace WorldCup.Data.Models;

public class ConfigSettings
{
    public string DataSource { get; set; } = "api";
    public string Gender { get; set; } = "men";
    public string Language { get; set; } = "en";
    public string FavoriteTeam { get; set; } = "CRO";
    public string Resolution { get; set; } = "1024x768";

}
