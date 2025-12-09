using System;

namespace Decorator
{
    public abstract class DeliverySystem
    {
        public abstract decimal CalculateCost();
        public abstract string GetDescription();
        public abstract DateTime GetEstimatedDeliveryDate();
    }

    public class CourierDelivery : DeliverySystem
    {
        private readonly decimal _baseCost;
        private readonly int _distanceKm;

        public CourierDelivery(decimal baseCost, int distanceKm)
        {
            _baseCost = baseCost;
            _distanceKm = distanceKm;
        }

        public override decimal CalculateCost()
        {
            return _baseCost + (_distanceKm * 10);
        }

        public override string GetDescription()
        {
            return $"Курьерская доставка ({_distanceKm} км)";
        }

        public override DateTime GetEstimatedDeliveryDate()
        {
            return DateTime.Now.AddDays(5 + new Random().Next(0, 3));
        }
    }

    public class PostalDelivery : DeliverySystem
    {
        private readonly decimal _baseCost;
        private readonly string _postalService;

        public PostalDelivery(decimal baseCost, string postalService = "Почта России")
        {
            _baseCost = baseCost;
            _postalService = postalService;
        }

        public override decimal CalculateCost()
        {
            return _baseCost;
        }

        public override string GetDescription()
        {
            return $"Почтовая доставка ({_postalService})";
        }

        public override DateTime GetEstimatedDeliveryDate()
        {
            return DateTime.Now.AddDays(7 + new Random().Next(0, 7));
        }
    }

    public class PickupDelivery : DeliverySystem
    {
        private readonly string _pickupPoint;

        public PickupDelivery(string pickupPoint)
        {
            _pickupPoint = pickupPoint;
        }

        public override decimal CalculateCost()
        {
            return 0; 
        }

        public override string GetDescription()
        {
            return $"Самовывоз из пункта выдачи: {_pickupPoint}";
        }

        public override DateTime GetEstimatedDeliveryDate()
        {
            return DateTime.Now.AddDays(1);
        }
    }

    public class ExpressDeliveryDecorator : DeliverySystem
    {
        private readonly DeliverySystem _deliverySystem;
        private readonly string _courierService;

        public ExpressDeliveryDecorator(DeliverySystem deliverySystem, string courierService = "Экспресс-служба")
        {
            _deliverySystem = deliverySystem;
            _courierService = courierService;
        }

        public override decimal CalculateCost()
        {
            decimal baseCost = _deliverySystem.CalculateCost();
            return baseCost * 1.5m;
        }

        public override string GetDescription()
        {
            return $"{_deliverySystem.GetDescription()} + Экспресс-доставка ({_courierService})";
        }

        public override DateTime GetEstimatedDeliveryDate()
        {
            DateTime baseDate = _deliverySystem.GetEstimatedDeliveryDate();
            int daysReduction = (int)((baseDate - DateTime.Now).TotalDays * 0.7); 
            return DateTime.Now.AddDays(Math.Max(1, daysReduction));
        }

        public string TrackDelivery()
        {
            string[] statuses = {
            "Заказ принят в обработку",
            "Передано курьеру",
            "В пути",
            "Прибыло в пункт назначения",
            "Доставлено"
        };

            Random rnd = new Random();
            int statusIndex = rnd.Next(0, statuses.Length);

            return $"[{_courierService}] Статус доставки: {statuses[statusIndex]}";
        }

        public decimal CalculateExpressCost()
        {
            decimal baseCost = _deliverySystem.CalculateCost();
            decimal expressSurcharge = baseCost * 0.5m; 
            return expressSurcharge;
        }
    }

    public class InsuranceDecorator : DeliverySystem
    {
        private readonly DeliverySystem _deliverySystem;
        private readonly decimal _insuranceRate;

        public InsuranceDecorator(DeliverySystem deliverySystem, decimal insuranceRate = 0.05m)
        {
            _deliverySystem = deliverySystem;
            _insuranceRate = insuranceRate;
        }

        public override decimal CalculateCost()
        {
            decimal baseCost = _deliverySystem.CalculateCost();
            decimal insuranceCost = baseCost * _insuranceRate;
            return baseCost + insuranceCost;
        }

        public override string GetDescription()
        {
            return $"{_deliverySystem.GetDescription()} + Страхование ({_insuranceRate * 100}%)";
        }

        public override DateTime GetEstimatedDeliveryDate()
        {
            return _deliverySystem.GetEstimatedDeliveryDate();
        }
    }

    public class SMSNotificationDecorator : DeliverySystem
    {
        private readonly DeliverySystem _deliverySystem;
        private readonly decimal _smsCost;

        public SMSNotificationDecorator(DeliverySystem deliverySystem, decimal smsCost = 10)
        {
            _deliverySystem = deliverySystem;
            _smsCost = smsCost;
        }

        public override decimal CalculateCost()
        {
            return _deliverySystem.CalculateCost() + _smsCost;
        }

        public override string GetDescription()
        {
            return $"{_deliverySystem.GetDescription()} + СМС-уведомления";
        }

