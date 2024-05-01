using System;
using System.Collections.Generic;

namespace Supermarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Shop shop = new Shop(5, 700, 1500, 4, 10);

            shop.Work();
        }
    }

    public class Shop
    {
        private int _money = 0;
        private Queue<Client> _clients = new Queue<Client>();
        private List<Product> _products;

        public Shop(int numberClients, int minAmountClientMoney, int maxAmountClientMoney, int minNumberProducts, int maxNumberProducts)
        {
            _products = new List<Product>()
                    {
                    new Product("Молоко", 85),
                    new Product("Чай", 75),
                    new Product("Свинина", 485),
                    new Product("Кофе", 250),
                    new Product("Сникерс", 95),
                    new Product("Макароны", 68),
                    new Product("Печенье", 150),
                    new Product("Мыло", 45),
                    new Product("Зубная паста", 175),
                    new Product("Сыр", 220),
                    };

            CreateNewClients(numberClients, minAmountClientMoney, maxAmountClientMoney, minNumberProducts, maxNumberProducts);
        }

        public void Work()
        {
            while (_clients.Count > 0)
            {
                Client client = _clients.Dequeue();
                Console.WriteLine("Баланс:" + _money + "\n");

                Console.WriteLine($"Колличество клиентов в очереди: {_clients.Count}\n");

                client.Fill(new List<Product>(_products));

                Console.WriteLine("Продукты которые собирается купить клиент:");
                client.ShowProducts();

                Console.WriteLine($"\nИтоговая стоимость корзины:{client.CalculateCostProducts()}");

                while (client.CanPayProducts() == false)
                {
                    Console.WriteLine("Клиент НЕможет оплатить весь товар! Необходимо выложить продукты.");
                    Console.ReadKey();
                    Console.WriteLine($"Удален продукт: {client.GetRemovedRandomProduct().Name}\n");
                }

                if (client.ProductsCount != 0)
                {
                    Console.WriteLine("\nКлиент может оплатить весь товар!");
                    Console.WriteLine("\nИтоговая корзина:");
                    client.ShowProducts();
                    Console.WriteLine($"\nИтоговая стоимость корзины:{client.CalculateCostProducts()}");
                    _money += client.Pay();
                    Console.WriteLine("Покупка прошла успешно!");
                }
                else
                {
                    Console.WriteLine("\nКлиенту не хватило денег на покупку!");
                }

                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine($"Все покупки завершены. Итоговая выручка составила: {_money}");
            Console.ReadKey();
        }

        private void CreateNewClients(int count, int minAmountClientMoney, int maxAmountClientMoney, int minNumberProducts, int maxNumberProducts)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(UserUtils.GenerateRandomNumber(minAmountClientMoney, maxAmountClientMoney),
                    UserUtils.GenerateRandomNumber(minNumberProducts, maxNumberProducts)));
            }
        }
    }

    public class Client
    {
        private List<Product> _products = new List<Product>();
        private int _money;
        private int _numberProducts;

        public Client(int money, int numberProducts)
        {
            _money = money;
            _numberProducts = numberProducts;
        }

        public int ProductsCount => _products.Count;

        public bool CanPayProducts() => _money >= CalculateCostProducts();

        public int Pay()
        {
            int costProducts = CalculateCostProducts();
            _money -= costProducts;
            return costProducts;
        }

        public void ShowProducts()
        {
            int sequenceNumber = 0;

            for (int i = 0; i < _products.Count; i++)
            {
                sequenceNumber++;
                Console.WriteLine($"{sequenceNumber}. {_products[i].Name} {_products[i].Price} рублей");
            }
        }

        public void Fill(List<Product> products)
        {
            for (int i = 0; i < _numberProducts; i++)
            {
                _products.Add(GetRandomProduct(products));
            }
        }

        public int CalculateCostProducts()
        {
            int cost = 0;

            foreach (Product product in _products)
            {
                cost += product.Price;
            }

            return cost;
        }

        public Product GetRemovedRandomProduct()
        {
            int numberProduct = UserUtils.GenerateRandomNumber(0, _products.Count);
            Product removedProduct = _products[numberProduct];
            _products.RemoveAt(numberProduct);

            return removedProduct;
        }

        private Product GetRandomProduct(List<Product> products) =>
            products[UserUtils.GenerateRandomNumber(0, products.Count)];
    }

    public class Product
    {
        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; private set; }
        public int Price { get; private set; }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int min, int max) =>
            s_random.Next(min, max);
    }
}
