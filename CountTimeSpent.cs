// See https://aka.ms/new-console-template for more information

Console.WriteLine("================Welcome================");

while (true)
{
    Console.WriteLine($"=======================================");

    // PATH CSV
    Console.Write("Enter path csv (type 'cancel' to exit): ");
    var pathCsv = Console.ReadLine();
    if(IsCancel(pathCsv))
        break;

    // NAME COLUMN
    Console.Write("Enter name column (type 'cancel' to exit): ");
    var nameColumn = Console.ReadLine();
    if (IsCancel(pathCsv))
        break;

    // ID USER
    Console.Write("Enter id (type 'cancel' to exit): ");
    var id = Console.ReadLine();
    if (IsCancel(pathCsv))
        break;

    //// MONTH
    //Console.Write("Enter number month (default = 1) (type 'cancel' to exit): ");
    //var monthInput = Console.ReadLine();
    //if (IsCancel(pathCsv))
    //    break;

    //int month = 1;
    //if (!string.IsNullOrEmpty(monthInput) && int.TryParse(monthInput, out int m) && m < 13)
    //    month = m;

    //// PAID LEAVE
    //Console.Write("Enter number day paid leave (default = 0) (type 'cancel' to exit): ");
    //var paidLeaveInput = Console.ReadLine();
    //if (IsCancel(pathCsv))
    //    break;

    //double paidLeave = 0;
    //if (!string.IsNullOrEmpty(paidLeaveInput) && double.TryParse(paidLeaveInput, out double p) && p < month * 30)
    //    paidLeave = p;

    //////////////////////////////////////////////////////////////////

    if (!File.Exists(pathCsv))
    {
        Console.WriteLine("[Error] File not exist.");
        continue;
    }

    if (Path.GetExtension(pathCsv) != ".csv")
    {
        Console.WriteLine("[Error] Currently support only .csv file.");
        continue;
    }

    //////////////////////////////////////////////////////////////////

    try
    {
        using (var reader = new StreamReader(pathCsv))
        {
            if (reader is null)
                continue;

            double totalSpentTime = 0;
            bool isHeader = true;
            string? line = null;
            int[]? columnIndexes = null;

            while ((line = reader!.ReadLine()) is not null)
            {
                string[] values = line.Split(',');

                if (isHeader)
                {
                    columnIndexes = values.Select((value, index) => new { value, index })
                                .Where(x => x.value.Trim().ToLower().StartsWith(nameColumn))
                                .Select(x => x.index).ToArray();

                    isHeader = false;
                }

                else if (columnIndexes?.Length == 0)
                {
                    Console.WriteLine("[Error] Column not exist.");
                    break;
                }

                else if (columnIndexes?.Length > 0)
                {
                    foreach (var columnIndex in columnIndexes)
                    {
                        var columnValue = values.Length > columnIndex ? values[columnIndex] : null;
                        if (columnValue is null)
                            continue;

                        var elements = columnValue.Split(';');
                        var lastElement = elements.LastOrDefault();
                        var nearLastElement = elements.SkipLast(1).LastOrDefault();

                        if (double.TryParse(lastElement, out var spentTime) && nearLastElement?.Trim() == id)
                        {
                            totalSpentTime += spentTime / 3600;
                        }
                    }

                }
            }

            Console.WriteLine($"===Total time: {totalSpentTime}h===");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[Error] " + ex.Message);
    }

    Console.WriteLine($"=======================================");
}

bool IsCancel(string? input)
{
    if (string.Equals(input, "cancel", StringComparison.OrdinalIgnoreCase))
        return true;

    return false;
}
