namespace ChessApp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ChessAppModel model = new ChessAppModel();

        public MainWindow()
        {
            InitializeComponent();
            
            this.DataContext = model;
            model.Game.Reset();
            model.Refresh();

            model.Game.AfterMove += (s, e) =>
            {
                model.Refresh();
                //Thread.Sleep(100);
            };
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    model.Game.Reset();
                    model.Game.AutoPlay();
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //model.RefreshBoard();
        }

        private async void TurnButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    model.Game.TakeTurn();
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
