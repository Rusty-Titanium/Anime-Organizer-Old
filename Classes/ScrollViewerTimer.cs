using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Anime_Organizer.Classes
{
    class ScrollViewerTimer
    {
        private DispatcherTimer timer;
        private double timerNum = 0.0;
        private int scrollValuePos = 0, scrollValueNeg = 0;

        public void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;

            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(100);
                timer.Start();
            }
            else
                timerNum = 0.0;


            e.Handled = true;

            if (e.Delta > 0)
                scrollValuePos += e.Delta;
            else
                scrollValueNeg += e.Delta;


            if (scrollValuePos >= 40) //true if scrolling is upwards
            {
                if (scrollViewer != null)
                {
                    scrollViewer.LineUp();
                    scrollViewer.LineUp();
                    scrollViewer.LineUp();
                }

                scrollValuePos = 0;
            }
            else if (scrollValueNeg <= -40)
            {
                if (scrollViewer != null)
                {
                    scrollViewer.LineDown();
                    scrollViewer.LineDown();
                    scrollViewer.LineDown();
                }

                scrollValueNeg = 0;
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            timerNum += 0.1;

            if (timerNum == 1.0)
            {
                timer.Stop();
                timer = null;
                scrollValuePos = 0;
                scrollValueNeg = 0;
            }
        }



    }

}
