using Microsoft.EntityFrameworkCore;
using LostAndFound.Models;

namespace LostAndFound.Data
{
    public class LostAndFoundContext : DbContext
    {
        public LostAndFoundContext(DbContextOptions<LostAndFoundContext> options) : base(options) { }

        public DbSet<LostItemList> Lost_Item_Lists { get; set; }

        public DbSet<FoundItemList> Found_Item_Lists { get; set; }

        public DbSet<ClaimRequests> ClaimRequests { get; set; }

        public DbSet<AdminAccount> Admins { get; set; }
    }
}
