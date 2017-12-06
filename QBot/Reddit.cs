using System.Collections.Generic;

namespace QBot
{
	static class Reddit
	{
		public class MediaEmbed { }

		public class SecureMediaEmbed { }

		public class Data2
		{
			public bool Contest_mode { get; set; }
			public string Subreddit_name_prefixed { get; set; }
			public object Banned_by { get; set; }
			public MediaEmbed Media_embed { get; set; }
			public object Thumbnail_width { get; set; }
			public string Subreddit { get; set; }
			public string Selftext_html { get; set; }
			public string Selftext { get; set; }
			public object Likes { get; set; }
			public object Suggested_sort { get; set; }
			public List<object> User_reports { get; set; }
			public object Secure_media { get; set; }
			public object Link_flair_text { get; set; }
			public string Id { get; set; }
			public object View_count { get; set; }
			public SecureMediaEmbed Secure_media_embed { get; set; }
			public bool Clicked { get; set; }
			public object Report_reasons { get; set; }
			public string Author { get; set; }
			public bool Saved { get; set; }
			public int Score { get; set; }
			public object Approved_by { get; set; }
			public bool Over_18 { get; set; }
			public string Domain { get; set; }
			public bool Hidden { get; set; }
			public int Num_comments { get; set; }
			public string Thumbnail { get; set; }
			public string Subreddit_id { get; set; }
			public bool Edited { get; set; }
			public object Link_flair_css_class { get; set; }
			public string Author_flair_css_class { get; set; }
			public int Gilded { get; set; }
			public int Downs { get; set; }
			public bool Brand_safe { get; set; }
			public bool Archived { get; set; }
			public object Removal_reason { get; set; }
			public bool Stickied { get; set; }
			public bool Can_gild { get; set; }
			public object Thumbnail_height { get; set; }
			public bool Hide_score { get; set; }
			public bool Spoiler { get; set; }
			public string Permalink { get; set; }
			public string Subreddit_type { get; set; }
			public bool Locked { get; set; }
			public string Name { get; set; }
			public double Created { get; set; }
			public string Url { get; set; }
			public object Author_flair_text { get; set; }
			public bool Quarantine { get; set; }
			public string Title { get; set; }
			public double Created_utc { get; set; }
			public int Ups { get; set; }
			public object Media { get; set; }
			public double Upvote_ratio { get; set; }
			public List<object> Mod_reports { get; set; }
			public bool Is_self { get; set; }
			public bool Visited { get; set; }
			public object Num_reports { get; set; }
			public object Distinguished { get; set; }
			public string Link_id { get; set; }
			public object Replies { get; set; }
			public string Parent_id { get; set; }
			public int? Controversiality { get; set; }
			public string Body { get; set; }
			public string Body_html { get; set; }
			public bool? Score_hidden { get; set; }
			public int? Depth { get; set; }
		}

		public class Child
		{
			public string Kind { get; set; }
			public Data2 Data { get; set; }
		}

		public class Data
		{
			public string Modhash { get; set; }
			public List<Child> Children { get; set; }
			public object After { get; set; }
			public object Before { get; set; }
		}

		public class RootObject
		{
			public string Kind { get; set; }
			public Data Data { get; set; }
		}
	}
}