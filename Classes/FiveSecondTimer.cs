using Anime_Organizer.Windows.Other;
using Anime_Organizer.Windows.UserControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Anime_Organizer.Classes
{
    class FiveSecondTimer
    {
        /**
         * Controls this is used in:
         * - RandomAnime - Done
         * - MALID - Done
         * - SearchForAnime - Done
         */

        private DispatcherTimer timer;
        public double timerNum = 5.0;

        private Label cooldown;
        private Object control;

        public FiveSecondTimer(Label label, Object obj)
        {
            cooldown = label;
            control = obj;

            timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Interval = TimeSpan.FromMilliseconds(94); // .94 Seconds
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// A visual timer that is on a 5 second cooldown when ever it is ran.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            timerNum = Math.Round(timerNum - 0.1, 1);
            cooldown.Content = "Cooldown: " + timerNum + " sec";

            if (timerNum == 0.0)
            {
                cooldown.Visibility = Visibility.Collapsed;
                timerNum = 5.0;
                cooldown.Content = "Cooldown: " + timerNum + " sec";
                
                switch (control.GetType().Name)
                {
                    case "MALID":
                        ((MALID)control).Timer_Finished();
                        break;
                    case "RandomAnime":
                        ((RandomAnime)control).Timer_Finished();
                        break;
                }

                timer.Stop();
            }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            timer.Start();
        }

    }
}
