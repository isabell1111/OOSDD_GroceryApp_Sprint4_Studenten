using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        //public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        //{


        //    throw new NotImplementedException();
        //}

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            var allItems = _groceriesRepository.GetAll();
            //de items die in boodschappenlijsten zijn toegevoegd worden opgehaald
            //uit de repository met de GetAll()-method.
            var allProducts = _productRepository.GetAll();


            var products = allItems
                .GroupBy(gli => gli.ProductId)
                //met GroupBy() komen alle items met hetzelfde ProductID in 1 groep.
                .Select(g => new
                {// er wordt met Select() een nieuw object aangemaakt voor elke goep.
                    ProductId = g.Key,

                    NrOfSells = g.Sum(gli => gli.Amount)  // Amount in plaats van Quantity
                })//met Sum() wordt het Amount geteld; alle waarden worden bij elkaar opgeteld.
                  //NrOfSells is hierbij het resultaat van de som.
                .OrderByDescending(p => p.NrOfSells)
                //met OrderByDescending gaat NrOfSells van hoog naar laag.
                .Take(topX)
                .ToList();

            var result = new List<BestSellingProducts>();
            //result is de nieuwe lege lijst waarin de eindresultaten worden opgeslagen.
            for (int i = 0; i < products.Count; i++)
            {
                var p = products[i];
                //er wordt geloopt door de alle producten door middel van i.
                var product = allProducts.FirstOrDefault(prod => prod.Id == p.ProductId);
                //met FirstOrDefault() wordt er gezocht naar het eerste product met het bijbehorende id.


                if (product != null)
                {
                    result.Add(new BestSellingProducts(
                        p.ProductId,
                        product.Name,
                        product.Stock,
                        p.NrOfSells,
                        i + 1
                    ));//als het product wel bestaat worden de bovenstaande gegevens (result) geretourneerd.
                }
            }

            return result;
        }


        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}

