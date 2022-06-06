using MetalCore.CQS.Sample.Core.UserContext;

namespace MetalCore.CQS.Sample.UserContext
{
    public class MyUserContext : IUserContext
    {
        public string UserName { get; set; }
        public string Language { get; set; }
    }
}
