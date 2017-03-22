﻿using Disa.Framework.Bots;
using SharpMTProto;
using SharpTelegram.Schema;
using System;
using System.Collections.Generic;
using System.Globalization;
// using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Disa.Framework.Telegram
{
    public partial class Telegram : IMentions
    {
        public Task GetTokens(Action<Dictionary<MentionType, char>> result)
        {
            return Task.Factory.StartNew(() =>
            {
                var tokens = new Dictionary<MentionType, char>();
                tokens.Add(MentionType.Username, '@');
                tokens.Add(MentionType.ContextBot, '@');
                tokens.Add(MentionType.BotCommand, '/');
                tokens.Add(MentionType.Hashtag, '#');

                result(tokens);
            });
        }

        // Usernames - we need to return usernames and names for a particular group
        // ContextBot - not supported yet
        // Hasthags - not supported yet
        // BotCommand - we need to return username, name and BotInfo for bots for a particular group
        public Task GetMentions(BubbleGroup group, Action<List<Mention>> result)
        {
            return Task.Factory.StartNew(() =>
            {
                var fullChat = MentionsFetchFullChat(group.Address, group.IsExtendedParty);
                var partyParticipants = MentionsGetPartyParticipants(fullChat);

                var resultList = new List<Mention>();
                if (!group.IsExtendedParty)
                {
                    foreach (var partyParticipant in partyParticipants.ChatParticipants)
                    {
                        var id = TelegramUtils.GetUserIdFromParticipant(partyParticipant);
                        if (id != null)
                        {
                            var user = _dialogs.GetUser(uint.Parse(id));
                            var username = TelegramUtils.GetUserHandle(user);
                            var name = TelegramUtils.GetUserName(user);
                            var groupUsernameMention  = new Mention
                            {
                                Type = MentionType.Username,
                                BubbleGroupId = group.ID,
                                Value = username,
                                Name = name,
                                Address = id
                            };
                            resultList.Add(groupUsernameMention);
                        }
                    }
                }
                else
                {
                    foreach (var partyParticipant in partyParticipants.ChannelParticipants)
                    {
                        var id = TelegramUtils.GetUserIdFromChannelParticipant(partyParticipant);
                        if (id != null)
                        {
                            var user = _dialogs.GetUser(uint.Parse(id));
                            var username = TelegramUtils.GetUserHandle(user);
                            var name = TelegramUtils.GetUserName(user);
                            var channelUsernameMention = new Mention
                            {
                                Type = MentionType.Username,
                                BubbleGroupId = group.ID,
                                Value = username,
                                Name = name,
                                Address = id
                            };

                            resultList.Add(channelUsernameMention);
                        }
                    }
                }

                var chatFull = fullChat.FullChat as ChatFull;
                if (chatFull != null)
                {
                    foreach(var chatFullBotInfo in chatFull.BotInfo)
                    {
                        var telegramBotInfo = chatFullBotInfo as SharpTelegram.Schema.BotInfo;
                        if (telegramBotInfo != null)
                        {
                            var user = _dialogs.GetUser(telegramBotInfo.UserId);
                            var username = TelegramUtils.GetUserHandle(user);
                            var name = TelegramUtils.GetUserName(user);

                            var botCommandMention = new Mention
                            {
                                Type = MentionType.BotCommand,
                                BubbleGroupId = group.ID,
                                Value = username,
                                Name = name,
                                Address = telegramBotInfo.UserId.ToString(CultureInfo.InvariantCulture)
                            };

                            var disaBotInfo = new Disa.Framework.Bots.BotInfo
                            {
                                Address = telegramBotInfo.UserId.ToString(CultureInfo.InvariantCulture),
                                Description = telegramBotInfo.Description,
                                Commands = new List<Disa.Framework.Bots.BotCommand>()
                            };

                            foreach(var c in telegramBotInfo.Commands)
                            {
                                var telegramBotCommand = c as SharpTelegram.Schema.BotCommand;
                                if (telegramBotCommand != null)
                                {
                                    disaBotInfo.Commands.Add(new Disa.Framework.Bots.BotCommand
                                    {
                                        Command = telegramBotCommand.Command,
                                        Description = telegramBotCommand.Description
                                    });
                                }
                            }

                            resultList.Add(botCommandMention);
                        }

                    }
                }

                result(resultList);
            });
        }

        // TODO
        public Task GetRecentHashtags(Action<List<Hashtag>> result)
        {
            return Task.Factory.StartNew(() =>
            {
                result(new List<Hashtag>());
            });
        }

        // TODO
        public Task SetRecentHashtags(List<Hashtag> hashtags, Action<bool> result)
        {
            return Task.Factory.StartNew(() =>
            {
                result(true);
            });
        }

        // TODO
        public Task ClearRecentHashtags(Action<bool> result)
        {
            return Task.Factory.StartNew(() =>
            {
                result(true);
            });
        }

        // TODO
        public Task GetContactsByUsername(string username, Action<List<Contact>> result)
        {
            return Task.Factory.StartNew(() =>
            {
                var peer = ResolvePeer(username);

                var contacts = new List<Contact>();
                foreach (var peerUser in peer.Users)
                {
                    var user = peerUser as User;
                    if (user != null)
                    {
                        var contact = CreateTelegramContact(user);
                        contacts.Add(contact);
                    }
                }

                result(contacts);
            });
        }

        // TODO
        public Task GetInlineBotResults(BotContact bot, string query, string offset, Action<BotResults> botResults)
        {
            throw new NotImplementedException();
        }

        // Separate implementation for Mentions - PartyOptions has its own as well
        private MessagesChatFull MentionsFetchFullChat(string address, bool superGroup)
        {
            MessagesChatFull fullChat = null;
            using (var client = new FullClientDisposable(this))
            {
                if (!superGroup)
                {
                    fullChat =
                        (MessagesChatFull)
                            TelegramUtils.RunSynchronously(
                                client.Client.Methods.MessagesGetFullChatAsync(new MessagesGetFullChatArgs
                                {
                                    ChatId = uint.Parse(address)
                                }));
                }
                else
                {
                    try
                    {
                        fullChat =
                            (MessagesChatFull)
                                TelegramUtils.RunSynchronously(
                                client.Client.Methods.ChannelsGetFullChannelAsync(new ChannelsGetFullChannelArgs
                                {
                                    Channel = new InputChannel
                                    {
                                        ChannelId = uint.Parse(address),
                                        AccessHash = TelegramUtils.GetChannelAccessHash(_dialogs.GetChat(uint.Parse(address)))
                                    }
                                }));
                    }
                    catch (Exception e)
                    {
                        DebugPrint(">>>> get full channel exception " + e);
                    }
                }

                _dialogs.AddUsers(fullChat.Users);
                _dialogs.AddChats(fullChat.Chats);

                return fullChat;
            }
        }

        // Separate implementation for Mentions - PartyOptions has its own as well
        private Participants MentionsGetPartyParticipants(MessagesChatFull fullChat)
        {
            Participants participants = null;

            var iChatFull = fullChat.FullChat;
            var chatFull = iChatFull as ChatFull;
            var channelFull = iChatFull as ChannelFull;
            if (chatFull != null)
            {
                var chatParticipants = chatFull.Participants as ChatParticipants;
                if (chatParticipants != null)
                {
                    participants = new Participants
                    {
                        Type = ParticipantsType.Chat,
                        ChatParticipants = chatParticipants.Participants
                    };
                    return participants;
                }
            }
            if (channelFull != null)
            {
                if (channelFull.CanViewParticipants == null)
                {
                    return new Participants
                    {
                        Type = ParticipantsType.Channel,
                        ChannelParticipants = new List<IChannelParticipant>()
                    };
                }

                var channelParticipants = GetChannelParticipants(channelFull, new ChannelParticipantsRecent());
                var channelAdmins = GetChannelParticipants(channelFull, new ChannelParticipantsAdmins());
                var mergedList = channelAdmins.Union(channelParticipants, new ChannelParticipantComparer()).ToList();
                participants = new Participants
                {
                    Type = ParticipantsType.Channel,
                    ChannelParticipants = mergedList
                };
                return participants;
            }

            return null;    
        }
    }
}

