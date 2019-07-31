using System.Linq;
using System.Collections.Generic;
using RestWCFServiceLibrary.WiiMote.Model;

namespace RestWCFServiceLibrary.WiiMote
{
    public class DataStore
    {
        // locked only for key modification
        private readonly Dictionary<string, AcquisitionFile> store;

        public DataStore()
        {
            store = new Dictionary<string, AcquisitionFile>();
        }

        public AcquisitionFile Get(string yeux)
        {
            lock(store)
            {
                return store[yeux];
            }
        }

        public void Clear(string yeux)
        {
            lock(store)
            {
                store.Remove(yeux);
            }
        }

        internal void Put(string yeux, AcquisitionFile file)
        {
            lock(store)
            {
                store[yeux] = file;
            }
        }

        public IList<string> YeuxDone()
        {
            lock(store)
            {
                return store.Keys.ToList();
            }
        }
    }
}
