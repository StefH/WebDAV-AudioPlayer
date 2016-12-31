namespace System.Windows.Forms
{
    public static class ListViewExtensions
    {
        public static void SetSelectedIndex(this ListView listView, int index)
        {
            listView.SelectedIndices.Clear();
            listView.SelectedIndices.Add(index);
        }
    }
}