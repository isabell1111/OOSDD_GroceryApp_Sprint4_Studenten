
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;


namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository = groceryListItemsRepository;
            _groceryListRepository = groceryListRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
        }
        //
        public List<BoughtProducts> Get(int productId)
        {
            // Haal alle GroceryListItems op met het gegeven productId
            var items = _groceryListItemsRepository.GetAll()
                .Where(gli => gli.ProductId == productId)
                .ToList();

            // Haal product informatie op
            var product = _productRepository.Get(productId);

            var result = new List<BoughtProducts>();

            foreach (var item in items)
            {
                // Haal de boodschappenlijst op
                var groceryList = _groceryListRepository.Get(item.GroceryListId);

                if (groceryList != null)
                {
                    // Haal de client op
                    var client = _clientRepository.Get(groceryList.ClientId);

                    if (client != null && product != null)
                    {
                        result.Add(new BoughtProduct
                        {
                            Client = client,
                            GroceryList = groceryList,
                            Product = product,
                            Amount = item.Amount
                        });
                    }
                }
            }

            return result;
        }
    }
}
