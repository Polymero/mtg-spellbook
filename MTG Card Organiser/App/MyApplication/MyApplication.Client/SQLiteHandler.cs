using SQLite;

namespace MyApplication.Client
{
    public class DatabaseHandler
    {
        private SQLiteAsyncConnection _db;

        #pragma warning disable CS8618
        public DatabaseHandler() {}
        #pragma warning restore CS8618

        async Task Init()
        {
            if (_db is not null)
                return;

            _db = new SQLiteAsyncConnection(DatabaseConstants.DatabaseFilepath, DatabaseConstants.Flags);
            await _db.CreateTableAsync<AppBinder>();
            await _db.CreateTableAsync<AppDeck>();
        }

        public async Task<List<AppBinder>> GetCollection()
        {
            await Init();
            return await _db.Table<AppBinder>().ToListAsync();
        }

        public async Task<AppBinder> GetBinder(int id)
        {
            await Init();
            return await _db.Table<AppBinder>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveBinder(AppBinder item)
        {
            await Init();
            if (item.Id != 0)
                return await _db.UpdateAsync(item);
            else
                return await _db.InsertAsync(item);
        }

        public async Task<int> DeleteBinder(AppBinder item)
        {
            await Init();
            return await _db.DeleteAsync(item);
        }

        public async Task<List<AppDeck>> GetAllDecks()
        {
            await Init();
            return await _db.Table<AppDeck>().ToListAsync();
        }

        public async Task<AppDeck> GetDeck(int id)
        {
            await Init();
            return await _db.Table<AppDeck>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveDeck(AppDeck item)
        {
            await Init();
            if (item.Id != 0)
                return await _db.UpdateAsync(item);
            else
                return await _db.InsertAsync(item);
        }

        public async Task<int> DeleteDeck(AppDeck item)
        {
            await Init();
            return await _db.DeleteAsync(item);
        }
    }
}