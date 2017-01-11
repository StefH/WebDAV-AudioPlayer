namespace System.Windows.Forms
{
    public static class ListViewExtensions
    {
        public static void SetSelectedIndex(this ListView listView, int index)
        {
            listView.SelectedIndices.Clear();
            listView.SelectedIndices.Add(index);
        }

        public static void SetCells(this ListView listView, int index, string totalLength, string bitrate)
        {
            var subItems = listView.Items[index].SubItems;
            subItems[2].Text = totalLength;
            subItems[3].Text = bitrate;
        }
    }
}