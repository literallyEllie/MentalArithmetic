using System.Collections.Generic;

using Android.Content;
using Android.Media;
using Android.Views;

namespace EllieLib
{
    // <summary>Wrapper class to easily manage the page.</summary>
    class PageWrapper
    {

        private Dictionary<int, object> store;
        private Dictionary<int, MediaPlayer> sounds;

        private List<int> active;

        public PageWrapper()
        {
            // Initialize stores.
            this.store = new Dictionary<int, object>();
            this.sounds = new Dictionary<int, MediaPlayer>();
            this.active = new List<int>();
        }

        // <summary>Gets the <c>EasyView</c> stored type of <c>T</c> where it is a <c>View</c>.</summary>  
        public EasyView<T> Get<T>(int id)
            where T : View
        {
            return store[id] as EasyView<T>;
        }

        // <summary>Deactivates all the views and clears the register.</summary>
        public void DeactivateAll()
        {
            foreach(int active in this.active)
            {
                EasyView<View> easyView = this.Get<View>(active);
                easyView.Hide(true);
            }
            active.Clear();
        }

        // <summary>Checks if <c>id</c> is active by checking if active contains that id.</summary>
        public bool IsActive(int id)
        {
            return active.Contains(id);
        }

        // <summary>Toggles a view active by id and shows it.</summary>
        public void ToggleActive(int id)
        {
            // If doesn't exist, return.
            EasyView<View> view = this.Get<View>(id);
            if (view == null)
                return;

            // If is active, hide and remove from active.
            // If isn't active, show and add the id to active.
            if (this.IsActive(id))
            {
                view.Hide(true);
                this.active.Remove(id);
            }
            else
            {
                view.Show();
                this.active.Add(id);
            }

        }

        // <summary>Gets an active view by id.</<summary>
        public T GetActive<T>(int id)
            where T : View
        {
            return store[id] as T;
        }

        // <summary>Allows for registering <c>EasyView</c>s in bulk.</summary>
        public void Add<T>(params EasyView<T>[] views)
            where T : View
        {
            foreach (EasyView<T> view in views)
            {
                store.Add(view.Id(), view);
                if (view.IsActive())
                    active.Add(view.Id());
            }
        }

        // <summary>Gets a registered sound by id.</summary>
        public MediaPlayer GetSound(int id)
        {
            return this.sounds[id];
        }

        // <summary>Registers a sound so it can be used in an <c>EasyView</c>.</summary>
        public MediaPlayer AddSound(int id, Context context)
        {
            MediaPlayer player = MediaPlayer.Create(context, id);
            this.sounds.Add(id, player);
            return player;
        }

    }
}