namespace LostAndFound.Models
{
    public class AdminDashboardViewModel
    {
        public List<FoundItemList> PendingFoundItems { get; set; }
        public List<FoundItemList> ApprovedFoundItems { get; set; }
        public List<LostItemList> StudentLostReports { get; set; }
        public List<ClaimRequests> ClaimRequests { get; set; }
    }
}
