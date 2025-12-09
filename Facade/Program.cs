using System;
using System.Collections.Generic;

namespace Facade
{
    public class UserDatabase
    {
        private Dictionary<int, User> _users = new Dictionary<int, User>
    {
        { 1, new User { Id = 1, Name = "Иван Иванов", Email = "ivan@mail.com", Phone = "+79001112233" } },
        { 2, new User { Id = 2, Name = "Петр Петров", Email = "petr@mail.com", Phone = "+79002223344" } },
        { 3, new User { Id = 3, Name = "Мария Сидорова", Email = "maria@mail.com", Phone = "+79003334455" } }
    };

        public User GetUser(int id)
        {
            Console.WriteLine($"[UserDatabase] Запрос пользователя с ID={id}");
            if (_users.ContainsKey(id))
                return _users[id];
            return null;
        }

        public void AddUser(User user)
        {
            Console.WriteLine($"[UserDatabase] Добавление пользователя: {user.Name}");
            _users[user.Id] = user;
        }

        public void UpdateUser(User user)
        {
            Console.WriteLine($"[UserDatabase] Обновление пользователя: {user.Name}");
            if (_users.ContainsKey(user.Id))
                _users[user.Id] = user;
        }

        public bool DeleteUser(int id)
        {
            Console.WriteLine($"[UserDatabase] Удаление пользователя с ID={id}");
            return _users.Remove(id);
        }

        public List<User> SearchUsers(string keyword)
        {
            Console.WriteLine($"[UserDatabase] Поиск пользователей по: {keyword}");
            var results = new List<User>();
            foreach (var user in _users.Values)
            {
                if (user.Name.Contains(keyword) || user.Email.Contains(keyword))
                    results.Add(user);
            }
            return results;
        }
    }

    public class OrderDatabase
    {
        private Dictionary<int, Order> _orders = new Dictionary<int, Order>
    {
        { 1001, new Order { Id = 1001, UserId = 1, TotalAmount = 14999.99m, Status = "Доставлен", CreatedDate = DateTime.Now.AddDays(-7) } },
        { 1002, new Order { Id = 1002, UserId = 1, TotalAmount = 5999.50m, Status = "В обработке", CreatedDate = DateTime.Now.AddDays(-3) } },
        { 1003, new Order { Id = 1003, UserId = 2, TotalAmount = 25999.00m, Status = "Оплачен", CreatedDate = DateTime.Now.AddDays(-1) } }
    };

        public Order GetOrder(int id)
        {
            Console.WriteLine($"[OrderDatabase] Запрос заказа с ID={id}");
            if (_orders.ContainsKey(id))
                return _orders[id];
            return null;
        }

        public List<Order> GetUserOrders(int userId)
        {
            Console.WriteLine($"[OrderDatabase] Запрос заказов пользователя с ID={userId}");
            var results = new List<Order>();
            foreach (var orderItem in _orders.Values)
            {
                if (orderItem.UserId == userId)
                    results.Add(orderItem);
            }
            return results;
        }

        public void CreateOrder(Order order)
        {
            Console.WriteLine($"[OrderDatabase] Создание заказа #{order.Id} для пользователя {order.UserId}");
            _orders[order.Id] = order;
        }

        public bool UpdateOrderStatus(int orderId, string newStatus)
        {
            Console.WriteLine($"[OrderDatabase] Обновление статуса заказа #{orderId} на '{newStatus}'");
            if (_orders.ContainsKey(orderId))
            {
                _orders[orderId].Status = newStatus;
                return true;
            }
            return false;
        }

