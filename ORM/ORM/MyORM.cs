using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORM
{
    
    class MyORM <T> where T : IData
    {
        private  SqlConnection _sqlConnection;
        public MyORM(SqlConnection connection)
        {
            _sqlConnection = connection;
        }
        public MyORM(string connectionString)
          : this(new SqlConnection(connectionString))
        {

        }
        private void InsertRecursively(object obj)
        {
            if (obj == null)
                return;
            var sql = new StringBuilder("Insert into ");
            var val = new StringBuilder(") values (");
            var objType = obj.GetType();
            sql.Append(objType.Name);
            sql.Append('(');
            var properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);
                var elems = propValue as IList;
                if (elems != null)
                {
                    foreach (var item in elems)
                    {
                        InsertRecursively(item);
                    }
                }
                else
                {
                    sql.Append(' ').Append(property.Name).Append(',');
                    val.Append($"'{propValue}'").Append(',');
                }
            }
            sql.Remove(sql.Length - 1, 1);
            val.Remove(val.Length - 1, 1);
            val.Append(");");
            var query = sql.ToString() + " " + val.ToString();
            ExecuteInsertUpdateDelete(query);
           
        }
        public void Insert(object obj)
        {
            InsertRecursively(obj);
            Console.WriteLine("Inserted!");
        }

        public void Delete(object obj)
        {
            DeleteRecursively(obj);
            Console.WriteLine("Deleted!");
        }
        private void DeleteRecursively(object obj)
        {
            if (obj == null)
                return;
            var sql = new StringBuilder("DELETE FROM ");
            var objType = obj.GetType();

            sql.Append(objType.Name);
            var properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);
                var elems = propValue as IList;
                if (elems != null)
                {
                    foreach (var item in elems)
                    {
                        DeleteRecursively(item);
                    }
                }
                else
                {
                    if (property.Name == "Id")
                    {
                        sql.Append($" WHERE Id = {propValue};");
                    }
                }
            }
            var query = sql.ToString();
            ExecuteInsertUpdateDelete(query);
        }
        public void Delete(int id)
        {
            var sql = new StringBuilder("DELETE FROM ");
            var type = typeof(T);
            sql.Append(type.Name).Append($" WHERE Id = {id};");
            var query = sql.ToString();
            ExecuteInsertUpdateDelete(query);
            Console.WriteLine("Deleted!");
        }
        public void Update(object obj)
        {
            UpdateRecursively(obj);
            Console.WriteLine("Updated!");
        }
        public void UpdateRecursively(object obj)
        {
            if (obj == null)
                return;
            var temp = new StringBuilder(" WHERE Id = ");
            var sql = new StringBuilder("UPDATE ");

            var objType = obj.GetType();
            sql.Append(objType.Name).Append(" SET");
            var properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);
                var elems = propValue as IList;
                if (elems != null)
                {
                    foreach (var item in elems)
                    {
                        UpdateRecursively(item);
                    }
                }
                else
                {
                    if (property.Name == "Id")
                        temp.Append($"{propValue};");
                    sql.Append($" {property.Name} = \'{propValue}\',");
                }
            }
            sql.Remove(sql.Length - 1, 1);
            var query = sql.ToString() + temp.ToString();
            ExecuteInsertUpdateDelete(query);
        }
        public T GetById(int id)
        {
            var sql = new StringBuilder("SELECT * FROM ");
            var type = typeof(T);
            sql.Append(type.Name).Append($" WHERE Id = {id};");
            var query = sql.ToString();

            using SqlCommand command = new SqlCommand(query, _sqlConnection);
            var reader = command.ExecuteReader();
            var properties = type.GetProperties();
            T obj = Activator.CreateInstance<T>();
            while (reader.Read())
            {
                int i = 0;
                foreach (var property in properties)
                {
                    if(!property.PropertyType.IsGenericType)
                    {
                        property.SetValue(obj, reader.GetValue(i), null);
                        i++;
                    }
                }
            }
            return obj;
        }
        private void ExecuteInsertUpdateDelete(string query)
        {
            using var command = new SqlCommand(query, _sqlConnection);
            command.ExecuteNonQuery();
        }

        public void GetAll(T item)
        {
            var type = item.GetType();
            GetByRecursive(type);
        }
        private void GetData(Type type)
        {
            StringBuilder x = new StringBuilder("");
            PropertyInfo[] properties = type.GetProperties();
            if (!type.IsGenericType)
            {
                Console.WriteLine($"Getting Data From--> {type.Name} Table");
                var sql = new StringBuilder("SELECT * FROM ");
                sql.Append(type.Name).Append(';');
                var query = sql.ToString();
                using SqlCommand command = new SqlCommand(query, _sqlConnection);
                var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    var obj = Activator.CreateInstance(type);
                    int i = 0;
                    foreach (var property in properties)
                    {
                        if (!property.PropertyType.IsGenericType)
                        {
                            Console.WriteLine($"{property.Name}: {reader.GetValue(i)}");
                            property.SetValue(obj, reader.GetValue(i), null);
                            i++;
                        }    
                    }
                }  
                reader.Close();
            }
        }

        private void GetByRecursive(Type type)
        {
            PropertyInfo[] pi = type.GetProperties();
            GetData(type);
            foreach (PropertyInfo p in pi)
            {
                if (!p.PropertyType.IsPrimitive && p.PropertyType.FullName != "System.String")
                {
                    GetByRecursive(p.PropertyType);
                }
            }
        }
    }
}
