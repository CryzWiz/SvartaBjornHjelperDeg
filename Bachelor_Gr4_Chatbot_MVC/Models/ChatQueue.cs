using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class ChatQueue
    {
        // Keep track of all users in queue to chat with a chat-worker
        // This mapping is for ALL groups
        private static ConcurrentQueue<int> _fullQueue = new ConcurrentQueue<int>();
        private static ConcurrentDictionary<string, int> _inFullQueue = new ConcurrentDictionary<string, int>();
        private static int _fullWaitTimeCounter = 0;
        private static TimeSpan _fullWaitTimeSum;
        public static string FullWaitTime { get; set; }
        public static int FullQueueCount { get { return _inFullQueue.Count; } }

        // Keep track of specified chatgroup queue
        public string ChatGroupName { get; set; }
        public string ChatGroupId { get; set; }
        private readonly ConcurrentQueue<int> _queue = new ConcurrentQueue<int>();
        private readonly ConcurrentDictionary<string, int> _inQueue = new ConcurrentDictionary<string, int>();
        public int Count { get { return _inQueue.Count; } }

        private int _waitTimeCounter = 0;
        private TimeSpan _waitTimeSum;
        public string CurrentWaitTime { get; set; }

       


        public ChatQueue()
        {

        }

        public ChatQueue(string chatGroupName, string chatGroupId)
        {
            ChatGroupName = chatGroupName;
            ChatGroupId = chatGroupId;
        }

        /// <summary>
        /// Try to enqueue given user. 
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userGroup"></param>
        /// <returns>Queuenumber (place in queue) if conversation was added to the queue successfully;
        /// null if the conversation alredy exists in queue</returns>
        public static int? Enqueue(int conversationId, string userGroup)
        {

            try
            {
                _fullQueue.Enqueue(conversationId);

                if (_inFullQueue.TryAdd(userGroup, conversationId))
                {

                    return _inFullQueue.Count();
                }

            } catch (ArgumentNullException anex) {
                
            } catch (OverflowException ofex)
            {

            } catch (Exception ex)
            {

            }
            return null;
        }

        public static bool RemoveFromFullQueue(string userGroup)
        {
           /* if(_inFullQueue.Values.Contains(conversationId))
            {
                _inFullQueue.Remove(userGroup, out int value);
                return true;
            }
            return false;*/

            if(_inFullQueue.Remove(userGroup, out int value))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Dequeu
        /// </summary>
        /// <returns>ConversationID if success, null otherwise</returns>
        public static int? Dequeue()
        {
            while (!_fullQueue.IsEmpty)
            {
                if (_fullQueue.TryDequeue(out int conversationId))
                {
                    if (_inFullQueue.Values.Contains(conversationId))
                    {
                        return conversationId;
                    }
                }

            }
            return null;
        }

        public int? DequeFromGroup()
        {
            // Remove from group queue and inqueue
            // remove from full inque 
            return null;
        }

        public void AddWaitTime(TimeSpan thisWaitTime)
        {
            _waitTimeSum += thisWaitTime;
            _waitTimeCounter ++;
            int avrageWaitTime = (int)_waitTimeSum.TotalSeconds / _waitTimeCounter;
            TimeSpan span = new TimeSpan(0, 0, avrageWaitTime);
            CurrentWaitTime = String.Format("{0}:{1:00}", (int)span.TotalMinutes, span.Seconds);
        }

        public static void AddFullWaitTime(TimeSpan thisWaitTime)
        {
            _fullWaitTimeSum += thisWaitTime;
            _fullWaitTimeCounter++;
            int avrageWaitTime = (int)_fullWaitTimeSum.TotalSeconds / _fullWaitTimeCounter;
            TimeSpan span = new TimeSpan(0, 0, avrageWaitTime);
            FullWaitTime = String.Format("{0}:{1:00}", (int)span.TotalMinutes, span.Seconds);
        }



    }
}
