using System;
using System.Collections.Generic;

using Android.Graphics;
using Android.Media;
using Android.Views;
using Android.Widget;
using static Android.Views.View;

namespace EllieLib
{

    // <summary>Class <c>EasyView</c> is an easy wrapper for any class extending <c>View</c>.</summary>
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

            // Register sound listeners.
            this.Click(OnClickEventSound);
            this.Hover(OnHoverEventSound);
        }

        // <summary>Wrapper but with an onclick event since it is used so often.</summary>
        public EasyView(PageWrapper appWrapper,
            View view,
            EventHandler onClick)
                : this(appWrapper, view)
        {
            this.view.Click += onClick;
        }

        // <summary>Gets the underlying view</summary>
        public T GetView()
        {
            return this.view as T;
        }

        // <summary>Checks if its active based on the visbility</summary>
        public bool IsActive()
        {
            return this.view.Visibility == ViewStates.Visible;
        }

        // <summary>Gets the tracking resource ID of the view</summary>
        public int Id()
        {
            return this.view.Id;
        }

        // <summary>Returns the text of the object. 
        // If does not implement <c>TextView</c> it will return an empty string.</summary>
        public string Text()
        {
            return this.view is TextView ? (this.view as TextView).Text : "";
        }

        // <summary>Sets the text of the object with optional arguements to format it</summary>
        public void Text(string text, params object[] args)
        {
            if (this.view is TextView)
            {
                (this.view as TextView).Text = String.Format(text, args);
            }
        }

        // <summary>Checks if the text value of the object is empty.</summary>
        public bool IsEmpty()
        {
            return this.view is TextView && (this.view as TextView).Text.Trim() == "";
        }

        // <summary>Sets the text color of a <c>TextView</c>.</summary>
        public void Color(Color color)
        {
            if (this.view is TextView)
            {
                (this.view as TextView).SetTextColor(color);
            }
        }

        // <summary>Sets the text color of a <c>TextView</c> using RGB set.</<summary>
        public void Color(byte red, byte green, byte blue)
        {
            this.Color(new Color(red, green, blue));
        }

        // <summary>Shows the element by setting the state to Visible.</summary>
        public void Show()
        {
            this.view.Visibility = ViewStates.Visible;
        }

        // <summary>Sets the image source of an <c>ImageView</c>.</summary>
        public void ImageSource(int resourceId)
        {
            if (this.view is ImageView)
            {
                (this.view as ImageView).SetImageResource(resourceId);
            }
        }

        // <summary>Hides the <c>View</c>.
        // If <c>gone</c> it will set the state to <c>Gone</c>, else <c>Invisible</c>.</summary>
        public void Hide(bool gone)
        {
            this.view.Visibility = gone ? ViewStates.Gone : ViewStates.Invisible;
        }

        // <summary>Alias for setting the <c>View</c> to enabled or not disabling interaction.</summary>
        public void AllowInteraction(bool allowInteraction)
        {
            this.view.Enabled = allowInteraction;
        }

        // <summary>Adds event handler for when they click.</summary>
        public void Click(EventHandler eventHandler)
        {
            this.view.Click += eventHandler;
        }

        // <summary>Adds event handler for when they hover</summary>
        public void Hover(EventHandler<HoverEventArgs> eventHandler)
        {
            this.view.Hover += eventHandler;
        }

        // <summary>Sets the sound which will play when the <c>SoundTrigger</c> is met.</summary>
        // The sound must be registered with the <c>PageWrapper</c>.</summary>
        public void SetSound(int id, SoundTrigger soundTrigger)
        {
            MediaPlayer toPlay = this.appWrapper.GetSound(id);
            // If the media doesn't exist in the wrapper, unregister here and return.
            if (toPlay == null)
            {
                this.soundTriggers.Remove(id);
                return;
            }

            // If is registered with this view, handle.
            if (this.soundTriggers.ContainsKey(id))
            {
                // If the new state is nothing, unregister
                if (soundTrigger == SoundTrigger.None)
                {
                    this.soundTriggers.Remove(id);
                    return;
                }

                // If the new state is the same as the old state, ignore.
                SoundTrigger existing = this.soundTriggers[id];
                if (soundTrigger == existing)
                    return;
            }
  
            // Register the sound object with this view.
            this.soundTriggers.Add(id, soundTrigger);
        }

        // <summary>Event handle which goes through every registered sound when the view is clicked and activates respectively.</summary>
        void OnClickEventSound(object sender, EventArgs args)
        {
            foreach (KeyValuePair<int, SoundTrigger> trigger in this.soundTriggers)
            {
                int soundId = trigger.Key;
                SoundTrigger soundTrigger = trigger.Value;

                // If isn't for click event.
                if (soundTrigger != SoundTrigger.OnClick) continue;
                // Passive Start just incase null.
                appWrapper.GetSound(soundId)?.Start();
            }
        }

        // <summary>Event handle which goes through every registered sound when the view is hovered and activates respectively.</summary>
        void OnHoverEventSound(object sender, HoverEventArgs e)
        {
            foreach (KeyValuePair<int, SoundTrigger> trigger in this.soundTriggers)
            {
                int soundId = trigger.Key;
                SoundTrigger soundTrigger = trigger.Value;

                // If isn't for hover event.
                if (soundTrigger != SoundTrigger.OnHover) continue;
                // Passive Start just incase null.
                appWrapper.GetSound(soundId)?.Start();
            }
        }

    }

}