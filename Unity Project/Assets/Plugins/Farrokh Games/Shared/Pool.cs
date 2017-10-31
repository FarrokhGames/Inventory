using System.Collections.Generic;
using System;
using System.Linq;

namespace FarrokhGames.Shared
{
    /// <summary>
    /// A generic pool of objects that can be retrieved and recycled without invoking additional allocations.
    /// 
	/// Please note that care must be taken when pooling objects, since the object
	/// has to be manually reset after retrieval from the pool. Its constructor will
	/// not be run again after the first time!
    /// </summary>
    public class Pool<T> where T : class
    {
        private List<T> _inactive = new List<T>();
        private List<T> _active = new List<T>();
        private Func<T> _creator;
        private bool _allowTakingWhenEmpty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">A function to be used for creating new objects</param>
        /// <param name="initialCount">Initial count of objects in the pool</param>
        /// <param name="allowTakingWhenEmpty">If true, objects can be taken from the pool even when the pool is empty</param>
        public Pool(Func<T> creator, int initialCount = 0, bool allowTakingWhenEmpty = true)
        {
            if (creator == null) throw new ArgumentNullException("pCreator");
            if (initialCount < 0) throw new ArgumentOutOfRangeException("pInitialCount", "Initial count cannot be negative");

            _creator = creator;
            _inactive.Capacity = initialCount;
            _allowTakingWhenEmpty = allowTakingWhenEmpty;

            // Create initial items
            while (_inactive.Count < initialCount)
            {
                _inactive.Add(_creator());
            }
        }

        /// <summary>
        /// Returns the current number of objects in the pool
        /// </summary>
        public int Count
        {
            get { return _inactive.Count; }
        }

        /// <summary>
        /// Returns true if the pool is empty.
		/// Note that objects can still be taken from an empty pool, if allowTakingWhenEmpty 
        /// is set to true, but that additional allocations will be imposed.
        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        /// <summary>
        /// Returns true of an item can be taken from the pool right now.
        /// This is not the same as whether or not the pool is empty.
        /// </summary>
        public bool CanTake
        {
            get { return _allowTakingWhenEmpty || !IsEmpty; }
        }

        /// <summary>
        /// Takes an object from the pool.
        /// If the pool is empty and allowTakingWhenEmpty is enabled, it will allocate
        /// a new object. Otherwise, a pooled object is returned. Either way, the object 
        /// should eventually be returned to the pool by calling Recycle().
        /// </summary>
        /// <returns>An object of type T, or null if nothing could be taken</returns>
        public T Take()
        {
            if (IsEmpty)
            {
                if (_allowTakingWhenEmpty)
                {
                    var obj = _creator();
                    _inactive.Add(obj);
                    return TakeInternal();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return TakeInternal();
            }
        }

        private T TakeInternal()
        {
            T obj = _inactive[_inactive.Count - 1];
            _inactive.RemoveAt(_inactive.Count - 1);
            _active.Add(obj);
            return obj;
        }

        /// <summary>
		/// Recycles an object into the pool. After this point, the object is
		/// considered dead and should not be used until taken from the pool
		/// again.
        /// </summary>
        public void Recycle(T item)
        {
            if (!_active.Contains(item)) { throw new InvalidOperationException("An item was recycled even though it was not part of the pool"); }
            _inactive.Add(item);
            _active.Remove(item);
        }

        /// <summary>
        /// Returns a copied list of all inactive items
        /// </summary>
        public List<T> GetInactive()
        {
            return _inactive.ToList();
        }

        /// <summary>
        /// Returns a copied list of all active items
        /// </summary>
        public List<T> GetActive()
        {
            return _active.ToList();
        }
    }
}