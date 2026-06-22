using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestD_1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadPartners();
        }

        private void LoadPartners()
        {
            try
            {
                using (var ctx = new Entities())
                {
                    var raw = ctx.Partners
                        .Select(p => new
                        {
                            p.PartnerID,
                            p.PartnerName,
                            p.Director,
                            p.Phone,
                            p.Reiting,
                            PartnerType = ctx.PartnerTypes
                                .Where(t => t.PartnerTypeID == p.PartnerTypeID)
                                .Select(t => t.PartnerType)
                                .FirstOrDefault() ?? "Не указан",
                            Total = ctx.PartnerProducts
                                .Where(pp => pp.PartnerID == p.PartnerID)
                                .Sum(pp => (double?)pp.CountPhoduct) ?? 0
                        })
                        .ToList();

                    cardsControl.ItemsSource = raw
                        .Select(p => new
                        {
                            p.PartnerID,
                            p.PartnerName,
                            p.Director,
                            p.Phone,
                            p.Reiting,
                            p.PartnerType,
                            Discount = GetDiscount(p.Total)
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetDiscount(double total)
        {
            if (total > 300000) return "15%";
            if (total >= 50000) return "10%";
            if (total >= 10000) return "5%";
            return "0%";
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            var win = new EditWindow(((Border)sender).DataContext);
            win.Owner = this;
            if (win.ShowDialog() == true) LoadPartners();
        }

        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            var win = new EditWindow();
            win.Owner = this;
            if (win.ShowDialog() == true) LoadPartners();
        }
    }
}
