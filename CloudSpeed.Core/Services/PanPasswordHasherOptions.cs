using Autofac;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CloudSpeed.Services
{
    public class PanPasswordHasherOptions
    {
        private static readonly RandomNumberGenerator _defaultRng = RandomNumberGenerator.Create(); // secure PRNG

        /// <summary>
        /// Gets or sets the number of iterations used when hashing passwords using PBKDF2. Default is 10,000.
        /// </summary>
        /// <value>
        /// The number of iterations used when hashing passwords using PBKDF2.
        /// </value>
        /// <remarks>
        /// This value is only used when the compatibility mode is set to 'V3'.
        /// The value must be a positive integer. 
        /// </remarks>
        public int IterationCount { get; set; } = 10000;

        // for unit testing
        internal RandomNumberGenerator Rng { get; set; } = _defaultRng;
    }
}
