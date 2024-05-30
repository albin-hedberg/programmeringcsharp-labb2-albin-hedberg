using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb2ProgTemplate.Enums;
using Labb2ProgTemplate.Products;

namespace Labb2ProgTemplate
{
    public class Shop
    {
        private CustomersFileManager _customersFileManager;
        private List<Customer> _allCustomers;

        public bool Quit { get; private set; }

        private Customer CurrentCustomer { get; set; }

        private List<Product> Products { get; set; }

        public Shop(List<Customer> customerList)
        {
            _customersFileManager = new CustomersFileManager();
            _allCustomers = customerList;
            Quit = false;
            CurrentCustomer = _allCustomers[0];
            PopulateProducts();
        }

        private void PopulateProducts()
        {
            Products = new List<Product>
            {
                new Korv(),
                new Hamburgare(),
                new Cola(),
                new Gurka(),
                new Apelsin()
            };
        }

        public void MainMenu()
        {
            Console.Clear();

            if (!CurrentCustomer.LoggedIn)
            {
                Console.WriteLine("Välj ett alternativ:\n1. Registrera ny kund\n2. Logga in\n3. Avsluta");

                switch (Console.ReadLine())
                {
                    case "1":
                        Register();
                        break;
                    case "2":
                        Login();
                        break;
                    case "3":
                        Quit = true;
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Välkommen {CurrentCustomer.Name}!");
                Console.WriteLine("Välj ett alternativ:\n1. Handla\n2. Se Kundvagn\n3. Gå till kassan\n4. Logga ut");

                switch (Console.ReadLine())
                {
                    case "1":
                        ShopMenu();
                        break;
                    case "2":
                        ViewCart();
                        break;
                    case "3":
                        Checkout();
                        break;
                    case "4":
                        Logout();
                        break;
                }
            }
        }

        private void Login()
        {
            Console.Clear();

            Console.WriteLine("Skriv namn:");
            string name = CredentialsInput();

            Console.WriteLine("Skriv lösenord:");
            string password = CredentialsInput();

            if (!CheckIfCustomerAlreadyExists(name))
            {
                Console.WriteLine("Kunden finns inte, vill du registrera en ny kund? (Y/N):");

                if (Console.ReadLine().ToUpper() == "Y")
                {
                    Register(name, password);
                }

                return;
            }

            int customerIndex = _allCustomers.FindIndex(customer => customer.Name == name);
            CurrentCustomer = _allCustomers[customerIndex];

            while (true)
            {
                if (CurrentCustomer.CheckPassword(password))
                {
                    CurrentCustomer.LoggedIn = true;
                    break;
                }

                Console.WriteLine($"Lösenordet för {name} stämmer inte, försök igen:");
                password = CredentialsInput();
            }
        }

        private void Login(string name)
        {
            Console.Clear();
            Console.WriteLine($"Namn: {name}");
            Console.WriteLine("Skriv lösenord:");
            string password = CredentialsInput();

            int customerIndex = _allCustomers.FindIndex(customer => customer.Name == name);
            CurrentCustomer = _allCustomers[customerIndex];

            while (true)
            {
                if (CurrentCustomer.CheckPassword(password))
                {
                    CurrentCustomer.LoggedIn = true;
                    break;
                }

                Console.WriteLine($"Lösenordet för {name} stämmer inte, försök igen:");
                password = CredentialsInput();
            }
        }

        private void Register()
        {
            Console.Clear();
            Console.WriteLine("Skriv namn:");
            string name = CredentialsInput();

            if (CheckIfCustomerAlreadyExists(name))
            {
                Console.WriteLine($"Kunden {name} är redan registrerad, logga in istället? (Y/N)");

                if (Console.ReadLine().ToUpper() == "Y")
                {
                    Login(name);
                }

                return;
            }

            Console.WriteLine("Skriv lösenord:");
            string password = CredentialsInput();

            CurrentCustomer = new Customer(name, password, Membership.Level.None) { LoggedIn = true };
            _allCustomers.Add(CurrentCustomer);
            _customersFileManager.UpdateCustomersFile(_allCustomers);

            Console.Clear();
            Console.WriteLine($"Kunden {name} har registrerats!\n\nTryck på valfri tangent...");
            Console.ReadKey();
        }

        private void Register(string name, string password)
        {
            CurrentCustomer = new Customer(name, password, Membership.Level.None) { LoggedIn = true };
            _allCustomers.Add(CurrentCustomer);
            _customersFileManager.UpdateCustomersFile(_allCustomers);

            Console.Clear();
            Console.WriteLine($"Kunden {name} har registrerats!\n\nTryck på valfri tangent...");
            Console.ReadKey();
        }

        private void Logout()
        {
            CurrentCustomer.LoggedIn = false;

            Console.Clear();
            Console.WriteLine("Du är nu utloggad.\n\nTryck på valfri tangent...");
            Console.ReadKey();
        }

        private static string CredentialsInput()
        {
            string credential = string.Empty;

            while (true)
            {
                credential = Console.ReadLine();

                if (credential.Length < 1)
                {
                    Console.WriteLine("Du måste skriva minst 1 tecken.");
                    continue;
                }

                break;
            }

            return credential;
        }

        private bool CheckIfCustomerAlreadyExists(string name)
        {
            return _allCustomers.Any(customer => customer.Name == name);
        }

        private void ShopMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Välj vilken vara du vill lägga till i kundvagnen:");

                Console.WriteLine("0. Tillbaka till huvudmenyn");

                for (int i = 0; i < Products.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Products[i].Name} - {Products[i].Price} kr");
                }

