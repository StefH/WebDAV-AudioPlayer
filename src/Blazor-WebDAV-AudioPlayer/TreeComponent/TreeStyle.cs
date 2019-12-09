namespace Blazor.WebDAV.AudioPlayer.TreeComponent
{
    public class TreeStyle
    {
        public static readonly TreeStyle Bootstrap = new TreeStyle
        {
            //ExpandNodeIconClass = "oi oi-plus cursor-pointer",
            //CollapseNodeIconClass = "oi oi-minus cursor-pointer",
            ExpandNodeIconClass = "oi oi-chevron-right cursor-pointer",
            CollapseNodeIconClass = "oi oi-chevron-bottom cursor-pointer",
            NodeTitleClass = "p-1 cursor-pointer",
            NodeTitleSelectedClass = "bg-primary text-white"
        };

        public string ExpandNodeIconClass { get; set; }
        public string CollapseNodeIconClass { get; set; }
        public string NodeTitleClass { get; set; }
        public string NodeTitleSelectedClass { get; set; }
    }
}