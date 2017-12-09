using System;

namespace QBot
{
	/// <summary>
	/// LiteDB uses this class to store members to the database
	/// </summary>
	public class GuildMember
	{
		/// <summary>
		/// Id for this class for LiteDB
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Their name on discord
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Their nickname for this server
		/// </summary>
		public string NickName { get; set; }

		/// <summary>
		/// Their discordId
		/// </summary>
		public ulong UniqueId { get; set; }

		/// <summary>
		/// The server they are connected to
		/// </summary>
		public ulong Server { get; set; }
		
		/// <summary>
		/// Blacklisted tags for e621
		/// </summary>
		public string BlackTags { get; set; }

	}
}