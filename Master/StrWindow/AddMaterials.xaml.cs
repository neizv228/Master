using Master.BDModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Master.StrWindow
{
    /// <summary>
    /// Логика взаимодействия для AddMaterials.xaml
    /// </summary>
    public partial class AddMaterials : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Tovar _currentproduct = new Tovar();
        public AddMaterials(Tovar selectedTovar)
        {
            InitializeComponent();
            DataContext = this;
            AddComboBox.ItemsSource = KompEntities.GetContext().Manufacturers.ToList();
            if (selectedTovar != null)
                _currentproduct = selectedTovar;
            DataContext = _currentproduct;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentproduct.TovarName))
                errors.AppendLine("Укажите наименование товара");
            if (_currentproduct.TovarCost < 0)
                errors.AppendLine("Стоимость не может быть отрицательной!");
            if (_currentproduct.TovarDiscountAmount < 0 || _currentproduct.TovarDiscountAmount > 100)
                errors.AppendLine("Размер скидки не может быть отрицательным!");
            if (string.IsNullOrWhiteSpace(_currentproduct.TovarDescription))
                errors.AppendLine("Укажите описание товара!");

            if (errors.Length > 0)
                MessageBox.Show(errors.ToString());
            else
            {
                if (_currentproduct.TovarID == 0)
                    KompEntities.GetContext().Tovar.Add(_currentproduct);
                try
                {
                    KompEntities.GetContext().SaveChanges();
                    MessageBox.Show("Информация сохранена!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void ChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog GetImageDialog = new OpenFileDialog();
            GetImageDialog.Filter = "Файлы изображений: (*.png, *.jpg, *.jpeg)|*.png; *.jpg; *.jpeg";
            GetImageDialog.InitialDirectory = Environment.GetEnvironmentVariable("/Resources/");
            if (GetImageDialog.ShowDialog() == true)
            {
                _currentproduct.TovarPhoto = GetImageDialog.FileName.Substring(Environment.CurrentDirectory.Length - 10);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("_currentproduct"));
                }
            }
        }
    }
}