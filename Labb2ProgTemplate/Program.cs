using Labb2ProgTemplate;

CustomersFileManager customersFileManager = new CustomersFileManager();
customersFileManager.CreateCustomersFile();

Shop shop = new Shop(customersFileManager.ReadCustomersFromFile());

while (!shop.Quit)
{
    shop.MainMenu();
}
