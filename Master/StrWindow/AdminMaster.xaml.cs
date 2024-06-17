using Master.BDModel;
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
    /// Логика взаимодействия для AdminMaster.xaml
    /// </summary>
    public partial class AdminMaster : Window, INotifyPropertyChanged
    {
        public string UserFIOass;
        public event PropertyChangedEventHandler PropertyChanged;
        private IEnumerable<Tovar> _MaterialsList;
        private int ManufacturerFilterID = 0;
        private int SortType = 0;
        public List<Manufacturers> ManufacturerList { get; set; }
        public string[] SortList { get; set; } =
        {
            "Без сортировки",
            "Стоимость по возрастанию",
            "Стоимость по убыванию",
            "Мастер по возрастанию",
            "Мастер по убыванию",
        };

        private void Invalidate(string ComponentName = "MaterialsList")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaterialsList"));
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(ComponentName));
        }

        private string SearchFilter = "";

        public IEnumerable<Tovar> MaterialsList
        {
            get
            {
                var Result = _MaterialsList;

                switch (SortType)
                {
                    case 0:
                        Result = KompEntities.GetContext().Tovar.ToList();
                        break;
                    case 1:
                        Result = KompEntities.GetContext().Tovar.OrderBy(p => p.TovarCost);
                        break;
                    case 2:
                        Result = KompEntities.GetContext().Tovar.OrderByDescending(p => p.TovarCost);
                        break;
                    case 3:
                        Result = KompEntities.GetContext().Tovar.OrderBy(p => p.Manufacturers.ManufacturerName);
                        break;
                    case 4:
                        Result = KompEntities.GetContext().Tovar.OrderByDescending(p => p.Manufacturers.ManufacturerName);
                        break;

                }
                if (SearchFilter != "")
                    Result = Result.Where(p =>
                    p.TovarName.IndexOf(SearchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    p.TovarDescription.IndexOf(SearchFilter, StringComparison.OrdinalIgnoreCase) >= 0);

                if (ManufacturerComboBox.SelectedIndex == 0)
                {
                    chislo1.Text = MaterialsListView.Items.Count.ToString();
                    chislo2.Text = MaterialsListView.Items.Count.ToString();
                }
                if (ManufacturerComboBox.SelectedIndex == 1)
                {
                    Result = Result.Where(i => i.TovarDiscountAmount >= 0 && i.TovarDiscountAmount <= 22.99m);
                    chislo1.Text = MaterialsListView.Items.Count.ToString();
                }
                if (ManufacturerComboBox.SelectedIndex == 2)
                {
                    Result = Result.Where(i => i.TovarDiscountAmount >= 23 && i.TovarDiscountAmount <= 26.99m);
                    chislo1.Text = MaterialsListView.Items.Count.ToString();
                }
                if (ManufacturerComboBox.SelectedIndex == 3)
                {
                    Result = Result.Where(i => i.TovarDiscountAmount >= 27);
                    chislo1.Text = MaterialsListView.Items.Count.ToString();
                }

               // if (ManufacturerFilterID > 0)
                 //   Result = Result.Where(i => i.Category.CategoryID == ManufacturerFilterID);

                return Result.Take(100);
            }
            set
            {
                _MaterialsList = value;
                Invalidate();
            }
        }

        public AdminMaster(string Faio)
        {
            InitializeComponent();
            DataContext = this;
            MaterialsList = KompEntities.GetContext().Tovar.ToList();
            ManufacturerList = KompEntities.GetContext().Manufacturers.ToList();
            ManufacturerList.Clear();
            ManufacturerList.Insert(0, new Manufacturers { ManufacturerName = "Все диапазоны" });
            ManufacturerList.Insert(1, new Manufacturers { ManufacturerName = "Скидки от 0% до 22,99%" });
            ManufacturerList.Insert(2, new Manufacturers { ManufacturerName = "Скидки от 23% до 26,99%" });    //удалить диапазоны
            ManufacturerList.Insert(3, new Manufacturers { ManufacturerName = "Больше 27%" });
            UserFIOass = Faio;
            usersFaio.Text = UserFIOass;
        }

        private void SearchFilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SearchFilter = SearchFilterTextBox.Text;
            Invalidate();
        }

        private void SortFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortType = SortTypeComboBox.SelectedIndex;
            Invalidate();
        }

        private void DiscountAmountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ManufacturerFilterID = ManufacturerComboBox.SelectedIndex;
            Invalidate();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            AdminMaster ad = new AdminMaster(UserFIOass);
            ad.Show();
            this.Close();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            AddMaterials add = new AddMaterials((sender as Button).DataContext as Tovar);
            add.ShowDialog();
            if (add.IsActive == false)
            {
                MaterialsListView.ItemsSource = KompEntities.GetContext().Tovar.ToList();
                chislo1.Text = MaterialsListView.Items.Count.ToString();
                chislo2.Text = MaterialsListView.Items.Count.ToString();
                Invalidate();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var MaterialsForRemoving = MaterialsListView.SelectedItems.Cast<Tovar>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {MaterialsForRemoving.Count()} элементы?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    KompEntities.GetContext().Tovar.RemoveRange(MaterialsForRemoving);
                    KompEntities.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    MaterialsListView.ItemsSource = KompEntities.GetContext().Tovar.ToList();
                    chislo1.Text = MaterialsListView.Items.Count.ToString();
                    chislo2.Text = MaterialsListView.Items.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = new MainWindow();
            m.Show();
            this.Close();
        }

        private void AddMaterials_Click(object sender, RoutedEventArgs e)
        {
            AddMaterials add = new AddMaterials(null);
            add.ShowDialog();
            if (add.IsActive == false)
            {
                MaterialsListView.ItemsSource = KompEntities.GetContext().Tovar.ToList();
                chislo1.Text = MaterialsListView.Items.Count.ToString();
                chislo2.Text = MaterialsListView.Items.Count.ToString();
                Invalidate();
            }
        }


    }
}

