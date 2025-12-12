using System.Windows;
using System.Windows.Controls;
using дерево;
using System.Collections.Generic;

namespace Explorer
{
    public partial class MainWindow : Window
    {
        private ObjectModelSystem system = new ObjectModelSystem();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Создание корня
        private void ButtonSetRoot_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxRoot.Text))
            {
                MessageBox.Show("Введите имя корня.");
                return;
            }

            system.CreateRoot(TextBoxRoot.Text.Trim());
            UpdateTreeView();

            ButtonSetRoot.IsEnabled = false;
            ButtonDelete.IsEnabled = true;
        }

        // Добавление узла
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (system.Root == null)
            {
                MessageBox.Show("Сначала создайте корень.");
                return;
            }

            string parentName = TextBoxParent.Text.Trim();
            string childName = TextBoxChild.Text.Trim();

            if (string.IsNullOrWhiteSpace(parentName) || string.IsNullOrWhiteSpace(childName))
            {
                MessageBox.Show("Заполните оба поля.");
                return;
            }

            TreeNode parentNode = system.Root.Find(parentName);
            if (parentNode == null)
            {
                MessageBox.Show("Родитель не найден.");
                return;
            }

            parentNode.AddChild(new TreeNode(childName));
            UpdateTreeView();
        }

        // Удаление выбранного узла
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Tree.SelectedItem is not TreeViewItem selectedItem)
            {
                MessageBox.Show("Выберите объект для удаления.");
                return;
            }

            TreeNode node = (TreeNode)selectedItem.Tag;

            if (node == system.Root)
            {
                // Если удалить корень, очистим только его детей
                system.Root.DeleteSubtree(system.Root.Name);
            }
            else
            {
                node.DeleteSubtree(node.Name);
            }

            UpdateTreeView();
        }

        // Обновление TreeView
        private void UpdateTreeView()
        {
            Tree.Items.Clear();

            if (system.Root != null)
                Tree.Items.Add(CreateTreeItem(system.Root));
        }

        // Рекурсивное создание TreeViewItem
        private TreeViewItem CreateTreeItem(TreeNode node)
        {
            TreeViewItem item = new TreeViewItem
            {
                Header = node.Name,
                Tag = node,
                IsExpanded = true
            };

            foreach (var child in node.Children)
            {
                item.Items.Add(CreateTreeItem(child));
            }

            return item;
        }
    }
}

namespace дерево
{
    // Класс для узла дерева
    public class TreeNode
    {
        public string Name { get; set; }
        public TreeNode Parent { get; set; }
        public LinkedList<TreeNode> Children { get; set; }

        public TreeNode(string name, TreeNode parent = null)
        {
            Name = name;
            Parent = parent;
            Children = new LinkedList<TreeNode>();
        }

        // Добавление ребёнка
        public void AddChild(TreeNode child)
        {
            child.Parent = this;
            Children.AddLast(child);
        }

        // Удаление ребёнка
        public bool RemoveChild(TreeNode child)
        {
            return Children.Remove(child);
        }

        // Поиск узла по имени (рекурсивно)
        public TreeNode Find(string name)
        {
            if (Name == name) return this;

            var current = Children.First;
            while (current != null)
            {
                var found = current.Value.Find(name);
                if (found != null) return found;
                current = current.Next;
            }

            return null;
        }

        // Удаление поддерева (всех детей и внуков)
        public bool DeleteSubtree(string nodeName)
        {
            TreeNode nodeToDelete = Find(nodeName);
            if (nodeToDelete == null) return false;

            if (nodeToDelete.Parent == null)
            {
                DeleteAllChildren(nodeToDelete);
                return true;
            }

            DeleteAllChildren(nodeToDelete);

            var parentList = nodeToDelete.Parent.Children;
            var node = parentList.First;
            while (node != null)
            {
                if (node.Value == nodeToDelete)
                {
                    parentList.Remove(node);
                    break;
                }
                node = node.Next;
            }

            return true;
        }

        // Очистка всех детей
        private void DeleteAllChildren(TreeNode root)
        {
            if (root.Children.Count == 0) return;

            Stack<TreeNode> stack = new Stack<TreeNode>();
            var childNode = root.Children.First;
            while (childNode != null)
            {
                stack.Push(childNode.Value);
                childNode = childNode.Next;
            }

            root.Children.Clear();

            while (stack.Count > 0)
            {
                TreeNode currentNode = stack.Pop();
                var currentChild = currentNode.Children.First;
                while (currentChild != null)
                {
                    stack.Push(currentChild.Value);
                    currentChild = currentChild.Next;
                }

                currentNode.Children.Clear();
                currentNode.Parent = null;
            }
        }

        // Получение корня дерева
        public TreeNode Root
        {
            get
            {
                var current = this;
                while (current.Parent != null)
                    current = current.Parent;
                return current;
            }
        }
    }

    // Основной класс системы (без консоли)
    public class ObjectModelSystem
    {
        public TreeNode Root { get; private set; }

        // Создание корня
        public void CreateRoot(string name)
        {
            Root = new TreeNode(name);
        }

        // Добавление объекта
        public bool AddObject(string parentName, string childName)
        {
            if (Root == null) return false;
            var parent = Root.Find(parentName);
            if (parent == null) return false;

            parent.AddChild(new TreeNode(childName));
            return true;
        }

        // Удаление объекта
        public bool RemoveObject(string name)
        {
            if (Root == null) return false;
            if (Root.Name == name)
            {
                Root.DeleteSubtree(name);
                return true;
            }

            return Root.DeleteSubtree(name);
        }
    }
}
