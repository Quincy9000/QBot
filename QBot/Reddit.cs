using System.Collections.Generic;

namespace QBot
{
    class Reddit
    {
        public class MediaEmbed { }

        public class SecureMediaEmbed { }

        public class Data2
        {
            public bool contest_mode { get; set; }
            public string subreddit_name_prefixed { get; set; }
            public object banned_by { get; set; }
            public MediaEmbed media_embed { get; set; }
            public object thumbnail_width { get; set; }
            public string subreddit { get; set; }
            public string selftext_html { get; set; }
            public string selftext { get; set; }
            public object likes { get; set; }
            public object suggested_sort { get; set; }
            public List<object> user_reports { get; set; }
            public object secure_media { get; set; }
            public object link_flair_text { get; set; }
            public string id { get; set; }
            public object view_count { get; set; }
            public SecureMediaEmbed secure_media_embed { get; set; }
            public bool clicked { get; set; }
            public object report_reasons { get; set; }
            public string author { get; set; }
            public bool saved { get; set; }
            public int score { get; set; }
            public object approved_by { get; set; }
            public bool over_18 { get; set; }
            public string domain { get; set; }
            public bool hidden { get; set; }
            public int num_comments { get; set; }
            public string thumbnail { get; set; }
            public string subreddit_id { get; set; }
            public bool edited { get; set; }
            public object link_flair_css_class { get; set; }
            public string author_flair_css_class { get; set; }
            public int gilded { get; set; }
            public int downs { get; set; }
            public bool brand_safe { get; set; }
            public bool archived { get; set; }
            public object removal_reason { get; set; }
            public bool stickied { get; set; }
            public bool can_gild { get; set; }
            public object thumbnail_height { get; set; }
            public bool hide_score { get; set; }
            public bool spoiler { get; set; }
            public string permalink { get; set; }
            public string subreddit_type { get; set; }
            public bool locked { get; set; }
            public string name { get; set; }
            public double created { get; set; }
            public string url { get; set; }
            public object author_flair_text { get; set; }
            public bool quarantine { get; set; }
            public string title { get; set; }
            public double created_utc { get; set; }
            public int ups { get; set; }
            public object media { get; set; }
            public double upvote_ratio { get; set; }
            public List<object> mod_reports { get; set; }
            public bool is_self { get; set; }
            public bool visited { get; set; }
            public object num_reports { get; set; }
            public object distinguished { get; set; }
            public string link_id { get; set; }
            public object replies { get; set; }
            public string parent_id { get; set; }
            public int? controversiality { get; set; }
            public string body { get; set; }
            public string body_html { get; set; }
            public bool? score_hidden { get; set; }
            public int? depth { get; set; }
        }

        public class Child
        {
            public string kind { get; set; }
            public Data2 data { get; set; }
        }

        public class Data
        {
            public string modhash { get; set; }
            public List<Child> children { get; set; }
            public object after { get; set; }
            public object before { get; set; }
        }

        public class RootObject
        {
            public string kind { get; set; }
            public Data data { get; set; }
        }
    }
}