using System.Collections.Generic;
using System.Linq;

namespace Adapter
{
    public class DataManager:IDataManager
    {
        private readonly List<DataEntry> _entries = new List<DataEntry>();

        public string GetCampaign(string user)
        {
            return (from l in _entries where l.User == user select l).FirstOrDefault()?.Campaign;
        }
    }

    public interface IDataManager
    {
        string GetCampaign(string user);
    }

    public class DataEntry
    {
        public string User { get; set; }

        public string Campaign { get; set; }
    }
}
