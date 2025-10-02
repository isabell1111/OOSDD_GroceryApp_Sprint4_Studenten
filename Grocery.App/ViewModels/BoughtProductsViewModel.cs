using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;


namespace Grocery.App.ViewModels
{
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;

        [ObservableProperty]
        Product selectedProduct;
        public ObservableCollection<BoughtProducts> BoughtProductsList { get; set; } = [];
        public ObservableCollection<Product> Products { get; set; }

        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
        }
        //nieuw:
        partial void OnSelectedProductChanged(Product? oldValue, Product newValue)
        {
            // Check of er een product is geselecteerd
            if (newValue == null)
            {
                // Maak de lijst leeg als er geen product geselecteerd is
                BoughtProductsList.Clear();
                return;
            }

            // Haal alle BoughtProducts op voor het geselecteerde product
            var boughtProducts = _boughtProductsService.Get(newValue.Id);

            // Leeg de huidige lijst
            BoughtProductsList.Clear();

            // Voeg alle nieuwe items toe aan de ObservableCollection
            foreach (var item in boughtProducts)
            {
                BoughtProductsList.Add(item);
            }
        }

        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    
    }
}
