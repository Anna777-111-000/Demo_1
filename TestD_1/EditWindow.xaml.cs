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
using System.Windows.Shapes;


namespace TestD_1
{
    public partial class EditWindow : Window
    {
        Entities db = new Entities();
        Partners partner;
        bool isEdit;

        public EditWindow(object data = null)
        {
            InitializeComponent();
            cmbPartnerType.ItemsSource = db.PartnerTypes.ToList();
            cmbPartnerType.SelectedIndex = 0;

            if (data != null)
            {
                isEdit = true;
                Title = "Редактирование";
                int id = (int)data.GetType().GetProperty("PartnerID").GetValue(data);
                partner = db.Partners.Find(id);
                txtPartnerName.Text = partner.PartnerName;
                txtDirector.Text = partner.Director;
                txtPhone.Text = partner.Phone;
                txtAddress.Text = partner.Fddres;
                txtEmail.Text = partner.Email;
                txtRating.Text = partner.Reiting?.ToString() ?? "0";
                cmbPartnerType.SelectedValue = partner.PartnerTypeID ?? 0;
            }
            else
            {
                isEdit = false;
                Title = "Добавление";
                partner = new Partners();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка рейтинга
                if (!int.TryParse(txtRating.Text, out int rating) || rating < 0)
                {
                    MessageBox.Show("Рейтинг - целое число >= 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                partner.PartnerName = txtPartnerName.Text;
                partner.Director = txtDirector.Text;
                partner.Phone = txtPhone.Text;
                partner.Fddres = txtAddress.Text;
                partner.Email = txtEmail.Text;
                partner.Reiting = rating;
                partner.PartnerTypeID = ((PartnerTypes)cmbPartnerType.SelectedItem).PartnerTypeID;

                if (!isEdit)
                {
                    partner.PartnerID = db.Partners.Any() ? db.Partners.Max(p => p.PartnerID) + 1 : 1;
                    db.Partners.Add(partner);
                }

                db.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}