                int productIndex = SelectProduct(Products.Count);
                if (productIndex == (int)UserInputFlags.BackToMainMenu)
                {
                    break;
                }

                Console.Clear();
                Console.WriteLine($"Hur många av produkten '{Products[productIndex - 1].Name}' vill du lägga till?");

                int productQuantity = SelectProductQuantity();

                CurrentCustomer.AddToCart(Products[productIndex - 1], productQuantity);

                Console.Clear();
                Console.WriteLine($"{productQuantity}st av '{Products[productIndex - 1].Name}' har lagts till i kundvagnen.\n\nTryck på valfri tangent...");
                Console.ReadKey();
            }
        }

        private static int SelectProduct(int highestIndex)
        {
            while (true)
            {
                string userInput = Console.ReadLine();

                if (userInput == "0")
                {
                    return (int)UserInputFlags.BackToMainMenu;
                }

                int productIndex = ProcessUserInput(userInput);

                if (productIndex <= highestIndex && productIndex != (int)UserInputFlags.InvalidInput)
                {
                    return productIndex;
                }

                Console.WriteLine("Kunde inte tyda valet av produkt, försök igen:");
            }
        }

        private static int SelectProductQuantity()
        {
            while (true)
            {
                string userInput = Console.ReadLine();
                int productQuantity = ProcessUserInput(userInput);

                if (productQuantity != (int)UserInputFlags.InvalidInput)
                {
                    return productQuantity;
                }

                Console.WriteLine("Kunde inte tyda antalet, försök igen:");
            }
        }

        private static int ProcessUserInput(string userInput)
        {
            if (int.TryParse(userInput, out int parsedInt) && parsedInt >= (int)UserInputFlags.LowestProductIndexAndQuantity)
            {
                return parsedInt;
            }

            return (int)UserInputFlags.InvalidInput;
        }

        private void ViewCart()
        {
            while (true)
            {
                Console.Clear();
                PrintCart();
                Console.WriteLine("\n0. Tillbaka till huvudmenyn\n");
                Console.WriteLine("Välj vilken vara du vill ta bort ifrån:");

                int productIndex = SelectProduct(CurrentCustomer.Cart.Count);
                if (productIndex == (int)UserInputFlags.BackToMainMenu)
                {
                    break;
                }

                Console.Clear();
                Console.WriteLine($"Hur många av produkten '{CurrentCustomer.Cart[productIndex - 1].Name}' vill du ta bort?");

                int productQuantity = SelectProductQuantity();

                CurrentCustomer.RemoveFromCart(CurrentCustomer.Cart[productIndex - 1], productQuantity);

                Console.Clear();
                Console.WriteLine($"{productQuantity}st har tagits bort från kundvagnen.\n\nTryck på valfri tangent...");
                Console.ReadKey();
            }
        }

        private void Checkout()
        {
            Console.Clear();
            PrintCart();
            Console.WriteLine("Bekräfta & checka ut? (Y/N)");

            if (Console.ReadLine().ToUpper() == "Y")
            {
                foreach (var product in CurrentCustomer.Cart)
                {
                    product.Quantity = 0;
                }

                CurrentCustomer.Cart.Clear();

                Console.WriteLine("Köpet har genomförts!\n\nTryck på valfri tangent...");
                Console.ReadKey();
            }
        }

        private void PrintCart()
        {
            Console.WriteLine("Kundvagn:");

            for (int i = 0; i < CurrentCustomer.Cart.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {CurrentCustomer.Cart[i].Name} {CurrentCustomer.Cart[i].Quantity}st {CurrentCustomer.Cart[i].Price}kr/st = {CurrentCustomer.Cart[i].Price * CurrentCustomer.Cart[i].Quantity}kr");
            }

            double price = CurrentCustomer.CartTotal();
            Console.WriteLine($"Totalt: {price} kr");

            if (CurrentCustomer.MembershipLevel != Membership.Level.None)
            {
                price = CurrentCustomer.CalculateDiscountedPrice(price);
                Console.WriteLine($"Rabatterat pris: {price}kr");
            }
        }
    }
}
