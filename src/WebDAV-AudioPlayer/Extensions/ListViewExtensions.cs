namespace System.Windows.Forms
{
    public static class ListViewExtensions
    {
        public static void SetSelectedIndex(this ListView listView, int index)
        {
            listView.SelectedIndices.Clear();
            listView.SelectedIndices.Add(index);
        }

        public static void SetBitrate(this ListView listView, int index, string bitrate)
        {
            listView.Items[index].SubItems[2].Text = bitrate;
        }
    }
}