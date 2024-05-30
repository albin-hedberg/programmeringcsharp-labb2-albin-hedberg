namespace Labb2ProgTemplate;

public class Membership
{
    public enum Level
    {
        None,
        Bronze = 5,
        Silver = 10,
        Gold = 15
    }

    public Level MembershipLevel { get; set; }

    public double CalculateDiscountedPrice(double price)
    {
        return price - ((double)MembershipLevel / 100.0 * price);
    }
}
