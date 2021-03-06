﻿using System;
using SharpTelegram.Schema;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Disa.Framework.Telegram
{
    public static class TelegramUtils
    {
        public static T RunSynchronously<T>(Task<T> task)
        {
            try
            {
                task.Wait();
                return task.Result;
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
        }

        public static void RunSynchronously(Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
        }

		public static IChat GetChatFromUpdate(Updates update)
		{
		    return update.Chats.FirstOrDefault();
		}

        public static IInputUser CastUserToInputUser(IUser user)
        {
            var userEmpty = user as UserEmpty;
			var userObj = user as User;

            if (userEmpty != null)
            {
                return new InputUserEmpty
                {
                    // nothing
                };
            }
            if (userObj != null)
            {
				if (userObj.Self != null)
				{
					return new InputUserSelf
					{
						
					};
				}
				return new InputUser
				{
					AccessHash = userObj.AccessHash,
					UserId = userObj.Id,
				};
            }

            return null;
        }

        public static string GetChatTitle(IChat chat)
        {
            var chatEmpty = chat as ChatEmpty;
            var chatForbidden = chat as ChatForbidden;
            var chatChat = chat as Chat;
            var chatChannel = chat as Channel;
            var chatChannelForbidden = chat as ChannelForbidden;

            if (chatEmpty != null)
            {
                return chatEmpty.Id.ToString(CultureInfo.InvariantCulture);
            }
            if (chatForbidden != null)
            {
                return chatForbidden.Title;
            }
            if (chatChat != null)
            {
                return chatChat.Title;
            }
            if (chatChannel != null)
            {
                return chatChannel.Title;
            }
            if (chatChannelForbidden != null)
            {
                return chatChannelForbidden.Title;
            }

            return null;
        }

        public static void SetChatTitle(IChat chat, string title)
        {
            var chatForbidden = chat as ChatForbidden;
            var chatChat = chat as Chat;
            var chatChannel = chat as Channel;
            var chatChannelForbidden = chat as ChannelForbidden;

            if (chatForbidden != null)
            {
                chatForbidden.Title = title;
            }
            if (chatChat != null)
            {
                chatChat.Title = title;
            }
            if (chatChannel != null)
            {
                chatChannel.Title = title;
            }
            if (chatChannelForbidden != null)
            {
                chatChannelForbidden.Title = title;
            }
        }

        public static bool GetChatUpgraded(IChat chat)
        {
            var chatForbidden = chat as ChatForbidden;
            var chatChat = chat as Chat;
            var chatChannel = chat as Channel;
            var chatChannelForbidden = chat as ChannelForbidden;

            if (chatForbidden != null)
            {
                return false;
            }
            if (chatChat != null)
            {
                return chatChat.MigratedTo != null;
            }
            if (chatChannel != null)
            {
                return false;
            }
            if (chatChannelForbidden != null)
            {
                return false;
            }
            return false;
        }

        public static bool GetChatKicked(IChat chat)
        {
            var chatForbidden = chat as ChatForbidden;
            var chatChat = chat as Chat;
            var chatChannel = chat as Channel;
            var chatChannelForbidden = chat as ChannelForbidden;

            if (chatForbidden != null)
            {
                return true;
            }
            if (chatChat != null)
            {
                return chatChat.Kicked != null;
            }
            if (chatChannel != null)
            {
                return chatChannel.Kicked != null;
            }
            if (chatChannelForbidden != null)
            {
                return true;
            }
            return false;
        }

        public static bool GetChatLeft(IChat chat)
        {
            var chatForbidden = chat as ChatForbidden;
            var chatChat = chat as Chat;
            var chatChannel = chat as Channel;
            var chatChannelForbidden = chat as ChannelForbidden;

            if (chatForbidden != null)
            {
                return true;
            }
            if (chatChat != null)
            {
                return chatChat.Left != null;
            }
            if (chatChannel != null)
            {
                return chatChannel.Left != null;
            }
            if (chatChannelForbidden != null)
            {
                return true;
            }
            return false;
        }

        public static string GetPeerId(IPeer peer)
        {
            var peerChat = peer as PeerChat;
            var peerUser = peer as PeerUser;
            var peerChannel = peer as PeerChannel;
            if (peerChat != null)
            {
                return peerChat.ChatId.ToString(CultureInfo.InvariantCulture);
            }
            if (peerUser != null)
            {
                return peerUser.UserId.ToString(CultureInfo.InvariantCulture);
            }
            if (peerChannel != null)
            {
                return peerChannel.ChannelId.ToString(CultureInfo.InvariantCulture);
            }
            return null;
        }

        public static string GetChatId(IChat chat)
        {
            var chatEmpty = chat as ChatEmpty;
            var chatForbidden = chat as ChatForbidden;
            var chatChat = chat as Chat;
            var chatChannel = chat as Channel;
            var chatChannelForbidden = chat as ChannelForbidden;

            if (chatEmpty != null)
            {
                return null;
            }
            if (chatForbidden != null)
            {
                return null;
            }
            if (chatChat != null)
            {
                return chatChat.Id.ToString(CultureInfo.InvariantCulture);
            }
            if (chatChannel != null)
            {
                return chatChannel.Id.ToString(CultureInfo.InvariantCulture);
            }
            if (chatChannelForbidden != null)
            {
                return null;
            }
            return null;
        }

        public static string ConvertTelegramPhoneNumberIntoInternational(string phoneNumber)
        {
            return PhoneBook.TryGetPhoneNumberLegible("+" + phoneNumber);
        }

        public static string GetUserPhoneNumber(IUser user)
        {
            var userEmpty = user as UserEmpty;
			var userObj = user as User;
			if (userEmpty != null)
			{
				return null;
			}
			if (userObj != null)
			{
				return userObj.Phone;
			}
            return null;
        }

        public static string GetUserName(IUser user)
        {
            var userEmpty = user as UserEmpty;
            var userObj = user as User;

			if (userEmpty != null)
            {
                return userEmpty.Id.ToString(CultureInfo.InvariantCulture);
            }
			if (userObj != null)
			{
				return userObj.FirstName + " " + userObj.LastName;
			}
            return null;
        }

		public static Tuple<string, string> GetName(IUser user)
		{
			var userEmpty = user as UserEmpty;
			var userObj = user as User;

			if (userEmpty != null)
			{
				return Tuple.Create(userEmpty.Id.ToString(CultureInfo.InvariantCulture), (string)null);
			}
			if (userObj != null)
			{
				return Tuple.Create(userObj.FirstName, userObj.LastName);
			}
			return null;
		}

        public static ulong GetAccessHash(IUser user)
        {
			var userObj = user as User;
			if (userObj != null)
			{
				return userObj.AccessHash;
			}
            return 0;
        }

        internal static ulong GetChannelAccessHash(IChat chat)
        {
            var channel = chat as Channel;
            if (channel != null)
            {
                return channel.AccessHash;
            }
            return 0;
        }

        public static string GetUserIdFromChannelParticipant(IChannelParticipant partyParticipant)
        {
            var channelParticipant = partyParticipant as ChannelParticipant;
            var channelParticipantSelf = partyParticipant as ChannelParticipantSelf;
            var channelParticipantEditor = partyParticipant as ChannelParticipantEditor;
            var channelParticipantModerator = partyParticipant as ChannelParticipantModerator;
            var channelParticipantCreator = partyParticipant as ChannelParticipantCreator;
            var channelParticipantKicked = partyParticipant as ChannelParticipantKicked;


            if (channelParticipant != null)
            {
                return channelParticipant.UserId.ToString(CultureInfo.InvariantCulture);
            }
            if (channelParticipantSelf != null)
            {
               return channelParticipantSelf.UserId.ToString(CultureInfo.InvariantCulture);
            }
            if (channelParticipantEditor != null)
            {
                return channelParticipantEditor.UserId.ToString(CultureInfo.InvariantCulture);
            }
            if (channelParticipantModerator != null)
            {
                return channelParticipantModerator.UserId.ToString(CultureInfo.InvariantCulture);
            }
            if (channelParticipantCreator != null)
            {
                return channelParticipantCreator.UserId.ToString(CultureInfo.InvariantCulture);
            }
            if (channelParticipantKicked != null)
            { 
                return channelParticipantKicked.UserId.ToString(CultureInfo.InvariantCulture);
            }
            return null;
        }

        public static FileLocation GetChatThumbnailLocation(IChat chat, bool small)
        {
            var chatEmpty = chat as ChatEmpty;
            var chatForbidden = chat as ChatForbidden;
            var chatChat = chat as Chat;
            var chatChannel = chat as Channel;
            var chatChannelForbidden = chat as ChannelForbidden;

            if (chatEmpty != null)
            {
                return null;
            }
            if (chatForbidden != null)
            {
                return null;
            }
            if (chatChat != null)
            {
                return GetFileLocationFromPhoto(chatChat.Photo, small);
            }
            if (chatChannel != null) 
            {
                return GetFileLocationFromPhoto(chatChannel.Photo, small);
            }
            if (chatChannelForbidden != null) 
            {
                return null;
            }
            return null;
        }

        public static FileLocation GetUserPhotoLocation(IUser user, bool small)
        {
            var userEmpty = user as UserEmpty;
			var userObj = user as User;
            if (userEmpty != null)
            {
                return null;
            }
			if (user != null)
			{
				return GetFileLocationFromPhoto(userObj.Photo, small);
			}
            return null;
        }

        private static FileLocation GetFileLocationFromPhoto(IChatPhoto photo, bool small)
        {
            var empty = photo as ChatPhotoEmpty;
            var full = photo as ChatPhoto;
            if (empty != null)
            {
                return null;
            }
            else if (full != null)
            {
                var iFileLocation = small ? full.PhotoSmall : full.PhotoBig;
                var fileLocation = iFileLocation as FileLocation;
                if (fileLocation != null)
                {
                    return fileLocation;
                }
                else
                {
                    // If the file location is empty, then we assume the chat hasn't set a photo.
                    // Fall-through
                }
            }
            return null;
        }

        private static FileLocation GetFileLocationFromPhoto(IUserProfilePhoto photo, bool small)
        {
            var empty = photo as UserProfilePhotoEmpty;
            var full = photo as UserProfilePhoto;
            if (empty != null)
            {
                return null;
            }
            else if (full != null)
            {
                var iFileLocation = small ? full.PhotoSmall : full.PhotoBig;
                var fileLocation = iFileLocation as FileLocation;
                if (fileLocation != null)
                {
                    return fileLocation;
                }
                else
                {
                    // If the file location is empty, then we assume the user hasn't set a photo.
                    // Fall-through
                }
            }
            return null;
        }

        public static string GetUserId(IUser user)
        {
            var userEmpty = user as UserEmpty;
			var userObj = user as User;
            if (userEmpty != null)
            {
                return userEmpty.Id.ToString(CultureInfo.InvariantCulture);
            }
            if (userObj != null)
            {
                return userObj.Id.ToString(CultureInfo.InvariantCulture);
            }
            return null;
        }

        public static long GetLastSeenTime(IUser user)
        {
            var status = GetStatus(user);
            return status is UserStatusOffline ? (status as UserStatusOffline).WasOnline : 0;
        }

        public static bool GetAvailable(IUser user)
        {
            var status = GetStatus(user);
            return GetAvailable(status);
        }

        public static bool GetAvailable(IUserStatus status)
        {
            return status is UserStatusOnline;
        }

        public static IUserStatus GetStatus(IUser user)
        {
            var userEmpty = user as UserEmpty;
			var userObj = user as User;
            if (userEmpty != null)
            {
                return null;
            }
			if (userObj != null)
			{
				return userObj.Status;
			}
            return null;
        }

        public static string GetUserIdFromParticipant(IChatParticipant partyParticipant)
        {
            var participant = partyParticipant as ChatParticipant;
            var participantCreator = partyParticipant as ChatParticipantCreator;
            var participantAdmin = partyParticipant as ChatParticipantAdmin;
            if (participant != null)
            {
                return participant.UserId.ToString(CultureInfo.InvariantCulture);
            }
            else if (participantCreator != null)
            {
                return participantCreator.UserId.ToString(CultureInfo.InvariantCulture);
            }
            else if (participantAdmin != null)
            {
                return participantAdmin.UserId.ToString(CultureInfo.InvariantCulture);
            }
            return null;
        }

        public static string GetUserHandle(IUser user)
        {
            var userEmpty = user as UserEmpty;
            var userObj = user as User;
            if (userEmpty != null)
            {
                return null;
            }
            if (userObj != null)
            {
                if (!string.IsNullOrEmpty(userObj.Username))
                {
                    return "@" + userObj.Username.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    return "";
                }
            }
            return null;
        }

        public static uint GetMessageId(IMessage iMessage)
        {
            var message = iMessage as Message;

            var messageService = iMessage as MessageService;

            var messageEmpty = iMessage as MessageEmpty;

            if (message != null)
            {
                return message.Id;
            }
            if (messageService != null)
            {
                return messageService.Id;
            }
            if (messageEmpty != null)
            {
                return messageEmpty.Id;
            }
            return 0;
        }

        public static List<IMessage> GetMessagesFromMessagesMessages(IMessagesMessages iMessagesMessages)
        {
            var messagesMessages = iMessagesMessages as MessagesMessages;
            var messagesMessagesSlice = iMessagesMessages as MessagesMessagesSlice;
            var messagesChannelMessages = iMessagesMessages as MessagesChannelMessages;

            if (messagesMessages != null)
            {
                return messagesMessages.Messages;
            }
            if (messagesMessagesSlice != null)
            {
                return messagesMessagesSlice.Messages;
            }
            if (messagesChannelMessages != null)
            {
                return messagesChannelMessages.Messages;
            }
            return null;
        }

        public static List<IChat> GetChatsFromMessagesMessages(IMessagesMessages iMessagesMessages)
        {
            var messagesMessages = iMessagesMessages as MessagesMessages;
            var messagesMessagesSlice = iMessagesMessages as MessagesMessagesSlice;
            var messagesChannelMessages = iMessagesMessages as MessagesChannelMessages;

            if (messagesMessages != null)
            {
                return messagesMessages.Chats;
            }
            if (messagesMessagesSlice != null)
            {
                return messagesMessagesSlice.Chats;
            }
            if (messagesChannelMessages != null)
            {
                return messagesChannelMessages.Chats;
            }
            return null;
        }


        public static List<IUser> GetUsersFromMessagesMessages(IMessagesMessages iMessagesMessages)
        {
            var messagesMessages = iMessagesMessages as MessagesMessages;
            var messagesMessagesSlice = iMessagesMessages as MessagesMessagesSlice;
            var messagesChannelMessages = iMessagesMessages as MessagesChannelMessages;

            if (messagesMessages != null)
            {
                return messagesMessages.Users;
            }
            if (messagesMessagesSlice != null)
            {
                return messagesMessagesSlice.Users;
            }
            if (messagesChannelMessages != null)
            {
                return messagesChannelMessages.Users;
            }
            return null;
        }

   }
}

