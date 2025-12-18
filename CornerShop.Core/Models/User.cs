using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CornerShop.Core.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        //public bool IsAdmin { get; set; } = false;
        [BsonRepresentation(BsonType.String)]
        public UserRole Role { get; set; } = UserRole.Customer;

        [BsonRepresentation(BsonType.String)]
        public UserLevel Level { get; set; } = UserLevel.Standard;

        public List<CartItem> Cart { get; set; } = new List<CartItem>();

        [BsonIgnore]
        public decimal DiscountRate
        {
            get
            {
                if (Role == UserRole.Admin)
                    return 0m;

                return Level switch
                {
                    UserLevel.Gold => 0.15m,
                    UserLevel.Silver => 0.10m,
                    UserLevel.Bronze => 0.05m,
                    _ => 0m
                };
            }
        }
        [BsonIgnore]
        public bool IsAdmin
        {
            get
            {
                return Role == UserRole.Admin;
            }
        }


      
    }
}
