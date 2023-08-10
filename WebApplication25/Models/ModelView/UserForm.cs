namespace WebApplication25.Models.ModelView
{
    public class UserForm
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? SamAccountName { get; set; }
        public IEnumerable<string>? RoleForm { get; set; }
    }
}
