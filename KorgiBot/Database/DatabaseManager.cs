using Chloe.SQLite;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using KorgiBot.Database.Tables;

namespace KorgiBot.Database
{
    public class DatabaseManager<TDatabaseConnector> : IDatabaseManager where TDatabaseConnector : IDatabaseConnector
    {
        private SQLiteContext _databaseContext;
        private IDatabaseConnector _databaseConnector;

        public DatabaseManager(TDatabaseConnector databaseConnector)
        {
            _databaseConnector = databaseConnector;
        }

        public void Initialize()
        {
            _databaseContext = _databaseConnector.GetDBContext();
        }

        public void AddOrUpdateTableDB<T>(T table) where T : ITable
        {
            int results = _databaseContext.Update(table);

            if (results == 0)
            {
                AddTableDB(table);
            }
            else
            {
                _databaseContext.Update(table);
            }
        }

        public void AddTableDB<T>(T table) where T : ITable
        {
            _databaseContext.Insert(table);
        }

        public async Task RemoveTable<T>(T table) where T : ITable
        {
            await _databaseContext.DeleteAsync(table);
        }

        public async Task<List<T>> GetMultyDataDB<T>() where T : ITable
        {
            return await _databaseContext.Query<T>().ToListAsync();
        }

        public async Task<List<T>> GetMultyDataDB<T>(Expression<Func<T, bool>> selector) where T : ITable
        {
            return await _databaseContext.Query<T>().Where(selector).ToListAsync();
        }

        public async Task<List<T>> GetMultyDataDBAsc<T, TOrder>(Expression<Func<T, TOrder>> orderSelector) where T : ITable
        {
            return await _databaseContext.Query<T>().OrderBy(orderSelector).ToListAsync();
        }

        public async Task<List<T>> GetMultyDataDBDesc<T, TOrder>(Expression<Func<T, TOrder>> orderSelector) where T : ITable
        {
            return await _databaseContext.Query<T>().OrderByDesc(orderSelector).ToListAsync();
        }

        public async Task<List<T>> GetMultyDataDBAsc<T, TOrder>(Expression<Func<T, bool>> selector, Expression<Func<T, TOrder>> orderSelector) where T : ITable
        {
            return await _databaseContext.Query<T>().Where(selector).OrderBy(orderSelector).ToListAsync();
        }

        public async Task<List<T>> GetMultyDataDBDesc<T, TOrder>(Expression<Func<T, bool>> selector, Expression<Func<T, TOrder>> orderSelector) where T : ITable
        {
            return await _databaseContext.Query<T>().Where(selector).OrderByDesc(orderSelector).ToListAsync();
        }

        public async Task<T> GetTableDB<T>(Expression<Func<T, bool>> selector) where T : ITable
        {
            return await _databaseContext.Query<T>().FirstOrDefaultAsync(selector);
        }

        public async Task<T> GetTableDB<T>() where T : ITable
        {
            return await _databaseContext.Query<T>().FirstAsync();
        }
    }
}