        public override DateTime GetEstimatedDeliveryDate()
        {
            return _deliverySystem.GetEstimatedDeliveryDate();
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Система доставки интернет-магазина ===\n");

            Console.WriteLine("1. Базовые способы доставки:");

            DeliverySystem courier = new CourierDelivery(300, 15);
            Console.WriteLine($"{courier.GetDescription()}");
            Console.WriteLine($"Стоимость: {courier.CalculateCost():C}");
            Console.WriteLine($"Примерная дата доставки: {courier.GetEstimatedDeliveryDate():dd.MM.yyyy}\n");

            DeliverySystem postal = new PostalDelivery(200, "Почта России");
            Console.WriteLine($"{postal.GetDescription()}");
            Console.WriteLine($"Стоимость: {postal.CalculateCost():C}");
            Console.WriteLine($"Примерная дата доставки: {postal.GetEstimatedDeliveryDate():dd.MM.yyyy}\n");

            DeliverySystem pickup = new PickupDelivery("ул. Ленина, 10");
            Console.WriteLine($"{pickup.GetDescription()}");
            Console.WriteLine($"Стоимость: {pickup.CalculateCost():C}");
            Console.WriteLine($"Примерная дата доставки: {pickup.GetEstimatedDeliveryDate():dd.MM.yyyy}\n");

            Console.WriteLine("2. Экспресс-доставка (декоратор):");

            DeliverySystem expressCourier = new ExpressDeliveryDecorator(courier, "DHL Express");
            Console.WriteLine($"{expressCourier.GetDescription()}");
            Console.WriteLine($"Стоимость: {expressCourier.CalculateCost():C}");
            Console.WriteLine($"Примерная дата доставки: {expressCourier.GetEstimatedDeliveryDate():dd.MM.yyyy}");

            var expressDecorator = expressCourier as ExpressDeliveryDecorator;
            if (expressDecorator != null)
            {
                Console.WriteLine($"Наценка за экспресс: {expressDecorator.CalculateExpressCost():C}");
                Console.WriteLine($"Отслеживание: {expressDecorator.TrackDelivery()}");
            }
            Console.WriteLine();

            DeliverySystem expressPostal = new ExpressDeliveryDecorator(postal);
            Console.WriteLine($"{expressPostal.GetDescription()}");
            Console.WriteLine($"Стоимость: {expressPostal.CalculateCost():C}");
            Console.WriteLine($"Примерная дата доставки: {expressPostal.GetEstimatedDeliveryDate():dd.MM.yyyy}\n");

            Console.WriteLine("3. Комбинация декораторов:");

            DeliverySystem fullDelivery = new CourierDelivery(300, 15);
            fullDelivery = new ExpressDeliveryDecorator(fullDelivery);
            fullDelivery = new InsuranceDecorator(fullDelivery, 0.03m);
            fullDelivery = new SMSNotificationDecorator(fullDelivery);

            Console.WriteLine($"{fullDelivery.GetDescription()}");
            Console.WriteLine($"Стоимость: {fullDelivery.CalculateCost():C}");
            Console.WriteLine($"Примерная дата доставки: {fullDelivery.GetEstimatedDeliveryDate():dd.MM.yyyy}\n");

            Console.WriteLine("4. Другая комбинация:");

            DeliverySystem premiumDelivery = new PostalDelivery(150, "EMS");
            premiumDelivery = new InsuranceDecorator(premiumDelivery, 0.1m);
            premiumDelivery = new ExpressDeliveryDecorator(premiumDelivery, "FedEx");
            premiumDelivery = new SMSNotificationDecorator(premiumDelivery, 15);

            Console.WriteLine($"{premiumDelivery.GetDescription()}");
            Console.WriteLine($"Стоимость: {premiumDelivery.CalculateCost():C}");
            Console.WriteLine($"Примерная дата доставки: {premiumDelivery.GetEstimatedDeliveryDate():dd.MM.yyyy}\n");

            Console.WriteLine("5. Самовывоз с СМС-уведомлениями:");

            DeliverySystem pickupWithSMS = new PickupDelivery("ТЦ 'Мега', 2 этаж");
            pickupWithSMS = new SMSNotificationDecorator(pickupWithSMS);

            Console.WriteLine($"{pickupWithSMS.GetDescription()}");
            Console.WriteLine($"Стоимость: {pickupWithSMS.CalculateCost():C}");
            Console.WriteLine($"Примерная дата доставки: {pickupWithSMS.GetEstimatedDeliveryDate():dd.MM.yyyy}\n");

            Console.WriteLine("6. Дополнительные возможности:");

            DeliverySystem[] deliveries = new DeliverySystem[]
            {
            new CourierDelivery(250, 10),
            new ExpressDeliveryDecorator(new CourierDelivery(250, 10)),
            new InsuranceDecorator(new ExpressDeliveryDecorator(new CourierDelivery(250, 10))),
            new CourierDelivery(250, 50),
            new ExpressDeliveryDecorator(new CourierDelivery(250, 50))
            };

            foreach (var delivery in deliveries)
            {
                Console.WriteLine($"{delivery.GetDescription()}");
                Console.WriteLine($"  Стоимость: {delivery.CalculateCost():C}");
                Console.WriteLine($"  Срок: {delivery.GetEstimatedDeliveryDate():dd.MM.yyyy}");
                Console.WriteLine();
            }
        }
    }
}
