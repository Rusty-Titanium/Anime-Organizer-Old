using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Anime_Organizer.Utilities
{
    public class DataGridScrollHelper
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // This area is the fix for the datagrid scrolling acting super strange
        // Huge Shout out to gekka for creating a working solution without any edits
        // Original post: https://docs.microsoft.com/en-us/answers/questions/102861/wpf-datagrid-resizing-column-causes-extreme-scroll.html
        // His Fix link: https://gist.github.com/gekka/4b85228ac7ad29457145db0978f9cfd4

        private const string LEFT = "PART_LeftHeaderGripper";
        private const string RIGHT = "PART_RightHeaderGripper";
        private Point startPoint;
        private double startWidth;
        private DataGridColumn targetColumn;

        //Used to be private but changed to public to attempt fixing something.
        public void DataGridColumnHeader_Loaded(object sender)
        {
            var header = (DataGridColumnHeader)sender;
            var thumbLeft = header.Template.FindName(LEFT, header) as Thumb;
            var thumbRight = header.Template.FindName(RIGHT, header) as Thumb;

            if (thumbLeft != null)
            {
                thumbLeft.AddHandler(Thumb.DragStartedEvent, (DragStartedEventHandler)Thumb_DragStarted, true);
                thumbLeft.AddHandler(Thumb.DragCompletedEvent, (DragCompletedEventHandler)Thumb_DragCompleted, true);

                thumbRight.AddHandler(Thumb.DragStartedEvent, (DragStartedEventHandler)Thumb_DragStarted, true);
                thumbRight.AddHandler(Thumb.DragCompletedEvent, (DragCompletedEventHandler)Thumb_DragCompleted, true);
            }
        }


        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var thumb = (Thumb)sender;
            DataGrid dg = GetParent<DataGrid>(thumb);
            startPoint = Mouse.GetPosition(dg);

            DataGridColumnHeader header = GetParent<DataGridColumnHeader>(thumb);

            if (thumb.Name == RIGHT)
            {
                targetColumn = header.Column;
            }
            else
            {
                int index = header.Column.DisplayIndex - 1;
                if (index < 0)
                {
                    return;
                }
                targetColumn = dg.Columns.FirstOrDefault(_ => _.DisplayIndex == index);
            }
            startWidth = targetColumn.ActualWidth;
            thumb.PreviewMouseMove += Thumb_PreviewMouseMove;
        }

        private void Thumb_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var thumb = (Thumb)sender;
            if (!thumb.IsDragging || targetColumn == null) { return; }

            e.Handled = true;

            DataGrid dg = GetParent<DataGrid>(thumb);
            Point currentPoint = Mouse.GetPosition(dg);
            double diffX = (currentPoint - startPoint).X;
            double newWidth = Math.Max(targetColumn.MinWidth, Math.Min(startWidth + diffX, targetColumn.MaxWidth));
            var length = new DataGridLength(newWidth);

            targetColumn.SetValue(DataGridColumn.WidthProperty, length);
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ((Thumb)sender).PreviewMouseMove -= Thumb_PreviewMouseMove;
            targetColumn = null;
        }

        private T GetParent<T>(DependencyObject d) where T : DependencyObject
        {
            T t = null;
            while (t == null) { t = d as T; d = VisualTreeHelper.GetParent(d); }
            return t;
        }

    }

}
