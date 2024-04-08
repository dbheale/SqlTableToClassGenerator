using System.Runtime.CompilerServices;

namespace SqlTableToClassGenerator
{
    public static class Argue
    {
        // Probably will never be needed.
        //public static void ItIsEmpty<T>(this IEnumerable<T> value,
        //                                [CallerArgumentExpression("value")] string message = "")
        //{
        //    if (value.Any())
        //    {
        //        throw new ArgumentException("Enumerable is not empty", message);
        //    }
        //}

        public static void ItIsNotEmpty<T>(this IEnumerable<T>? value,
            [CallerArgumentExpression("value")] string message = "")
        {
            if (value?.Any() == false)
            {
                throw new ArgumentException("Enumerable is empty", message);
            }
        }

        public static void ItIsNotNull<T>(T? value,
            [CallerArgumentExpression("value")] string message = "")
        {
            if (value is null)
            {
                throw new ArgumentNullException(message);
            }
        }

        public static void HasContent(string? value,
            [CallerArgumentExpression("value")] string message = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(message);
            }
        }

        public static void ItIsTrue(bool value,
            [CallerArgumentExpression("value")] string message = "")
        {
            if (!value)
            {
                throw new ArgumentException(message);
            }
        }

        public static void ItIsFalse(bool value,
            [CallerArgumentExpression("value")] string message = "")
        {
            if (!value)
            {
                throw new ArgumentException(message);
            }
        }
    }
}
