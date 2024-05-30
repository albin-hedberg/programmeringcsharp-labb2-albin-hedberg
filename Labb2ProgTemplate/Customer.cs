using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Labb2ProgTemplate
{
    public class Customer : Membership
    {
        public string Name { get; private set; }

        private string Password { get; set; }

        private List<Product> _cart;
        public List<Product> Cart { get { return _cart; } }

        public bool LoggedIn { get; set; } // = false;

        public Customer(string name, string password, Level membershipLevel)
        {
            Name = name;
            Password = password;
            _cart = new List<Product>();
            LoggedIn = false;
            MembershipLevel = membershipLevel;
        }

        public bool CheckPassword(string password)
        {
            return password == Password;
        }

        public void AddToCart(Product product, int quantity)
        {
            if (_cart.Contains(product))
            {
                _cart[_cart.IndexOf(product)].Quantity += quantity;
            }
            else
            {
                _cart.Add(product);
                _cart[_cart.IndexOf(product)].Quantity += quantity;
            }
        }

        public void RemoveFromCart(Product product, int quantity)
        {
            if (!_cart.Contains(product))
            {
                return;
            }

            int productIndex = _cart.IndexOf(product);

            if (_cart[productIndex].Quantity > 1 && _cart[productIndex].Quantity - quantity > 0)
            {
                _cart[productIndex].Quantity -= quantity;
            }
            else
            {
                _cart[productIndex].Quantity = 0;
                _cart.Remove(product);
            }
        }

        public double CartTotal()
        {
            return _cart.Sum(product => product.Price * product.Quantity);
        }

        public override string ToString()
        {
            string customerInfo = $"Namn: {Name}\nLösenord: {Password}\nNivå: {MembershipLevel}\nKundvagn:\n";

            foreach (var product in _cart)
            {
                customerInfo += $"{product.Name} {product.Quantity}st {product.Price}kr/st = {product.Price * product.Quantity}kr\n";
            }

            customerInfo += $"Totalt: {CartTotal()}\n";

            return customerInfo;
        }
    }
}
