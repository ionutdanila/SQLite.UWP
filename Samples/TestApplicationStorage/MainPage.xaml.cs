using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TestApplicationStorage
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            txt1.Text = SQLQuery.TableExists<KeyValueBase>() ? "Table Exists" : "Database Creation failed";

            SQLQuery.InsertOrUpdateEntityProperty(new KeyValueBase
            {
                Key = "Test",
                Value = "ValueOfEntitiy"
            });

            txt2.Text = SQLQuery.IsEntityAvailable<KeyValueBase>("Test") ? "Test Entity was corrected inserted" : "Test entity was not corrected inserted";
            txt2.Text = $"Entitiy Value: {SQLQuery.GetEntityPropertyByKey<KeyValueBase>("Test")?.Value}";
        }
    }
}
