using System;
using System.Collections.Generic;

namespace Composite
{
    public interface IOrderComponent
    {
        decimal GetPrice();
        string Display(int depth = 0);
    }

    public class Product : IOrderComponent
    {
        public string Name { get; }
        public decimal Price { get; }

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public decimal GetPrice() => Price;

        public string Display(int depth = 0)
        {
            string indent = new string(' ', depth * 2);
            return indent + $"Продукт: {Name} - {Price:C}";
        }
    }

    public class Box : IOrderComponent
    {
        private readonly List<IOrderComponent> _children;
        public string Name { get; }

        public Box(string name)
        {
            Name = name;
            _children = new List<IOrderComponent>();
        }

        public void Add(IOrderComponent component)
        {
            _children.Add(component);
        }

        public void Remove(IOrderComponent component)
        {
            _children.Remove(component);
        }

        public decimal GetPrice()
        {
            decimal total = 0;
            foreach (var child in _children)
            {
                total += child.GetPrice();
            }
            return total;
        }

        public string Display(int depth = 0)
        {
            string indent = new string(' ', depth * 2);
            string result = indent + $"Коробка: {Name} (Общая цена: {GetPrice():C})\n";

            foreach (var child in _children)
            {
                result += child.Display(depth + 1) + "\n";
            }

            return result.TrimEnd();
        }
    }

    public class Order : IOrderComponent
    {
        private readonly List<IOrderComponent> _components;
        public string OrderNumber { get; }

        public Order(string orderNumber)
        {
            OrderNumber = orderNumber;
            _components = new List<IOrderComponent>();
        }

        public void AddComponent(IOrderComponent component)
        {
            _components.Add(component);
        }

        public decimal GetPrice()
        {
            decimal total = 0;
            foreach (var component in _components)
            {
                total += component.GetPrice();
            }
            return total;
        }

        public string Display(int depth = 0)
        {
            string indent = new string(' ', depth * 2);
            string result = indent + $"Заказ №{OrderNumber}\n";
            result += indent + "Состав заказа:\n";

            foreach (var component in _components)
            {
                result += component.Display(depth + 1) + "\n";
            }

            result += indent + $"Итого: {GetPrice():C}";

            return result;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Product laptop = new Product("Ноутбук", 50000);
            Product mouse = new Product("Мышь", 1500);
            Product keyboard = new Product("Клавиатура", 3000);
            Product headphones = new Product("Наушники", 7000);
            Product charger = new Product("Зарядное устройство", 2500);

            Box smallBox1 = new Box("Маленькая коробка 1");
            smallBox1.Add(mouse);
            smallBox1.Add(keyboard);

            Box smallBox2 = new Box("Маленькая коробка 2");
            smallBox2.Add(headphones);
            smallBox2.Add(charger);

            Box mediumBox = new Box("Средняя коробка");
            mediumBox.Add(smallBox1);
            mediumBox.Add(smallBox2);

            Box largeBox = new Box("Большая коробка");
            largeBox.Add(laptop);
            largeBox.Add(mediumBox);

            Order order = new Order("ORD-2024-001");

            order.AddComponent(new Product("Коврик для мыши", 800));
            order.AddComponent(largeBox);
            order.AddComponent(new Product("Внешний HDD", 6000));

            Console.WriteLine(order.Display());
            Console.WriteLine();

            Console.WriteLine("--- Простой заказ ---");
            Order simpleOrder = new Order("ORD-2024-002");
            simpleOrder.AddComponent(new Product("Книга", 500));
            simpleOrder.AddComponent(new Product("Ручка", 50));
            Console.WriteLine(simpleOrder.Display());
            Console.WriteLine();

            Console.WriteLine("--- Заказ с вложенными коробками ---");
            Order nestedOrder = new Order("ORD-2024-003");

            Box innerBox = new Box("Внутренняя коробка");
            innerBox.Add(new Product("Мелкий предмет 1", 100));
            innerBox.Add(new Product("Мелкий предмет 2", 200));

            Box outerBox = new Box("Внешняя коробка");
            outerBox.Add(new Product("Крупный предмет", 1000));
            outerBox.Add(innerBox);

            nestedOrder.AddComponent(outerBox);
            Console.WriteLine(nestedOrder.Display());
        }
    }
}
