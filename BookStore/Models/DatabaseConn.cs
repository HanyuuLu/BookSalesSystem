using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class DatabaseConn
    {
        public string ConnectString { get; set; }
        public DatabaseConn(string connectString)
        {
            this.ConnectString = connectString;
        }
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectString);
        }
        public List<Book> GetBooks(string isbn, string title, string author)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                List<Book> res = new List<Book>();
                MySqlCommand command = new MySqlCommand($"select * from book where isbn like '{isbn}%' and author like '{author}%' and title like '{title}%';", connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        res.Add(new Book()
                        {
                            isbn = reader.GetString("isbn"),
                            title = reader.GetString("title"),
                            author = reader.GetString("author"),
                            reserve = reader.GetInt32("reserve")
                        }
                        );
                    }
                }
                return res;
            }
        }
        public Book GetBookInformation(string isbn)
        {
            Book res = new Book();
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"select * from book where isbn = '{isbn}'", connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        res.isbn = reader.GetString("isbn");
                        res.title = reader.GetString("title");
                        res.author = reader.GetString("author");
                        res.reserve = reader.GetInt32("reserve");
                    }
                    else
                    {
                        res = null;
                    }
                }
                return res;
            }
        }
        public User Login(string username)
        {
            User res = new User();
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"select * from user where username = '{username}'", connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        res.uuid = reader.GetInt32("uuid");
                        res.username = reader.GetString("username");
                        res.password = reader.GetString("password");
                        return res;
                    }
                    else
                    {
                        res.uuid = 0;
                        return res;
                    }
                }
            }
        }
        public User Register(string username, string password)
        {
            User res = new User();
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlCommand command_pre = new MySqlCommand($"select * from user where username = '{username}'", connection);
                using (MySqlDataReader reader = command_pre.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        res.uuid = 0;
                        return res;
                    }
                }
                MySqlCommand command = new MySqlCommand($"insert into user (username,password) values('{username}','{password}');", connection);
                int row = command.ExecuteNonQuery();
                if (row > 0)
                {
                    res.uuid = 1;
                    return res;
                }
                else
                {
                    res.uuid = 0;
                    return res;
                }
            }
        }
        public User Order(string uuid,string isbn,int quantity,string payaccount,string address)
        {
            User res = new User();
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"insert into sell (user,isbn,quantity,payaccount,address) values ('{uuid}','{isbn}','{quantity}','{payaccount}','{address}');", connection);
                int row = command.ExecuteNonQuery();
                if (row > 0)
                {
                    res.uuid = 1;
                    MySqlCommand minus =new MySqlCommand($"update book set reserve = reserve - {quantity} where isbn = '{isbn}'",connection);
                    minus.ExecuteNonQuery();
                    return res;
                }
                else
                {
                    res.uuid = 0;
                    return res;
                }
            }
        }
    }
}
