using System.Windows;
using System.Windows.Controls;
using дерево;

namespace Explorer
{
    public partial class MainWindow : Window
    {
        private ObjectModelSystem system = new ObjectModelSystem();

        public MainWindow()
        {
            InitializeComponent();
        }

        // СОЗДАНИЕ КОРНЯ
        private void ButtonSetRoot_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxRoot.Text))
            {
                MessageBox.Show("Введите имя корня.");
                return;
            }

            system.SetRoot(TextBoxRoot.Text.Trim());
            RefreshTree();
            ButtonDelete.IsEnabled = true;
        }

        // ДОБАВЛЕНИЕ ОБЪЕКТА
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (system.Root == null)
            {
                MessageBox.Show("Сперва создайте корень.");
                return;
            }

            string parent = TextBoxParent.Text.Trim();
            string child = TextBoxChild.Text.Trim();

            if (parent == "" || child == "")
            {
                MessageBox.Show("Заполните оба поля.");
                return;
            }

            if (!system.AddObject(parent, child))
            {
                MessageBox.Show("Родитель не найден.");
                return;
            }

            RefreshTree();
        }

        // УДАЛЕНИЕ ПО ВЫБОРУ
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Tree.SelectedItem is not TreeViewItem item)
            {
                MessageBox.Show("Выберите объект.");
                return;
            }

            TreeNode node = (TreeNode)item.Tag;

            if (node == system.Root)
            {
                MessageBox.Show("Корень удалить нельзя.");
                return;
            }

            system.RemoveObject(node.Name);
            RefreshTree();
        }

        // ОБНОВЛЕНИЕ ДЕРЕВА
        private void RefreshTree()
        {
            Tree.Items.Clear();

            if (system.Root != null)
                Tree.Items.Add(CreateTreeItem(system.Root));
        }

        private TreeViewItem CreateTreeItem(TreeNode node)
        {
            TreeViewItem item = new TreeViewItem
            {
                Header = node.Name,
                Tag = node,
                IsExpanded = true
            };

            foreach (var child in node.Children)
                item.Items.Add(CreateTreeItem(child));

            return item;
        }
    }
}


namespace дерево
{
    // Узел дерева
    public class TreeNode
    {
        public string Name { get; set; }
        public List<TreeNode> Children { get; set; }
        public TreeNode Parent { get; set; }

        public TreeNode(string name)
        {
            Name = name;
            Children = new List<TreeNode>();
        }

        public void AddChild(TreeNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(TreeNode child)
        {
            Children.Remove(child);
        }

        // Поиск узла в поддереве
        public TreeNode FindNode(string name)
        {
            if (this.Name == name)
                return this;

            foreach (var child in Children)
            {
                var found = child.FindNode(name);
                if (found != null)
                    return found;
            }

            return null;
        }

        // Удаление узла с поддеревом
        public bool RemoveNode(string name)
        {
            foreach (var child in Children)
            {
                if (child.Name == name)
                {
                    Children.Remove(child);
                    return true;
                }

                if (child.RemoveNode(name))
                    return true;
            }
            return false;
        }
    }

    // Чистая модель, без консоли
    public class ObjectModelSystem
    {
        public TreeNode Root { get; private set; }

        public void SetRoot(string name)
        {
            Root = new TreeNode(name);
        }

        public bool AddObject(string parentName, string newNodeName)
        {
            if (Root == null) return false;

            TreeNode parent = Root.FindNode(parentName);
            if (parent == null) return false;

            parent.AddChild(new TreeNode(newNodeName));
            return true;
        }

        public bool RemoveObject(string name)
        {
            if (Root == null) return false;
            if (Root.Name == name) return false; // нельзя удалить корень

            return Root.RemoveNode(name);
        }
    }
}
