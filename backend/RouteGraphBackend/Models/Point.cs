public class Point
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Height { get; set; }

    // Внешний ключ на маршрут
    public int RouteId { get; set; }
    public Route Route { get; set; }
}
