using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool added;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Loaded += (s, e) =>
            {
                this.DataContext = new { height = 100 };
                var style = new Style();
                style.TargetType = typeof(MenuItem);
                style.Setters.Add(new Setter(HeightProperty, 100.0));
                this.ContextMenu.Resources.Add(typeof(MenuItem), style);
            };
            ContextMenu.Items.Add(new MenuItem() { Header = "felkfjl" });
        }
        public class data
        {
            public double height { get; set; } = 100;
        }

        private void MouseMove1(object sender, MouseEventArgs e)
        {
            //Trace.WriteLine(sender.GetType().Name);
            if (sender is MenuItem label && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(label, new DataObject(typeof(MenuItem), label), DragDropEffects.Move);
                Trace.WriteLine("aaa");
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var cm = ContextMenu;
            Trace.WriteLine("aaaaaaaaaa");
        }

        void SetTarget(MenuItem o)
        {
            o.AllowDrop = true;
            o.Drop += OnDrop;
            o.DragEnter += OnEnter;
            foreach (var i in o.Items)
            {
                if (i is MenuItem ii)
                {
                    ii.AllowDrop = true;
                    ii.Drop += OnDrop;
                    ii.DragEnter += OnEnter;
                    SetTarget(ii);
                }
            }

        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (this.added)
                return;
            this.added = true;

            MenuItem myData = (MenuItem)e.Data.GetData(typeof(MenuItem));
            MenuItem newItem = new MenuItem();
            // copy the menuitem, becouse the original already have a parent,
            // and i don't want to remove it from the upper menu
            string gridXaml = XamlWriter.Save(myData);
            StringReader stringReader = new StringReader(gridXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            newItem = (MenuItem)XamlReader.Load(xmlReader);

            SetTarget(newItem);
            if (sender is Menu menu)
            {
                menu.Items.Add(newItem);
            }
            else
            {
                if (sender is MenuItem menuItem)
                {
                    menuItem.Items.Add(newItem);
                }
            }
        }

        // open the target menu so we can insert items in the submenus
        private void OnEnter(object Sender, DragEventArgs e)
        {
            if (Sender is MenuItem m)
            {
                m.IsSubmenuOpen = true;
            }
        }
    }
}
