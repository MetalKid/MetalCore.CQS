using MetalCore.CQS.Sample.Core.UserContext;

namespace MetalCore.CQS.Sample.UI.MVC.UserContext
{
    public class MyUserContext : IUserContext
    {
        public string? UserName { get; set; }
        public string? Language { get; set; }
    }
}
