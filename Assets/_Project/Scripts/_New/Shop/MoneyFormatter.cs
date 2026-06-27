public static class MoneyFormatter
{
    private static readonly string[] suffixes = { "", "K", "M", "B", "T", "Qd" };

    public static string Format(int amount)
    {
        if (amount < 1000)
            return amount.ToString();

        int order = 0;
        double scaled = amount;

        while (scaled >= 1000 && order < suffixes.Length - 1)
        {
            order++;
            scaled /= 1000.0;
        }

        return scaled.ToString("F2").Replace('.', ',') + suffixes[order];
    }
}