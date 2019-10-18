using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Views.View;

namespace EllieLib
{
    class EasyView<T>
        where T : View
    {

        private readonly PageWrapper appWrapper;
        private readonly View view;
        public enum SoundTrigger { None, OnClick, OnHover };

        private Dictionary<int, SoundTrigger> soundTriggers;

        public EasyView(PageWrapper appWrapper, View view)
        {
            this.appWrapper = appWrapper;
            this.view = view;
            this.soundTriggers = new Dictionary<int, SoundTrigger>();
        }

        public EasyView(PageWrapper appWrapper,
            View view,
            EventHandler onClick)
                : this(appWrapper, view)
        {
            this.view.Click += onClick;
        }

        public T GetView()
        {
            return this.view as T;
        }

        public bool IsActive()
        {
            return this.view.Visibility == ViewStates.Visible;
        }

        public int Id()
        {
            return this.view.Id;
        }

        public string Text()
        {
            return this.view is TextView ? (this.view as TextView).Text : "";
        }
 
        public void Text(string text, params object[] args)
        {
            if (this.view is TextView)
            {
                (this.view as TextView).Text = String.Format(text, args);
            }
        }

        public void Color(Color color)
        {
            if (this.view is TextView)
            {
                (this.view as TextView).SetTextColor(color);
            }
        }

        public void Color(byte red, byte green, byte blue)
        {
            this.Color(new Color(red, green, blue));
        }

        public void Show()
        {
            this.view.Visibility = ViewStates.Visible;
        }

        public void ImageSource(int resourceId)
        {
            if (this.view is ImageView)
            {
                (this.view as ImageView).SetImageResource(resourceId);
            }
        }

        public void Hide(bool gone)
        {
            this.view.Visibility = gone ? ViewStates.Gone : ViewStates.Invisible;
        }

        public void Click(EventHandler eventHandler)
        {
            this.view.Click += eventHandler;
        }

        public void Hover(EventHandler<HoverEventArgs> eventHandler)
        {
            this.view.Hover += eventHandler;
        }

        public void SetSound(int id, SoundTrigger soundTrigger)
        {
            MediaPlayer toPlay = this.appWrapper.GetSound(id);
            if (toPlay == null)
            {
                this.soundTriggers.Remove(id);
                return;
            }

            if (this.soundTriggers.ContainsKey(id))
            {
                if (soundTrigger == SoundTrigger.None)
                {
                    this.soundTriggers.Remove(id);
                    return;
                }

                SoundTrigger existing = this.soundTriggers[id];
                if (soundTrigger == existing)
                    return;
            }
   
            switch (soundTrigger)
            {
                case SoundTrigger.OnClick:
                    Click(OnClickEventSound);
                    break;
                case SoundTrigger.OnHover:
                    Hover(OnHoverEventSound);
                    break;
            }

            this.soundTriggers.Add(id, soundTrigger);
        }

        void OnClickEventSound(object sender, EventArgs args)
        {
            foreach (KeyValuePair<int, SoundTrigger> trigger in this.soundTriggers)
            {
                int soundId = trigger.Key;
                SoundTrigger soundTrigger = trigger.Value;

                if (soundTrigger != SoundTrigger.OnClick) continue;
                appWrapper.GetSound(soundId)?.Start();
            }
        }

        void OnHoverEventSound(object sender, HoverEventArgs e)
        {
            foreach (KeyValuePair<int, SoundTrigger> trigger in this.soundTriggers)
            {
                int soundId = trigger.Key;
                SoundTrigger soundTrigger = trigger.Value;

                if (soundTrigger != SoundTrigger.OnHover) continue;
                appWrapper.GetSound(soundId)?.Start();
            }
        }

    }

}