using StorageSystem.Models;

namespace API.Helpers
{
    public static class EntityValidationHelper
    {
        public static string GenerateNotFoundMessage(Product product, Customer customer, int productID, int customerID)
        {
            string message = string.Empty;
            if (product == null)
            {
                message += $"Product with ID {productID} not found. ";
            }
            if (customer == null)
            {
                message += $"Customer with ID {customerID} not found. ";
            }
            return message;
        }
    }
}
