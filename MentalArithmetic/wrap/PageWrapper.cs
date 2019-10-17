using System.Collections.Generic;

using Android.Content;
using Android.Media;
using Android.Views;

namespace EllieLib
{
    class PageWrapper
    {
        // todo add add view.

        private Dictionary<int, object> store;
        private Dictionary<int, MediaPlayer> sounds;

        private List<int> active;

        public PageWrapper()
        {
            this.store = new Dictionary<int, object>();
            this.sounds = new Dictionary<int, MediaPlayer>();
            this.active = new List<int>();
        }

        public EasyView<T> Get<T>(int id)
            where T : View
        {
            return store[id] as EasyView<T>;
        }

        public void DeactivateAll()
        {
            foreach(int active in this.active)
            {
                EasyView<View> easyView = this.Get<View>(active);
                easyView.Hide(true);
            }
            active.Clear();
        }

        public bool IsActive(int id)
        {
            return active.Contains(id);
        }

        public void ToggleActive(int id)
        {
            EasyView<View> view = this.Get<View>(id);
            if (view == null)
                return;

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

        public T GetActive<T>(int id)
            where T : View
        {
            return store[id] as T;
        }

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

        public MediaPlayer GetSound(int id)
        {
            return this.sounds[id];
        }

        public MediaPlayer AddSound(int id, Context context)
        {
            MediaPlayer player = MediaPlayer.Create(context, id);
            this.sounds.Add(id, player);
            return player;
        }

    }
}