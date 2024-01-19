using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using KorgiBot.Configs;
using KorgiBot.Server.Raids.Commands;

namespace KorgiBot.Raids
{
	public class RaidsManager
	{
		private readonly MessageManager _messageManager;
		private readonly RaidsConfig _raidsConfig;
		private readonly RaidsCommandsManager _commandsManager;

		private Dictionary<RaidInfo, Raid> _activeRaids = new();

		public RaidsManager(Bot bot, MessageManager messageManager, RaidsConfig raidsConfig,)
		{
			bot.MessageCreated += async (source, args) => HandleMessageCreated(args);

			_messageManager = messageManager;
			_raidsConfig = raidsConfig;
			_commandsManager = new RaidsCommandsManager(this, _messageManager, _raidsConfig);

			Task.Run(() => _commandsManager.Initialize());
		}

		public void CreateRaid(ulong creatorId, DiscordChannel channel, string description, string startTime, string rawMembers)
		{
			var members = new List<RaidMember>();
			foreach (Match line in Regex.Matches(rawMembers, @"[0-9]{1,2}\.\D{1,}\-(\<\@[0-9]{1,}\>|)"))
			{
				var split = line.Value.Split('-');
				var roleName = Regex.Match(split[0], @"[a-zA-Zа-яА-Я]{1,}").Value;
				ulong roleOwnerId = split[1] == string.Empty ? 0 : ulong.Parse(Regex.Match(split[1], @"[0-9]{1,}").Value);

				members.Add(new RaidMember(roleName, roleOwnerId));
			}

			var raid = new Raid(_raidsConfig, creatorId, description, startTime, members);
			var sentMessage = _messageManager.SendMessageAsync(channel, raid.ToString()).Result;
			var thread = sentMessage.CreateThreadAsync(startTime, AutoArchiveDuration.Day).Result;

			_activeRaids.Add(new RaidInfo(thread.Id, sentMessage.Id), raid);
		}

		public bool TryAddRaidMember(ulong threadId, )

		private void HandleMessageCreated(MessageCreateEventArgs args)
		{
			Console.WriteLine(123);
			if (!_activeRaids.Keys.Any(key => key.ThreadId == args.Channel.Id)) return;

			_commandsManager.HandleCommand(args);
		}

		private void OnMembersUpdate()
		{

		}
	}
}
