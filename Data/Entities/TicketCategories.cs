namespace CineMate.Data.Entities
{
    public static class TicketCategories
    {
        public static readonly Dictionary<string, decimal> Prices = new()
    {
        { "Adult", 16m },
        { "Teen", 11m },
        { "Kids", 7m }
    };
    }
}