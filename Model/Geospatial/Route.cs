namespace PHPAPI.Model.Geospatial
{
    public class Route
    {
        public string Geometry { get; set; }
        public List<Leg> Legs { get; set; }
    }
}
