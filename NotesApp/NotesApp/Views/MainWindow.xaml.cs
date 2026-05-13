using NotesApp.Models;
using NotesApp.ViewModels; // Обязательно добавить этот using!
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotesApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        // Этот метод срабатывает, когда ты кликаешь на элемент в дереве
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var vm = (MainViewModel)this.DataContext;
            vm.SelectedItem = e.NewValue; // Записываем выбранный объект (Проект или Задача)

            if (e.NewValue is TaskItem selectedTask)
            {
                vm.SelectedTask = selectedTask; // Для обратной совместимости с редактором
                SetRtfToEditor(selectedTask.ContentRtf);
            }
            else
            {
                vm.SelectedTask = null;
                TextEditor.Document.Blocks.Clear();
            }
        }
        private string GetRtfFromEditor()
        {
            TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                range.Save(ms, DataFormats.Rtf);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        // Метод 2: Берем RTF-строку из базы и загружаем в редактор
        private void SetRtfToEditor(string rtfString)
        {
            if (string.IsNullOrEmpty(rtfString))
            {
                TextEditor.Document.Blocks.Clear();
                return;
            }

            try
            {
                var byteArray = Encoding.UTF8.GetBytes(rtfString);
                using (MemoryStream ms = new MemoryStream(byteArray))
                {
                    TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
                    range.Load(ms, DataFormats.Rtf);
                }
            }
            catch
            {
                // Если в базе был обычный текст, а не RTF
                TextEditor.Document.Blocks.Clear();
                TextEditor.Document.Blocks.Add(new Paragraph(new Run(rtfString)));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = (MainViewModel)this.DataContext;
            // Вызываем команду и передаем ей текст из редактора
            string currentRtf = GetRtfFromEditor();
            vm.SaveCommand.Execute(currentRtf);
        }
        private void Tree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Этот код заставляет TreeView выбирать элемент при нажатии правой кнопкой мыши
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                treeViewItem.IsSelected = true;
                e.Handled = true;
            }
        }

        // Вспомогательный метод для поиска элемента дерева под мышкой
        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
    }
}