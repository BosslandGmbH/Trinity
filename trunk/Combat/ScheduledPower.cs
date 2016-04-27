using System;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    /// <summary>
    /// Data for Scheduled Power
    /// </summary>
    internal class ScheduledPower
    {
        /// <summary>
        /// Gets or sets the power.
        /// </summary>
        /// <value>The <see cref="SNOPower"/>.</value>
        public SNOPower Power
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the delay in ms between 2 cast.
        /// </summary>
        /// <value>The delay.</value>
        public int Delay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date of last usage.
        /// </summary>
        /// <value>The last usage.</value>
        public DateTime LastUsage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the activation condition.
        /// </summary>
        /// <value>The condition.</value>
        public Func<SNOPower, bool> Condition
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is ScheduledPower && Power == ((ScheduledPower)obj).Power);            
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1} ms) -> {2}", Power, Delay, LastUsage);
        }
    }
}
