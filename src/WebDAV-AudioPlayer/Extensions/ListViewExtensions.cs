namespace System.Windows.Forms
{
    public static class ListViewExtensions
    {
        public static void SetSelectedIndex(this ListView listView, int selectedIndex)
        {
            for (int index = 0; index < listView.Items.Count; index++)
            {
                listView.Items[index].Selected = index == selectedIndex;
            }
        }

        public static void SetCells(this ListView listView, int index, string totalLength, string bitrate)
        {
            var subItems = listView.Items[index].SubItems;
            subItems[2].Text = totalLength;
            subItems[3].Text = bitrate;
        }
    }
}