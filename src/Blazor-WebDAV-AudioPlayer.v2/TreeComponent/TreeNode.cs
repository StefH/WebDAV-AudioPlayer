using System.Collections.Generic;

namespace Blazor.WebDAV.AudioPlayer.TreeComponent
{
    public class TreeNode<TNode>
    {
        public TNode Item { get; set; }

        public List<TreeNode<TNode>> ChildNodes { get; set; }

        public bool IsExpanded { get; set; }
    }
}