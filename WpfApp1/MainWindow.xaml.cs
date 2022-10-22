using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        bool IsClicked = false;
        List<Coordinates> coordis = new List<Coordinates>();
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsClicked == false)
            {
                IsClicked = true;
            }
            Point ClickPos = e.GetPosition((IInputElement)sender);

            int ClickX = (int)ClickPos.X;
            int ClickY = 270 - (int)ClickPos.Y;

            XY.Content = "현재 좌표 : " + ClickX + " , " + ClickY;
            Console.WriteLine("MouseDown 위치 : " + ClickX + " " + ClickY);
        }

        private void Img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point ClickPos = e.GetPosition((IInputElement)sender);

            int ClickX = (int)ClickPos.X;
            int ClickY = 270 - (int)ClickPos.Y;
            if (IsClicked == true)
            {
                coordis.Add(new Coordinates() { index = coordis.Count, x = ClickX, y = ClickY });
                drawCircle(ClickX,270-ClickY,false);
                CList.ItemsSource = coordis;
                CList.Items.Refresh();
                IsClicked = false;
            }
            
            Console.WriteLine("MouseUp 위치 : " + ClickX + " " + ClickY);
        }

        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
            Point ClickPos = e.GetPosition((IInputElement)sender);

            int ClickX = (int)ClickPos.X;
            int ClickY = 270 - (int)ClickPos.Y;

            XY.Content = "현재 좌표 : " + ClickX + " , " + ClickY;
            Console.WriteLine("MouseMove 위치 : " + ClickX + " " + ClickY);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                if (File.Exists(openDialog.FileName))
                {
                    AllClear();
                    string fileExtention = System.IO.Path.GetExtension(openDialog.FileName);
                    if (fileExtention == ".jpg" || fileExtention == ".png" ||
                    fileExtention == ".bmp" || fileExtention == ".rle" ||
                    fileExtention == ".dib" || fileExtention == ".gif" ||
                    fileExtention == ".tif" || fileExtention == ".tiff" ||
                    fileExtention == ".raw")
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(openDialog.FileName, UriKind.RelativeOrAbsolute));
                        TransformedBitmap targetBitmap = new TransformedBitmap(bitmapImage, new ScaleTransform(360 / bitmapImage.Width, 270 / bitmapImage.Height));
                        Image.Source = targetBitmap;
                    }
                    else
                    {
                        MessageBox.Show("이미지 파일만 넣어주세요.", "꺼지셈", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (Image.Source != null)
            {
                if (MessageBox.Show("초기화 하시겠습니까?", "정말로?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MessageBox.Show("초기화 되어버렸습니다....");
                    AllClear();
                }
                else
                {
                    MessageBox.Show("초기화 되어버렸습니다....", "인생에 두 번의 기회는 없습니다");
                    AllClear();
                }
            }
        }

        private void DeletButton_Click(object sender, RoutedEventArgs e)
        {
            if (CList.SelectedIndex > -1)
            {
                coordis.RemoveAt(CList.SelectedIndex);
                canvas.Children.RemoveAt(CList.SelectedIndex);
                int count = 0;
                foreach (Coordinates c in coordis)
                {
                    c.index = count;
                    count++;
                }
                CList.Items.Refresh();
            }
            
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            string path = ".\\coordinates.txt";
            string text = "int X[" + coordis.Count + "] = {";
            foreach (Coordinates c in coordis)
            {
                text = text + c.x + ",";
            }
            text = text.TrimEnd(',');
            text += "};\n";
            text += "int Y[" + coordis.Count + "] = {";
            foreach (Coordinates c in coordis)
            {
                text = text + c.y + ",";
            }
            text = text.TrimEnd(',');
            text += "};";
            File.WriteAllText(path, text);

            Task.Delay(100);

            System.Diagnostics.Process.Start("Notepad.exe", ".\\coordinates.txt");
        }

        private void drawCircle(int x, int y, bool isSelected)
        {
            Ellipse ell = new Ellipse();
            if (isSelected)
            {
                ell.Width = 8;
                ell.Height = 8;
                ell.Fill = new SolidColorBrush(Color.FromRgb(49, 253, 46));
                ell.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                ell.StrokeThickness = 2;
            }
            else
            {
                ell.Width = 6;
                ell.Height = 6;
                ell.Fill = new SolidColorBrush(Color.FromRgb(208, 238, 23));
                ell.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                ell.StrokeThickness = 2;
            }
            

            canvas.Children.Add(ell);

            Canvas.SetLeft(ell, x);
            Canvas.SetTop(ell, y);
        }

        private void CList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            canvas.Children.Clear();
            foreach (Coordinates c in coordis)
            {
                if (c.index == CList.SelectedIndex)
                {
                    drawCircle(c.x, 270 - c.y, true);
                }
                else
                {
                    drawCircle(c.x, 270 - c.y, false);
                }

            }
        }
        private void AllClear()
        {
            Image.Source = null;
            coordis.Clear();
            canvas.Children.Clear();
            CList.Items.Refresh();
        }
    }

    public class Coordinates
    {
        public int index { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }
}
