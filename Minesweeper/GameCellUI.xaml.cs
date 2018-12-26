using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for GameCellUI.xaml
    /// </summary>
    public partial class GameCellUI : UserControl
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(GameCellUI));
        public static readonly DependencyProperty HasNumberProperty = DependencyProperty.Register(nameof(HasNumber), typeof(bool), typeof(GameCellUI), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsFlaggedProeprty = DependencyProperty.Register(nameof(IsFlagged), typeof(bool), typeof(GameCellUI), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsMineProperty = DependencyProperty.Register(nameof(IsMine), typeof(bool), typeof(GameCellUI), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsRevealedProperty = DependencyProperty.Register(nameof(IsRevealed), typeof(bool), typeof(GameCellUI), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsWrongProperty = DependencyProperty.Register(nameof(IsWrong), typeof(bool), typeof(GameCellUI), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty MineVisibleProperty = DependencyProperty.Register(nameof(MineVisible), typeof(bool), typeof(GameCellUI), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty RightCommandProperty = DependencyProperty.Register(nameof(RightCommand), typeof(ICommand), typeof(GameCellUI));

        public GameCellUI()
        {
            InitializeComponent();
        }

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public bool HasNumber { get => (bool)GetValue(HasNumberProperty); set => SetValue(HasNumberProperty, value); }

        public bool IsFlagged { get => (bool)GetValue(IsFlaggedProeprty); set => SetValue(IsFlaggedProeprty, value); }

        public bool IsMine { get => (bool)GetValue(IsMineProperty); set => SetValue(IsMineProperty, value); }

        public bool IsRevealed { get => (bool)GetValue(IsRevealedProperty); set => SetValue(IsRevealedProperty, value); }

        public bool IsWrong { get => (bool)GetValue(IsWrongProperty); set => SetValue(IsWrongProperty, value); }

        public bool MineVisible { get => (bool)GetValue(MineVisibleProperty); set => SetValue(MineVisibleProperty, value); }

        public ICommand RightCommand { get => (ICommand)GetValue(RightCommandProperty); set => SetValue(RightCommandProperty, value); }
    }
}