using Quicker.Utilities._3rd;
using Quicker.Utilities.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TextListEditor
{
    public class MainViewModel
    {
        public FullyObservableCollection<MyDataClass> TextBlockList { get; set; } = new FullyObservableCollection<MyDataClass>();
        public ICommand SendMessageCommand { get; private set; }
        public ICommand NewLineCommand { get; private set; }
        public ICommand RemoveBlockCommand { get; private set; }
        public ICommand NewBlockCommand { get; private set; }
        public string MessageText { get; private set; }
        public void AddBlock(string title)
        {
            TextBlockList.Add(new MyDataClass()
            {
                Title = title
            });
        }
        public void Load()
        {
            NewLineCommand = new CustomCommand(p =>
            {
                if (!(p is TextBox textBox)) return;
                //int caretIdx = textBox.CaretIndex;
                textBox.SelectedText = "\r\n";
                textBox.CaretIndex += 2;
                textBox.SelectionLength = 0;
                //textBox.CaretIndex = caretIdx + 1;
            });
            NewBlockCommand = new CustomCommand(p =>
            {
                if (!(p is TextBox textBox)) return;
                var listBox = UIHelper.FindParent<ListBox>(textBox);
                var listboxItem = UIHelper.FindParent<ListBoxItem>(textBox);
                //var  li = listBox.ItemContainerGenerator.con
                var index = listBox.ItemContainerGenerator.IndexFromContainer(listboxItem);
                var carIndex = textBox.CaretIndex;
                var text = textBox.Text;
                textBox.Text = text.Substring(carIndex).TrimStart('\r', '\n');
                var data = new MyDataClass() { Title = text.Substring(0, carIndex).TrimEnd('\r', '\n') };
                TextBlockList.Insert(index, data); //前方插入
                var listboxItem1 = listBox.ItemContainerGenerator.ContainerFromIndex(index + 1) as ListBoxItem;
                var textBox1 = FindVisualChild<TextBox>(listboxItem1);
                textBox1.Focus();
            });

            RemoveBlockCommand = new CustomCommand(p =>
            {
                if (!(p is TextBox tb)) return;
                var lb = UIHelper.FindParent<ListBox>(tb);
                //tb.Text = lb.ContainerFromElement(tb).GetType().Name;
            }, p =>
            {
                return false;
            });

        }
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
    public class MyDataClass : INotifyPropertyChanged
    {
        private string title;
        #region For INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected void OnPropertyChanged(params string[] vs)
        {
            foreach (var s in vs)
            {
                OnPropertyChanged(s);
            }
        }
        #endregion
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Description { get; set; }
        public string Data { get; set; } = "";
    }
    // Interface 
    public interface ICustomCommand : ICommand
    {
        event EventHandler<object> Executed;
    }

    // Command Class
    public class CustomCommand : ICustomCommand
    {
        #region Private Fields
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        #endregion

        #region Constructor
        public CustomCommand(Action<object> execute) : this(execute, null)
        {
        }

        public CustomCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (x => true);
        }
        #endregion

        #region Public Methods
        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter = null)
        {
            Refresh();
            _execute(parameter);
            Executed?.Invoke(this, parameter);
            Refresh();
        }

        public void Refresh()
        {
            CommandManager.InvalidateRequerySuggested();
        }
        #endregion

        #region Events
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        public event EventHandler<object> Executed;
        #endregion
    }
}
