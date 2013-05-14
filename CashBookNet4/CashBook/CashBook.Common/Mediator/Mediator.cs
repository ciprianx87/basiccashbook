using System;
using System.Collections.Generic;
using System.Diagnostics;
using CashBook.Common.Mediator;

namespace CashBook.Common.Mediator
{
    public sealed class Mediator
    {
        #region Fields

        private static Mediator instance = null;

        private readonly Dictionary<string, List<Action<object>>> callbacks = new Dictionary<string, List<Action<object>>>();

        #endregion

        #region Singleton Instance

        /// <summary>
        /// Singleton instance for mediator
        /// </summary>
        public static Mediator Instance
        {
            get
            {
                if (instance == null)
                    instance = new Mediator();
                return instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register to receive messages
        /// </summary>
        /// <param name="id">Id of an action</param>
        /// <param name="action">The action which will be performed when a message is redeived for the Id</param>
        public void Register(MediatorActionType id, Action<object> action)
        {
            if (!IsRegistered(id.ToString(), action))
            {
                if (!callbacks.ContainsKey(id.ToString()))
                {
                    callbacks[id.ToString()] = new List<Action<object>>();
                }
                callbacks[id.ToString()].Add(action);
            }
        }

        /// <summary>
        /// When a control is disposed unregister all actions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        public void Unregister(MediatorActionType id, Action<object> action)
        {
            if (callbacks.ContainsKey(id.ToString()))
            {
                callbacks[id.ToString()].Remove(action);

                if (callbacks[id.ToString()].Count == 0)
                {
                    callbacks.Remove(id.ToString());
                }
            }
        }

        /// <summary>
        /// Send message (call all delegates) for an Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public void SendMessage(MediatorActionType id, object message)
        {
            try
            {
                if (callbacks.ContainsKey(id.ToString()))
                {
                    callbacks[id.ToString()].ForEach(action => action(message));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                Debug.WriteLine(ex.Message);
            }
        }

        public bool IsRegistered(string id, Action<object> action)
        {
            bool rez = false;
            if (callbacks.ContainsKey(id))
            {
                rez = callbacks[id].Contains(action);
            }
            return rez;
        }

        #endregion
    }

}