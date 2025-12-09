using System;

namespace Bridge
{
    public interface ILogImplementation
    {
        void WriteLog(string message, LogLevel level);
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    public class ConsoleLogImplementation : ILogImplementation
    {
        public void WriteLog(string message, LogLevel level)
        {
            Console.ForegroundColor = GetColorForLevel(level);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{level}] {message}");
            Console.ResetColor();
        }

        private ConsoleColor GetColorForLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Debug => ConsoleColor.Gray,
                _ => ConsoleColor.White
            };
        }
    }

    public class FileLogImplementation : ILogImplementation
    {
        private readonly string _filePath;

        public FileLogImplementation(string filePath)
        {
            _filePath = filePath;
        }

        public void WriteLog(string message, LogLevel level)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n";
            System.IO.File.AppendAllText(_filePath, logEntry);
        }
    }

    public class DatabaseLogImplementation : ILogImplementation
    {
        private readonly string _connectionString;

        public DatabaseLogImplementation(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void WriteLog(string message, LogLevel level)
        {
            Console.WriteLine($"[DB] Сохранено в базу данных: [{level}] {message}");
        }
    }
    public abstract class Logger
    {
        protected ILogImplementation _implementation;

        protected Logger(ILogImplementation implementation)
        {
            _implementation = implementation;
        }

        public void SetImplementation(ILogImplementation implementation)
        {
            _implementation = implementation;
        }

        public abstract void Log(string message);
    }
    public class OrderLogger : Logger
    {
        public OrderLogger(ILogImplementation implementation) : base(implementation)
        {
        }

        public override void Log(string message)
        {
            _implementation.WriteLog($"ЗАКАЗ: {message}", LogLevel.Info);
        }

        public void LogOrderCreated(int orderId, decimal amount)
        {
            Log($"Создан заказ #{orderId} на сумму {amount:C}");
        }

        public void LogOrderStatusChanged(int orderId, string newStatus)
        {
            Log($"Статус заказа #{orderId} изменен на: {newStatus}");
        }
    }

    public class ErrorLogger : Logger
    {
        public ErrorLogger(ILogImplementation implementation) : base(implementation)
        {
        }

        public override void Log(string message)
        {
            _implementation.WriteLog($"ОШИБКА: {message}", LogLevel.Error);
        }

        public void LogException(Exception ex)
        {
            Log($"{ex.GetType().Name}: {ex.Message}");
        }

        public void LogValidationError(string fieldName, string error)
        {
            Log($"Ошибка валидации поля '{fieldName}': {error}");
        }
    }

    public class UserActivityLogger : Logger
    {
        public UserActivityLogger(ILogImplementation implementation) : base(implementation)
        {
        }

        public override void Log(string message)
        {
            _implementation.WriteLog($"АКТИВНОСТЬ: {message}", LogLevel.Info);
        }

        public void LogUserLogin(string username)
        {
            Log($"Пользователь '{username}' вошел в систему");
        }

        public void LogUserLogout(string username)
        {
            Log($"Пользователь '{username}' вышел из системы");
        }

        public void LogUserAction(string username, string action)
        {
            Log($"Пользователь '{username}' выполнил действие: {action}");
        }
    }

    public class DebugLogger : Logger
    {
        public DebugLogger(ILogImplementation implementation) : base(implementation)
        {
        }

        public override void Log(string message)
        {
            _implementation.WriteLog($"ОТЛАДКА: {message}", LogLevel.Debug);
        }

        public void LogMethodCall(string methodName, params object[] parameters)
        {
            string paramsString = parameters.Length > 0 ?
                string.Join(", ", parameters) : "без параметров";
            Log($"Вызван метод {methodName}({paramsString})");
        }

        public void LogVariable(string variableName, object value)
        {
            Log($"Переменная {variableName} = {value}");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Демонстрация паттерна Bridge для логирования ===\n");

            ILogImplementation consoleLog = new ConsoleLogImplementation();
            ILogImplementation fileLog = new FileLogImplementation("log.txt");
            ILogImplementation dbLog = new DatabaseLogImplementation("Server=localhost;Database=Shop;");

            Logger orderLogger = new OrderLogger(consoleLog);
            orderLogger.Log("Система логирования заказов инициализирована");
            ((OrderLogger)orderLogger).LogOrderCreated(1001, 14999.99m);
            ((OrderLogger)orderLogger).LogOrderStatusChanged(1001, "В обработке");

            Console.WriteLine();

            Logger errorLogger = new ErrorLogger(fileLog);
            errorLogger.Log("Обнаружена ошибка в платежной системе");
            ((ErrorLogger)errorLogger).LogValidationError("Email", "Неверный формат");

            errorLogger.SetImplementation(consoleLog);
            ((ErrorLogger)errorLogger).LogException(new InvalidOperationException("Товар не найден"));

            Console.WriteLine();


            Logger userLogger = new UserActivityLogger(dbLog);
            ((UserActivityLogger)userLogger).LogUserLogin("ivan.petrov");
            ((UserActivityLogger)userLogger).LogUserAction("ivan.petrov", "добавил товар в корзину");
            ((UserActivityLogger)userLogger).LogUserLogout("ivan.petrov");

            Console.WriteLine();

            Logger debugLogger = new DebugLogger(consoleLog);
            ((DebugLogger)debugLogger).LogMethodCall("CalculatePrice", 100, 0.2, "RUB");
            ((DebugLogger)debugLogger).LogVariable("totalAmount", 12000);

            Console.WriteLine("\n=== Демонстрация независимости изменений ===");

            Logger logger = new OrderLogger(consoleLog);
            logger.Log("Тестовое сообщение 1 (в консоль)");

            logger.SetImplementation(fileLog);
            logger.Log("Тестовое сообщение 2 (в файл)");

            logger.SetImplementation(dbLog);
            logger.Log("Тестовое сообщение 3 (в базу данных)");

            Console.WriteLine("\n=== Использование в разных частях магазина ===");

            var cartLogger = new UserActivityLogger(consoleLog);
            ((UserActivityLogger)cartLogger).LogUserAction("alex.smith", "добавил 'Ноутбук' в корзину");

            var paymentLogger = new ErrorLogger(consoleLog);
            paymentLogger.Log("Платеж обрабатывается...");

            var catalogLogger = new DebugLogger(consoleLog);
            ((DebugLogger)catalogLogger).LogMethodCall("GetProduct", 12345);

            Console.WriteLine("\nДля просмотра логов в файле откройте файл 'log.txt'");
        }
    }
}
