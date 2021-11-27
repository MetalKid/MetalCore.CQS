namespace MetalCore.CQS.Sample.UserContext
{
    public interface IUserContext
    {
        string UserName { get; }
        string Language { get; }
    }

    public class MyUserContext : IUserContext
    {
        public string UserName { get; set; }
        public string Language { get; set; }
    }
}
