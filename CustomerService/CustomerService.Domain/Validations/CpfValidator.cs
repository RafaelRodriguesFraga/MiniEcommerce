namespace CustomerService.Domain.Validations;

public static class CpfValidator
{
    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return false;
        }

        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11)
        {
            return false;
        }

        if (cpf.Distinct().Count() == 1)
        {
            return false;
        }

        var tempCpf = cpf.Substring(0, 9);
        var sum = 0;
        int[] multiplier = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        for (int i = 0; i < 9; i++)
        {
            sum += int.Parse(tempCpf[i].ToString()) * multiplier[i];
        }

        var remainder = sum % 11;
        remainder = remainder < 2 ? 0 : 11 - remainder;

        var digit = remainder.ToString();
        tempCpf += digit;
        sum = 0;
        int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        for (int i = 0; i < 10; i++)
        {
            sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];
        }

        remainder = sum % 11;
        remainder = remainder < 2 ? 0 : 11 - remainder;
        digit = remainder.ToString();



        return cpf.EndsWith(digit);
    }

}