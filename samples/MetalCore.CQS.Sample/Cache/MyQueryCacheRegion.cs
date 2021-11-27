using MetalCore.CQS.Query;
using MetalCore.CQS.Sample.UserContext;

namespace MetalCore.CQS.Sample.Cache
{
    public class MyQueryCacheRegion : QueryCacheRegion
    {
        private readonly IUserContext _userContext;

        public MyQueryCacheRegion(IUserContext userContext)
        {
            _userContext = userContext;
        }

        protected override string GetLanguage()
        {
            return _userContext.Language;
        }

        protected override string GetUserName()
        {
            return _userContext.UserName;
        }
    }
}
