using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Model;

namespace WpfApp1.ViewModel
{
    public class MainViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _message;

        public MainViewModel()
        {
            Items = new ObservableCollection<Item>();
            ReactiveItems = new ReactiveList<Item>();
            ReactiveItems.ItemsAdded.Subscribe(e => ItemsAdding(e), ItemsAdded);
            ReactiveItems.ShouldReset.Subscribe(_ => ItemsAdded());

            ClickCommand = new RelayCommand(OnClick);
            ClearCommand = new RelayCommand(OnClear);

            TextChangedCommand = new RelayCommand(OnTextChanged);

            //_message = this.WhenAnyValue(vm => vm.Text)
            //                .Throttle(TimeSpan.FromMilliseconds(50))
            //                .ToProperty(this, vm => vm.Message);
        }

        public string Message => _message.Value;

        public ICommand ClickCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand TextChangedCommand { get; set; }

        private void OnClick(object args)
        {
            List<Item> list = new List<Item>();

            for(int item = 0; item < 10000000; item++ )
            {
                list.Add(new Item
                {
                    Id = "Id: " + item.ToString(),
                    Name = "Name"
                });
            }

            ReactiveItems.AddRange(list);
        }

        private void OnClear(object obj)
        {
            ReactiveItems.Clear();
        }

        private void OnTextChanged(object obj)
        {
            Text = (string) obj;
        }

        [Reactive]
        public string Text { get; set; }

        public ObservableCollection<Item> Items { get; set; }

        public ReactiveList<Item> ReactiveItems { get; set; }

        private IEnumerable<int> GetItems()
        {
            return Enumerable.Range(1, 100000);
        }

        private void ItemsAdding(Item item)
        {
            if (item.Id == "Id: 99999")
                Text = item.Id + " Added." ;
        }

        private void ItemsAdded()
        {
                Text = "Added.";
        }
    }
}
