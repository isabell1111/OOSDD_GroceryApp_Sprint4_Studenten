using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services;
public class BoughtProductsService : IBoughtProductsService
{
    private readonly IGroceryListItemsRepository _groceryListItemsRepository;
    private readonly IGroceryListRepository _groceryListRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IProductRepository _productRepository;

    public BoughtProductsService(
        IGroceryListItemsRepository groceryListItemsRepository,
        IGroceryListRepository groceryListRepository,
        IClientRepository clientRepository,
        IProductRepository productRepository)
    {
        _groceryListItemsRepository = groceryListItemsRepository;
        _groceryListRepository = groceryListRepository;
        _clientRepository = clientRepository;
        _productRepository = productRepository;
    }

    public List<BoughtProducts> Get(int? productId)
    {
        // Check of productId een waarde heeft
        if (productId == null || !productId.HasValue)
        {
            return new List<BoughtProducts>();
        }

        // Haal alle GroceryListItems op met het gegeven productId
        var items = _groceryListItemsRepository.GetAll()
            .Where(gli => gli.ProductId == productId.Value)
            .ToList();

        // Haal product informatie op
        var product = _productRepository.Get(productId.Value);

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
                    result.Add(new BoughtProducts(
                    client,       // 1e parameter: Client
                    groceryList,  // 2e parameter: GroceryList
                    product       // 3e parameter: Product
                    ));
                }
            }
        }

        return result;
    }
}