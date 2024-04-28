using System;
using System.Collections.Generic;

namespace Supermarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Warehouse warehouse = new Warehouse
                    (new List<Product>()
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
                    });

            Shop shop = new Shop(warehouse);

            shop.Work();
        }
    }

    public class Shop
    {
        private int _money = 0;
        private Queue<Client> _clients = new Queue<Client>();
        private int _numberClients = 5;
        private int _minAmountClientMoney = 700;
        private int _maxAmountClientMoney = 1500;
        private int _minNumberProducts = 4;
        private int _maxNumberProducts = 10;
        private Warehouse _warehouse;

        public Shop(Warehouse warehouse)
        {
            Random random = new Random();

            _warehouse = warehouse;
            CreateNewClients(_numberClients, random);
        }

        public void CreateNewClients(int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(random.Next(_minAmountClientMoney, _maxAmountClientMoney),
                                            random.Next(_minNumberProducts, _maxNumberProducts)));
            }
        }

        public void Work()
        {
            while (_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine("Баланс:" + _money + "\n");

                Console.WriteLine($"Колличество клиентов в очереди: {_clients.Count}\n");

                newClient.FillShoppingCart(_warehouse);

                Console.WriteLine("Продукты которые собирается купить клиент:");
                newClient.ShowProducts();

                Console.WriteLine($"\nИтоговая стоимость корзины:{newClient.CalculateCostProducts()}");

                if (newClient.CanPayProducts())
                {
                    Console.WriteLine("Клиент может оплатить весь товар!");
                    _money += newClient.Pay();
                    Console.WriteLine("Покупка прошла успешно!");
                }
                else
                {
                    while (newClient.CanPayProducts() == false)
                    {
                        Console.WriteLine("Клиент неможет оплатить весь товар! Необходимо выложить продукты.");
                        Console.ReadKey();
                        newClient.LayOutProduct();
                        Console.WriteLine($"Удален продукт: {newClient.GetRemovedProduct().Name}\n");
                    }

                    Console.WriteLine("Клиент может оплатить весь товар!");
                    Console.WriteLine("Итоговая корзина:");
                    newClient.ShowProducts();
                    Console.WriteLine($"\nИтоговая стоимость корзины:{newClient.CalculateCostProducts()}");
                    _money += newClient.Pay();
                    Console.WriteLine("Покупка прошла успешно!");
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

        public void ShowProducts()
        {
            _shoppingCart.ShowProducts();
        }

        public void FillShoppingCart(Warehouse warehouse)
        {
            _shoppingCart.AddProducts(warehouse);
        }

        public int CalculateCostProducts()
        {
            return _shoppingCart.CalculateCostProducts();
        }

        public bool CanPayProducts()
        {
            return _money >= _shoppingCart.CalculateCostProducts();
        }

        public int Pay()
        {
            _money -= _shoppingCart.CalculateCostProducts();
            return _shoppingCart.CalculateCostProducts();
        }

        public void LayOutProduct()
        {
            Random random = new Random();

            _shoppingCart.LayOutRandomProduct(random);
        }

        public Product GetRemovedProduct()
        {
            return _shoppingCart.GetRemovedProduct();
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

        public Product GetRemovedProduct()
        {
            return _removedProduct;
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

        public void AddProducts(Warehouse warehouse)
        {
            Random random = new Random();

            for (int i = 0; i < _numberProducts; i++)
            {
                _products.Add(warehouse.GetProduct(random));
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

        public void LayOutRandomProduct(Random random)
        {
            int numberProduct = random.Next(0, _products.Count);

            _removedProduct = _products[numberProduct];

            _products.RemoveAt(numberProduct);
        }
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

    public class Warehouse
    {
        private List<Product> _products;

        public Warehouse(List<Product> products)
        {
            _products = products;
        }

        public Product GetProduct(Random random)
        {
            return _products[random.Next(0, _products.Count)];
        }
    }
}
