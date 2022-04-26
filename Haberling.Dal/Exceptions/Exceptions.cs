using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.Dal
{
  
        /// <summary>
        /// InvalidTaskTypeException
        /// </summary>
        [Serializable]
        public class InvalidTaskTypeException : Exception
        {
            /// <summary>
            /// Konstruktor
            /// </summary>
            public InvalidTaskTypeException() { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            public InvalidTaskTypeException(string message) : base(message) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            /// <param name="inner">Inner-Exception</param>
            public InvalidTaskTypeException(string message, Exception inner) : base(message, inner) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="info">Serialization Info</param>
            /// <param name="context">Context</param>
            protected InvalidTaskTypeException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// BelegImportException
        /// </summary>
        [Serializable]
        public class BelegImportException : Exception
        {
            /// <summary>
            /// Konstruktor
            /// </summary>
            public BelegImportException() { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            public BelegImportException(string message) : base(message) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            /// <param name="inner">Inner-Exception</param>
            public BelegImportException(string message, Exception inner) : base(message, inner) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="info">Serialization Info</param>
            /// <param name="context">Context</param>
            protected BelegImportException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// BelegTransformException
        /// </summary>
        [Serializable]
        public class BelegTransformException : Exception
        {
            /// <summary>
            /// Konstruktor
            /// </summary>
            public BelegTransformException() { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            public BelegTransformException(string message) : base(message) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            /// <param name="inner">Inner-Exception</param>
            public BelegTransformException(string message, Exception inner) : base(message, inner) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="info">Serialization Info</param>
            /// <param name="context">Context</param>
            protected BelegTransformException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// LagerbuchungException
        /// </summary>
        [Serializable]
        public class LagerbuchungException : Exception
        {
            /// <summary>
            /// Konstruktor
            /// </summary>
            public LagerbuchungException() { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            public LagerbuchungException(string message) : base(message) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            /// <param name="inner">Inner-Exception</param>
            public LagerbuchungException(string message, Exception inner) : base(message, inner) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="info">Serialization Info</param>
            /// <param name="context">Context</param>
            protected LagerbuchungException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// ReweBuchungException
        /// </summary>
        [Serializable]
        public class ReweBuchungException : Exception
        {
            /// <summary>
            /// Konstruktor
            /// </summary>
            public ReweBuchungException() { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            public ReweBuchungException(string message) : base(message) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="message">Fehlermeldung</param>
            /// <param name="inner">Inner-Exception</param>
            public ReweBuchungException(string message, Exception inner) : base(message, inner) { }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="info">Serialization Info</param>
            /// <param name="context">Context</param>
            protected ReweBuchungException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
