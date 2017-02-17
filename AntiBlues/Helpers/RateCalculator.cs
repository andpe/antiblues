using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiBlues.Helpers
{
    class RateTrigger
    {
        #region Private properties
        private List<DateTime> mHits = new List<DateTime>();
        private int SamplePeriod = 20;
        private int ActivationPoint = 7;
        #endregion

        #region Public properties
        /// <summary>
        /// Number of hits in the last sample time.
        /// </summary>
        public int Hits
        {
            get {
                return mHits.Count(x => (DateTime.Now - x).TotalMilliseconds < SamplePeriod);
            }
        }

        public delegate void TriggerCallback();
        public event TriggerCallback ActivationPointReached;
        #endregion

        public RateTrigger(int ActivationPoint, int SamplePeriod)
        {
            this.ActivationPoint = ActivationPoint;
            this.SamplePeriod = SamplePeriod;
        }
        
        /// <summary>
        /// Trigger an update of the number of hits.
        /// </summary>
        public void Hit()
        {
            mHits.Add(DateTime.Now);
            ActivationCheck();
        }

        private void ActivationCheck()
        {
            if(this.Hits > this.ActivationPoint)
            {
                mHits.RemoveAll(x => (DateTime.Now - x).TotalMilliseconds > SamplePeriod);
                ActivationPointReached?.Invoke();
            }
        }
    }
}
