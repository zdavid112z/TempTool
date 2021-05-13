

namespace CloudAPI
{
    public class CloudAPIManager
    {
        private static CloudAPIManager sInstance = null;

        public static CloudAPIManager GetInstance()
        {
            if (sInstance == null)
                sInstance = new CloudAPIManager();
            return sInstance;
        }

        private CloudAPIManager()
        {
            cloud = CloudAPIFactory.CreateDefaultAPI();
        }

        public readonly ICloudAPI cloud;
    }

}
