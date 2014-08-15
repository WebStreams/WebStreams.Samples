// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The chat room controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebStreams.Sample
{
    using System;
    using System.Collections.Concurrent;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Dapr.WebStreams.Server;

    /// <summary>
    /// The chat room controller.
    /// </summary>
    [RoutePrefix("/chat")]
    public class ChatRoomController
    {
        /// <summary>
        /// The chat rooms.
        /// </summary>
        private readonly ConcurrentDictionary<string, ISubject<ChatEvent>> rooms = new ConcurrentDictionary<string, ISubject<ChatEvent>>();

        /// <summary>
        /// The stream of room updates.
        /// </summary>
        private readonly ISubject<string> roomUpdates = new Subject<string>();

        /// <summary>
        /// Joins the calling user to a chat room.
        /// </summary>
        /// <param name="room">
        /// The room name.
        /// </param>
        /// <param name="user">
        /// The joining user's name.
        /// </param>
        /// <param name="messages">
        /// The stream of chat messages from the user.
        /// </param>
        /// <returns>
        /// The stream of chat events.
        /// </returns>
        [Route("join")]
        public IObservable<ChatEvent> JoinRoom(string room, string user, IObservable<string> messages)
        {
            // Get or create the room being requested.
            var roomStream = this.GetOrAddRoom(room);

            // Send a happy little join message.
            roomStream.OnNext(new ChatEvent
                              {
                                  User = user,
                                  Message = "Joined!",
                                  Time = DateTime.UtcNow,
                                  Type = "presence"
                              });

            // Turn incoming messages into chat events and pipe them into the room.
            messages.Select(message => new ChatEvent
                                       {
                                           User = user,
                                           Message = message,
                                           Time = DateTime.UtcNow
                                       })
                .Subscribe(
                    roomStream.OnNext,
                    () => roomStream.OnNext(new ChatEvent
                                            {
                                                User = user,
                                                Message = "Left.",
                                                Time = DateTime.UtcNow,
                                                Type = "presence"
                                            }));

            return roomStream;
        }

        /// <summary>
        /// Returns the stream of chat rooms.
        /// </summary>
        /// <returns>The stream of chat rooms.</returns>
        [Route("rooms")]
        public IObservable<string> GetRooms()
        {
            var result = new ReplaySubject<string>();
            this.roomUpdates.Subscribe(result);
            foreach (var channel in this.rooms.Keys)
            {
                result.OnNext(channel);
            }

            return result;
        }

        /// <summary>
        /// Returns the chat room with the provided <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The room name.
        /// </param>
        /// <returns>
        /// The chat room with the provided <paramref name="name"/>.
        /// </returns>
        private ISubject<ChatEvent> GetOrAddRoom(string name)
        {
            var added = default(ISubject<ChatEvent>);
            var result = this.rooms.GetOrAdd(
                name,
                _ => added = new ReplaySubject<ChatEvent>(100));
            
            // If a new room was actually added, fire an update.
            if (result.Equals(added))
            {
                this.roomUpdates.OnNext(name);
            }

            return result;
        }
    }
}