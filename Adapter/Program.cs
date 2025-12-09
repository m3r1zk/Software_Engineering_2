using System;

namespace Adapter
{
    public interface IRoad
    {
        string Name { get; }
        int Length { get; }
    }

    public class Road : IRoad
    {
        public string Name { get; }
        public int Length { get; }

        public Road(string name, int length)
        {
            Name = name;
            Length = length;
        }
    }

    public interface ITransport
    {
        void Ride(IRoad road);
    }

    public class Car : ITransport
    {
        public string Model { get; }

        public Car(string model)
        {
            Model = model;
        }

        public void Ride(IRoad road)
        {
            Console.WriteLine($"Машина '{Model}' едет по дороге '{road.Name}' ({road.Length} км)");
        }
    }

    public class Donkey
    {
        public string Name { get; }

        public Donkey(string name)
        {
            Name = name;
        }

        public void Eat()
        {
            Console.WriteLine($"Осёл '{Name}' кушает");
        }

        public void Walk()
        {
            Console.WriteLine($"Осёл '{Name}' идёт");
        }
    }

    public class Saddle : ITransport
    {
        private readonly Donkey _donkey;

        public Saddle(Donkey donkey)
        {
            _donkey = donkey;
        }

        public void Ride(IRoad road)
        {
            Console.WriteLine($"Осёл '{_donkey.Name}' с седлом готов к поездке");
            _donkey.Walk();
            Console.WriteLine($"Осёл '{_donkey.Name}' с седлом перемещается по дороге '{road.Name}'");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IRoad highway = new Road("Шоссе", 100);
            IRoad countryRoad = new Road("Просёлочная дорога", 50);

            ITransport car = new Car("Toyota Camry");
            car.Ride(highway);
            Console.WriteLine();

            Donkey donkey = new Donkey("Иа");
            donkey.Eat();
            donkey.Walk();
            Console.WriteLine();

            ITransport donkeyWithSaddle = new Saddle(donkey);
            donkeyWithSaddle.Ride(countryRoad);

            // ВОПРОС: На сколько адекватно данная задача притянута к задаче реализации паттерна Adapter?
            // ОТВЕТ: Задача притянута не полностью адекватно. Хотя технически она демонстрирует механизм адаптера
            // (преобразование интерфейса Donkey в интерфейс ITransport), предметная область выбрана неудачно.
            // В реальности седло - физический объект, а не программный адаптер. Более естественным примером было бы
            // наличие старого класса с несовместимым интерфейсом и необходимость его интеграции в новую систему.
            // Однако задача корректно иллюстрирует принцип: объект (осёл) получает новый интерфейс (транспорт)
            // через обёртку (седло), что соответствует определению паттерна Adapter.
        }
    }
}
