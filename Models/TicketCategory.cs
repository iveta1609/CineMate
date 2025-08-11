namespace CineMate.Models
{
    public enum TicketCategory
    {
        Adult, // 16
        Teen,  // 11
        Kids   // 7
    }

    public static class TicketCategoryExtensions
    {
        public static decimal GetPrice(this TicketCategory cat) =>
            cat switch
            {
                TicketCategory.Adult => 16m,
                TicketCategory.Teen => 11m,
                TicketCategory.Kids => 7m,
                _ => 0m
            };

        public static string DisplayName(this TicketCategory cat) =>
            cat switch
            {
                TicketCategory.Adult => "Adult (над 18)",
                TicketCategory.Teen => "Teen (12–17)",
                TicketCategory.Kids => "Kids (5–10)",
                _ => cat.ToString()
            };
    }
}
