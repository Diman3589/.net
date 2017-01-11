using Domain;

namespace Orm
{
    using System.Data.Entity;

    public class VkContext : DbContext
    {
        public VkContext()
            : base("name=vk")
        {
            //Database.SetInitializer<VkContext>(null);
        }

        public DbSet<VkUser> Users { get; set; }

        public DbSet<VkPost> Posts { get; set; }
    }
}