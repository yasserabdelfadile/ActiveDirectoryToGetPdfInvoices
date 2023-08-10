namespace WebApplication25.Models.ModelView
{
    public class RoleUserView
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public List<SiteView>? sites { get; set; }
    }
}
