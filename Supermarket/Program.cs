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
                Client newClient = _clients.Dequeue();
                Console.WriteLine("Баланс:" + _money + "\n");

                Console.WriteLine($"Колличество клиентов в очереди: {_clients.Count}\n");

                newClient.Fill(_products);

                Console.WriteLine("Продукты которые собирается купить клиент:");
                newClient.ShowProducts();

                Console.WriteLine($"\nИтоговая стоимость корзины:{newClient.CalculateCostProducts()}");

                while (newClient.CanPayProducts() == false)
                {
                    Console.WriteLine("Клиент НЕможет оплатить весь товар! Необходимо выложить продукты.");
                    Console.ReadKey();
                    newClient.LayOutRandomProduct();
                    Console.WriteLine($"Удален продукт: {newClient.GetRemovedProduct().Name}\n");
                }

                if (newClient.GetNumberProducts() != 0)
                {
                    Console.WriteLine("\nКлиент может оплатить весь товар!");
                    Console.WriteLine("\nИтоговая корзина:");
                    newClient.ShowProducts();
                    Console.WriteLine($"\nИтоговая стоимость корзины:{newClient.CalculateCostProducts()}");
                    _money += newClient.Pay();
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
        private Product _removedProduct;
        private int _money;
        private int _numberProducts;

        public Client(int money, int numberProducts)
        {
            _money = money;
            _numberProducts = numberProducts;
        }

        public bool CanPayProducts() => _money >= CalculateCostProducts();

        public int Pay()
        {
            _money -= CalculateCostProducts();
            return CalculateCostProducts();
        }

        public Product GetRemovedProduct() =>
            _removedProduct;

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
                _products.Add(GetProduct(products));
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

        public void LayOutRandomProduct()
        {
            int numberProduct = UserUtils.GenerateRandomNumber(0, _products.Count);

            _removedProduct = _products[numberProduct];

            _products.RemoveAt(numberProduct);
        }

        public int GetNumberProducts() =>
            _products.Count;

        private Product GetProduct(List<Product> products) =>
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
