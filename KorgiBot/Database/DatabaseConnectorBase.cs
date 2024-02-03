using Chloe.SQLite;
using System;
using System.IO;

namespace KorgiBot.Database
{
    public abstract class DatabaseConnectorBase : IDatabaseConnector
    {
        public abstract string DatabasePath { get; }

        protected DatabaseConnectorBase() { }

        public virtual SQLiteContext GetDBContext()
        {
            var context = new SQLiteContext(() => new System.Data.SQLite.SQLiteConnection($"Data Source={DatabasePath}"));

            CreateDatabaseIfNotExists(context);

            return context;
        }

        private void CreateDatabaseIfNotExists(SQLiteContext context)
        {
            try
            {
                if (File.Exists(DatabasePath)) return;

                File.Create(DatabasePath).Close();
			}
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
