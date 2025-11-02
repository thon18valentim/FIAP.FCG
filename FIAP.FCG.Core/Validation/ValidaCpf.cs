using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FIAP.FCG.Core.Validation
{
    public static class ValidaCpf
    {
        /// <summary>
        /// Método usado diretamente pelo atributo [CustomValidation]
        /// </summary>
        public static ValidationResult? ValidarCpf(string? cpf, ValidationContext context)
        {
            return IsCpf(cpf)
                ? ValidationResult.Success
                : new ValidationResult("CPF inválido.");
        }

        /// <summary>
        /// Valida o CPF segundo as regras oficiais (com cálculo dos dígitos verificadores).
        /// </summary>
        public static bool IsCpf(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove caracteres não numéricos
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            // Verifica tamanho
            if (cpf.Length != 11)
                return false;

            // Elimina CPFs com todos os dígitos iguais (ex: 11111111111)
            if (new string(cpf[0], cpf.Length) == cpf)
                return false;

            // Calcula os dois dígitos verificadores
            int[] multiplicador1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multiplicador2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

            string tempCpf = cpf[..9];
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            int primeiroDigito = resto < 2 ? 0 : 11 - resto;

            tempCpf += primeiroDigito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            int segundoDigito = resto < 2 ? 0 : 11 - resto;

            string cpfCalculado = tempCpf + segundoDigito;

            return cpf.EndsWith(cpfCalculado[^2..]);
        }
    }
}
