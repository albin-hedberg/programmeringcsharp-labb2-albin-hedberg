namespace Labb2ProgTemplate;

public class CustomersFileManager
{
    private readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Customers.txt");

    private readonly List<Customer> _customersList = new List<Customer>()
    {
        new Customer("test", "test", Membership.Level.None),
        new Customer("Knatte", "123", Membership.Level.Bronze),
        new Customer("Fnatte", "321", Membership.Level.Silver),
        new Customer("Tjatte", "213", Membership.Level.Gold)
    };

    public void CreateCustomersFile()
    {
        if (!File.Exists(_path))
        {
            using StreamWriter sw = new StreamWriter(_path);
            foreach (var customer in _customersList)
            {
                sw.WriteLine(customer);
            }
        }
    }

    public void UpdateCustomersFile(List<Customer> customersList)
    {
        using StreamWriter sw = new StreamWriter(_path);
        foreach (var customer in customersList)
        {
            sw.WriteLine(customer);
        }
    }

    public List<Customer> ReadCustomersFromFile()
    {
        if (!File.Exists(_path))
        {
            return _customersList;
        }

        List<Customer> customerList = new List<Customer>();
        string? line = string.Empty;
        string name = string.Empty;
        string password = string.Empty;
        Membership.Level level = Membership.Level.None;

        using StreamReader sr = new StreamReader(_path);
        while ((line = sr.ReadLine()) != null)
        {
            if (line.StartsWith("Namn:"))
            {
                name = line.Substring(6);
            }
            else if (line.StartsWith("Lösenord:"))
            {
                password = line.Substring(10);
            }
            else if (line.StartsWith("Nivå:"))
            {
                if (!Enum.TryParse(line.Substring(6), out level))
                {
                    level = Membership.Level.None;
                }
            }
            else if (line.Length == 0)
            {
                Customer tempCustomer = new Customer(name, password, level);
                customerList.Add(tempCustomer);
                Console.WriteLine(tempCustomer.Name);
            }
        }

        return customerList;
    }
}
