namespace IPWebsiteMVC.Models;

public class Land
{
    public int LandID { get; set; }
    public string Name { get; set; } = "";
    public List<Autor> Autoren { get; set; } = [];
}
