using KorgiBot.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KorgiBot.Database
{
    public interface IDatabaseManager
    {
        void AddOrUpdateTableDB<T>(T table) where T : ITable;
        void AddTableDB<T>(T table) where T : ITable;
        Task<List<T>> GetMultyDataDB<T>() where T : ITable;
        Task<List<T>> GetMultyDataDB<T>(Expression<Func<T, bool>> selector) where T : ITable;
        Task<List<T>> GetMultyDataDBAsc<T, TOrder>(Expression<Func<T, bool>> selector, Expression<Func<T, TOrder>> orderSelector) where T : ITable;
        Task<List<T>> GetMultyDataDBAsc<T, TOrder>(Expression<Func<T, TOrder>> orderSelector) where T : ITable;
        Task<List<T>> GetMultyDataDBDesc<T, TOrder>(Expression<Func<T, bool>> selector, Expression<Func<T, TOrder>> orderSelector) where T : ITable;
        Task<List<T>> GetMultyDataDBDesc<T, TOrder>(Expression<Func<T, TOrder>> orderSelector) where T : ITable;
        Task<T> GetTableDB<T>() where T : ITable;
        Task<T> GetTableDB<T>(Expression<Func<T, bool>> selector) where T : ITable;
        Task RemoveTable<T>(T table) where T : ITable;
    }
}