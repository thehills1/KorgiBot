﻿using Chloe;
using Chloe.Descriptors;
using Chloe.Infrastructure;
using Chloe.Reflection;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace KorgiBot.Database
{
    public static class TableHelper
    {
        public static void InitTable<TEntity>(DbContext context)
        {
            InitTable(typeof(TEntity), context);
        }

        public static void InitTable(Type entityType, DbContext context)
        {
            var typeDescriptor = EntityTypeContainer.GetDescriptor(entityType);
            string createTableScript = CreateTableScript(typeDescriptor);

            context.Session.ExecuteNonQuery(createTableScript);
        }

        static string CreateTableScript(TypeDescriptor typeDescriptor)
        {
            string tableName = typeDescriptor.Table.Name;

            StringBuilder sb = new StringBuilder();
            sb.Append($"CREATE TABLE IF NOT EXISTS {QuoteName(tableName)}(");

            string c = "";
            foreach (var propertyDescriptor in typeDescriptor.PrimitivePropertyDescriptors)
            {
                sb.AppendLine(c);
                sb.Append($"  {BuildColumnPart(propertyDescriptor)}");
                c = ",";
            }

            if (typeDescriptor.PrimaryKeys.Count > 0)
            {
                string key = string.Join(", ",
                    typeDescriptor.PrimaryKeys.Select(propertyDescriptor =>
                    QuoteName(propertyDescriptor.Column.Name, propertyDescriptor.IsAutoIncrement)));

                sb.AppendLine(c);
                sb.Append($"PRIMARY KEY ({key})");
            }

            sb.AppendLine();
            sb.AppendLine(");");

            return sb.ToString();
        }

        static string QuoteName(string name, bool autoIncrement = false)
        {
            return string.Concat("\"", name.ToLower(), "\"", autoIncrement ? " AUTOINCREMENT" : null);
        }

        static string BuildColumnPart(PrimitivePropertyDescriptor propertyDescriptor)
        {
            string part = $"{QuoteName(propertyDescriptor.Column.Name)} {GetMappedDbTypeName(propertyDescriptor)}";

            if (!propertyDescriptor.IsNullable)
            {
                part += " NOT NULL";
            }
            else
            {
                part += " NULL";
            }

            return part;
        }

        static string GetMappedDbTypeName(PrimitivePropertyDescriptor propertyDescriptor)
        {
            Type type = propertyDescriptor.PropertyType.GetUnderlyingType();
            if (type.IsEnum)
            {
                type = type.GetEnumUnderlyingType();
            }

            if (type == typeof(bool))
            {
                return "NUMERIC";
            }

            if (type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong))
            {
                return "INTEGER";
            }

            if (type == typeof(float)
                || type == typeof(decimal)
                || type == typeof(double))
            {
                return "REAL";
            }

            if (type == typeof(Guid)
                || type == typeof(string)
                || type == typeof(DateTime)
                || propertyDescriptor.Column.DbType == DbType.DateTime)
            {
                return "TEXT";
            }

            throw new NotSupportedException(type.FullName);
        }
    }
}