        public List<Order> GetOrdersByStatus(string status)
        {
            Console.WriteLine($"[OrderDatabase] Поиск заказов со статусом: {status}");
            var results = new List<Order>();
            foreach (var orderItem in _orders.Values)
            {
                if (orderItem.Status == status)
                    results.Add(orderItem);
            }
            return results;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public override string ToString()
        {
            return $"Пользователь #{Id}: {Name} ({Email})";
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public override string ToString()
        {
            return $"Заказ #{Id}: сумма {TotalAmount:C}, статус: {Status}, дата: {CreatedDate:dd.MM.yyyy}";
        }
    }

    public class UserOrderInfo
    {
        public User User { get; set; }
        public List<Order> Orders { get; set; }
        public int TotalOrders => Orders?.Count ?? 0;
        public decimal TotalSpent { get; set; }

        public override string ToString()
        {
            return $"{User}\nВсего заказов: {TotalOrders}\nОбщая сумма: {TotalSpent:C}";
        }
    }

    public class DatabaseFacade
    {
        private UserDatabase _userDb;
        private OrderDatabase _orderDb;

        public DatabaseFacade()
        {
            _userDb = new UserDatabase();
            _orderDb = new OrderDatabase();
        }

        public User GetUser(int userId)
        {
            return _userDb.GetUser(userId);
        }

        public Order GetOrder(int orderId)
        {
            return _orderDb.GetOrder(orderId);
        }

        public UserOrderInfo GetUserFullInfo(int userId)
        {
            var user = _userDb.GetUser(userId);
            if (user == null)
                return null;

            var orders = _orderDb.GetUserOrders(userId);
            var totalSpent = 0m;
            foreach (var orderItem in orders)
            {
                totalSpent += orderItem.TotalAmount;
            }

            return new UserOrderInfo
            {
                User = user,
                Orders = orders,
                TotalSpent = totalSpent
            };
        }

        public bool CreateOrderForUser(int userId, decimal amount)
        {
            var user = _userDb.GetUser(userId);
            if (user == null)
                return false;

            int newOrderId = 1000 + new Random().Next(1000, 9999);
            var newOrder = new Order
            {
                Id = newOrderId,
                UserId = userId,
                TotalAmount = amount,
                Status = "Новый",
                CreatedDate = DateTime.Now
            };

            _orderDb.CreateOrder(newOrder);
            return true;
        }

        public List<UserOrderInfo> GetUsersWithActiveOrders()
        {
            var result = new List<UserOrderInfo>();

            var activeOrders = _orderDb.GetOrdersByStatus("В обработке");
            var paidOrders = _orderDb.GetOrdersByStatus("Оплачен");
            activeOrders.AddRange(paidOrders);

            var processedUsers = new HashSet<int>();
            foreach (var orderItem in activeOrders)
            {
                if (!processedUsers.Contains(orderItem.UserId))
                {
                    processedUsers.Add(orderItem.UserId);
                    var userInfo = GetUserFullInfo(orderItem.UserId);
                    result.Add(userInfo);
                }
            }

            return result;
        }

        public List<UserOrderInfo> SearchUsersAndOrders(string keyword)
        {
            var result = new List<UserOrderInfo>();

            var foundUsers = _userDb.SearchUsers(keyword);

            foreach (var user in foundUsers)
            {
                var userInfo = GetUserFullInfo(user.Id);
                result.Add(userInfo);
            }

            return result;
        }

        public bool DeleteUserAndOrders(int userId)
        {
            var userOrders = _orderDb.GetUserOrders(userId);

            bool userDeleted = _userDb.DeleteUser(userId);

            return userDeleted;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            DatabaseFacade dbFacade = new DatabaseFacade();

            Console.WriteLine("1. Полная информация о пользователе:");
            var userInfo = dbFacade.GetUserFullInfo(1);
            Console.WriteLine(userInfo);
            Console.WriteLine("Заказы пользователя:");
            foreach (var orderItem in userInfo.Orders)
            {
                Console.WriteLine($"  {orderItem}");
            }
            Console.WriteLine();

            Console.WriteLine("2. Создание нового заказа:");
            bool orderCreated = dbFacade.CreateOrderForUser(2, 12999.99m);
            Console.WriteLine($"Заказ создан: {orderCreated}\n");

            Console.WriteLine("3. Пользователи с активными заказами:");
            var activeUsers = dbFacade.GetUsersWithActiveOrders();
            foreach (var activeUser in activeUsers)
            {
                Console.WriteLine($"- {activeUser.User.Name}: {activeUser.TotalOrders} активных заказов");
            }
            Console.WriteLine();

            Console.WriteLine("4. Результаты поиска 'Иван':");
            var searchResults = dbFacade.SearchUsersAndOrders("Иван");
            foreach (var result in searchResults)
            {
                Console.WriteLine($"- {result.User.Name}, заказов: {result.TotalOrders}");
            }
            Console.WriteLine();

            Console.WriteLine("5. Простые операции:");
            var user = dbFacade.GetUser(3);
            Console.WriteLine($"Найден пользователь: {user}");

            var order = dbFacade.GetOrder(1001);
            Console.WriteLine($"Найден заказ: {order}");
            Console.WriteLine();

            Console.WriteLine("6. Удаление пользователя:");
            bool deleted = dbFacade.DeleteUserAndOrders(3);
            Console.WriteLine($"Пользователь удален: {deleted}");
        }
    }
}
