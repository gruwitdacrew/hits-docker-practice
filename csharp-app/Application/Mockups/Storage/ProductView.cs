using System.ComponentModel.DataAnnotations;

namespace Mockups.Storage
{
    public class CartAddition
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MenuItemId { get; set; }

        public MenuItem MenuItem { get; set; }

        public DateTime AdditionDate { get; set; }

        public string? UserId { get; set; }  // Could be null for anonymous users

        public string? SessionId { get; set; }  // To track anonymous users by session

        public string? IpAddress { get; set; }  // To help identify unique users
    }
}