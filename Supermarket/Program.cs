using System;
using System.Collections.Generic;

namespace Supermarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Shop shop = new Shop();

            shop.Work();
        }
    }

    public class Shop
    {
        private int _money = 0;
        private Queue<Client> _clients = new Queue<Client>();
        private int _numberClients = 5;
        private int _minAmountClientMoney = 0;
        private int _maxAmountClientMoney = 50;
        private int _minNumberProducts = 4;
        private int _maxNumberProducts = 10;
        private List<Product> _products = new List<Product>()
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

        public Shop()
        {
            CreateNewClients(_numberClients);
        }

        public void CreateNewClients(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(UserUtils.GenerateRandomNumber(_minAmountClientMoney, _maxAmountClientMoney),
                    UserUtils.GenerateRandomNumber(_minNumberProducts, _maxNumberProducts)));
            }
        }

        public void Work()
        {
            while (_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine("Баланс:" + _money + "\n");

                Console.WriteLine($"Колличество клиентов в очереди: {_clients.Count}\n");

                newClient.ShoppingCart.Fill(_products);

                Console.WriteLine("Продукты которые собирается купить клиент:");
                newClient.ShoppingCart.ShowProducts();

                Console.WriteLine($"\nИтоговая стоимость корзины:{newClient.ShoppingCart.CalculateCostProducts()}");

                while (newClient.CanPayProducts() == false)
                {
                    Console.WriteLine("Клиент НЕможет оплатить весь товар! Необходимо выложить продукты.");
                    Console.ReadKey();
                    newClient.ShoppingCart.LayOutRandomProduct();
                    Console.WriteLine($"Удален продукт: {newClient.ShoppingCart.GetRemovedProduct().Name}\n");
                }

                if (newClient.ShoppingCart.GetNumberProducts() != 0)
                {
                    Console.WriteLine("Клиент может оплатить весь товар!");
                    Console.WriteLine("Итоговая корзина:");
                    newClient.ShoppingCart.ShowProducts();
                    Console.WriteLine($"\nИтоговая стоимость корзины:{newClient.ShoppingCart.CalculateCostProducts()}");
                    _money += newClient.Pay();
                    Console.WriteLine("Покупка прошла успешно!");

                }
                else
                {
                    Console.WriteLine("Клиенту не хватило денег на покупку!");
                }

                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine($"Все покупки завершены. Итоговая выручка составила: {_money}");
            Console.ReadKey();
        }
    }

    public class Client
    {
        private int _money;
        private ShoppingCart _shoppingCart;

        public Client(int money, int numberProducts)
        {
            _money = money;
            _shoppingCart = new ShoppingCart(numberProducts);
        }

        public ShoppingCart ShoppingCart { get; private set; }

        public bool CanPayProducts() => _money >= _shoppingCart.CalculateCostProducts();

        public int Pay()
        {
            _money -= _shoppingCart.CalculateCostProducts();
            return _shoppingCart.CalculateCostProducts();
        }
    }

    public class ShoppingCart
    {
        private List<Product> _products = new List<Product>();
        private int _numberProducts;
        private Product _removedProduct;

        public ShoppingCart(int numberProducts)
        {
            _numberProducts = numberProducts;
        }

        public Product GetRemovedProduct() => _removedProduct;

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

        public int GetNumberProducts() => _products.Count;

        private Product GetProduct(List<Product> products) => products[UserUtils.GenerateRandomNumber(0, _products.Count)];
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

        public static int GenerateRandomNumber(int min, int max) => s_random.Next(min, max);
    }
}
